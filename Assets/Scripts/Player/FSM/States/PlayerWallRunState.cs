using Codice.CM.Client.Differences;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Player
{
    public class PlayerWallRunState : PlayerState
    {
        [SerializeField] private PlayerState idleState = null;
        [SerializeField] private PlayerState parabolicJumpState = null;
        [SerializeField] private PlayerState dashState = null;
        [SerializeField] protected Gravity gravity = null;

        public bool rightSide { get; set; } = true;
        private RaycastHit wallHit;
        private Vector3 wallMovementDirection = Vector3.zero;
        private Vector3 wallCheckOrigin = Vector3.zero;
        private Vector3 wallNormal = Vector3.zero;
        private InputAction jumpAction = null;
        private InputAction dashAction = null;

        private const string RUN_ANIMATION = "Run";

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            jumpAction = player.controls.Player.Jump;
            dashAction = player.controls.Player.Dash;
        }

        public override async UniTask Enter()
        {
            Debug.Log("Entered WALL RUN STATE");
            player.playerAnimator.SetBool(RUN_ANIMATION, true);
            gravity.enabled = false;

            player.fpCamera.TiltCameraZAxis(rightSide ? player.playerModel.cameraTiltAngle : -player.playerModel.cameraTiltAngle, player.playerModel.cameraTiltChangeSpeed);

            wallCheckOrigin = player.transform.position + Vector3.up * player.playerModel.wallRayHeightOffset;
            Physics.Raycast(wallCheckOrigin, rightSide ? player.transform.right : -player.transform.right, out wallHit, player.playerModel.wallRayLenght, player.playerModel.wallCheckLayers);
            wallNormal = wallHit.normal;

            wallMovementDirection = Vector3.Cross(player.transform.up, wallNormal);
            if (Vector3.Dot(player.transform.forward, wallMovementDirection) < 0)
                wallMovementDirection *= -1;

            await UniTask.NextFrame();
        }

        public override async UniTask Exit()
        {
            player.fpCamera.TiltCameraZAxis(0.0f, player.playerModel.cameraTiltChangeSpeed);
            player.playerAnimator.SetBool(RUN_ANIMATION, false);

            if (controller.nextTargetState == parabolicJumpState)
            {
                Vector3 jumpDirection = Quaternion.AngleAxis(rightSide ? -player.playerModel.minJumpDirectionAngle : player.playerModel.minJumpDirectionAngle, Vector3.up) * wallMovementDirection;
                float jumpDotThreshold = Mathf.Sin(player.playerModel.minJumpDirectionAngle);
                
                if (Vector3.Dot(player.transform.forward, wallMovementDirection) < jumpDotThreshold
                    && Vector3.Dot(player.transform.forward, wallNormal) > 0)
                {
                    jumpDirection = player.transform.forward;
                }

                ((PlayerParabolicJumpState)parabolicJumpState).jumpDirection = jumpDirection;
            }
            else
                gravity.enabled = true;

            await UniTask.NextFrame();
        }

        public override void Tick()
        {
            /*
             player will fall if one of the following conditions occur:
                a) no input movement
                b) input movement is not in the same direction of the wall movement direction (considering the forwardDotFallThreshold)
                c) player forward direction is not in the same direction of the wall movement direction (considering the forwardDotFallThreshold)
                d) wall ends
             */

            wallCheckOrigin = player.transform.position + Vector3.up * player.playerModel.wallRayHeightOffset;

            if (Vector3.Dot(player.movementDirection, wallMovementDirection) < player.playerModel.forwardDotFallThreshold
                || Vector3.Dot(player.transform.forward, wallMovementDirection) < player.playerModel.forwardDotFallThreshold
                || !Physics.Raycast(wallCheckOrigin, -wallNormal, out wallHit, player.playerModel.wallRayLenght, player.playerModel.wallCheckLayers))
            {
                controller.ChangeState(idleState).Forget();
            }
            else
            {
                if (jumpAction.triggered)
                    controller.ChangeState(parabolicJumpState).Forget();

                if(dashAction.triggered)
                    controller.ChangeState(dashState).Forget();

                ((PlayerMovementStateController)controller).characterController.Move(wallMovementDirection * player.playerModel.wallRunSpeed * Time.deltaTime);
            }
        }
    }
}
