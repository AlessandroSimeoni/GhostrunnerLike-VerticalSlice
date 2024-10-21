using UnityEngine;
using UnityEngine.UI;

namespace GameCamera
{
    public class CameraSensitivitySettings : MonoBehaviour
    {
        [SerializeField] private FirstPersonCamera firstPersonCam = null;
        [SerializeField] private float minSensitivityValue = 0.05f;
        [SerializeField] private Slider verticalSensitivitySlider = null;
        [SerializeField] private Slider horizontalSensitivitySlider = null;

        private const int SENSITIVITY_SLIDER_MAX_VALUE = 10;

        private void Start()
        {
            verticalSensitivitySlider.maxValue = SENSITIVITY_SLIDER_MAX_VALUE;
            verticalSensitivitySlider.value = PlayerPrefs.GetFloat(FirstPersonCamera.VERTICAL_SENSITIVITY, firstPersonCam.verticalSensitivity * SENSITIVITY_SLIDER_MAX_VALUE);
            verticalSensitivitySlider.onValueChanged.AddListener(UpdateVerticalSensitivity);
            firstPersonCam.verticalSensitivity = CalculateCameraSensitivityValue(verticalSensitivitySlider.value);

            horizontalSensitivitySlider.maxValue = SENSITIVITY_SLIDER_MAX_VALUE;
            horizontalSensitivitySlider.value = PlayerPrefs.GetFloat(FirstPersonCamera.HORIZONTAL_SENSITIVITY, firstPersonCam.horizontalSensitivity * SENSITIVITY_SLIDER_MAX_VALUE);
            horizontalSensitivitySlider.onValueChanged.AddListener(UpdateHorizontalSensitivity);
            firstPersonCam.horizontalSensitivity = CalculateCameraSensitivityValue(horizontalSensitivitySlider.value);
        }

        private void UpdateVerticalSensitivity(float value)
        {
            PlayerPrefs.SetFloat(FirstPersonCamera.VERTICAL_SENSITIVITY, value);
            firstPersonCam.verticalSensitivity = CalculateCameraSensitivityValue(value);
        }

        private void UpdateHorizontalSensitivity(float value)
        {
            PlayerPrefs.SetFloat(FirstPersonCamera.HORIZONTAL_SENSITIVITY, value);
            firstPersonCam.horizontalSensitivity = CalculateCameraSensitivityValue(value);
        }

        private float CalculateCameraSensitivityValue(float value)
        {
            float finalValue = value / SENSITIVITY_SLIDER_MAX_VALUE;
            if (finalValue == 0)
                finalValue = minSensitivityValue;

            return finalValue;
        }
    }
}
