using Enemy;
using System;
using UnityEngine;

namespace GameController
{
    public class StandardLevelController : LevelController
    {
        [Space]
        [Header("Enemies")]
        [SerializeField] private TurretCannonEnemy[] enemyArray = Array.Empty<TurretCannonEnemy>();

        private int remainingEnemies = 0;
        protected override void Start()
        {
            base.Start();
            remainingEnemies = enemyArray.Length;
            foreach (TurretCannonEnemy tce in enemyArray)
                tce.OnDeath += HandleEnemyDeath;
        }

        private void HandleEnemyDeath()
        {
            remainingEnemies--;

            if (remainingEnemies == 0)
                HandleWin();
        }

        protected override void HandleWin()
        {
            base.HandleWin();
            player.DisableControls();
            win.EnterGameEnd();
        }
    }
}
