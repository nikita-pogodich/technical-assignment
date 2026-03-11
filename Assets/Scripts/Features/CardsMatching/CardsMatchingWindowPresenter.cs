using System.Collections.Generic;
using Core.MVPImplementation;
using Core.ResourcesManager;
using Core.ViewProvider;
using Core.WindowManager;
using Cysharp.Threading.Tasks;
using Features.MainMenu;
using Features.StagesCompleted;
using R3;
using Settings;
using ViewInterfaces;

namespace Features.CardsMatching
{
    public class CardsMatchingWindowPresenter : BaseWindowPresenter<ICardsMatchingWindow, CardsMatchingModel>
    {
        private readonly ILocalSettings _localSettings;
        private readonly IViewProvider _viewProvider;
        private readonly IWindowManager _windowManager;
        private readonly IResourcesManager _resourcesManager;
        private readonly List<CardPresenter> _cardPresenters = new();

        public CardsMatchingWindowPresenter(
            ILocalSettings localSettings,
            IViewProvider viewProvider,
            IWindowManager windowManager,
            IResourcesManager resourcesManager)
        {
            _localSettings = localSettings;
            _viewProvider = viewProvider;
            _windowManager = windowManager;
            _resourcesManager = resourcesManager;
        }

        protected override void OnInit(ref DisposableBuilder disposableBuilder)
        {
            View.BackToMainMenu.Subscribe(OnBackToMainMenu).AddTo(ref disposableBuilder);
            Model.CurrentGameState.Subscribe(OnCurrentGameStateChanged).AddTo(ref disposableBuilder);
            Model.CurrentScore.Subscribe(OnCurrentScoreChanged).AddTo(ref disposableBuilder);
        }

        private async UniTaskVoid CreateCardsAsync()
        {
            ClearCards();

            foreach (CardModel cardModel in Model.CurrentCardModelByPositions.Values)
            {
                var cardView = await _viewProvider.GetAsync<ICardView>(_localSettings.ViewNames.CardView);
                cardView.InjectDependencies(_resourcesManager);
                await cardView.LoadIconAsync(cardModel.IconResourceKey);

                var cardPresenter = new CardPresenter();
                cardPresenter.Init(cardView, cardModel);
                _cardPresenters.Add(cardPresenter);

                View.AddCard(cardModel.Position, cardView);
            }

            foreach (CardPresenter cardPresenter in _cardPresenters)
            {
                cardPresenter.SetShown(true);
            }

            await Model.CompleteCardsCreationAsync();
        }

        private void ClearCards()
        {
            foreach (CardPresenter cardPresenter in _cardPresenters)
            {
                cardPresenter.SetShown(false);
                _viewProvider.Release(_localSettings.ViewNames.CardView, cardPresenter.View);
            }

            _cardPresenters.Clear();
            View.ClearCards();
        }

        private void OnCurrentGameStateChanged(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.CardsCreation:
                    CreateCardsAsync().Forget();
                    View.SetStageIndex(Model.CurrentStageIndex);
                    break;
                case GameState.Remembering:
                    View.SetAllCardsFilled(isFlipped: true, isInstantly: true);
                    break;
                case GameState.Matching:
                    View.SetAllCardsFilled(isFlipped: false, isInstantly: false);
                    break;
                case GameState.StageCompleted:
                    Model.StartNextStage();
                    break;
                case GameState.AllStagesCompleted:
                    _windowManager.ShowWindowAsync<IStagesCompletedWindowView, StagesCompletedWindowModel>(
                        _localSettings.ViewNames.StagesCompletedWindow,
                        beforeShow: OnBeforeStagesCompletedWindowShow).Forget();

                    SetShown(false);
                    break;
            }
        }

        private void OnBeforeStagesCompletedWindowShow(StagesCompletedWindowModel model)
        {
            model.SetScore(Model.CurrentScore.CurrentValue);
        }

        private void OnCurrentScoreChanged(int score)
        {
            View.Score.Value = score;
        }

        private void OnBackToMainMenu(Unit _)
        {
            if (IsShown == false)
            {
                return;
            }

            ClearCards();
            Model.EndGame();

            SetShown(false);
            _windowManager.ShowWindowAsync<IMainMenuWindowView, MainMenuWindowModel>(
                _localSettings.ViewNames.MainMenuWindow).Forget();
        }
    }
}