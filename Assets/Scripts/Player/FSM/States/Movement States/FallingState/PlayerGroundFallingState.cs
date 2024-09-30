using UnityEngine;

namespace Player
{
    public class PlayerGroundFallingState : BaseFallingState
    {
        [SerializeField] private PlayerMovingStateModel movingStateModel = null;

        public override void Tick()
        {
            base.Tick();
            player.characterController.Move(player.inputMovementDirection * movingStateModel.movementSpeed * Time.deltaTime);
        }
    }
}
