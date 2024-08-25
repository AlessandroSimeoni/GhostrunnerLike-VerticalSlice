using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerSlideJumpState : BasePlayerJumpState
    {
        [SerializeField] private PlayerState slideState = null;

        private float initialVelocity = 0.0f;
        private Vector3 jumpVector = Vector3.up;

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            // formula moto parabolico
            initialVelocity = Mathf.Sqrt(2 * player.playerModel.maxSlideJumpHeight * (-1) * Physics.gravity.y * player.playerModel.slidedJumpGravityMultiplier / Mathf.Pow(Mathf.Sin(player.playerModel.slideJumpDegree * Mathf.Deg2Rad),2));
        }

        public override async UniTask Enter()
        {
            Debug.Log("ENTERED SLIDED JUMP STATE");
            await base.Enter();
            jumpVector = ((PlayerSlideState)slideState).slideDirection * initialVelocity;
            // velocità iniziale verticale necessaria per far sì che il player raggiunga l'altezza massima
            velocity = Mathf.Sqrt(2 * player.playerModel.maxSlideJumpHeight * (-1) * Physics.gravity.y * player.playerModel.slidedJumpGravityMultiplier); 
        }

        public override void Tick()
        {
            base.Tick();

            jumpVector.y = velocity;
            ((PlayerMovementStateController)controller).characterController.Move(jumpVector * Time.deltaTime);
            velocity += Physics.gravity.y * player.playerModel.slidedJumpGravityMultiplier * Time.deltaTime;
            
            ((PlayerMovementStateController)controller).characterController.Move(player.movementDirection * player.playerModel.slidedMidAirSpeed * Time.deltaTime);
        }
    }
}
