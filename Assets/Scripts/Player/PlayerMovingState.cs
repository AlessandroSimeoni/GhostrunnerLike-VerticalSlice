using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerMovingState : PlayerState
    {
        [SerializeField] private PlayerState idleState = null;
        [SerializeField] private PlayerState jumpState = null;

        public override async UniTask Enter()
        {
            Debug.Log("Entered MOVING STATE");
            await UniTask.NextFrame();
        }

        public override async UniTask Exit() => await UniTask.NextFrame();

        public override void Tick()
        {
            if (controller.movementDirectionBuffer == Vector3.zero)
            {
                controller.ChangeState(idleState).Forget();
                return;
            }

            if (controller.jumpRequest)
            {
                controller.ChangeState(jumpState).Forget();
                return;
            }

            ConsumeMovement();
        }

        private void ConsumeMovement()
        {
            Vector3 direction = controller.movementDirectionBuffer;
            controller.movementDirectionBuffer = Vector3.zero;

            controller.characterController.Move(direction * controller.playerModel.movementSpeed * Time.deltaTime);
        }
    }
}
