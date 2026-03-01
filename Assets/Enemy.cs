using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float speed = 2.5f;
    public int health = 3;
    [Header("Drops")]
    public int xpDropAmount = 5;
    public float damage = 10f;
    public float damageInterval = 1f;
    [Header("Audio")]
    public AudioClip damageSound;
    private const float DamageSoundVolume = 0.2f;
    [Header("Damage Flash")]
    public Color damageFlashColor = new Color(1f, 0.35f, 0.35f, 1f);
    public float damageFlashDuration = 0.08f;

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    private Color defaultSpriteColor = Color.white;
    private Coroutine damageFlashCoroutine;
    private float nextDamageTime;
    private bool isDead;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultSpriteColor = spriteRenderer.color;
        }

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
        if (isDead) return;

        PlayDamageSound();
        TriggerDamageFlash();
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        if (xpDropAmount > 0)
        {
            ExperiencePickup.Spawn(transform.position, xpDropAmount, spriteRenderer);
        }

        Destroy(gameObject);
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

    protected virtual AudioClip GetDamageSound()
    {
        return damageSound;
    }

    protected virtual float GetDamageSoundVolume()
    {
        return DamageSoundVolume;
    }

    protected void PlayDamageSound()
    {
        AudioClip clip = GetDamageSound();
        float volume = Mathf.Clamp01(GetDamageSoundVolume());

        if (clip == null || volume <= 0f)
        {
            return;
        }

        GameObject soundObject = new GameObject("EnemyDamageSound");
        soundObject.transform.position = transform.position;

        AudioSource soundSource = soundObject.AddComponent<AudioSource>();
        soundSource.clip = clip;
        soundSource.playOnAwake = false;
        soundSource.spatialBlend = 0f;
        soundSource.volume = volume;
        soundSource.Play();

        Destroy(soundObject, clip.length + 0.05f);
    }

    private void TriggerDamageFlash()
    {
        if (spriteRenderer == null || damageFlashDuration <= 0f)
        {
            return;
        }

        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
        }

        damageFlashCoroutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        spriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(damageFlashDuration);
        spriteRenderer.color = defaultSpriteColor;
        damageFlashCoroutine = null;
    }
}
