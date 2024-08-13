using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerBaseAttackState : PlayerState
    {
        [SerializeField] private PlayerState firstAttackState = null;

        private InputAction attackAction = null;

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            attackAction = player.controls.Player.Attack;
        }

        public override void Tick()
        {
            if (attackAction.triggered)
                controller.ChangeState(firstAttackState).Forget();
        }
    }
}
