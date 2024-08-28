using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Player
{
    public class BasePlayerJumpState : PlayerState
    {
        [SerializeField] protected PlayerState idleState = null;
        [SerializeField] protected PlayerState movingState = null;
        [SerializeField] protected PlayerState dashState = null;
        [SerializeField] protected Gravity gravity = null;

        protected float verticalVelocity = 0.0f;
        protected InputAction dashAction = null;

        protected RaycastHit rightWallHitInfo;
        protected RaycastHit leftWallHitInfo;

        public const string JUMP_ANIMATION = "Jump";

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            dashAction = player.controls.Player.Dash;
        }

        public override async UniTask Enter()
        {
            await UniTask.NextFrame();
            gravity.enabled = false;
            player.playerAnimator.SetBool(JUMP_ANIMATION, true);
        }

        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(JUMP_ANIMATION, false);
            verticalVelocity = 0.0f;
            gravity.enabled = true;
            await UniTask.NextFrame();
        }

        public override void Tick()
        {
            if (dashAction.triggered && ((PlayerDashState)dashState).dashReady)
            {
                controller.ChangeState(dashState).Forget();
                return;
            }

            if (player.groundCheck.isGrounded && verticalVelocity < 0)
                controller.ChangeState(player.movementDirection == Vector3.zero ? idleState : movingState).Forget();

            //Wall check for wall run state transition
            Vector3 wallCheckOrigin = player.transform.position + Vector3.up * player.playerModel.wallRayHeightOffset;
            Ray rightRay = new Ray(wallCheckOrigin, player.transform.right);
            Ray leftRay = new Ray(wallCheckOrigin, -player.transform.right);
            bool rightHit = Physics.Raycast(rightRay, out rightWallHitInfo, player.playerModel.wallRayLenght, player.playerModel.wallCheckLayers);
            bool leftHit = Physics.Raycast(leftRay, out leftWallHitInfo, player.playerModel.wallRayLenght, player.playerModel.wallCheckLayers);

            if (rightHit && IsVerticalWall(rightWallHitInfo))
            {
                Debug.Log($"MURO VERTICALE A DESTRA: {rightWallHitInfo.transform.name}");
                // TODO: CAMBIO STATO A WALL RUN DESTRO
            }

            if (leftHit && IsVerticalWall(leftWallHitInfo))
            {
                Debug.Log($"MURO VERTICALE A SINISTRA: {leftWallHitInfo.transform.name}");
                // TODO: CAMBIO STATO A WALL RUN SINISTRO
            }
        }

        private bool IsVerticalWall(RaycastHit hitInfo) => Vector3.Dot(player.transform.up, hitInfo.normal) == 0.0f;
    }
}
