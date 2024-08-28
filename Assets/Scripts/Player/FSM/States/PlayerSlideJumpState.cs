using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerSlideJumpState : BasePlayerJumpState
    {
        [SerializeField] private PlayerState slideState = null;
        [SerializeField] private PlayerState boostedSlideState = null;
        [SerializeField] private bool boostedState = false;

        private float initialVelocity = 0.0f;
        private Vector3 jumpDirection = Vector3.zero;
        private Vector3 jumpVector = Vector3.up;
        private float maxSlideJumpHeight = 0.0f;
        private float slidedJumpGravityMultiplier = 0.0f;
        private float slideJumpDegree = 0.0f;
        private float slidedMidAirSpeed = 0.0f;
        private float forwardMovementWeight = 0.0f;

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);

            maxSlideJumpHeight = boostedState ? player.playerModel.boostedMaxSlideJumpHeight : player.playerModel.maxSlideJumpHeight;
            slidedJumpGravityMultiplier = boostedState ? player.playerModel.boostedSlidedJumpGravityMultiplier : player.playerModel.slidedJumpGravityMultiplier;
            slideJumpDegree = boostedState ? player.playerModel.boostedSlideJumpDegree : player.playerModel.slideJumpDegree;
            slidedMidAirSpeed = boostedState ? player.playerModel.boostedSlidedMidAirSpeed : player.playerModel.slidedMidAirSpeed;
            forwardMovementWeight = boostedState ? player.playerModel.boostedforwardMovementWeight : player.playerModel.forwardMovementWeight;

            // formula moto parabolico
            initialVelocity = Mathf.Sqrt(2 * maxSlideJumpHeight * (-1) * Physics.gravity.y * slidedJumpGravityMultiplier / Mathf.Pow(Mathf.Sin(slideJumpDegree * Mathf.Deg2Rad),2));
        }

        public override async UniTask Enter()
        {
            Debug.Log("ENTERED SLIDED JUMP STATE");
            await base.Enter();

            jumpDirection = (boostedState ? ((PlayerSlideState)boostedSlideState).slideDirection : ((PlayerSlideState)slideState).slideDirection);
            jumpVector = jumpDirection * initialVelocity;

            // velocità iniziale verticale necessaria per far sì che il player raggiunga l'altezza massima
            velocity = Mathf.Sqrt(2 * maxSlideJumpHeight * (-1) * Physics.gravity.y * slidedJumpGravityMultiplier); 
        }

        public override void Tick()
        {
            base.Tick();

            jumpVector.y = velocity;
            ((PlayerMovementStateController)controller).characterController.Move(jumpVector * Time.deltaTime);
            velocity += Physics.gravity.y * slidedJumpGravityMultiplier * Time.deltaTime;

            Vector3 forwardMovementDirection = Vector3.Project(player.movementDirection, jumpDirection);
            Vector3 horizontalMovementDirection = player.movementDirection - forwardMovementDirection;
            ((PlayerMovementStateController)controller).characterController.Move((forwardMovementDirection * forwardMovementWeight + horizontalMovementDirection) * slidedMidAirSpeed * Time.deltaTime);
        }
    }
}
