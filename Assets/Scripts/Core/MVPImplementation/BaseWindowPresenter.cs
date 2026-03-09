using Core.MVP;

namespace Core.MVPImplementation
{
    public abstract class BaseWindowPresenter<TView, TModel> :
        BasePresenter<TView, TModel>,
        IWindowPresenter<TView, TModel>
        where TView : IWindowView
        where TModel : IModel
    {
    }
}