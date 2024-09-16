using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerBaseCombatState : PlayerState
    {
        [SerializeField] private PlayerState firstAttackState = null;
        [SerializeField] private PlayerState defenseState = null;

        private InputAction attackAction = null;
        

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
            attackAction = player.controls.Player.Attack;
        }

        public override void Tick()
        {
            if (((PlayerCombatStateController)controller).defenseRequested && player.stamina.currentStamina > 0)
                controller.ChangeState(defenseState).Forget();

            if (attackAction.triggered)
                controller.ChangeState(firstAttackState).Forget();
        }
    }
}
