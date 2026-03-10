using Core.MVPImplementation;
using Core.WindowManager;
using Cysharp.Threading.Tasks;
using Features.CardsMatching;
using Features.MainMenu;
using R3;
using Settings;
using ViewInterfaces;

namespace Features.StagesCompleted
{
    public class StagesCompletedWindowPresenter :
        BaseWindowPresenter<IStagesCompletedWindowView, StagesCompletedWindowModel>
    {
        private readonly ILocalSettings _localSettings;
        private readonly IWindowManager _windowManager;
        private readonly CardsMatchingModel _cardsMatchingModel;

        public StagesCompletedWindowPresenter(
            ILocalSettings localSettings,
            IWindowManager windowManager,
            CardsMatchingModel cardsMatchingModel)
        {
            _localSettings = localSettings;
            _windowManager = windowManager;
            _cardsMatchingModel = cardsMatchingModel;
        }

        protected override void OnInit(ref DisposableBuilder disposableBuilder)
        {
            View.NewGame.Subscribe(OnNewGame).AddTo(ref disposableBuilder);
            View.Menu.Subscribe(OnMenu).AddTo(ref disposableBuilder);
        }

        protected override void OnShow()
        {
            View.SetScore(Model.Score);
        }

        private void OnNewGame(Unit _)
        {
            if (IsShown == false)
            {
                return;
            }

            SetShown(false);

            _cardsMatchingModel.StartNewGame();
            _windowManager.ShowWindowAsync<ICardsMatchingWindow, CardsMatchingModel>(
                _localSettings.ViewNames.CardsMatchingWindow).Forget();
        }

        private void OnMenu(Unit _)
        {
            if (IsShown == false)
            {
                return;
            }

            SetShown(false);
            _windowManager.ShowWindowAsync<IMainMenuWindowView, MainMenuWindowModel>(
                _localSettings.ViewNames.MainMenuWindow).Forget();
        }
    }
}