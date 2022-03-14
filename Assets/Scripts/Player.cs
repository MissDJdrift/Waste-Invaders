using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public Projectile[] laserPrefab;
    public int selectedWeapon = 0;
    public int Health = 8;
    public System.Action killed;
    public bool laserActive { get; private set; }
    public Image selectedWeaponImage;
    public Color[] WeaponColors;
    public LayerMask EnemyMask;

    // Handles movement and shooting
    private void Update()
    {
        Vector3 position = transform.position;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            position.x -= speed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            position.x += speed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            selectedWeapon = 0;
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeapon = 2;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (killed != null)
                killed.Invoke();
        }

        // Clamps to prevent player from moving off the map
        position.x = Mathf.Clamp(position.x, Camera.main.ViewportToWorldPoint(Vector3.zero).x, Camera.main.ViewportToWorldPoint(Vector3.right).x);
        transform.position = position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.up), 30f, EnemyMask);

        if (hit)
        {
            selectedWeaponImage.color = WeaponColors[hit.collider.gameObject.GetComponent<Enemy>().EnemyType - 1];
        }
    }

    // Shoots a laser if one does not already exist
    private void Shoot()
    {
        if (!laserActive)
        {
            laserActive = true;

            Projectile laser = Instantiate(laserPrefab[selectedWeapon], transform.position, Quaternion.identity);
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

    public void IncorrectHit()
    {
        Health -= 1;
        if (Health == 0)
        {
            if (killed != null)
                killed.Invoke();
        }
    }

}
