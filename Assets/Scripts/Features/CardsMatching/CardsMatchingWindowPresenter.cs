using System.Collections.Generic;
using System.Threading;
using Core.AudioManager;
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
        private readonly IAudioManager _audioManager;
        private readonly List<CardPresenter> _cardPresenters = new();

        private CancellationTokenSource _cancellationTokenSource;

        public CardsMatchingWindowPresenter(
            ILocalSettings localSettings,
            IViewProvider viewProvider,
            IWindowManager windowManager,
            IResourcesManager resourcesManager,
            IAudioManager audioManager)
        {
            _localSettings = localSettings;
            _viewProvider = viewProvider;
            _windowManager = windowManager;
            _resourcesManager = resourcesManager;
            _audioManager = audioManager;
        }

        protected override void OnInit(ref DisposableBuilder disposableBuilder)
        {
            View.BackToMainMenu.Subscribe(OnBackToMainMenu).AddTo(ref disposableBuilder);

            Model.CurrentScore.Subscribe(View.SetScore).AddTo(ref disposableBuilder);
            Model.ComboMultiplier.Subscribe(View.SetComboMultiplier).AddTo(ref disposableBuilder);
            Model.CurrentGameState.Subscribe(OnCurrentGameStateChanged).AddTo(ref disposableBuilder);
            Model.CardsMatched.Subscribe(OnCardsMatched).AddTo(ref disposableBuilder);
            Model.CardsMismatched.Subscribe(OnCardsMismatched).AddTo(ref disposableBuilder);
            Model.ComboMultiplierIncreased.Subscribe(OnComboMultiplierIncreased).AddTo(ref disposableBuilder);
            Model.ComboMultiplierLost.Subscribe(OnComboMultiplierLost).AddTo(ref disposableBuilder);
        }

        protected override void OnDeinit()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private async UniTaskVoid CreateCardsAsync()
        {
            ClearCards();

            _cancellationTokenSource = new CancellationTokenSource();

            foreach (CardModel cardModel in Model.CurrentCardModelByPositions.Values)
            {
                var cardView = await _viewProvider.GetAsync<ICardView>(_localSettings.ViewNames.CardView);
                cardView.InjectDependencies(_localSettings, _resourcesManager, _audioManager);
                await cardView.LoadIconAsync(cardModel.IconResourceKey);

                var cardPresenter = new CardPresenter(_localSettings, _audioManager);
                cardPresenter.Init(cardView, cardModel);
                _cardPresenters.Add(cardPresenter);

                View.AddCard(cardView);
            }

            View.UpdateCardPositions();

            await View.DealCardsAsync(_cancellationTokenSource.Token);

            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            foreach (CardPresenter cardPresenter in _cardPresenters)
            {
                cardPresenter.SetShown(true);
            }

            await Model.CompleteCardsCreationAsync();
        }

        private void ClearCards()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

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
                    _audioManager.PlaySound(_localSettings.ResourceNames.CardFlipSound);
                    View.SetAllCardsFilled(isFlipped: false, isInstantly: false);
                    break;
                case GameState.StageCompleted:
                    Model.StartNextStage();
                    break;
                case GameState.AllStagesCompleted:
                    _audioManager.PlaySound(_localSettings.ResourceNames.VictorySound);

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

        private void OnCardsMatched(Unit _)
        {
            if (IsShown == false)
            {
                return;
            }

            _audioManager.PlaySound(_localSettings.ResourceNames.CardMatchedSound);
        }

        private void OnCardsMismatched(Unit _)
        {
            if (IsShown == false)
            {
                return;
            }

            _audioManager.PlaySound(_localSettings.ResourceNames.CardMismatchedSound);
        }

        private void OnComboMultiplierIncreased(Unit _)
        {
            if (IsShown == false)
            {
                return;
            }

            _audioManager.PlaySound(_localSettings.ResourceNames.ComboMultiplierIncreased);
        }

        private void OnComboMultiplierLost(Unit _)
        {
            if (IsShown == false)
            {
                return;
            }

            _audioManager.PlaySound(_localSettings.ResourceNames.ComboMultiplierLost);
        }
    }
}