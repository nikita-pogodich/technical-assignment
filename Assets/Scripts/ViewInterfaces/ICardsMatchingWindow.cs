using System.Threading;
using Core.MVP;
using Core.OrientationDetector;
using Cysharp.Threading.Tasks;
using R3;
using Settings;

namespace ViewInterfaces
{
    public interface ICardsMatchingWindow : IWindowView
    {
        Observable<Unit> BackToMainMenu { get; }
        void SetScore(int score);
        void SetComboMultiplier(int comboMultiplier);
        void InjectDependencies(ILocalSettings localSettings, IOrientationDetector orientationDetector);
        void AddCard(ICardView cardView);
        void UpdateCardPositions();
        void ClearCards();
        void SetStageIndex(int stageIndex);
        UniTask DealCardsAsync(CancellationToken cancellationToken);
        UniTask HideAllCardsAsync(CancellationToken cancellationToken);
    }
}