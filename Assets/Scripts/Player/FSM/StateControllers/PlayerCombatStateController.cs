using Architecture;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerCombatStateController : BaseStateController<PlayerState>
    {
        public bool defenseRequested { get; private set; } = false;

        private InputAction defenseAction = null;

        public override void Init<T1>(T1 entity)
        {
            base.Init(entity);

            defenseAction = (entity as PlayerCharacter).controls.Player.Defense;
            defenseAction.performed += DefensePerformed;
            defenseAction.canceled += DefenseCanceled;
        }

        private void DefenseCanceled(InputAction.CallbackContext context) => CancelDefense();

        private void DefensePerformed(InputAction.CallbackContext context) => defenseRequested = true;

        public void CancelDefense() => defenseRequested = false;
    }
}
