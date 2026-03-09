using Cysharp.Threading.Tasks;

namespace Core.MVP
{
    public interface IModel
    {
        int UniqueId { get; }
        public UniTask InitAsync();
    }
}