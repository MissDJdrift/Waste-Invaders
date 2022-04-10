using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public Projectile[] laserPrefab;

    public Sprite hitSprite;
    public Sprite idleSprite;
    public Sprite[] movingSprites;
    public Sprite[] shootSprites;
    public float animationSpeed;

    private int shootingFrame;
    private int movingFrame;
    private bool shooting;
    private bool damaged;
    public bool moving;
    public bool movingLeft;

    public int selectedWeapon = 0;
    public int Health = 8;
    public Slider healthSlider;

    public bool laserActive { get; private set; }

    public Text neededWeaponText;
    //public Text neededWeaponNumberText;
    public Image neededWeaponImage;
    public Sprite[] neededWeaponImages;

    public Color[] WeaponColors;
    public LayerMask EnemyMask;

    public System.Action killed;

    private SpriteRenderer spriteRenderer;

    public AudioManager audioManager;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimatePlayer), animationSpeed, animationSpeed);
    }

    // Handles movement and shooting
    private void Update()
    {
        Vector3 position = transform.position;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            position.x -= speed * Time.deltaTime;
            movingLeft = true;
            moving = true;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            position.x += speed * Time.deltaTime;
            movingLeft = false;
            moving = true;
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            moving = false;
        }
        else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            moving = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            selectedWeapon = 0;
            if (!laserActive)
                audioManager.Play("Shoot1");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
            if (!laserActive)
                audioManager.Play("Shoot2");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeapon = 2;
            if (!laserActive)
                audioManager.Play("Shoot3");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedWeapon = 3;
            if (!laserActive)
                audioManager.Play("Shoot4");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedWeapon = 4;
            if (!laserActive)
                audioManager.Play("Shoot5");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            selectedWeapon = 5;
            if (!laserActive)
                audioManager.Play("Shoot6");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            selectedWeapon = 6;
            if (!laserActive)
                audioManager.Play("Shoot7");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            selectedWeapon = 7;
            if (!laserActive)
                audioManager.Play("Shoot8");
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
            int hitEnemy = hit.collider.gameObject.GetComponent<Enemy>().EnemyType - 1;

            if (hitEnemy == 0)
            {
                neededWeaponText.text = "Plastic Type 1: PET";
                //neededWeaponNumberText.text = "1";
                neededWeaponImage.sprite = neededWeaponImages[1];
            }
            else if (hitEnemy == 1)
            {
                neededWeaponText.text = "Plastic Type 2: HDPE";
                //neededWeaponNumberText.text = "2";
                neededWeaponImage.sprite = neededWeaponImages[2];
            }
            else if (hitEnemy == 2)
            {
                neededWeaponText.text = "Plastic Type 3: PVC";
                //neededWeaponNumberText.text = "3";
                neededWeaponImage.sprite = neededWeaponImages[3];
            }
            else if (hitEnemy == 3)
            {
                neededWeaponText.text = "Plastic Type 4: LDPE";
                //neededWeaponNumberText.text = "4";
                neededWeaponImage.sprite = neededWeaponImages[4];
            }
            else if (hitEnemy == 4)
            {
                neededWeaponText.text = "Plastic Type 5: PP";
                //neededWeaponNumberText.text = "5";
                neededWeaponImage.sprite = neededWeaponImages[5];
            }
            else if (hitEnemy == 5)
            {
                neededWeaponText.text = "Plastic Type 6: PS";
                //neededWeaponNumberText.text = "6";
                neededWeaponImage.sprite = neededWeaponImages[6];
            }
            else if (hitEnemy == 6)
            {
                neededWeaponText.text = "Plastic Type 7";
                //neededWeaponNumberText.text = "7";
                neededWeaponImage.sprite = neededWeaponImages[7];
            }
            else if (hitEnemy == 7)
            {
                neededWeaponText.text = "Trash";
                //neededWeaponNumberText.text = "8";
                neededWeaponImage.sprite = neededWeaponImages[8];
            }
        }
        else
        {
            neededWeaponText.text = "";
            //neededWeaponNumberText.text = "";
            neededWeaponImage.sprite = neededWeaponImages[0];
        }

        if (movingLeft)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // Shoots a laser if one does not already exist
    private void Shoot()
    {
        shooting = true;
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
        damaged = true;

        healthSlider.value = Health; 
    }

    private void AnimatePlayer()
    {
        if (shooting)
        {
            shootingFrame++;

            if (shootingFrame >= shootSprites.Length)
            {
                shootingFrame = 0;
                shooting = false;
            }

            spriteRenderer.sprite = shootSprites[shootingFrame];
        }
        else if (moving)
        {
            movingFrame++;

            if (movingFrame >= movingSprites.Length)
            {
                movingFrame = 0;
            }

            spriteRenderer.sprite = movingSprites[movingFrame];
        }
        else if (damaged)
        {
            spriteRenderer.sprite = hitSprite;
            damaged = false;
        }
        else
        {
            spriteRenderer.sprite = idleSprite;
        }
    }
}
