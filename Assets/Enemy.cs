using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float speed = 2.5f;
    public int health = 1;
    public float damage = 10f;
    public float damageInterval = 1f;

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    private float nextDamageTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                PlayerMovement pm = FindObjectOfType<PlayerMovement>();
                if (pm != null)
                {
                    player = pm.transform;
                }
            }
        }

        if (player != null)
        {
            Debug.Log($"[Enemy] Chasing target: '{player.name}' at position {player.position}. (If position stays at 0,0,0, check if you assigned a Prefab instead of the Scene Object!)");
        }
        else
        {
            Debug.LogError("[Enemy] Target NOT found! Ensure Player has 'Player' tag or 'PlayerMovement' script.");
        }
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            if (animator != null)
            {
                animator.SetFloat("Horizontal", direction.x);
                animator.SetFloat("Vertical", direction.y);
                animator.SetFloat("Speed", direction.magnitude);
            }

            if (spriteRenderer != null)
            {

                if (direction.x < 0)
                {
                    spriteRenderer.flipX = true;
                }
                else if (direction.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
            }
        }
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextDamageTime)
            {
                PlayerStats stats = collision.gameObject.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.TakeDamage(damage);
                    nextDamageTime = Time.time + damageInterval;
                }
            }
        }
    }
}
