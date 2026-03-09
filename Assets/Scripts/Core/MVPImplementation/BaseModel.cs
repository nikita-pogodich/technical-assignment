using Core.MVP;
using Cysharp.Threading.Tasks;

namespace Core.MVPImplementation
{
    public abstract class BaseModel : IModel
    {
        public int UniqueId { get; }

        protected BaseModel(int uniqueId)
        {
            UniqueId = uniqueId;
        }

        public async UniTask InitAsync()
        {
            await OnInit();
        }

        public void Deinit()
        {
            OnDeinit();
        }

        protected virtual async UniTask OnInit()
        {
            await UniTask.CompletedTask;
        }

        protected virtual void OnDeinit()
        {
        }

        public override bool Equals(object obj)
        {
            return obj is BaseModel other && UniqueId == other.UniqueId;
        }

        public override int GetHashCode()
        {
            return UniqueId.GetHashCode();
        }
    }
}