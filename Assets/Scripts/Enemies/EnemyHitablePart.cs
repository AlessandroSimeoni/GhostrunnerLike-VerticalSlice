using MeshCut;
using Projectiles;

namespace Enemy
{
    public class EnemyHitablePart : Sliceable, IHitable
    {
        public delegate void HitEvent(Bullet hittingBullet);
        public event HitEvent OnHit = null;

        public void Hit(Bullet hittingBullet) => OnHit?.Invoke(hittingBullet);
    }
}
