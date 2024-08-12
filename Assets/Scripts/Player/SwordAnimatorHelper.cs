using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Animator))]
    public class SwordAnimatorHelper : MonoBehaviour
    {
        [SerializeField] private Sword sword = null;

        private void EndAttack() => sword.EndAttack();
        private void OpenComboWindow() => sword.OpenComboWindow();
        private void InvertCutNormal() => sword.InvertCutNormal();
    }
}
