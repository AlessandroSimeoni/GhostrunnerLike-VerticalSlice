using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class BaseFallingState : PlayerState
    {        
        [SerializeField] private PlayerWallRunStateModel wallRunStateModel = null;
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
        protected bool canDash = true;

        public const string IDLE_ANIMATION = "Idle";

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
            dashAction = player.controls.Player.Dash;
        }

        public override async UniTask Enter()
        {
            player.playerAnimator.SetBool(IDLE_ANIMATION, true);
            canDash = controller.previousState != dashState;
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
        }

        protected void CheckGround()
        {
            if (player.groundCheck.isGrounded)
                controller.ChangeState(player.inputMovementDirection == Vector3.zero ? idleState : movingState).Forget();
        }

        protected void CheckDashAction()
        {
            if (canDash && dashAction.triggered && player.stamina.currentStamina > 0)
            {
                canDash = false;
                controller.ChangeState(dashState).Forget();
            }
        }

        protected void CheckWallRun()
        {
            Vector3 wallCheckOrigin = player.transform.position + Vector3.up * wallRunStateModel.wallRayHeightOffset;
            rightHit = Physics.Raycast(wallCheckOrigin, player.transform.right, out rightWallHitInfo, wallRunStateModel.wallRayLenght, wallRunStateModel.wallCheckLayers);
            leftHit = Physics.Raycast(wallCheckOrigin, -player.transform.right, out leftWallHitInfo, wallRunStateModel.wallRayLenght, wallRunStateModel.wallCheckLayers);

            if ((rightHit && IsVerticalWall(rightWallHitInfo)) || (leftHit && IsVerticalWall(leftWallHitInfo)))
            {
                wallDirection = Vector3.Cross(player.transform.up, rightHit ? rightWallHitInfo.normal : leftWallHitInfo.normal);
                if (Vector3.Dot(player.transform.forward, wallDirection) < 0)
                    wallDirection *= -1;
            }
            else
                wallDirection = Vector3.zero;

            if (wallDirection != Vector3.zero
                && Vector3.Dot(player.inputMovementDirection, wallDirection) > wallRunStateModel.forwardDotFallThreshold
                && Vector3.Dot(player.transform.forward, wallDirection) > wallRunStateModel.forwardDotFallThreshold)
            {
                controller.ChangeState(wallRunState).Forget();
            }
        }

        protected bool IsVerticalWall(RaycastHit hitInfo) => Vector3.Dot(player.transform.up, hitInfo.normal) == 0.0f;
    }
}
