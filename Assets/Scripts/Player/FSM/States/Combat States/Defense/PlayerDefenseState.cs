using Architecture;
using Cysharp.Threading.Tasks;
using Projectiles;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerDefenseState : PlayerState
    {
        [SerializeField] private PlayerDefenseModel defenseModel = null;
        [SerializeField] private PlayerState firstAttackState = null;

        public const string DEFENSE_ANIMATION = "Defense";

        private InputAction attackAction = null;
        private bool defending = false;
        private float thresholdCosine = 1.0f;
        private float timeSinceEnter = 0.0f;

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
            attackAction = player.controls.Player.Attack;
            player.OnBulletHit += EvaluateBulletHit;
            thresholdCosine = Mathf.Cos(Mathf.Deg2Rad * defenseModel.defenseDegreeThreshold);
        }

        public override async UniTask Enter()
        {
            player.playerAnimator.SetBool(DEFENSE_ANIMATION, true);
            player.stamina.PauseRegen();
            defending = true;
            timeSinceEnter = 0.0f;
            await UniTask.NextFrame();
        }

        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(DEFENSE_ANIMATION, false);
            player.stamina.ResumeRegen();
            if (player.stamina.currentStamina == 0)
                ((PlayerCombatStateController)controller).CancelDefense();
            defending = false;
            await UniTask.NextFrame();
        }

        public override void Tick()
        {
            timeSinceEnter += Time.deltaTime;

            if (!((PlayerCombatStateController)controller).defenseRequested || player.stamina.currentStamina == 0)
                controller.ChangeState(controller.initialState).Forget();

            if (attackAction.triggered)
                controller.ChangeState(firstAttackState).Forget();
        }

        private void EvaluateBulletHit(Bullet bullet)
        {
            if (defending && Vector3.Dot(player.fpCamera.transform.forward, -bullet.transform.forward) > thresholdCosine)
            {
                if (timeSinceEnter <= defenseModel.parryWindow)
                {
                    float angle = Random.Range(0f, Mathf.PI);
                    float radius = Random.Range(defenseModel.minRepositioningRadius, defenseModel.maxRepositioningRadius);
                    Vector3 randomOffset = player.fpCamera.transform.right * Mathf.Cos(angle) * radius + player.fpCamera.transform.up * Mathf.Sin(angle) * radius;
                    bullet.transform.position += randomOffset;
                    bullet.Parry();
                }
                else
                    player.stamina.ConsumeStamina(bullet.bulletModel.playerStaminaConsume);
            }
            else
                player.Death();
        }
    }
}
