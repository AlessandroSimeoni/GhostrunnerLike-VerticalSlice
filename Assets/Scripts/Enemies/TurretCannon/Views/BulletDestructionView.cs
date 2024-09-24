using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemy
{
    public class BulletDestructionView : AView
    {
        [SerializeField] private GameObject bulletDestructionEffectGO = null;
        [SerializeField] private GameObject smokeEffectGO = null;

        public override async UniTask ChangeView()
        {
            bulletDestructionEffectGO.SetActive(true);
            smokeEffectGO.SetActive(true);
            await UniTask.NextFrame();
        }
    }
}
