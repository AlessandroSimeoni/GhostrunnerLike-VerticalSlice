using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerSlideJumpState : BasePlayerJumpState
    {
        [SerializeField] private PlayerState slideState = null;
        [SerializeField] private PlayerState boostedSlideState = null;
        [SerializeField] private bool boostedState = false;

        private float initialVerticalVelocity = 0.0f;
        private float totalTime = 0.0f;
        private float initialHorizontalSpeed = 0.0f;
        private Vector3 jumpDirection = Vector3.zero;
        private Vector3 jumpVector = Vector3.up;
        private float slideJumpRange = 0.0f;
        private float maxSlideJumpHeight = 0.0f;
        private float slidedJumpGravityMultiplier = 0.0f;
        private float slidedMidAirSpeed = 0.0f;
        private float forwardMovementWeight = 0.0f;

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);

            slideJumpRange = boostedState ? player.playerModel.boostedSlideJumpRange : player.playerModel.slideJumpRange;
            maxSlideJumpHeight = boostedState ? player.playerModel.boostedMaxSlideJumpHeight : player.playerModel.maxSlideJumpHeight;
            slidedJumpGravityMultiplier = boostedState ? player.playerModel.boostedSlidedJumpGravityMultiplier : player.playerModel.slidedJumpGravityMultiplier;
            slidedMidAirSpeed = boostedState ? player.playerModel.boostedSlidedMidAirSpeed : player.playerModel.slidedMidAirSpeed;
            forwardMovementWeight = boostedState ? player.playerModel.boostedforwardMovementWeight : player.playerModel.forwardMovementWeight;

            // formule moto parabolico
            initialVerticalVelocity = Mathf.Sqrt(2 * maxSlideJumpHeight * (-1) * Physics.gravity.y * slidedJumpGravityMultiplier);
            totalTime = 2 * initialVerticalVelocity / ((-1) * Physics.gravity.y * slidedJumpGravityMultiplier);
            initialHorizontalSpeed = slideJumpRange / totalTime;
        }

        public override async UniTask Enter()
        {
            Debug.Log("ENTERED SLIDED JUMP STATE");
            await base.Enter();

            verticalVelocity = initialVerticalVelocity;
            jumpDirection = (boostedState ? ((PlayerSlideState)boostedSlideState).slideDirection : ((PlayerSlideState)slideState).slideDirection);
            jumpVector = jumpDirection * initialHorizontalSpeed;
            jumpVector.y = verticalVelocity;
        }

        public override void Tick()
        {
            base.Tick();

            jumpVector.y = verticalVelocity;
            ((PlayerMovementStateController)controller).characterController.Move(jumpVector * Time.deltaTime);
            verticalVelocity += Physics.gravity.y * slidedJumpGravityMultiplier * Time.deltaTime;

            Vector3 forwardMovementDirection = Vector3.Project(player.movementDirection, jumpDirection);
            Vector3 horizontalMovementDirection = player.movementDirection - forwardMovementDirection;
            ((PlayerMovementStateController)controller).characterController.Move((forwardMovementDirection * forwardMovementWeight + horizontalMovementDirection) * slidedMidAirSpeed * Time.deltaTime);
        }
    }
}
