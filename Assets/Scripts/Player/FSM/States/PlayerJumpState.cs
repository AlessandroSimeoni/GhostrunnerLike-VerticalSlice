using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerJumpState : BasePlayerJumpState
    {
        public override async UniTask Enter()
        {
            Debug.Log("ENTERED JUMP STATE");
            await base.Enter();
            verticalVelocity += Mathf.Sqrt(player.playerModel.jumpHeight * -2.0f * Physics.gravity.y * player.playerModel.jumpGravityMultiplier);
        }

        public override void Tick()
        {
            base.Tick();

            ((PlayerMovementStateController)controller).characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
            verticalVelocity += Physics.gravity.y * player.playerModel.jumpGravityMultiplier * Time.deltaTime;

            ((PlayerMovementStateController)controller).characterController.Move(player.movementDirection * player.playerModel.midAirSpeed * Time.deltaTime);
        }
    }
}
