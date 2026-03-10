using Core.MVP;
using Core.ResourcesManager;
using R3;

namespace ViewInterfaces
{
    public interface ICardView : IView
    {
        ReactiveProperty<string> IconResourceKey { get; }
        Observable<Unit> Selected { get; }
        void InjectDependencies(IResourcesManager resourcesManager);
        void SetFlipped(bool isFlipped);
        void SetMatched(bool isMatched);
    }
}