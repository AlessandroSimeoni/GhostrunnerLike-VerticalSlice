using Architecture;
using Cysharp.Threading.Tasks;

namespace Enemy
{
    public class TurretCannonEnemyStateController : BaseStateController<TurretEnemyState>
    {
        public TurretEnemyState deathState = null;

        public void Death() => ChangeState(deathState).Forget();
    }
}
