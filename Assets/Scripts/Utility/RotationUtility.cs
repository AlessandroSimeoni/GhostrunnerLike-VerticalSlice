using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Utility
{
	public static class RotationUtility
	{
		/// <summary>
		/// Rotate the target transform to the target rotation with speed degrees per second
		/// </summary>
		/// <param name="targetTransform">the transform to rotate</param>
		/// <param name="targetRotation">the target rotation</param>
		/// <param name="speed">the speed [degrees/second]</param>
		/// <param name="token">the cancellation token which can be used to interrupt the task</param>
		public static async UniTask RotateTo(Transform targetTransform, Vector3 targetRotation, float speed, CancellationToken token)
		{
			bool cancelled = false;
            float degreeIncrement = 0.0f;
			Quaternion targetQuaternionRotation;

            targetRotation = new Vector3(NormalizeTargetAngle(targetRotation.x, targetTransform.eulerAngles.x),
										 NormalizeTargetAngle(targetRotation.y, targetTransform.eulerAngles.y),
										 NormalizeTargetAngle(targetRotation.z, targetTransform.eulerAngles.z));

			targetQuaternionRotation = Quaternion.Euler(targetRotation);

            while (true)
			{
				if (token.IsCancellationRequested)
				{
					cancelled = true;
					break;
				}

				degreeIncrement = speed * Time.deltaTime;
				targetTransform.rotation = Quaternion.RotateTowards(targetTransform.rotation, targetQuaternionRotation, degreeIncrement);
				if (targetTransform.rotation == targetQuaternionRotation)
					break;

				await UniTask.NextFrame();
			}

			if (!cancelled)
				targetTransform.rotation = targetQuaternionRotation;
		}

        /// <summary>
        /// Constantly rotates the rotatingTransform towards the targetTransform
        /// </summary>
        /// <param name="rotatingTransform">the transform to rotate</param>
        /// <param name="targetTransform">the target transform to look at</param>
        /// <param name="targetOffset">a target offset</param>
        /// <param name="speed">rotating speed [degrees/second]</param>
        /// <param name="token">the cancellation token to quit the rotation</param>
        /// <param name="xRotation">rotation constraint on x axis: true (by default) to rotate around this axis, false to block rotation around this axis</param>
        /// <param name="yRotation">rotation constraint on y axis: true (by default) to rotate around this axis, false to block rotation around this axis</param>
        /// <param name="zRotation">rotation constraint on z axis: true (by default) to rotate around this axis, false to block rotation around this axis</param>
        public static async UniTask FollowTarget(Transform rotatingTransform, Transform targetTransform, Vector3 targetOffset, float speed, CancellationToken token, bool xRotation = true, bool yRotation = true, bool zRotation = true)
		{
			Vector3 axisConstraint = new Vector3(xRotation ? 1.0f : 0.0f, yRotation ? 1.0f : 0.0f, zRotation ? 1.0f : 0.0f);
            float degreeIncrement = 0.0f;
			Vector3 targetDirection;
			Quaternion targetRotation;

            while (true)
			{
                if (token.IsCancellationRequested)
                    break;

                degreeIncrement = speed * Time.deltaTime;
                targetDirection = (targetTransform.position + targetOffset) - rotatingTransform.position;
                targetRotation = Quaternion.LookRotation(targetDirection, rotatingTransform.up);

                targetRotation = Quaternion.Euler(new Vector3(xRotation ? targetRotation.eulerAngles.x : rotatingTransform.rotation.eulerAngles.x,
															  yRotation ? targetRotation.eulerAngles.y : rotatingTransform.rotation.eulerAngles.y,
															  zRotation ? targetRotation.eulerAngles.z : rotatingTransform.rotation.eulerAngles.z));

                rotatingTransform.rotation = Quaternion.RotateTowards(rotatingTransform.rotation, targetRotation, degreeIncrement);

                await UniTask.NextFrame();
            }
		}

        public static async UniTask FollowCharacterController(Transform rotatingTransform, Transform targetTransform, CharacterController charController, float offsetFromCharacterHeight, float speed, CancellationToken token, bool xRotation = true, bool yRotation = true, bool zRotation = true)
        {
            Vector3 axisConstraint = new Vector3(xRotation ? 1.0f : 0.0f, yRotation ? 1.0f : 0.0f, zRotation ? 1.0f : 0.0f);
            float degreeIncrement = 0.0f;
            Vector3 targetDirection;
            Quaternion targetRotation;

            while (true)
            {
                if (token.IsCancellationRequested)
                    break;

                degreeIncrement = speed * Time.deltaTime;
                targetDirection = targetTransform.position + Vector3.up * (charController.height + offsetFromCharacterHeight) - rotatingTransform.position;
                targetRotation = Quaternion.LookRotation(targetDirection, rotatingTransform.up);

                targetRotation = Quaternion.Euler(new Vector3(xRotation ? targetRotation.eulerAngles.x : rotatingTransform.rotation.eulerAngles.x,
                                                              yRotation ? targetRotation.eulerAngles.y : rotatingTransform.rotation.eulerAngles.y,
                                                              zRotation ? targetRotation.eulerAngles.z : rotatingTransform.rotation.eulerAngles.z));

                rotatingTransform.rotation = Quaternion.RotateTowards(rotatingTransform.rotation, targetRotation, degreeIncrement);

                await UniTask.NextFrame();
            }
        }

        /// <summary>
        /// normalize the target angle so that the rotation follow the shortest side (left or right rotation)
        /// </summary>
        /// <param name="targetAngle">the target angle</param>
        /// <param name="initialEulerAngle">the current euler angle</param>
        /// <returns>the normalized target angle</returns>
        private static float NormalizeTargetAngle(float targetAngle, float initialEulerAngle)
		{
			if (targetAngle - initialEulerAngle > 180)
				return targetAngle -= 360;

			if (initialEulerAngle - targetAngle > 180)
				return targetAngle += 360;

			return targetAngle;
		}
	}
}
