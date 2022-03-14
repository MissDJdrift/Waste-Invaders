using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int score = 10;
    public int EnemyType = 0;
    public System.Action<Enemy> killed;
    public System.Action<Enemy> IncorrectHit;

    // When hit by a laser, send out death message
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            if (collision.gameObject.GetComponent<Projectile>().LaserType == EnemyType)
            {
                killed?.Invoke(this);
            }
            else
            {
                IncorrectHit?.Invoke(this);
            }
        }
    }

}
