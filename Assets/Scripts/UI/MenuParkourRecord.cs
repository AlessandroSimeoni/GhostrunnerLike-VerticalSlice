using System;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class MenuParkourRecord : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI recordTextArea = null;

        private void Start()
        {
            float record = PlayerPrefs.GetFloat("ParkourTime", -1);
            if (record != -1)
                recordTextArea.text = "Best time: " + TimeSpan.FromSeconds(record).ToString(@"hh\:mm\:ss\:fff");
            else
                gameObject.SetActive(false);
        }
    }
}
