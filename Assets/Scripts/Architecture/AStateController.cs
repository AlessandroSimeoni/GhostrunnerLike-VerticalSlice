using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Architecture
{
    public abstract class AStateController : MonoBehaviour
    {
        public abstract void Init<T>(T entity) where T : MonoBehaviour;
        public abstract UniTask ChangeState(IState state);
    }
}
