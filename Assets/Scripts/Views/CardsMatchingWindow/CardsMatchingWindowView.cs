using System.Collections.Generic;
using System.Threading;
using Core.MVPImplementation;
using Cysharp.Threading.Tasks;
using R3;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewInterfaces;

namespace Views.CardsMatchingWindow
{
    public class CardsMatchingWindowView : BaseWindowView, ICardsMatchingWindow
    {
        [SerializeField]
        private Button _backToMainMenuButton;

        [SerializeField]
        private TextMeshProUGUI _scoreValue;

        [SerializeField]
        private string _scoreFormat = "Score: {0}";

        [SerializeField]
        private RectTransform _content;

        [SerializeField]
        private RectTransform _dealingOrigin;

        [SerializeField]
        private GridLayoutGroup _gridLayout;

        private readonly ReactiveCommand _exitGame = new();
        private readonly List<ICardView> _cardViews = new();
        private ILocalSettings _localSettings;

        public Observable<Unit> BackToMainMenu => _exitGame;
        public ReactiveProperty<int> Score { get; } = new();

        public void InjectDependencies(ILocalSettings localSettings)
        {
            _localSettings = localSettings;
        }

        public override void SetShown(bool isShown)
        {
            base.SetShown(isShown);
            SetCanvasEnabled(isShown);

            //TODO: Add show/hide animation
        }

        public void AddCard(ICardView cardView)
        {
            if (cardView is not BaseView itemView)
            {
                return;
            }

            Transform itemTransform = itemView.transform;
            itemTransform.SetParent(_content);
            itemTransform.localScale = Vector3.one;
            itemTransform.localEulerAngles = Vector3.zero;

            itemTransform.SetSiblingIndex(cardView.Position);

            _cardViews.Add(cardView);
        }

        public void UpdateCardPositions()
        {
            foreach (ICardView cardView in _cardViews)
            {
                cardView.UpdatePosition();
            }
        }

        public void SetAllCardsFilled(bool isFlipped, bool isInstantly)
        {
            foreach (ICardView cardView in _cardViews)
            {
                cardView.SetFlipped(isFlipped, isInstantly);
            }
        }

        public void ClearCards()
        {
            _cardViews.Clear();
        }

        public void SetStageIndex(int stageIndex)
        {
            if (_localSettings == null ||
                stageIndex >= _localSettings.GameSettings.StageSettings.Count)
            {
                return;
            }

            StageSetting stageSetting = _localSettings.GameSettings.StageSettings[stageIndex];
            _gridLayout.cellSize = stageSetting.CardSize;

            _content.sizeDelta = new Vector2(stageSetting.GridWidth, _content.sizeDelta.y);
        }

        private readonly List<UniTask> _cardsDealingTasks = new();

        public async UniTask DealCardsAsync(CancellationToken cancellationToken)
        {
            _cardsDealingTasks.Clear();

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

            foreach (ICardView cardView in _cardViews)
            {
                _cardsDealingTasks.Add(cardView.DealCardAsync(_dealingOrigin.position, cancellationToken));
            }

            await UniTask.WhenAll(_cardsDealingTasks);
        }

        protected override void OnInit(ref DisposableBuilder disposableBuilder)
        {
            _backToMainMenuButton.OnClickAsObservable().Subscribe(OnBackToMainMenu).AddTo(ref disposableBuilder);
            Score.Subscribe(OnScoreChanged).AddTo(ref disposableBuilder);
        }

        protected override void OnDeinit()
        {
            _cardViews.Clear();
        }

        private void OnBackToMainMenu(Unit _)
        {
            _exitGame.Execute(Unit.Default);
        }

        private void OnScoreChanged(int score)
        {
            _scoreValue.text = string.Format(_scoreFormat, score.ToString());
        }
    }
}