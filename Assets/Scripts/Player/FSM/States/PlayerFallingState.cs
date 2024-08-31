using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerFallingState : PlayerState
    {
        [SerializeField] protected PlayerState idleState = null;
        [SerializeField] protected PlayerState movingState = null;
        [SerializeField] protected PlayerState dashState = null;
        [SerializeField] protected PlayerState wallRunState = null;

        protected InputAction dashAction = null; 
        protected bool rightHit = false;
        protected bool leftHit = false;
        protected Vector3 wallDirection = Vector3.zero;
        protected RaycastHit rightWallHitInfo;
        protected RaycastHit leftWallHitInfo;

        public const string IDLE_ANIMATION = "Idle";

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            dashAction = player.controls.Player.Dash;
        }

        public override async UniTask Enter()
        {
            Debug.Log("Entered FALLING STATE");
            player.playerAnimator.SetBool(IDLE_ANIMATION, true);
            await UniTask.NextFrame();
        }

        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(IDLE_ANIMATION, false);

            if (controller.nextTargetState == wallRunState)
                ((PlayerWallRunState)wallRunState).rightSide = (rightHit && IsVerticalWall(rightWallHitInfo));

            await UniTask.NextFrame();
        }

        public override void Tick()
        {
            CheckDashAction();
            CheckGround();
            CheckWallRun();
            ((PlayerMovementStateController)controller).characterController.Move(player.movementDirection * player.playerModel.movementSpeed * Time.deltaTime);
        }

        protected void CheckGround()
        {
            if (player.groundCheck.isGrounded)
                controller.ChangeState(player.movementDirection == Vector3.zero ? idleState : movingState).Forget();
        }

        protected void CheckDashAction()
        {
            if (dashAction.triggered && ((PlayerDashState)dashState).dashReady)
                controller.ChangeState(dashState).Forget();
        }

        protected void CheckWallRun()
        {
            Vector3 wallCheckOrigin = player.transform.position + Vector3.up * player.playerModel.wallRayHeightOffset;
            rightHit = Physics.Raycast(wallCheckOrigin, player.transform.right, out rightWallHitInfo, player.playerModel.wallRayLenght, player.playerModel.wallCheckLayers);
            leftHit = Physics.Raycast(wallCheckOrigin, -player.transform.right, out leftWallHitInfo, player.playerModel.wallRayLenght, player.playerModel.wallCheckLayers);

            if ((rightHit && IsVerticalWall(rightWallHitInfo)) || (leftHit && IsVerticalWall(leftWallHitInfo)))
            {
                wallDirection = Vector3.Cross(player.transform.up, rightHit ? rightWallHitInfo.normal : leftWallHitInfo.normal);
                if (Vector3.Dot(player.transform.forward, wallDirection) < 0)
                    wallDirection *= -1;
            }
            else
                wallDirection = Vector3.zero;

            if (wallDirection != Vector3.zero
                && Vector3.Dot(player.movementDirection, wallDirection) > player.playerModel.forwardDotFallThreshold
                && Vector3.Dot(player.transform.forward, wallDirection) > player.playerModel.forwardDotFallThreshold)
            {
                controller.ChangeState(wallRunState).Forget();
            }
        }

        protected bool IsVerticalWall(RaycastHit hitInfo) => Vector3.Dot(player.transform.up, hitInfo.normal) == 0.0f;
    }
}
