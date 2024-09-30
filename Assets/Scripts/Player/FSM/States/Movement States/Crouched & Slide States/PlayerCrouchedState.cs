using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Player
{
	public class PlayerCrouchedState : BasePlayerCrouchedState
	{
		[SerializeField] private PlayerState idleState = null;
		[SerializeField] private PlayerState movingState = null;
		[SerializeField] private PlayerState jumpState = null;
		[SerializeField] private PlayerState slopeSlideState = null;

		private InputAction jumpAction = null;
		private InputAction crouchAction = null;

		private const string IDLE_ANIMATION = "Idle";                   // TODO: CREATE CROUCHED ANIMATION

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
			jumpAction = player.controls.Player.Jump;
			crouchAction = player.controls.Player.Crouch;
        }

		public override async UniTask Enter()
		{
			player.playerAnimator.SetBool(IDLE_ANIMATION, true);        // TODO: CREATE CROUCHED ANIMATION

			if (controller.previousState.GetType() != typeof(PlayerSlideState) && controller.previousState.GetType() != typeof(PlayerSlopeSlideState))
				await base.Enter();
		}

		public override async UniTask Exit()
		{
			player.playerAnimator.SetBool(IDLE_ANIMATION, false);       // TODO: CREATE CROUCHED ANIMATION
            if (controller.nextTargetState.GetType() != typeof(PlayerSlopeSlideState))
                await base.Exit();
		}

		public override void Tick()
		{
			if (crouchAction.triggered && player.groundedMovementDirection == Vector3.zero)
			{
				controller.ChangeState(idleState).Forget();
				return;
			}

			if (crouchAction.triggered && player.groundedMovementDirection != Vector3.zero)
			{
				controller.ChangeState(movingState).Forget();
				return;
			}

			if (jumpAction.triggered)
			{
				controller.ChangeState(jumpState).Forget();
				return;
			}

			if (player.groundedMovementDirection != Vector3.zero && player.groundCheck.IsSlope() && MyUtility.SameDirection(player.groundCheck.groundNormal, player.inputMovementDirection))
			{
                controller.ChangeState(slopeSlideState).Forget();
                return;
            }

			player.characterController.Move(player.groundedMovementDirection * crouchedModel.crouchedMovementSpeed * Time.deltaTime);
		}
	}
}
