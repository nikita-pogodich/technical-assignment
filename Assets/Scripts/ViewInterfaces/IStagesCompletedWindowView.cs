using Core.MVP;
using R3;

namespace ViewInterfaces
{
    public interface IStagesCompletedWindowView : IWindowView
    {
        Observable<Unit> NewGame { get; }
        Observable<Unit> Menu { get; }
        void SetScore(int score);
    }
}