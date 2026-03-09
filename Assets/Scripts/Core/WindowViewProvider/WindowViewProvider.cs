using System;
using System.Threading;
using Core.MVP;
using Core.MVPImplementation;
using Core.ResourcesManager;
using Core.ViewProvider;
using Cysharp.Threading.Tasks;
using Settings;
using UnityEngine;

namespace Core.WindowViewProvider
{
    public class WindowViewProvider : IWindowViewProvider
    {
        private readonly ILocalSettings _localSettings;
        private readonly IResourcesManager _resourcesManager;
        private readonly IViewProvider _viewProvider;

        private WindowViewProviderRoots _windowViewProviderRoots;

        public WindowViewProvider(
            ILocalSettings localSettings,
            IResourcesManager resourcesManager,
            IViewProvider viewProvider)
        {
            _localSettings = localSettings;
            _resourcesManager = resourcesManager;
            _viewProvider = viewProvider;
        }

        public async UniTask InitializeAsync(CancellationToken cancellation)
        {
            GameObject windowRootsGameObject = await _resourcesManager.InstantiateAsync(
                _localSettings.ResourceNames.WindowRoots,
                cancellationToken: cancellation);

            UnityEngine.Object.DontDestroyOnLoad(windowRootsGameObject);

            _windowViewProviderRoots = windowRootsGameObject.GetComponent<WindowViewProviderRoots>();
        }

        public TView Get<TView>(string resourceKey, WindowType windowType) where TView : IWindowView
        {
            TView view = _viewProvider.Get<TView>(resourceKey);

            if (view == null)
            {
                return default;
            }

            if (view is not BaseView baseView)
            {
                return default;
            }

            SetWindowViewParent(baseView, windowType);

            return view;
        }

        public async UniTask<TView> GetAsync<TView>(string resourceKey, WindowType windowType) where TView : IWindowView
        {
            TView view = await _viewProvider.GetAsync<TView>(resourceKey);

            if (view == null)
            {
                return default;
            }

            if (view is not BaseView baseView)
            {
                return default;
            }

            SetWindowViewParent(baseView, windowType);

            return view;
        }

        public void Release<TView>(string resourceKey, TView view) where TView : IWindowView
        {
            if (view is BaseWindowView baseView)
            {
                _resourcesManager.ReleaseGameObject(resourceKey, baseView.gameObject);
            }
        }

        private void SetWindowViewParent(BaseView baseView, WindowType windowType)
        {
            WindowTypeRoot resultWindowTypeRoot = _windowViewProviderRoots.DefaultWindowTypeRoot;

            foreach (WindowTypeRoot windowTypeRoot in _windowViewProviderRoots.WindowTypeRoots)
            {
                if (windowType == windowTypeRoot.WindowType)
                {
                    resultWindowTypeRoot = windowTypeRoot;
                    break;
                }
            }

            var viewRectTransform = baseView.GetComponent<RectTransform>();
            viewRectTransform.SetParent(resultWindowTypeRoot.Root);
            viewRectTransform.localScale = Vector2.one;
            viewRectTransform.anchoredPosition = Vector2.zero;
            viewRectTransform.anchorMin = Vector2.zero;
            viewRectTransform.anchorMax = Vector2.one;
            viewRectTransform.offsetMin = Vector2.zero;
            viewRectTransform.offsetMax = Vector2.zero;
        }
    }

    [Serializable]
    public class WindowTypeRoot
    {
        public Transform Root;
        public WindowType WindowType;
    }
}