using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerFirstAttackState : PlayerState
    {
        [SerializeField] private PlayerState nextAttackState = null;

        public const string ATTACK_ANIMATION = "Attack1";

        private InputAction attackAction = null;
        private bool comboWindow = false;
        private bool comboTrigger = false;

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            attackAction = player.controls.Player.Attack;
        }

        private void OpenComboWindow() => comboWindow = true;

        private void EndAttack()
        {
            if (comboTrigger)
                controller.ChangeState(nextAttackState).Forget();
            else
                controller.ChangeState(controller.initialState).Forget();
        }

        public override async UniTask Enter()
        {
            ((PlayerAttackStateController)controller).sword.OnAttackEnded += EndAttack;
            ((PlayerAttackStateController)controller).sword.OnComboWindow += OpenComboWindow;
            player.playerAnimator.SetBool(ATTACK_ANIMATION, true);
            await UniTask.NextFrame();
        }

        public override async UniTask Exit()
        {
            ((PlayerAttackStateController)controller).sword.OnAttackEnded -= EndAttack;
            ((PlayerAttackStateController)controller).sword.OnComboWindow -= OpenComboWindow;

            if (!comboTrigger)
                player.playerAnimator.SetBool(ATTACK_ANIMATION, false);

            comboWindow = false;
            comboTrigger = false;
            await UniTask.NextFrame();
        }

        public override void Tick()
        {
            if (!comboWindow)
                return;

            if (attackAction.triggered)
                comboTrigger = true;
        }
    }
}
