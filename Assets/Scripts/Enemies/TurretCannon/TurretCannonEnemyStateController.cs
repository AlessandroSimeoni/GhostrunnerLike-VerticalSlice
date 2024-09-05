using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemy
{
    public class TurretCannonEnemyStateController : BaseStateController<EnemyState>
    {
        public virtual void Init(TurretCannonEnemy playerCharacter)
        {
            foreach (EnemyState state in state)
                state.Init(playerCharacter, this);

            ChangeState(initialState).Forget();
        }
    }
}
