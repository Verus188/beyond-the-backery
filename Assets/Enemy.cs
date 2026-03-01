using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private static Material bloodParticleMaterial;

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
    [Header("Death Blood")]
    public bool spawnBloodOnDeath = true;
    public Color bloodColor = Color.red;
    public int bloodParticles = 24;
    public float bloodBurstSpeed = 4f;
    public float bloodLifetime = 0.45f;

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

        SpawnDeathBloodEffect();
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

    private void SpawnDeathBloodEffect()
    {
        if (!spawnBloodOnDeath || bloodParticles <= 0 || bloodLifetime <= 0f)
        {
            return;
        }

        GameObject bloodObject = new GameObject("EnemyBloodBurst");
        bloodObject.transform.position = transform.position;

        ParticleSystem ps = bloodObject.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 0.12f;
        main.startLifetime = bloodLifetime;
        main.startSpeed = bloodBurstSpeed;
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.16f);
        main.startColor = bloodColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = Mathf.Max(8, bloodParticles + 8);
        main.gravityModifier = 0.6f;

        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, (short)bloodParticles)
        });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 35f;
        shape.radius = 0.08f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient fadeGradient = new Gradient();
        fadeGradient.SetKeys(
            new[]
            {
                new GradientColorKey(bloodColor, 0f),
                new GradientColorKey(new Color(0.6f, 0f, 0f, 1f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.2f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fadeGradient);

        ParticleSystemRenderer psRenderer = bloodObject.GetComponent<ParticleSystemRenderer>();
        if (psRenderer != null)
        {
            psRenderer.material = GetBloodParticleMaterial();
        }

        ps.Play();
        Destroy(bloodObject, bloodLifetime + 0.6f);
    }

    private static Material GetBloodParticleMaterial()
    {
        if (bloodParticleMaterial != null)
        {
            return bloodParticleMaterial;
        }

        string[] candidateShaders =
        {
            "Universal Render Pipeline/Particles/Unlit",
            "Particles/Standard Unlit",
            "Sprites/Default"
        };

        for (int i = 0; i < candidateShaders.Length; i++)
        {
            Shader shader = Shader.Find(candidateShaders[i]);
            if (shader != null)
            {
                bloodParticleMaterial = new Material(shader)
                {
                    hideFlags = HideFlags.DontSave
                };
                return bloodParticleMaterial;
            }
        }

        return null;
    }
}
