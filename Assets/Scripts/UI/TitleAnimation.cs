using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class TitleAnimation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private float initialCharacterSpacing = -60.0f;
        [SerializeField] private float targetCharacterSpacing = 3.0f;
        [SerializeField] private float speed = 2.0f;


        private void Start() => TextAnimation().Forget();

        private async UniTask TextAnimation()
        {
            float currentSpacing = initialCharacterSpacing;

            while (currentSpacing != targetCharacterSpacing)
            {
                currentSpacing = Mathf.MoveTowards(currentSpacing, targetCharacterSpacing, Time.deltaTime * Mathf.Log(currentSpacing) * speed);
                text.characterSpacing = currentSpacing;
                await UniTask.NextFrame();
            }
            currentSpacing = targetCharacterSpacing;
        }
    }
}
