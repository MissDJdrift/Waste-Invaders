using UnityEngine;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
    private Player player;
    private EnemyController enemyController;
    private BonusShip bonusShip;

    public GameObject gameOverUI;
    public Text scoreText;
    public Text livesText;

    public int score { get; private set; }
    public int lives { get; private set; }
    
    // Find references
    private void Awake()
    {
        player = FindObjectOfType<Player>();
        enemyController = FindObjectOfType<EnemyController>();
        bonusShip = FindObjectOfType<BonusShip>();
    }

    // Reference death actions, and start new game
    private void Start()
    {
        player.killed += OnPlayerKilled;
        bonusShip.killed += OnBonusShipKilled;
        enemyController.killed += OnEnemyKilled;

        NewGame();
    }

    // If no lives and player hits enter, restart
    private void Update()
    {
        if (lives <= 0 && Input.GetKeyDown(KeyCode.Return)) 
        {
            NewGame();
        }
    }

    // Reset score and lives, disable game over ui, and restart game
    private void NewGame()
    {
        gameOverUI.SetActive(false);

        SetScore(0);
        SetLives(3);
        NewRound();
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
        player.gameObject.SetActive(true);
    }

    // activate game over gui and disable game
    private void GameOver()
    {
        gameOverUI.SetActive(true);
        enemyController.gameObject.SetActive(false);
    }

    // Sets the score text
    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString().PadLeft(4, '0');
    }

    // Sets lives left text
    private void SetLives(int lives)
    {
        this.lives = Mathf.Max(lives, 0);
        livesText.text = lives.ToString();
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

    // When bonus ship killed, add its score
    private void OnBonusShipKilled(BonusShip bonusShip)
    {
        SetScore(score + bonusShip.score);
    }

}
