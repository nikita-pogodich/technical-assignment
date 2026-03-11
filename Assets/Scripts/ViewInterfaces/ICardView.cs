using Core.MVP;
using Core.ResourcesManager;
using Cysharp.Threading.Tasks;
using R3;

namespace ViewInterfaces
{
    public interface ICardView : IView
    {
        Observable<Unit> Selected { get; }
        UniTask LoadIconAsync(string iconResourceKey);
        void InjectDependencies(IResourcesManager resourcesManager);
        void SetFlipped(bool isFlipped, bool isInstantly = false);
        void SetMatched(bool isMatched);
    }
}