using Core.MVP;
using R3;

namespace ViewInterfaces
{
    public interface IMainMenuWindowView : IWindowView
    {
        Observable<Unit> NewGame { get; }
        Observable<Unit> ContinueGame { get; }
        Observable<Unit> ExitGame { get; }

        void SetContinueButtonShown(bool isShown);
    }
}