using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerState : MonoBehaviour, IState
    {
        protected PlayerCharacter player = null;
        protected BaseStateController<PlayerState> controller = null;

        public virtual void Init<T>(T entity, AStateController controller) where T : MonoBehaviour
        {
            player = entity as PlayerCharacter;
            this.controller = (BaseStateController<PlayerState>)controller;
        }

        public virtual async UniTask Enter() => await UniTask.NextFrame();
        public virtual async UniTask Exit() => await UniTask.NextFrame();
        public virtual void Tick() { }
    }
}
