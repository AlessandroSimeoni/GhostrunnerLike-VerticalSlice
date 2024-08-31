using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace GameCamera
{
    public class FirstPersonCamera : ACamera
    {
        [Header("Position")]
        public float heightOffset = 1.75f;
        [SerializeField] private float depthOffset = 0.0f;
        [Header("Axis")]
        [SerializeField] private bool invertedVerticalAxis = false;
        [SerializeField] private bool invertedHorizontalAxis = false;
        [Header("Vision Angle")]
        [SerializeField, Range(0.0f, 89.0f)] private float maxLookUpAngle = 75.0f;
        [SerializeField, Range(0.0f, 89.0f)] private float maxLookDownAngle = 75.0f;
        [Header("Sensitivities")]
        [SerializeField, Min(0.0f)] private float verticalSensitivity = 2.0f;
        [SerializeField, Min(0.0f)] private float horizontalSensitivity = 2.0f;

        private float currentVerticalAngle = 0.0f;
        private float cameraZRotation = 0.0f;
        private CancellationTokenSource cameraZRotationCTS = new CancellationTokenSource();

        private void LateUpdate() => SetPosition();

        public override void ProcessMovement(Vector3 input)
        {
            AddVerticalAngle(input.y);
            SetRotation(input.x);
        }

        private void SetRotation(float deltaAngle)
        {
            float horizontalDeltaRotation = (invertedHorizontalAxis ? -deltaAngle : deltaAngle) * horizontalSensitivity;
            target.forward = Quaternion.AngleAxis(horizontalDeltaRotation, target.up) * target.forward;
            transform.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(currentVerticalAngle, target.right) * target.forward, target.up) * Quaternion.Euler(0.0f, 0.0f, cameraZRotation);
        }

        private void AddVerticalAngle(float deltaAngle)
        {
            float desiredVerticalAngle = currentVerticalAngle + (invertedVerticalAxis ? -deltaAngle : deltaAngle) * verticalSensitivity;
            currentVerticalAngle = Mathf.Clamp(desiredVerticalAngle, -maxLookUpAngle, maxLookDownAngle);
        }

        private void SetPosition()
        {
            transform.position = target.transform.position + target.transform.up * heightOffset + target.transform.forward * depthOffset;
        }

        public void TiltCameraZAxis(float tiltValue, float speed)
        {
            cameraZRotationCTS.Cancel();
            cameraZRotationCTS.Dispose();
            cameraZRotationCTS = new CancellationTokenSource();
            LerpZAxisCameraRotation(tiltValue, speed, cameraZRotationCTS.Token).Forget();
        }

        private async UniTask LerpZAxisCameraRotation(float tiltValue, float speed, CancellationToken token)
        {
            bool cancelled = false;
            float initialZRotation = transform.eulerAngles.z;   // returns a value between 0 and 360. es: if on the editor the rotation around z axis is -15, this return 345
            float targetZRotation = tiltValue;
            float lerp = 0.0f;

            // normalize angle, otherwise it can cause a 360 spin
            if (targetZRotation - initialZRotation > 180)
                targetZRotation -= 360;
            else if (initialZRotation - targetZRotation > 180)
                targetZRotation += 360;

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    cancelled = true;
                    break;
                }

                lerp += speed * Time.deltaTime;
                cameraZRotation = Mathf.Lerp(initialZRotation, targetZRotation, lerp);
                if (cameraZRotation == tiltValue)
                    break;

                await UniTask.NextFrame();

            }

            if (!cancelled)
                cameraZRotation = tiltValue;
        }
    }
}
