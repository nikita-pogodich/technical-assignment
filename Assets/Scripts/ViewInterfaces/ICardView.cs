using System.Threading;
using Core.AudioManager;
using Core.MVP;
using Core.ResourcesManager;
using Cysharp.Threading.Tasks;
using R3;
using Settings;
using UnityEngine;

namespace ViewInterfaces
{
    public interface ICardView : IView
    {
        int Position { get; set; }
        Observable<Unit> Selected { get; }
        UniTask LoadIconAsync(string iconResourceKey);
        UniTask DealCardAsync(Vector3 dealingOrigin, CancellationToken cancellationToken);

        void InjectDependencies(
            ILocalSettings localSettings,
            IResourcesManager resourcesManager,
            IAudioManager audioManager);

        void SetFlipped(bool isFlipped, bool isInstantly = false);
        void SetMatched(bool isMatched);
        void UpdatePosition();
    }
}