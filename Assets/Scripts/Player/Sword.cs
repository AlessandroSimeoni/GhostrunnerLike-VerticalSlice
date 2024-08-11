using MeshCut;
using UnityEngine;

namespace Player
{
    public class Sword : MeshCutter
    {
        [SerializeField] private float cutForce = 25.0f;

        public delegate void AttackEvent();
        public event AttackEvent OnAttackEnded;
        public event AttackEvent OnComboWindow;

        private bool invertCutNormal = false;

        private void OnTriggerEnter(Collider other)
        {
            Sliceable target = other.GetComponent<Sliceable>();
            if (target != null && target.cutReady)
            {
                target.cutReady = false;
                Cut(target, invertCutNormal ? -transform.forward : transform.forward, transform.position, cutForce);
                Debug.Log("Colpito");
            }
        }

        private void EndAttack()
        {
            invertCutNormal = false;
            OnAttackEnded?.Invoke();
        }
        private void OpenComboWindow() => OnComboWindow?.Invoke();
        private void InvertCutNormal() => invertCutNormal = true;
    }
}
