using UnityEngine;
using UserInterface;
using Utilities;

namespace GameController
{
    public class ParkourLevelController : LevelController
    {
        [SerializeField] private MyTimer timer = null;
        [SerializeField] private ParkourWinCanvas winCanvas = null;

        protected override void Start()
        {
            base.Start();
            player.OnDeath += timer.DisableTextArea;
            timer.OnTimerStop += HandleWin;
        }

        protected override void HandleWin()
        {
            base.HandleWin();
            float record = PlayerPrefs.GetFloat("ParkourTime", -1.0f);
            if (timer.currentTime < record || record == -1)
                PlayerPrefs.SetFloat("ParkourTime", timer.currentTime);

            winCanvas.SetTimesText(timer.currentTime);
            player.DisableControls();
            win.EnterGameEnd();
        }
    }
}
