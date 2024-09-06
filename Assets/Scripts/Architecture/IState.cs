using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Architecture
{
    public interface IState
    {
        public void Init<T>(T entity, AStateController controller) where T : MonoBehaviour;
        public UniTask Enter();
        public void Tick();
        public UniTask Exit();
    }
}
