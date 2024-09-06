using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemy
{
    public class TurretEnemyState : MonoBehaviour, IState
    {
        protected TurretCannonEnemy enemy = null;
        protected TurretCannonEnemyStateController controller = null;

        public void Init<T>(T entity, AStateController controller) where T : MonoBehaviour
        {
            enemy = entity as TurretCannonEnemy;
            this.controller = (TurretCannonEnemyStateController)controller;
        }

        public virtual async UniTask Enter() => await UniTask.NextFrame();
        public virtual async UniTask Exit() => await UniTask.NextFrame();
        public void Tick() { }
    }
}
