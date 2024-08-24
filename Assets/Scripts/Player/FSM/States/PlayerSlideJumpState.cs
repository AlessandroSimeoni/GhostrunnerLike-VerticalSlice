using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;

namespace Player
{
    public class PlayerSlideJumpState : PlayerState
    {
        [SerializeField] private PlayerState idleState = null;
        [SerializeField] private PlayerState movingState = null;
        [SerializeField] private PlayerState slideState = null;
        [SerializeField] private Gravity gravity = null;

        private float initialVelocity = 0.0f;
        private Vector3 jumpVelocity = Vector3.up;

        public const string JUMP_ANIMATION = "Jump";

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            // formula moto parabolico
            initialVelocity = Mathf.Sqrt(2 * player.playerModel.maxSlideJumpHeight * (-1) * Physics.gravity.y * player.playerModel.slidedJumpGravityMultiplier / Mathf.Pow(Mathf.Sin(player.playerModel.slideJumpDegree * Mathf.Deg2Rad),2));
        }

        public override async UniTask Enter()
        {
            Debug.Log("ENTERED SLIDED JUMP STATE");
            await UniTask.NextFrame();
            jumpVelocity = ((PlayerSlideState)slideState).slideDirection * initialVelocity;
            // velocità iniziale verticale necessaria per far sì che il player raggiunga l'altezza massima
            jumpVelocity.y = Mathf.Sqrt(2 * player.playerModel.maxSlideJumpHeight * (-1) * Physics.gravity.y * player.playerModel.slidedJumpGravityMultiplier); 
            gravity.enabled = false;
            player.playerAnimator.SetBool(JUMP_ANIMATION, true);
        }

        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(JUMP_ANIMATION, false);
            await base.Exit();
        }

        public override void Tick()
        {
            jumpVelocity.y += Physics.gravity.y * player.playerModel.slidedJumpGravityMultiplier * Time.deltaTime;
            ((PlayerMovementStateController)controller).characterController.Move(jumpVelocity * Time.deltaTime);

            if (player.groundCheck.isGrounded && jumpVelocity.y < 0)
            {
                jumpVelocity.y = 0.0f;
                gravity.enabled = true;
                controller.ChangeState(player.movementDirection == Vector3.zero ? idleState : movingState).Forget();
            }

            ((PlayerMovementStateController)controller).characterController.Move(player.movementDirection * player.playerModel.slidedMidAirSpeed * Time.deltaTime);
        }
    }
}
