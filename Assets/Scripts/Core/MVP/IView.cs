namespace Core.MVP
{
    public interface IView
    {
        string ViewName { get; }
        void Init(string viewName);
        void Deinit();
        void SetShown(bool isShown);
    }
}