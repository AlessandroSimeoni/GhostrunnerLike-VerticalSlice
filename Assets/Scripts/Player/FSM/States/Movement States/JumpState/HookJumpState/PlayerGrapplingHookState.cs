using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerGrapplingHookState : BasePlayerJumpState
    {
        [SerializeField] private PlayerGrapplingHookStateModel hookJumpModel = null;
        [SerializeField] private LineRenderer lineRenderer = null;
        [SerializeField] private AnimationCurve lineCurvatureWeightOverLength = null;
        [SerializeField] private Transform lineOriginTransform = null;

        private float damping = 0.0f;
        private Vector3 startingLinePosition = Vector3.zero;

        private Vector3 hookDirection = Vector3.zero;
        private float fallTime = 0.0f;
        private float currentTime = 0.0f;
        private float speed = 0.0f;
        private Vector3 hookPosition = Vector3.zero;

        public delegate void HookedEvent();
        public event HookedEvent OnTargetHooked = null;

        public override async UniTask Enter()
        {
            hookDirection = hookPosition - player.transform.position;
            float hookDistance = hookDirection.magnitude;
            float lerpValue = ((hookDistance - hookJumpModel.minSpeedDistance) < 0 ? 0 : hookDistance - hookJumpModel.minSpeedDistance) / hookJumpModel.maxSpeedDistance;
            speed = Mathf.Lerp(hookJumpModel.minSpeed, hookJumpModel.maxSpeed, lerpValue);
            fallTime = (hookDistance * hookJumpModel.fallRatioOverDistance) / speed;
            currentTime = 0.0f;
            hookDirection = hookDirection.normalized;

            await base.Enter();
        }

        public override void Tick()
        {
            player.characterController.Move(hookDirection * speed * Time.deltaTime);
            currentTime += Time.deltaTime;
            if (currentTime > fallTime)
            {
                hookDirection.y += Physics.gravity.y * hookJumpModel.gravityMultiplier * Time.deltaTime;
                CheckGround();
                CheckWallRun();
            }

            CheckDashAction();

            Vector3 forwardMovementDirection = Vector3.Project(player.inputMovementDirection, hookDirection);
            Vector3 horizontalMovementDirection = player.inputMovementDirection - forwardMovementDirection;
            player.characterController.Move((forwardMovementDirection * hookJumpModel.forwardMovementWeight + horizontalMovementDirection) * hookJumpModel.midAirMovementSpeed * Time.deltaTime);
        }

        public async UniTask AnimateLineRenderer()
        {
            player.controls.Player.GrapplingHook.Disable();
            player.controls.Camera.Disable();

            hookPosition = player.hookTransform.position;
            startingLinePosition = lineOriginTransform.position;
            Vector3 currentPosition = startingLinePosition;
            Vector3 finalPos = player.hookTransform.position;
            Vector3 upDirection = Vector3.Cross(hookPosition - player.transform.position, player.transform.right);
            float time = 0.05f;

            lineRenderer.positionCount = hookJumpModel.lineRendererVertexCount;
            for (int i = 0; i < lineRenderer.positionCount; i++)
                lineRenderer.SetPosition(i, startingLinePosition);

            lineRenderer.enabled = true;

            while (currentPosition != finalPos)
            {
                currentPosition = Vector3.MoveTowards(currentPosition, finalPos, hookJumpModel.lineSpeed * Time.deltaTime);
                startingLinePosition = lineOriginTransform.position;
                for (int i = lineRenderer.positionCount - 1; i >= 0; i--)
                    UpdateLineRendererVertex(i, time, currentPosition, upDirection);

                time += Time.deltaTime;
                await UniTask.NextFrame();
            }

            OnTargetHooked?.Invoke();

            /*
            damping = 1.0f;
            while (damping > 0.001f)
            {
                damping = Mathf.Exp(-waveDamping * time);
                for (int i = 0; i < lineRenderer.positionCount; i++)
                    UpdateLineRendererVertex(i, time, currentPosition, upDirection, damping);

                time += Time.deltaTime * timeSpeed;
                await UniTask.NextFrame();
            }
             */

            RetrieveLine(currentPosition, upDirection).Forget();
            
            player.controls.Camera.Enable();
        }

        private async UniTask RetrieveLine(Vector3 currentPosition, Vector3 upDirection)
        {
            float time = 0.05f;
            while (currentPosition != startingLinePosition)
            {
                damping = Mathf.Exp(-hookJumpModel.waveDamping * time);
                currentPosition = Vector3.MoveTowards(currentPosition, startingLinePosition, hookJumpModel.lineSpeed * Time.deltaTime * hookJumpModel.timeSpeed);

                for (int i = 0; i < lineRenderer.positionCount; i++)
                    UpdateLineRendererVertex(i, time, currentPosition, upDirection, damping);

                time += Time.deltaTime * hookJumpModel.timeSpeed;
                await UniTask.NextFrame();
            }
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;

            player.controls.Player.GrapplingHook.Enable();
        }

        private void UpdateLineRendererVertex(int vertexIndex, float time, Vector3 currentPosition, Vector3 upDirection, float dampingValue = -1.0f)
        {
            float percentage = (float)vertexIndex / (lineRenderer.positionCount - 1);
            Vector3 vertexPosition = Vector3.Lerp(startingLinePosition, currentPosition, percentage);
            float sinWave = hookJumpModel.sinWaveAmplitude * Mathf.Sin(2 * Mathf.PI * percentage * time * hookJumpModel.sinFrequency);
            float cosWave = hookJumpModel.cosWaveAmplitude * Mathf.Cos(2 * Mathf.PI * percentage * time * hookJumpModel.cosFrequency);
            if (dampingValue == -1.0f)
                dampingValue = Mathf.Exp(-hookJumpModel.waveDamping * (1 - percentage));
            float animCurveVal = lineCurvatureWeightOverLength.Evaluate(percentage);
            sinWave *= dampingValue;
            sinWave *= animCurveVal;
            cosWave *= dampingValue;
            cosWave *= animCurveVal;
            vertexPosition += upDirection * sinWave + player.transform.right * cosWave;
            lineRenderer.SetPosition(vertexIndex, vertexPosition);
        }
    }
}
