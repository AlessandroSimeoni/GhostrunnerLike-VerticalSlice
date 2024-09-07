using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerJumpState : BasePlayerJumpState
    {
        [SerializeField] private PlayerJumpStateModel jumpStateModel = null;
        public override async UniTask Enter()
        {
            await base.Enter();
            verticalVelocity += Mathf.Sqrt(jumpStateModel.jumpHeight * -2.0f * Physics.gravity.y * jumpStateModel.jumpGravityMultiplier);
        }

        public override void Tick()
        {
            base.Tick();

            player.characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
            verticalVelocity += Physics.gravity.y * jumpStateModel.jumpGravityMultiplier * Time.deltaTime;

            player.characterController.Move(player.movementDirection * jumpStateModel.midAirSpeed * Time.deltaTime);
        }
    }
}
