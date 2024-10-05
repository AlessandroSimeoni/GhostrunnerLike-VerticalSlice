using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerParabolicJumpState : BasePlayerJumpState
    {
        [SerializeField] private PlayerParabolicJumpStateModel parabolicJumpModel = null;

        public Vector3 jumpDirection { get; set; } = Vector3.zero;
        public ParabolicJumpConfig jumpModel { get; set; } = null;
        
        private float initialVerticalVelocity = 0.0f;
        private float totalTime = 0.0f;
        private float initialHorizontalSpeed = 0.0f;
        private Vector3 jumpVector = Vector3.up;

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
            if (parabolicJumpModel == null)     // act as trampoline jump
                return;

            jumpModel = parabolicJumpModel.jumpModel;
            CalculateJumpValues();
        }

        public override async UniTask Enter()
        {
            await base.Enter();

            if (parabolicJumpModel == null)     // act as trampoline jump
                CalculateJumpValues();

            verticalVelocity = initialVerticalVelocity;
            jumpVector = jumpDirection * initialHorizontalSpeed;
            jumpVector.y = verticalVelocity;
        }

        public override void Tick()
        {
            base.Tick();

            jumpVector.y = verticalVelocity;
            player.characterController.Move(jumpVector * Time.deltaTime);
            verticalVelocity += Physics.gravity.y * jumpModel.jumpGravityMultiplier * Time.deltaTime;

            Vector3 forwardMovementDirection = Vector3.Project(player.inputMovementDirection, jumpDirection);
            Vector3 horizontalMovementDirection = player.inputMovementDirection - forwardMovementDirection;
            player.characterController.Move((forwardMovementDirection * jumpModel.forwardMovementWeight + horizontalMovementDirection) * jumpModel.midAirSpeed * Time.deltaTime);
        }
        private void CalculateJumpValues()
        {
            // formule moto parabolico
            initialVerticalVelocity = Mathf.Sqrt(2 * jumpModel.maxJumpHeight * (-1) * Physics.gravity.y * jumpModel.jumpGravityMultiplier);
            totalTime = 2 * initialVerticalVelocity / ((-1) * Physics.gravity.y * jumpModel.jumpGravityMultiplier);
            initialHorizontalSpeed = jumpModel.jumpRange / totalTime;
        }
    }
}
