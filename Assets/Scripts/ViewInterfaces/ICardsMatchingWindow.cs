using Core.MVP;
using R3;
using Settings;

namespace ViewInterfaces
{
    public interface ICardsMatchingWindow : IWindowView
    {
        Observable<Unit> BackToMainMenu { get; }
        ReactiveProperty<int> Score { get; }
        void InjectDependencies(ILocalSettings localSettings);
        void AddCard(int position, ICardView cardView);
        void SetAllCardsFilled(bool isFlipped);
        void ClearCards();
        void SetStageIndex(int stageIndex);
    }
}