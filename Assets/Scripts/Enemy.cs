using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int score = 10;
    public System.Action<Enemy> killed;

    // When hit by a laser, send out death message
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Laser"))
            killed?.Invoke(this);
    }

}
