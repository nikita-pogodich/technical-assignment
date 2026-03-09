namespace Core.MVP
{
    public interface IPresenter
    {
        bool IsShown { get; }
        void SetShown(bool isShown);
        void Deinit();
    }

    public interface IPresenter<TView, TModel> : IPresenter 
        where TView : IView
        where TModel : IModel
    {
        TView View { get; }
        TModel Model { get; }
        void Init(TView view, TModel model);
    }
}