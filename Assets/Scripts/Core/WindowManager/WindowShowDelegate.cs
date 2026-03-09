using Core.MVP;

namespace Core.WindowManager
{
    public delegate void WindowShowDelegate<in TModel>(TModel model) where TModel : IModel;
}