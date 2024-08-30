using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
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
        private Ray ray;
        private RaycastHit wallHit;
        private Vector3 wallMovementDirection = Vector3.zero;
        private Vector3 wallCheckOrigin = Vector3.zero;
        private InputAction jumpAction = null;
        private InputAction dashAction = null;

        public override async UniTask Enter()
        {
            Debug.Log("Entered WALL RUN STATE");

            jumpAction = player.controls.Player.Jump;
            dashAction = player.controls.Player.Dash;

            wallCheckOrigin = player.transform.position + Vector3.up * player.playerModel.wallRayHeightOffset;
            ray = new Ray(wallCheckOrigin, rightSide ? player.transform.right : -player.transform.right);
            Physics.Raycast(ray, out wallHit, player.playerModel.wallRayLenght, player.playerModel.wallCheckLayers);

            wallMovementDirection = Vector3.Cross(player.transform.up, wallHit.normal);
            if (Vector3.Dot(player.transform.forward, wallMovementDirection) < 0)
                wallMovementDirection *= -1;

            await UniTask.NextFrame();
        }

        public override async UniTask Exit()
        {
            if (controller.nextTargetState == parabolicJumpState)
                ((PlayerParabolicJumpState)parabolicJumpState).jumpDirection = player.transform.forward;
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
            ray = new Ray(wallCheckOrigin, rightSide ? player.transform.right : -player.transform.right);

            if (player.movementDirection == Vector3.zero
                || Vector3.Dot(player.movementDirection, wallMovementDirection) < player.playerModel.forwardDotFallThreshold
                || Vector3.Dot(player.transform.forward, wallMovementDirection) < player.playerModel.forwardDotFallThreshold
                || !Physics.Raycast(ray, out wallHit, player.playerModel.wallRayLenght, player.playerModel.wallCheckLayers))
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
