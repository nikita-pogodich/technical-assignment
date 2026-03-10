using Core.MVPImplementation;
using R3;
using UnityEngine;
using UnityEngine.UI;
using ViewInterfaces;

namespace Views.MainMenuWindow
{
    public class MainMenuWindowView : BaseWindowView, IMainMenuWindowView
    {
        [SerializeField]
        private Button _newGameButton;

        [SerializeField]
        private Button _continueGameButton;

        [SerializeField]
        private Button _exitButton;

        private readonly ReactiveCommand _newGame = new();
        private readonly ReactiveCommand _continueGame = new();
        private readonly ReactiveCommand _exitGame = new();

        public Observable<Unit> NewGame => _newGame;
        public Observable<Unit> ContinueGame => _continueGame;
        public Observable<Unit> ExitGame => _exitGame;

        public override void SetShown(bool isShown)
        {
            base.SetShown(isShown);
            SetCanvasEnabled(isShown);
        }

        public void SetContinueButtonShown(bool isShown)
        {
            _continueGameButton.gameObject.SetActive(isShown);
        }

        protected override void OnInit(ref DisposableBuilder disposableBuilder)
        {
            _newGameButton.OnClickAsObservable().Subscribe(OnNewGame).AddTo(ref disposableBuilder);
            _continueGameButton.OnClickAsObservable().Subscribe(OnContinueGame).AddTo(ref disposableBuilder);
            _exitButton.OnClickAsObservable().Subscribe(OnExitGame).AddTo(ref disposableBuilder);
        }

        private void OnNewGame(Unit _)
        {
            _newGame.Execute(Unit.Default);
        }

        private void OnContinueGame(Unit _)
        {
            _continueGame.Execute(Unit.Default);
        }

        private void OnExitGame(Unit _)
        {
            _exitGame.Execute(Unit.Default);
        }
    }
}