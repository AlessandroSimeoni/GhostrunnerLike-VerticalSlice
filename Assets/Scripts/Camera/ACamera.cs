using UnityEngine;

namespace GameCamera
{
    public abstract class ACamera : MonoBehaviour
    {
        [SerializeField] protected Transform target;

        protected virtual void Start() => Cursor.lockState = CursorLockMode.Locked;

        protected abstract void ProcessMovement();
    }
}
