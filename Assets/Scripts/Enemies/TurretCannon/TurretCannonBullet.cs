using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    public class TurretCannonBullet : MonoBehaviour
    {
        public Rigidbody rb { get; private set; } = null;
        public IObjectPool<TurretCannonBullet> objectPool { get; set; } = null;

        private Coroutine flyTimeCoroutine = null;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void StartFlyTime(float maxFlyTime) => flyTimeCoroutine = StartCoroutine(MaxFlyTimeCoroutine(maxFlyTime));

        private IEnumerator MaxFlyTimeCoroutine(float maxFlyTime)
        {
            yield return new WaitForSeconds(maxFlyTime);
            ReleaseBulletToPool();
        }

        private void ReleaseBulletToPool()
        {
            rb.velocity = new Vector3(0f, 0f, 0f);
            objectPool.Release(this);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (flyTimeCoroutine != null)
                StopCoroutine(flyTimeCoroutine);

            //TODO: gestire eventuali danni al giocatore

            ReleaseBulletToPool();
        }
    }
}
