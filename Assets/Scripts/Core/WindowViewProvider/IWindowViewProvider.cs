using Core.MVP;
using Cysharp.Threading.Tasks;

namespace Core.WindowViewProvider
{
    public interface IWindowViewProvider
    {
        TView Get<TView>(string resourceKey, WindowType windowType) where TView : IWindowView;
        UniTask<TView> GetAsync<TView>(string resourceKey, WindowType windowType) where TView : IWindowView;
        void Release<TView>(string resourceKey, TView view) where TView : IWindowView;
    }
}