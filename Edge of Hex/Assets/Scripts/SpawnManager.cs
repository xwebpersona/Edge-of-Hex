using UnityEngine;
using TMPro;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private GameObject player;

    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text highScoreText;

    private int enemyCount;
    private int waveNumber = 1;
    private int highScore;

    private bool playerDead = false;
    private bool canSpawn = false;

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUI();

        StartCoroutine(StartSpawningWithDelay(0.5f));
    }

    private IEnumerator StartSpawningWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canSpawn = true;
        SpawnNextWave();
        UpdateUI();
    }

    void Update()
    {
        if (playerDead || !canSpawn) return;

        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCount == 0)
        {
            waveNumber++;

            if (waveNumber > highScore)
            {
                highScore = waveNumber;
                PlayerPrefs.SetInt("HighScore", highScore);
            }

            UpdateUI();
            SpawnNextWave();
        }
    }

    void SpawnNextWave()
    {
        for (int i = 0; i < waveNumber; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }

        if (GameObject.FindGameObjectWithTag("PowerUp") == null)
        {
            Instantiate(powerUpPrefab, GenerateSpawnPosition(), powerUpPrefab.transform.rotation);
        }
    }

    Vector3 GenerateSpawnPosition()
    {
        float spawnRange = 9.0f;
        return new Vector3(Random.Range(-spawnRange, spawnRange), 0.5f, Random.Range(-spawnRange, spawnRange));
    }

    void UpdateUI()
    {
        waveText.text = $"Wave: {waveNumber}";
        highScoreText.text = $"High Score: {highScore}";
    }

    public void OnPlayerDeath()
    {
        playerDead = true;
    }
}
