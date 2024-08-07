using Architecture;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Utility;

namespace Player
{
    [RequireComponent (typeof(GroundCheck))]
    [RequireComponent (typeof(CharacterController))]
    public class PlayerStateController : AStateController
    {

        [SerializeField] private PlayerState[] state = Array.Empty<PlayerState>();
        [SerializeField] private PlayerState initialState = null;
        public PlayerModel playerModel = null;

        private PlayerState currentState = null;
        private bool changingState = false;

        public GroundCheck groundCheck { get; private set; } = null;
        public CharacterController characterController { get; private set; } = null;
        public Vector3 movementDirectionBuffer {  get; set; } = Vector3.zero;
        public bool jumpRequest { get; set; } = false;

        private void Awake()
        {
            groundCheck = GetComponent<GroundCheck>();
            characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            transform.position += Vector3.up * characterController.skinWidth;
            Init();
        }

        private void Update()
        {
            if (currentState == null || changingState)
                return;

            currentState.Tick();
        }

        protected override void Init()
        {
            foreach (IState state in state)
                state.Init(this);

            ChangeState(initialState).Forget();
        }

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

        /*

        private void ChangeState(Type stateType)
        {
            foreach(AState state in state)
            {
                if (state.GetType() == stateType)
                {
                    ChangeState(state).Forget();
                    return;
                }
            }
        }
         */

        public void AddMovementDirection(Vector3 value) => movementDirectionBuffer += value;

        public void RequestJump()
        {
            if (!groundCheck.isGrounded || currentState.GetType() == typeof(PlayerJumpState))
                return;

            jumpRequest = true;
        }
    }
}
