using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerState : MonoBehaviour, IState
    {
        protected PlayerCharacter player = null;
        protected PlayerStateController controller = null;

        public virtual void Init(PlayerCharacter player, PlayerStateController controller)
        {
            this.player = player;
            this.controller = controller;
        }
        public void Init(AStateController controller) { }

        public virtual async UniTask Enter() => await UniTask.NextFrame();
        public virtual async UniTask Exit() => await UniTask.NextFrame();
        public virtual void Tick() { }
    }
}
