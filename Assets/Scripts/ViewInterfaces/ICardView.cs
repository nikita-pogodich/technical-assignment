using System.Threading;
using Core.MVP;
using Core.ResourcesManager;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace ViewInterfaces
{
    public interface ICardView : IView
    {
        int Position { get; set; }
        Observable<Unit> Selected { get; }
        UniTask LoadIconAsync(string iconResourceKey);
        UniTask DealCardAsync(Vector3 dealingOrigin, CancellationToken cancellationToken);
        void InjectDependencies(IResourcesManager resourcesManager);
        void SetFlipped(bool isFlipped, bool isInstantly = false);
        void SetMatched(bool isMatched);
        void UpdatePosition();
    }
}