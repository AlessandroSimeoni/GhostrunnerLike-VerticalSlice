using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerParabolicJumpState : BasePlayerJumpState
    {
        [SerializeField] private PlayerParabolicJumpStateModel parabolicJumpModel = null;
        [SerializeField] private PlayerState slideState = null;

        public Vector3 jumpDirection { get; set; } = Vector3.zero;

        private float initialVerticalVelocity = 0.0f;
        private float totalTime = 0.0f;
        private float initialHorizontalSpeed = 0.0f;
        private Vector3 jumpVector = Vector3.up;

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);

            // formule moto parabolico
            initialVerticalVelocity = Mathf.Sqrt(2 * parabolicJumpModel.maxSlideJumpHeight * (-1) * Physics.gravity.y * parabolicJumpModel.slidedJumpGravityMultiplier);
            totalTime = 2 * initialVerticalVelocity / ((-1) * Physics.gravity.y * parabolicJumpModel.slidedJumpGravityMultiplier);
            initialHorizontalSpeed = parabolicJumpModel.slideJumpRange / totalTime;
        }

        public override async UniTask Enter()
        {
            Debug.Log("ENTERED SLIDED JUMP STATE");
            await base.Enter();

            verticalVelocity = initialVerticalVelocity;
            if (controller.previousState == slideState)
                jumpDirection = ((PlayerSlideState)slideState).slideDirection;
            jumpVector = jumpDirection * initialHorizontalSpeed;
            jumpVector.y = verticalVelocity;
        }

        public override void Tick()
        {
            base.Tick();

            jumpVector.y = verticalVelocity;
            player.characterController.Move(jumpVector * Time.deltaTime);
            verticalVelocity += Physics.gravity.y * parabolicJumpModel.slidedJumpGravityMultiplier * Time.deltaTime;

            Vector3 forwardMovementDirection = Vector3.Project(player.movementDirection, jumpDirection);
            Vector3 horizontalMovementDirection = player.movementDirection - forwardMovementDirection;
            player.characterController.Move((forwardMovementDirection * parabolicJumpModel.forwardMovementWeight + horizontalMovementDirection) * parabolicJumpModel.slidedMidAirSpeed * Time.deltaTime);
        }
    }
}
