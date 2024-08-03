using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerIdleState : PlayerState
    {
        [SerializeField] private PlayerState movingState = null;
        [SerializeField] private PlayerState jumpState = null;

        public override async UniTask Enter()
        {
            Debug.Log("Entered IDLE STATE");
            await UniTask.NextFrame();
        }

        public override async UniTask Exit() => await UniTask.NextFrame();

        public override void Tick()
        {
            if (controller.movementDirectionBuffer != Vector3.zero)
            {
                controller.ChangeState(movingState).Forget();
                return;
            }

            if(controller.jumpRequest)
            {
                controller.ChangeState(jumpState).Forget();
                return;
            }
        }
    }
}
