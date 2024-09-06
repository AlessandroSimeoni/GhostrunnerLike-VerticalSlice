using Architecture;
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
        private float cameraTiltAngle = 0.0f;
        private float cameraTiltChangeSpeed = 0.0f;
        private float wallRayHeightOffset = 0.0f;
        private float wallRayLenght = 0.0f;
        private LayerMask wallCheckLayers;
        private float minJumpDirectionAngle = 0.0f;
        private float forwardDotFallThreshold = 0.0f;
        private float wallRunSpeed = 0.0f;

        private const string RUN_ANIMATION = "Run";

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
            jumpAction = player.controls.Player.Jump;
            dashAction = player.controls.Player.Dash;
            cameraTiltAngle = ((PlayerWallRunStateModel)stateModel).cameraTiltAngle;
            cameraTiltChangeSpeed = ((PlayerWallRunStateModel)stateModel).cameraTiltChangeSpeed;
            wallRayHeightOffset = ((PlayerWallRunStateModel)stateModel).wallRayHeightOffset;
            wallRayLenght = ((PlayerWallRunStateModel)stateModel).wallRayLenght;
            wallCheckLayers = ((PlayerWallRunStateModel)stateModel).wallCheckLayers;
            minJumpDirectionAngle = ((PlayerWallRunStateModel)stateModel).minJumpDirectionAngle;
            forwardDotFallThreshold = ((PlayerWallRunStateModel)stateModel).forwardDotFallThreshold;
            wallRunSpeed = ((PlayerWallRunStateModel)stateModel).wallRunSpeed;
        }

        public override async UniTask Enter()
        {
            Debug.Log("Entered WALL RUN STATE");
            player.playerAnimator.SetBool(RUN_ANIMATION, true);
            gravity.enabled = false;

            player.fpCamera.TiltCameraZAxis(rightSide ? cameraTiltAngle : -cameraTiltAngle, cameraTiltChangeSpeed);

            wallCheckOrigin = player.transform.position + Vector3.up * wallRayHeightOffset;
            Physics.Raycast(wallCheckOrigin, rightSide ? player.transform.right : -player.transform.right, out wallHit, wallRayLenght, wallCheckLayers);
            wallNormal = wallHit.normal;

            wallMovementDirection = Vector3.Cross(player.transform.up, wallNormal);
            if (Vector3.Dot(player.transform.forward, wallMovementDirection) < 0)
                wallMovementDirection *= -1;

            await UniTask.NextFrame();
        }

        public override async UniTask Exit()
        {
            player.fpCamera.TiltCameraZAxis(0.0f, cameraTiltChangeSpeed);
            player.playerAnimator.SetBool(RUN_ANIMATION, false);

            if (controller.nextTargetState == parabolicJumpState)
            {
                Vector3 jumpDirection = Quaternion.AngleAxis(rightSide ? -minJumpDirectionAngle : minJumpDirectionAngle, Vector3.up) * wallMovementDirection;
                float jumpDotThreshold = Mathf.Sin(minJumpDirectionAngle);
                
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

            wallCheckOrigin = player.transform.position + Vector3.up * wallRayHeightOffset;

            if (Vector3.Dot(player.movementDirection, wallMovementDirection) < forwardDotFallThreshold
                || Vector3.Dot(player.transform.forward, wallMovementDirection) < forwardDotFallThreshold
                || !Physics.Raycast(wallCheckOrigin, -wallNormal, out wallHit, wallRayLenght, wallCheckLayers))
            {
                controller.ChangeState(idleState).Forget();
            }
            else
            {
                if (jumpAction.triggered)
                    controller.ChangeState(parabolicJumpState).Forget();

                if(dashAction.triggered)
                    controller.ChangeState(dashState).Forget();

                player.characterController.Move(wallMovementDirection * wallRunSpeed * Time.deltaTime);
            }
        }
    }
}
