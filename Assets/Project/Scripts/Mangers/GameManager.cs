using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private float matchDuration = 120f;
    private float timeRemaining;
    [SerializeField] private int sickLimit = 3;
    [SerializeField] private int badServiceLimit = 5;
    private int sickCount;
    private int badServiceCount;
    private int satisfiedCount;
    private bool isGameOver;
    [SerializeField] private GameObject panelVictory;
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private TextMeshProUGUI victoryReasonText;
    [SerializeField] private TextMeshProUGUI gameOverReasonText;
    private SpawnManager spawnManager;
    [SerializeField] private TextMeshProUGUI scoreboardText;


    private void Awake()
    {
        spawnManager = FindFirstObjectByType<SpawnManager>();
        Instance = this;
        timeRemaining = matchDuration;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (isGameOver) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndGame(true);
        }

        UpdateScoreboard();
    }

    private void EndGame(bool playerWon)
    {
        isGameOver = true;
        Time.timeScale = 0f;
        spawnManager.StopSpawning();

        if (playerWon)
        {
            panelVictory.SetActive(true);
            victoryReasonText.text = $"You survived 2 minutes — Satisfied customers: {satisfiedCount}";
        }
        else
        {
            panelGameOver.SetActive(true);
            gameOverReasonText.text = $"{sickCount} sick customers, {badServiceCount} dissatisfied customers";
        }
    }

    public void RegisterSatisfied()
    {
        if (isGameOver) return;
        satisfiedCount++;
    }

    public void RegisterSick()
    {
        if (isGameOver) return;
        sickCount++;
        if (sickCount >= sickLimit)
        {
            EndGame(false);
        }
    }

    public void RegisterBadService()
    {
        if (isGameOver) return;
        badServiceCount++;
        if (badServiceCount >= badServiceLimit)
        {
            EndGame(false);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes}:{seconds:00}";
    }

    private void UpdateScoreboard()
    {
        scoreboardText.text = $"Time: {FormatTime(timeRemaining)} | Sick: {sickCount} | Unsatisfied: {badServiceCount} | Satisfied: {satisfiedCount}";
    }
}
