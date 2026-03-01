using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootRate = 2f;

    [Header("Audio")]
    public AudioClip shootingSound;
    public AudioClip walkSnowSound;

    private const float ShootVolume = 0.4f;
    private const float WalkSoundVolume = 0.2f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource shootAudioSource;
    private AudioSource walkAudioSource;
    private bool missingShootSoundWarningShown;
    private bool missingWalkSoundWarningShown;
    private float nextShootTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        AudioSource[] existingSources = GetComponents<AudioSource>();
        if (existingSources.Length > 0)
        {
            shootAudioSource = existingSources[0];
        }
        else
        {
            shootAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (existingSources.Length > 1)
        {
            walkAudioSource = existingSources[1];
        }
        else
        {
            walkAudioSource = gameObject.AddComponent<AudioSource>();
        }

        shootAudioSource.playOnAwake = false;
        shootAudioSource.spatialBlend = 0f;
        shootAudioSource.loop = false;
        shootAudioSource.volume = 1f;

        walkAudioSource.playOnAwake = false;
        walkAudioSource.spatialBlend = 0f;
        walkAudioSource.loop = true;
        walkAudioSource.volume = Mathf.Clamp01(WalkSoundVolume);
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        bool isMoving = movement.sqrMagnitude > 0f;
        transform.Translate(movement * speed * Time.deltaTime);

        if (isMoving)
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

        UpdateWalkSound(isMoving);

        Transform nearestEnemy = FindNearestEnemy();
        if (nearestEnemy == null) return;

        Vector2 lookDir = (Vector2)nearestEnemy.position - (Vector2)firePoint.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        if (shootRate > 0f && Time.time >= nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + (1f / shootRate);
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

        if (shootingSound != null && shootAudioSource != null)
        {
            shootAudioSource.PlayOneShot(shootingSound, ShootVolume);
        }
        else if (!missingShootSoundWarningShown)
        {
            missingShootSoundWarningShown = true;
            Debug.LogWarning("[PlayerMovement] Shoot sound is not assigned. Set 'Shoot Sound' in the inspector.");
        }
    }

    void UpdateWalkSound(bool isMoving)
    {
        if (walkAudioSource == null)
        {
            return;
        }

        if (walkSnowSound == null)
        {
            if (walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }

            if (isMoving && !missingWalkSoundWarningShown)
            {
                missingWalkSoundWarningShown = true;
                Debug.LogWarning("[PlayerMovement] Walk sound is not assigned. Set 'Walk Snow Sound' in the inspector.");
            }

            return;
        }

        if (walkAudioSource.clip != walkSnowSound)
        {
            walkAudioSource.clip = walkSnowSound;
        }

        if (WalkSoundVolume <= 0f || !isMoving)
        {
            if (walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }

            return;
        }

        walkAudioSource.volume = WalkSoundVolume;
        if (!walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
        }
    }
}
