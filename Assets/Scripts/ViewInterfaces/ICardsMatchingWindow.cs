using Core.MVP;
using R3;

namespace ViewInterfaces
{
    public interface ICardsMatchingWindow : IWindowView
    {
        Observable<Unit> BackToMainMenu { get; }
        void AddCard(int position, ICardView cardView);
        void SetAllCardsFilled(bool isFlipped);
        void ClearCards();
    }
}