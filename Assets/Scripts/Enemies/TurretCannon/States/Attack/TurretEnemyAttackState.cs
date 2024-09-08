using Architecture;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Utility;

namespace Enemy
{
    public class TurretEnemyAttackState : TurretEnemyState
    {
        [SerializeField] private TurretEnemyAttackStateModel attackModel = null;
        [SerializeField] private TurretEnemyState idleState = null;
        [SerializeField] private Transform upperBlockTransform = null;
        [SerializeField] private Transform barrelTransform = null;
        [SerializeField] private Transform shootPointTransform = null;

        private CancellationTokenSource rotationCTS = new CancellationTokenSource();
        private Vector3 targetDirection = Vector3.zero;
        private RaycastHit hit;
        private float firePeriod = 0.0f;
        private float currentTime = 0.0f;

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
            firePeriod = 1.0f / attackModel.fireFrequency;
        }

        public override async UniTask Enter()
        {
            await UniTask.NextFrame();
            enemy.OnPlayerNotInRange += ReturnToIdle;
            Debug.Log("Enemy ATTACK STATE");
            rotationCTS = new CancellationTokenSource();
            RotationUtility.FollowTarget(upperBlockTransform, enemy.player.transform, Vector3.zero, attackModel.rotationSpeed, rotationCTS.Token, false, true, false).Forget();
            RotationUtility.FollowCharacterController(barrelTransform, enemy.player.transform, enemy.player.characterController, attackModel.barrelTargetOffsetHeight, attackModel.rotationSpeed, rotationCTS.Token, true, false, false).Forget();
        }

        public override async UniTask Exit()
        {
            await UniTask.NextFrame();
            rotationCTS.Cancel();
            enemy.OnPlayerNotInRange -= ReturnToIdle;
        }

        public override void Tick()
        {
            targetDirection = (enemy.player.transform.position + Vector3.up * (enemy.player.characterController.height + attackModel.barrelTargetOffsetHeight)) - enemy.transform.position;

            if (Vector3.Dot(barrelTransform.forward, targetDirection.normalized) > attackModel.fireAlignmentThreshold
                && Physics.Raycast(shootPointTransform.position, shootPointTransform.forward, out hit, targetDirection.magnitude)
                && hit.transform.gameObject.layer == LayerMask.NameToLayer(TurretCannonEnemy.PLAYER_LAYER))
            {
                currentTime += Time.deltaTime;
                if (currentTime >= firePeriod)
                {
                    currentTime -= firePeriod;
                    enemy.FireBullet();
                }
            }
        }

        private void ReturnToIdle() => controller.ChangeState(idleState).Forget();
    }
}
