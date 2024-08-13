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
            transform.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(currentVerticalAngle, target.right) * target.forward, target.up);
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
    }
}
