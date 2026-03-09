using Core.MVP;
using Cysharp.Threading.Tasks;

namespace Core.WindowManager
{
    public interface IWindowFactory
    {
        bool IsAllowMultipleInstances { get; }
        string ViewName { get; }

        UniTask<IWindowPresenter> CreateAsync();
    }
}