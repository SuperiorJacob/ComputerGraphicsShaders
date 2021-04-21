using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPersonPlayerShooter
{
    /// <summary>
    /// Spawner class used to spawn Enemies for the player. The enemies are decided on a rarity table.
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        public static int spawnedEnemies = 0;

        #region EditorDataOrganisers (structs)
        [System.Serializable]
        public struct EnemyProbability
        {
            public string name;
            public int rarity;
            public GameObject enemyPrefab;
        }
        #endregion

        #region Fields
        public float _spawnTimer;
        public float _maxSpawns = 30;

        public Transform _spawnPosition;
        public EnemyProbability[] _enemies;

        public int[] enemyProbability;
        #endregion

        #region Functions
        // Start is called before the first frame update
        void Start()
        {
            spawnedEnemies = 0;
            // Using INT array vs a GameObject array is a micro memory optimisation haha

            int enemyArray = 0;

            // Creating the probability array
            for (int i = 0; i < _enemies.Length; i++)
            {
                EnemyProbability e = _enemies[i];
                enemyArray += e.rarity;
            }

            enemyProbability = new int[enemyArray];

            int enemyArrayIndex = 0;

            // Shuffling in the array
            for (int i = 0; i < _enemies.Length; i++)
            {
                EnemyProbability e = _enemies[i];

                for (int a = 0; a < e.rarity; a++)
                {
                    enemyProbability[enemyArrayIndex] = i;
                    enemyArrayIndex++;
                }
            }

            // Shuffling
            System.Random rnd = new System.Random();
            int[] res = new int[enemyArray];

            res[0] = enemyProbability[0];
            for (int i = 1; i < enemyArray; i++)
            {
                int j = rnd.Next(i);
                res[i] = res[j];
                res[j] = enemyProbability[i];
            }

            enemyProbability = res;

            // Coroutine
            StartCoroutine("SpawnEnemy");
        }

        // UI
        public void MaxEnemies(float a_max)
        {
            _maxSpawns = a_max;
        }

        /// <summary>
        /// Enemy spawner, spawns enemies per duration.
        /// </summary>
        private IEnumerator SpawnEnemy()
        {
            while (true)
            {
                yield return new WaitForSeconds(_spawnTimer);

                if (spawnedEnemies < _maxSpawns && Cursor.lockState != CursorLockMode.Confined)
                {
                    int r = Random.Range(0, enemyProbability.Length - 1);

                    EnemyProbability e = _enemies[enemyProbability[r]];

                    Instantiate(e.enemyPrefab, _spawnPosition.position, _spawnPosition.rotation, null);

                    spawnedEnemies++;
                }
            }
        }
        #endregion
    }
}