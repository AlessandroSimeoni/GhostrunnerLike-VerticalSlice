using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Architecture
{
    public abstract class AView : MonoBehaviour
    {
        public abstract UniTask ChangeView();
    }
}
