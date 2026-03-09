using Core.MVP;
using Cysharp.Threading.Tasks;

namespace Core.WindowManager
{
    public interface IWindowManager
    {
        void RegisterWindowFactory(IWindowFactory windowFactory);

        UniTask ShowWindowAsync<TView, TModel>(
            string viewName,
            WindowShowDelegate<TModel> beforeShow = null,
            WindowShowDelegate<TModel> alreadyShown = null)
            where TView : IView
            where TModel : IModel;

        void HideWindow(IModel model);
        void HideAllWindows();
    }
}