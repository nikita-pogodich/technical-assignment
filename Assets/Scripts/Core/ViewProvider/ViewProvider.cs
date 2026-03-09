using Core.MVP;
using Core.MVPImplementation;
using Core.ResourcesManager;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.ViewProvider
{
    public class ViewProvider : IViewProvider
    {
        private readonly IResourcesManager _resourcesManager;

        public ViewProvider(IResourcesManager resourcesManager)
        {
            _resourcesManager = resourcesManager;
        }

        public TView Get<TView>(string resourceKey) where TView : IView
        {
            GameObject resource = _resourcesManager.Instantiate(resourceKey);
            TView view = resource.GetComponent<TView>();

            view.Init(resourceKey);

            return view;
        }

        public async UniTask<TView> GetAsync<TView>(string resourceKey) where TView : IView
        {
            GameObject resource = await _resourcesManager.InstantiateAsync(resourceKey);
            TView view = resource.GetComponent<TView>();

            view.Init(resourceKey);

            return view;
        }

        public void Release<TView>(string resourceKey, TView view) where TView : IView
        {
            if (view == null)
            {
                return;
            }

            if (view is not BaseView baseView)
            {
                return;
            }

            if (baseView == null)
            {
                return;
            }

            _resourcesManager.ReleaseGameObject(resourceKey, baseView.gameObject);
        }
    }
}