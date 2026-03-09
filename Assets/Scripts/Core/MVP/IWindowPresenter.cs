namespace Core.MVP
{
    public interface IWindowPresenter : IPresenter
    {
    }

    public interface IWindowPresenter<TView, TModel> : IPresenter<TView, TModel>, IWindowPresenter
        where TView : IView
        where TModel : IModel
    {
    }
}