using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Utilities;

namespace Enemy
{
    public class TurretEnemyIdleState : TurretEnemyState
    {
        [SerializeField] private TurretEnemyIdleStateModel idleStateModel = null;
        [SerializeField] private TurretEnemyState attackState = null;
        [SerializeField] private Transform upperBlockTransform = null;
        [SerializeField] private Transform barrelTransform = null;

        private CancellationTokenSource rotationCTS = new CancellationTokenSource();

        public override async UniTask Enter()
        {
            await UniTask.NextFrame();
            enemy.OnPlayerRange += GoToAttackState;
            RotationSequence().Forget();
        }

        public override async UniTask Exit()
        {
            rotationCTS.Cancel();
            enemy.OnPlayerRange -= GoToAttackState;
            await UniTask.NextFrame();
        }

        private async UniTask RotationSequence()
        {
            rotationCTS = new CancellationTokenSource();
            await RotationUtility.RotateTo(upperBlockTransform, new Vector3(0.0f, idleStateModel.idleRotation.y, 0.0f), idleStateModel.toIdleRotationSpeed, rotationCTS.Token);
            RotationUtility.RotateTo(barrelTransform, new Vector3(idleStateModel.idleRotation.x, 0.0f, 0.0f), idleStateModel.toIdleRotationSpeed, rotationCTS.Token).Forget();
        }

        private void GoToAttackState() => controller.ChangeState(attackState).Forget();
    }
}
