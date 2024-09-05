using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemy
{
    public class EnemyState : MonoBehaviour, IState
    {
        protected BaseEnemy enemy = null;
        protected BaseStateController<EnemyState> controller = null;

        public virtual void Init(BaseEnemy enemy, BaseStateController<EnemyState> controller)
        {
            this.enemy = enemy;
            this.controller = controller;
        }
        public void Init(AStateController controller) { }
        public virtual async UniTask Enter() => await UniTask.NextFrame();
        public virtual async UniTask Exit() => await UniTask.NextFrame();

        public void Tick() { }
    }
}
