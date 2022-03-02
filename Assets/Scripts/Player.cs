using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public Projectile laserPrefab;
    public System.Action killed;
    public bool laserActive { get; private set; }

    // Handles movement and shooting
    private void Update()
    {
        Vector3 position = transform.position;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            position.x -= speed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            position.x += speed * Time.deltaTime;

        // Clamps to prevent player from moving off the map
        position.x = Mathf.Clamp(position.x, Camera.main.ViewportToWorldPoint(Vector3.zero).x, Camera.main.ViewportToWorldPoint(Vector3.right).x);
        transform.position = position;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            Shoot();
    }

    // Shoots a laser if one does not already exist
    private void Shoot()
    {
        if (!laserActive)
        {
            laserActive = true;

            Projectile laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            laser.destroyed += OnLaserDestroyed;
        }
    }

    // On destruction, sets laser bool to false to allow next to be shot
    private void OnLaserDestroyed(Projectile laser)
    {
        laserActive = false;
    }

    // When touching an enemy, die
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (killed != null)
                killed.Invoke();
            
        }
    }

}
