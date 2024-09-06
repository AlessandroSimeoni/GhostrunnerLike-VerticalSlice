using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Architecture
{
    public class BaseStateController<T> : AStateController where T : MonoBehaviour, IState
    {
        [SerializeField] protected T[] state = Array.Empty<T>();
        public T initialState = null;
        public T previousState { get; protected set; } = null;
        public T nextTargetState { get; protected set; } = null;

        protected T currentState = null;
        protected bool changingState = false;

        protected virtual void Update()
        {
            if (currentState == null || changingState)
                return;

            currentState.Tick();
        }
        public override void Init<T1>(T1 entity)
        {
            foreach (T state in state)
                state.Init<T1>(entity, this);

            ChangeState(initialState).Forget();
        }

        public override async UniTask ChangeState(IState state)
        {
            if (currentState == (T)state || changingState)
                return;

            changingState = true;
            nextTargetState = (T)state;

            if (currentState != null)
                await currentState.Exit();

            nextTargetState = null;
            previousState = currentState;

            await state.Enter();
            currentState = (T)state;
            changingState = false;
        }
    }
}
