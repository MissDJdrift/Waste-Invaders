using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
    private Player player;
    private EnemyController enemyController;

    public GameObject gameOverUI;
    public Text scoreText;
    public Text LivesText;
    public Text HighScoreText;
    public GameObject InGameUI;
    public GameObject MenuUI;
    public GameObject CreditsUI;
    public GameObject HighScoresUI;
    public GameObject TutorialUI;
    public GameObject SettingsUI;
    public EventSystem eventSystem;
    public Transform[] toggleOnGO;
    public GameObject TutorialObjects;

    public int score { get; private set; }
    private int HighScore;
    public int lives { get; private set; }
    public bool TutorialActive { get; private set; }

    public AudioManager audioManager;

    // Find references
    private void Awake()
    {
        player = FindObjectOfType<Player>();
        enemyController = FindObjectOfType<EnemyController>();
    }

    // Reference death actions, and start new game
    private void Start()
    {
        player.killed += OnPlayerKilled;
        enemyController.killed += OnEnemyKilled;
        enemyController.IncorrectHit += OnEnemyIncorrectlyHit;

        audioManager.Play("MenuMusic");
        audioManager.Stop("Level1Music");

        MainMenu();
    }

    // If no lives and player hits enter, restart
    private void Update()
    {
        if (InGameUI.activeSelf)
        {
            if (lives <= 0 && Input.GetKeyDown(KeyCode.Return))
            {
                NewGame();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (InGameUI.activeSelf)
            {
                audioManager.Play("MenuMusic");
                audioManager.Stop("Level1Music");
            }
            MainMenu();
        }
    }

    private void MainMenu()
    {
        foreach (Transform item in toggleOnGO)
        {
            item.gameObject.SetActive(false);
        }

        InGameUI.SetActive(false);
        MenuUI.SetActive(true);
        CreditsUI.SetActive(false);
        HighScoresUI.SetActive(false);
        TutorialUI.SetActive(false);
        SettingsUI.SetActive(false);

        eventSystem.SetSelectedGameObject(MenuUI.GetComponentInChildren<Button>().gameObject, new BaseEventData(eventSystem));
    }

    // Reset score and lives, disable game over ui, and restart game
    public void NewGame()
    {
        gameOverUI.SetActive(false);

        foreach (Transform item in toggleOnGO)
        {
            item.gameObject.SetActive(true);
        }

        InGameUI.SetActive(true);
        MenuUI.SetActive(false);
        TutorialUI.SetActive(false);

        SetScore(0);
        SetLives(3);
        NewRound();

        audioManager.Stop("MenuMusic");
        audioManager.Play("Level1Music");

        StartCoroutine(Countdown());
    }

    public void StartTutorial()
    {
        gameOverUI.SetActive(false);

        foreach (Transform item in toggleOnGO)
        {
            item.gameObject.SetActive(true);
        }

        InGameUI.SetActive(true);
        MenuUI.SetActive(false);
        TutorialUI.SetActive(false);
        TutorialObjects.SetActive(true);
    }

    public void MenusToggleOff(GameObject Toggle)
    {
        Toggle.SetActive(false);
        if (!MenuUI.gameObject.activeSelf)
        {
            if (CreditsUI.activeSelf)
            {
                eventSystem.SetSelectedGameObject(CreditsUI.GetComponentInChildren<Button>().gameObject, new BaseEventData(eventSystem));
            }
            else if (HighScoresUI.activeSelf)
            {
                eventSystem.SetSelectedGameObject(HighScoresUI.GetComponentInChildren<Button>().gameObject, new BaseEventData(eventSystem));
            }
            else if (TutorialUI.activeSelf)
            {
                eventSystem.SetSelectedGameObject(TutorialUI.GetComponentInChildren<Button>().gameObject, new BaseEventData(eventSystem));
            }
            else if (SettingsUI.activeSelf)
            {
                eventSystem.SetSelectedGameObject(SettingsUI.GetComponentInChildren<Button>().gameObject, new BaseEventData(eventSystem));
            }
        }
        else
        {
            eventSystem.SetSelectedGameObject(MenuUI.GetComponentInChildren<Button>().gameObject, new BaseEventData(eventSystem));
        }
    }

    public void MenusToggleOn(GameObject Toggle)
    {
        Toggle.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit(0);
    }

    // Respawn enemies to base areas
    private void NewRound()
    {
        enemyController.ResetEnemies();
        enemyController.gameObject.SetActive(true);

        Respawn();
    }

    // Respawn player
    private void Respawn()
    {
        Vector3 position = player.transform.position;
        position.x = 0f;
        player.transform.position = position;
        player.Health = 8;
        player.gameObject.SetActive(true);

        player.healthSlider.value = player.Health;
    }

    // activate game over gui and disable game
    private void GameOver()
    {
        foreach (Transform item in toggleOnGO)
        {
            item.gameObject.SetActive(false);
        }
        gameOverUI.SetActive(true);
        lives = 0;
    }

    // Sets the score text
    private void SetScore(int score)
    {
        this.score = score;
        if (score >= 0)
        {
            scoreText.text = score.ToString().PadLeft(4, '0');
        }
        else
        {
            score *= -1;
            scoreText.text = "-" + score.ToString().PadLeft(4, '0');
            score *= -1;
        }

        SetHighScore(score);
    }

    private void SetHighScore(int score)
    {
        if (HighScore < score)
        {
            HighScore = score;
            HighScoreText.text = HighScore.ToString().PadLeft(4, '0');
        }
    }

    // Sets lives left text
    private void SetLives(int lives)
    {
        this.lives = Mathf.Max(lives, 0);
        LivesText.text = lives.ToString();
    }

    // Remove life, if some left, respawn and restart round, else, send to game over
    private void OnPlayerKilled()
    {
        SetLives(lives - 1);

        player.gameObject.SetActive(false);

        if (lives > 0)
            Invoke(nameof(NewRound), 1f);
        else
            GameOver();
    }

    // When an enemy dies, add score, if all dead, start next round
    private void OnEnemyKilled(Enemy enemy)
    {
        SetScore(score + enemy.score);

        if (enemyController.NumberKilled == enemyController.TotalEnemies)
            NewRound();
    }

    private void OnEnemyIncorrectlyHit(Enemy enemy)
    {
        SetScore(score - (enemy.score / 2));
    }

    private IEnumerator Countdown()
    {
        float duration = 780f;
        float totalTime = 0;
        while (totalTime <= duration)
        {
            totalTime += Time.deltaTime;
            if (lives <= 0)
            {
                yield break;
            }
            if (MenuUI.activeSelf)
            {
                yield break;
            }
            yield return null;
        }
        GameOver();
        yield return null;
    }
}
