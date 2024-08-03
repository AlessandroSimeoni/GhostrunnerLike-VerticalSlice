using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerState : MonoBehaviour, IState
    {
        protected PlayerStateController controller = null;

        public virtual void Init(AStateController controller) => this.controller = (PlayerStateController)controller;
        public virtual async UniTask Enter() => await UniTask.NextFrame();
        public virtual async UniTask Exit() => await UniTask.NextFrame();
        public virtual void Tick() { }
    }
}
