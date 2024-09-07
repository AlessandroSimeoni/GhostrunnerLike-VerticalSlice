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

        private CancellationTokenSource rotationCTS = new CancellationTokenSource();

        public override async UniTask Enter()
        {
            await UniTask.NextFrame();
            enemy.OnPlayerNotInRange += ReturnToIdle;
            Debug.Log("Enemy ATTACK STATE");
            rotationCTS = new CancellationTokenSource();
            RotationUtility.FollowTarget(upperBlockTransform, enemy.playerTransform, Vector3.zero, attackModel.rotationSpeed, rotationCTS.Token, false, true, false).Forget();
            RotationUtility.FollowTarget(barrelTransform, enemy.playerTransform, Vector3.up * attackModel.barrelTargetOffsetHeight, attackModel.rotationSpeed, rotationCTS.Token, true, false, false).Forget();
        }

        public override async UniTask Exit()
        {
            await UniTask.NextFrame();
            rotationCTS.Cancel();
            enemy.OnPlayerNotInRange -= ReturnToIdle;
        }

        private void ReturnToIdle() => controller.ChangeState(idleState).Forget();
    }
}
