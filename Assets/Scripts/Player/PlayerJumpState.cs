using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerJumpState : PlayerState
    {
        [SerializeField] private IState exitState = null;

        public override UniTask Enter()
        {
            throw new System.NotImplementedException();
            // SALTO

        }


        public override void Tick()
        {
            // TODO: APPENA NON SONO PIU GROUNDED, CAMBIO STATO

            controller.ChangeState(exitState).Forget();

        }
    }
}
