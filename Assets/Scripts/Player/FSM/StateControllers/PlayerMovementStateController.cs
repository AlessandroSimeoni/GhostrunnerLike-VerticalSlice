using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerMovementStateController : BaseStateController<PlayerState>
    {
        [SerializeField] private PlayerGrapplingHookState grapplingHookState = null;

        public override void Init<T1>(T1 entity)
        {
            base.Init(entity);

            grapplingHookState.OnTargetHooked += GoToGrapplingHookState;
        }

        public void HandleGrapplingHookRequest() => grapplingHookState.AnimateLineRenderer().Forget();
        private void GoToGrapplingHookState()
        {
            if (currentState == grapplingHookState)
                currentState = null;

            ChangeState(grapplingHookState).Forget();
        }
    }
}
