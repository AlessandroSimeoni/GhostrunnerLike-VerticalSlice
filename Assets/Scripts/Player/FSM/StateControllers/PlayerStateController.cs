using Architecture;
using Cysharp.Threading.Tasks;

namespace Player
{
    public class PlayerStateController : BaseStateController<PlayerState>
    {
        public virtual void Init(PlayerCharacter playerCharacter)
        {
            foreach (PlayerState state in state)
                state.Init(playerCharacter, this);

            ChangeState(initialState).Forget();
        }
    }
}
