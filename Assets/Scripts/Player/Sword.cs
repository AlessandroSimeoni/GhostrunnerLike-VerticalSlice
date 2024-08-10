using UnityEngine;

namespace Player
{
    public class Sword : MonoBehaviour
    {
        public delegate void AttackEvent();
        public event AttackEvent OnAttackEnded;
        public event AttackEvent OnComboWindow;

        private void EndAttack() => OnAttackEnded?.Invoke();

        private void OpenComboWindow() => OnComboWindow?.Invoke();
    }
}
