using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int score = 10;
    public int EnemyType = 0;

    public Sprite[] animationImages;
    public float animationTime = 1.0f;
    private SpriteRenderer spriteRenderer;
    private int animationFrame;

    public System.Action<Enemy> killed;
    public System.Action<Enemy> IncorrectHit;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateEnemy), animationTime, animationTime);
    }

    private void AnimateEnemy()
    {
        animationFrame++;
        if (animationFrame >= animationImages.Length)
        {
            animationFrame = 0;
        }

        spriteRenderer.sprite = animationImages[animationFrame];
    }

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
