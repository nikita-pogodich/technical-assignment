using Core.MVPImplementation;
using Core.WindowManager;
using Cysharp.Threading.Tasks;
using Features.CardsMatching;
using R3;
using Settings;
using UnityEngine;
using ViewInterfaces;

namespace Features.MainMenu
{
    public class MainMenuWindowPresenter : BaseWindowPresenter<IMainMenuWindowView, MainMenuWindowModel>
    {
        private readonly ILocalSettings _localSettings;
        private readonly IWindowManager _windowManager;
        private readonly CardsMatchingModel _cardsMatchingModel;

        public MainMenuWindowPresenter(
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
            View.ContinueGame.Subscribe(OnContinueGame).AddTo(ref disposableBuilder);
            View.ExitGame.Subscribe(OnExitGame).AddTo(ref disposableBuilder);
        }

        protected override void OnShow()
        {
            View.SetContinueButtonShown(false);
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

        private void OnContinueGame(Unit _)
        {
            if (IsShown == false)
            {
                return;
            }

            SetShown(false);
        }

        private void OnExitGame(Unit _)
        {
            if (IsShown == false)
            {
                return;
            }

            Application.Quit();
        }
    }
}