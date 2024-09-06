using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerState : MonoBehaviour, IState
    {
        [SerializeField] protected ScriptableObject stateModel = null;
        protected PlayerCharacter player = null;
        protected PlayerStateController controller = null;

        public virtual void Init<T>(T entity, AStateController controller) where T : MonoBehaviour
        {
            player = entity as PlayerCharacter;
            this.controller = (PlayerStateController)controller;
        }

        public virtual async UniTask Enter() => await UniTask.NextFrame();
        public virtual async UniTask Exit() => await UniTask.NextFrame();
        public virtual void Tick() { }
    }
}
