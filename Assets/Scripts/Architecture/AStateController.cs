using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Architecture
{
    public abstract class AStateController : MonoBehaviour
    {
        protected abstract void Init();
        public abstract UniTask ChangeState(IState state);
    }
}
