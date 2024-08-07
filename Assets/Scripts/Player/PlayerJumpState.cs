using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;

namespace Player
{
    public class PlayerJumpState : PlayerState
    {
        [SerializeField] private PlayerState idleState = null;
        [SerializeField] private PlayerState movingState = null;
        [SerializeField] private Gravity gravity = null;

        private float verticalVelocity = 0.0f;

        public override async UniTask Enter()
        {
            Debug.Log("ENTERED JUMP STATE");
            await UniTask.NextFrame();
            verticalVelocity += Mathf.Sqrt(controller.playerModel.jumpHeight * -2.0f * Physics.gravity.y * controller.playerModel.gravityMultiplier);
            gravity.enabled = false;
        }

        public override void Tick()
        {
            if (controller.groundCheck.isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = 0.0f;
                controller.jumpRequest = false;
                gravity.enabled = true;
                controller.ChangeState(controller.movementDirectionBuffer == Vector3.zero ? idleState : movingState).Forget();
            }

            controller.characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
            verticalVelocity += Physics.gravity.y * controller.playerModel.gravityMultiplier * Time.deltaTime;

            MidAirMovement();
        }

        private void MidAirMovement()
        {
            Vector3 direction = controller.movementDirectionBuffer;
            controller.movementDirectionBuffer = Vector3.zero;

            controller.characterController.Move(direction * controller.playerModel.midAirSpeed * Time.deltaTime);
        }
    }
}
