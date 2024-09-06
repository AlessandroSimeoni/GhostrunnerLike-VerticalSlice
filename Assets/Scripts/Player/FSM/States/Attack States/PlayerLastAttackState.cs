using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerLastAttackState : PlayerState
    {
        private const string ATTACK_ANIMATION = "Attack2";

        private void EndAttack()
        {
            controller.ChangeState(controller.initialState).Forget();
        }

        public override async UniTask Enter()
        {
            player.sword.OnAttackEnded += EndAttack;
            player.playerAnimator.SetBool(ATTACK_ANIMATION, true);
            await UniTask.NextFrame();
        }

        public override async UniTask Exit()
        {
            player.sword.OnAttackEnded -= EndAttack;
            player.playerAnimator.SetBool(PlayerFirstAttackState.ATTACK_ANIMATION, false);
            player.playerAnimator.SetBool(ATTACK_ANIMATION, false);
            await UniTask.NextFrame();
        }
    }
}
