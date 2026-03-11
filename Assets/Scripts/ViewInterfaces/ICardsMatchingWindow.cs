using System.Threading;
using Core.MVP;
using Cysharp.Threading.Tasks;
using R3;
using Settings;

namespace ViewInterfaces
{
    public interface ICardsMatchingWindow : IWindowView
    {
        Observable<Unit> BackToMainMenu { get; }
        ReactiveProperty<int> Score { get; }
        void InjectDependencies(ILocalSettings localSettings);
        void AddCard(ICardView cardView);
        void UpdateCardPositions();
        void SetAllCardsFilled(bool isFlipped, bool isInstantly);
        void ClearCards();
        void SetStageIndex(int stageIndex);
        UniTask DealCardsAsync(CancellationToken cancellationToken);
    }
}