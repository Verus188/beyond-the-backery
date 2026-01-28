using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CreateBackground();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        transform.Translate(movement * speed * Time.deltaTime);

        if (movement.magnitude > 0)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.magnitude);

            if (movement.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (movement.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - (Vector2)firePoint.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    void CreateBackground()
    {
        if (GameObject.Find("Background") != null) return;

        GameObject bg = new GameObject("Background");
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("snow-tile");
        if (sprites != null && sprites.Length > 0)
        {
            sr.sprite = sprites[0];
        }
        else
        {
            Debug.LogError("Failed to load background sprite 'snow-tile'");
        }

        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(100, 100);
        sr.sortingOrder = -100;
        bg.transform.position = Vector3.zero;
    }
}
