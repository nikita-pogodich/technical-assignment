using Core.MVPImplementation;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewInterfaces;

namespace Views.StagesCompletedWindow
{
    public class StagesCompletedWindowView : BaseWindowView, IStagesCompletedWindowView
    {
        [SerializeField]
        private TextMeshProUGUI _score;

        [SerializeField]
        private Button _newGameButton;

        [SerializeField]
        private Button _menuButton;

        private readonly ReactiveCommand _newGame = new();
        private readonly ReactiveCommand _menu = new();

        public Observable<Unit> NewGame => _newGame;
        public Observable<Unit> Menu => _menu;
        
        public override void SetShown(bool isShown)
        {
            base.SetShown(isShown);
            SetCanvasEnabled(isShown);

            //TODO: Add show/hide animation
        }

        public void SetScore(int score)
        {
            _score.text = score.ToString();
        }

        protected override void OnInit(ref DisposableBuilder disposableBuilder)
        {
            _newGameButton.OnClickAsObservable().Subscribe(OnNewGame).AddTo(ref disposableBuilder);
            _menuButton.OnClickAsObservable().Subscribe(OnMenu).AddTo(ref disposableBuilder);
        }

        private void OnNewGame(Unit _)
        {
            _newGame.Execute(Unit.Default);
        }

        private void OnMenu(Unit _)
        {
            _menu.Execute(Unit.Default);
        }
    }
}