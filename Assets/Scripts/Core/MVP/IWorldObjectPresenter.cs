namespace Core.MVP
{
    public interface IWorldObjectPresenter : IPresenter
    {
    }

    public interface IWorldObjectPresenter<TView, TModel> : IPresenter<TView, TModel>, IWorldObjectPresenter
        where TView : IView
        where TModel : IModel
    {
    }
}