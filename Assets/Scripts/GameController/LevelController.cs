using Enemy;
using Player;
using SceneLoad;
using System;
using UnityEngine;

namespace GameController
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter player = null;
        [SerializeField] private GameEnd gameOver = null;
        [SerializeField] private GameEnd win = null;
        [Space]
        [Header("Enemies")]
        [SerializeField] private TurretCannonEnemy[] enemyArray = Array.Empty<TurretCannonEnemy>();

        private int remainingEnemies = 0;

        private void Start()
        {
            SceneLoader.instance.OnLoadingCompleted += player.EnableControls;
            player.OnDeath += gameOver.EnterGameEnd;

            remainingEnemies = enemyArray.Length;
            foreach (TurretCannonEnemy tce in enemyArray)
                tce.OnDeath += HandleEnemyDeath;
        }

        private void HandleEnemyDeath()
        {
            remainingEnemies--;

            if (remainingEnemies == 0)
            {
                player.DisableControls();
                win.EnterGameEnd();
            }
        }
    }
}
