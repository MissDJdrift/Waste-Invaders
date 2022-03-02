using UnityEngine;

public class BonusShip : MonoBehaviour
{
    public float speed = 5f;
    public float respawnTime = 30f;
    public int score = 250;
    public System.Action<BonusShip> killed;

    public Vector3 leftDest { get; private set; }
    public Vector3 rightDest { get; private set; }
    public int dir { get; private set; } = -1;
    public bool spawned { get; private set; }

    // Set left and right stopping points, despawn initially
    private void Start()
    {
        Vector3 left = transform.position;
        left.x = Camera.main.ViewportToWorldPoint(Vector3.zero).x - 2f;
        leftDest = left;

        Vector3 right = transform.position;
        right.x = Camera.main.ViewportToWorldPoint(Vector3.right).x + 2f;
        rightDest = right;

        transform.position = leftDest;
        Despawn();
    }

    // Set movement direction if spawned
    private void Update()
    {
        if (!spawned) 
            return;

        if (dir == 1) 
            MoveRight();
        else
            MoveLeft();
    }

    // Move in right direction, if outside map, then despawn
    private void MoveRight()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (transform.position.x >= rightDest.x)
            Despawn();
    }

    // Move in left direction, if outside map, then despawn
    private void MoveLeft()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= leftDest.x)
            Despawn();
    }

    // Spawn and pick direction opposite of last time
    private void Spawn()
    {
        dir *= -1;

        if (dir == 1)
            transform.position = leftDest;
        else
            transform.position = rightDest;

        spawned = true;
    }

    // Deactivate self, and reactivate in respawnTime
    private void Despawn()
    {
        spawned = false;

        if (dir == 1)
            transform.position = rightDest;
        else
            transform.position = leftDest;

        Invoke(nameof(Spawn), respawnTime);
    }

    // When hit by a laser, despawn and send death message
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            Despawn();

            if (killed != null)
                killed.Invoke(this);
        }
    }

}
