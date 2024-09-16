using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerDefenseState : PlayerState
    {
        [SerializeField] private PlayerState firstAttackState = null;

        public const string DEFENSE_ANIMATION = "Defense";

        private InputAction attackAction = null;

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
            attackAction = player.controls.Player.Attack;
        }

        public override async UniTask Enter()
        {
            player.playerAnimator.SetBool(DEFENSE_ANIMATION, true);
            player.stamina.PauseRegen();
            await UniTask.NextFrame();
        }

        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(DEFENSE_ANIMATION, false);
            player.stamina.ResumeRegen();
            if (player.stamina.currentStamina == 0)
                ((PlayerCombatStateController)controller).CancelDefense();
            await UniTask.NextFrame();
        }

        public override void Tick()
        {
            if (!((PlayerCombatStateController)controller).defenseRequested || player.stamina.currentStamina == 0)
                controller.ChangeState(controller.initialState).Forget();

            if (attackAction.triggered)
                controller.ChangeState(firstAttackState).Forget();
        }
    }
}
