using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerCrouchedState : BasePlayerCrouchedState
	{
		[SerializeField] private PlayerState idleState = null;
		[SerializeField] private PlayerState movingState = null;
		[SerializeField] private PlayerState jumpState = null;

		private InputAction jumpAction = null;
		private InputAction crouchAction = null;
		private float speed = 0.0f;

		private const string IDLE_ANIMATION = "Idle";                   // TODO: CREATE CROUCHED ANIMATION

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
			jumpAction = player.controls.Player.Jump;
			crouchAction = player.controls.Player.Crouch;
            speed = ((PlayerCrouchedStateModel)stateModel).crouchedMovementSpeed;
        }

		public override async UniTask Enter()
		{
			Debug.Log("Entered CROUCHED STATE");
			player.playerAnimator.SetBool(IDLE_ANIMATION, true);        // TODO: CREATE CROUCHED ANIMATION

			if (controller.previousState.GetType() != typeof(PlayerSlideState))
				await base.Enter();
		}

		public override async UniTask Exit()
		{
			player.playerAnimator.SetBool(IDLE_ANIMATION, false);       // TODO: CREATE CROUCHED ANIMATION
			await base.Exit();
		}

		public override void Tick()
		{
			if (crouchAction.triggered && player.movementDirection == Vector3.zero)
			{
				controller.ChangeState(idleState).Forget();
				return;
			}

			if (crouchAction.triggered && player.movementDirection != Vector3.zero)
			{
				controller.ChangeState(movingState).Forget();
				return;
			}

			if (jumpAction.triggered)
			{
				controller.ChangeState(jumpState).Forget();
				return;
			}

			player.characterController.Move(player.movementDirection * speed * Time.deltaTime);
		}
	}
}
