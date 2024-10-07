using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Utilities
{
    public class MyTimer : MonoBehaviour
    {
        [SerializeField] private bool playAtStart = false;
        [SerializeField] private TextMeshProUGUI timerTextArea = null;

        public float currentTime { get; private set; } = 0.0f;

        private CancellationTokenSource timerCTS;
        private TimeSpan timeSpan;

        public delegate void TimerState();
        public TimerState OnTimerStart = null;
        public TimerState OnTimerStop = null;

        private void Start()
        {
            DisableTextArea();

            if (!playAtStart)
                return;

            StartTimer();
        }

        public void StartTimer()
        {
            timerCTS = new CancellationTokenSource();
            TimerTask().Forget();
            OnTimerStart?.Invoke();
            timerTextArea.gameObject.SetActive(true);
        }

        public void StopTimer()
        {
            timerCTS.Cancel();
            DisableTextArea();
            OnTimerStop?.Invoke();
        }

        public void DisableTextArea() => timerTextArea.gameObject.SetActive(false);

        private async UniTask TimerTask()
        {
            currentTime = 0.0f;

            while (true)
            {
                if (timerCTS.IsCancellationRequested)
                    break;

                timeSpan = TimeSpan.FromSeconds(currentTime);
                timerTextArea.text = timeSpan.ToString(@"hh\:mm\:ss\:fff");

                currentTime += Time.deltaTime;
                await UniTask.NextFrame();
            }
        }
    }
}
