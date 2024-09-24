using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemy
{
    public class CutDestructionView : AView
    {
        [SerializeField] private GameObject cutDestructionEffectGO = null;
        [SerializeField] private GameObject smokeEffectGO = null;

        public override async UniTask ChangeView()
        {
            cutDestructionEffectGO.SetActive(true);
            smokeEffectGO.SetActive(true);
            await UniTask.NextFrame();
        }
    }
}
