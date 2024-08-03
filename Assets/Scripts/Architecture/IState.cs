using Cysharp.Threading.Tasks;

namespace Architecture
{
    public interface IState
    {
        public void Init(AStateController controller);
        public UniTask Enter();
        public void Tick();
        public UniTask Exit();
    }
}
