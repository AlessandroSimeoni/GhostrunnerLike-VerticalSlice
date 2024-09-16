using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class Stamina : MonoBehaviour
    {
        [SerializeField] private StaminaModel model = null;
        [Header("UI")]
        [SerializeField] private Slider staminaSlider = null;
        [SerializeField] private Image sliderBGImage = null;
        [SerializeField] private Color sliderDefaultBGColor = Color.black;
        [SerializeField] private Color sliderEmptyBGColor = Color.red;
        [SerializeField] private float sliderBGFlashFrequence = 2.0f;

        public float currentStamina { get; private set; } = 0;

        private bool regenActive = true;

        private void Start()
        {
            currentStamina = model.maxStamina;
            staminaSlider.maxValue = model.maxStamina;
            staminaSlider.value = staminaSlider.maxValue;
            StaminaRegen().Forget();
        }

        private async UniTask StaminaRegen()
        {
            while (true)
            {
                if (currentStamina == 0)
                    await EmptySliderFlash(model.staminaRegenDelayWhenZero);

                if (regenActive && currentStamina < model.maxStamina)
                {
                    currentStamina = Mathf.Clamp(currentStamina + model.staminaRegenSpeed * Time.deltaTime, 0.0f, model.maxStamina);
                    staminaSlider.value = currentStamina;
                }

                await UniTask.NextFrame();
            }
        }

        private async UniTask EmptySliderFlash(float time)
        {
            float currentTime = 0.0f;
            float phase = 0.0f;

            while (currentTime < time)
            {
                phase = (Mathf.Sin(2 * Mathf.PI * sliderBGFlashFrequence * currentTime - Mathf.PI / 2) + 1) / 2;         // - Mathf.PI/2 --> in this way the sin value will start from zero; without this, at time 0, the sin would evaluate to 0.5 once normalized
                sliderBGImage.color = Color.Lerp(sliderDefaultBGColor, sliderEmptyBGColor, phase);
                currentTime += Time.deltaTime;

                await UniTask.NextFrame();
            }

            sliderBGImage.color = sliderDefaultBGColor;
        }

        public void UseStamina(float value)
        {
            currentStamina = Mathf.Clamp(currentStamina - value, 0.0f, staminaSlider.maxValue);
            staminaSlider.value = currentStamina;
        }

        public void PauseRegen() => regenActive = false;
        public void ResumeRegen() => regenActive = true;
    }
}
