using System.Collections.Generic;
using Core.MVPImplementation;
using R3;
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
        private Transform _content;

        private readonly ReactiveCommand _exitGame = new();
        private readonly List<ICardView> _cardViews = new();

        public Observable<Unit> BackToMainMenu => _exitGame;

        public void AddCard(int position, ICardView cardView)
        {
            if (cardView is not BaseView itemView)
            {
                return;
            }

            Transform itemTransform = itemView.transform;
            itemTransform.SetParent(_content);
            itemTransform.localScale = Vector3.one;
            itemTransform.localEulerAngles = Vector3.zero;

            itemTransform.SetSiblingIndex(position);

            _cardViews.Add(cardView);
        }

        public void SetAllCardsFilled(bool isFlipped)
        {
            foreach (ICardView cardView in _cardViews)
            {
                cardView.SetFlipped(isFlipped);
            }
        }

        public void ClearCards()
        {
            _cardViews.Clear();
        }

        protected override void OnInit(ref DisposableBuilder disposableBuilder)
        {
            _backToMainMenuButton.OnClickAsObservable().Subscribe(OnBackToMainMenu).AddTo(ref disposableBuilder);
        }

        protected override void OnDeinit()
        {
            _cardViews.Clear();
        }

        private void OnBackToMainMenu(Unit _)
        {
            _exitGame.Execute(Unit.Default);
        }
    }
}