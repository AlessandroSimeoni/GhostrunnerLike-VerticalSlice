using System;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class ParkourWinCanvas : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI recordTextArea = null;
        [SerializeField] private TextMeshProUGUI currentTimeTextArea = null;

        private TimeSpan timeSpan;

        public void SetTimesText(float currentScore)
        {
            float record = PlayerPrefs.GetFloat("ParkourTime");
            recordTextArea.text = "Best time: " + TimeSpan.FromSeconds(record).ToString(@"hh\:mm\:ss\:fff");
            currentTimeTextArea.text = "Your time: " + TimeSpan.FromSeconds(currentScore).ToString(@"hh\:mm\:ss\:fff");
        }
    }
}
