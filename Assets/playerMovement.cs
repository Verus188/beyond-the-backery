using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootRate = 2f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float nextShootTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        Transform nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Vector2 lookDir = (Vector2)nearestEnemy.position - (Vector2)firePoint.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            firePoint.rotation = Quaternion.Euler(0, 0, angle);

            if (shootRate > 0f && Time.time >= nextShootTime)
            {
                Shoot();
                nextShootTime = Time.time + (1f / shootRate);
            }
        }
    }

    Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearest = null;
        float nearestDistance = float.MaxValue;
        Vector2 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = ((Vector2)enemy.transform.position - currentPosition).sqrMagnitude;
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
