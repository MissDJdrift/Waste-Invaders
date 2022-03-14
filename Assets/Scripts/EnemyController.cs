using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Enemy[] prefabs = new Enemy[5];
    public AnimationCurve Speed = new AnimationCurve();
    public Player player;
    public Vector3 direction { get; private set; } = Vector3.right;
    public Vector3 initialPos { get; private set; }
    public System.Action<Enemy> killed;
    public System.Action<Enemy> IncorrectHit;

    public int NumberKilled { get; private set; }
    public int NumberAlive => TotalEnemies - NumberKilled;
    public int TotalEnemies => rows * columns;
    public float PercentKilled => (float)NumberKilled / (float)TotalEnemies;

    public int rows = 5;
    public int columns = 11;

    public float HorizontalSpacing = 2.0f;
    public float VerticalSpacing = 2.0f;

    private bool bottomCheck = false;

    // Initial spawn of enemies
    private void Awake()
    {
        initialPos = transform.position;

        for (int i = 0; i < rows; i++)
        {
            Vector2 centerOffset = new Vector2((-HorizontalSpacing * (columns - 1)) * 0.5f, (-VerticalSpacing * (rows - 1)) * 0.5f);
            Vector3 rowPosition = new Vector3(centerOffset.x, (VerticalSpacing * i) + centerOffset.y, 0f);

            for (int j = 0; j < columns; j++)
            {
                Enemy enemy = Instantiate(prefabs[i], transform);
                enemy.killed += EnemyDeath;
                enemy.IncorrectHit += IncorrectlyHit;

                Vector3 pos = rowPosition;
                pos.x += HorizontalSpacing * j;
                enemy.transform.localPosition = pos;
            }
        }
    }

    // Update speed based on # of enemies left, and move enemies, if any hitting the wall, move down, if any below map, restart
    private void Update()
    {
        float speed = Speed.Evaluate(PercentKilled);
        transform.position += direction * speed * Time.deltaTime;

        foreach (Transform enemy in transform)
        {
            if (!enemy.gameObject.activeInHierarchy)
                continue;

            if (direction == Vector3.right && enemy.position.x >= (Camera.main.ViewportToWorldPoint(Vector3.right).x - 1f))
            {
                NextRow();
                break;
            }
            else if (direction == Vector3.left && enemy.position.x <= (Camera.main.ViewportToWorldPoint(Vector3.zero).x + 1f))
            {
                NextRow();
                break;
            }

            if (!bottomCheck && enemy.position.y <= Camera.main.ViewportToWorldPoint(Vector3.zero).y)
            {
                bottomCheck = true;

                if (player.killed != null)
                    player.killed.Invoke();

                break;
            }
        }
    }

    // Move to the next row
    private void NextRow()
    {
        direction = new Vector3(-direction.x, 0f, 0f);

        Vector3 pos = transform.position;
        pos.y -= 1f;
        transform.position = pos;
    }

    // When enemy dies, deactivate it and move up the counter
    private void EnemyDeath(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        NumberKilled++;
        killed(enemy);
    }

    private void IncorrectlyHit(Enemy enemy)
    {
        player.IncorrectHit();
        IncorrectHit(enemy);
    }

    // Reset enemies
    public void ResetEnemies()
    {
        NumberKilled = 0;
        direction = Vector3.right;
        transform.position = initialPos;

        foreach (Transform invader in transform) 
        {
            invader.gameObject.SetActive(true);
        }

        bottomCheck = false;
    }

}
