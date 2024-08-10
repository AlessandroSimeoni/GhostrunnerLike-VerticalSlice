using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Architecture
{
    public abstract class AStateController : MonoBehaviour
    {
        public abstract void Init();
        public abstract UniTask ChangeState(IState state);
    }
}
