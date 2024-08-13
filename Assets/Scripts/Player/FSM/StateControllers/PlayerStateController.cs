using Architecture;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Player
{
    public class PlayerStateController : AStateController
    {
        [SerializeField] private PlayerState[] state = Array.Empty<PlayerState>();
        public PlayerState initialState = null;

        private PlayerState currentState = null;
        private bool changingState = false;

        protected  virtual void Update()
        {
            if (currentState == null || changingState)
                return;

            currentState.Tick();
        }

        public virtual void Init(PlayerCharacter playerCharacter)
        {
            foreach (PlayerState state in state)
                state.Init(playerCharacter, this);

            ChangeState(initialState).Forget();
        }

        public override void Init() { }

        public override async UniTask ChangeState(IState state)
        {
            if (currentState == (PlayerState)state || changingState)
                return;

            changingState = true;
            if (currentState != null)
                await currentState.Exit();

            await state.Enter();
            currentState = (PlayerState)state;
            changingState = false;
        }

        public void ChangeState(Type stateType)
        {
            foreach (PlayerState state in state)
            {
                if (state.GetType() == stateType)
                {
                    ChangeState(state).Forget();
                    return;
                }
            }
        }
    }
}
