using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Utility;

namespace Enemy
{
    public class TurretEnemyDeathState : TurretEnemyState
    {
        [SerializeField] private TurretEnemyIdleStateModel deathModel = null;
        [SerializeField] private Transform barrelTransform = null;
        [SerializeField] private SphereCollider triggerArea = null;

        private CancellationTokenSource rotationCTS = new CancellationTokenSource();

        public override async UniTask Enter()
        {
            await UniTask.NextFrame();
            RotationUtility.RotateTo(barrelTransform, new Vector3(deathModel.idleRotation.x, barrelTransform.eulerAngles.y, barrelTransform.eulerAngles.z), deathModel.toIdleRotationSpeed, rotationCTS.Token).Forget();
            triggerArea.enabled = false;
            controller.enabled = false;
        }
    }
}
