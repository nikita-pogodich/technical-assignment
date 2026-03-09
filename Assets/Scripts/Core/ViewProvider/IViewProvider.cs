using Core.MVP;
using Cysharp.Threading.Tasks;

namespace Core.ViewProvider
{
    public interface IViewProvider
    {
        TView Get<TView>(string resourceKey) where TView : IView;
        UniTask<TView> GetAsync<TView>(string resourceKey) where TView : IView;
        void Release<TView>(string resourceKey, TView view) where TView : IView;
    }
}