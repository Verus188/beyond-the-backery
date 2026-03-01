using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    [Header("Audio")]
    public AudioClip shootingSound;
    public AudioClip walkSnowSound;
    // Change this value in code; prefab/inspector values will not override it.
    private const float ShootVolume = 0.4f;
    private const float WalkSoundVolume = 0.2f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private AudioSource shootAudioSource;
    private AudioSource walkAudioSource;
    private bool missingShootSoundWarningShown;
    private bool missingWalkSoundWarningShown;

    void Start()
    {
        mainCamera = Camera.main;
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

        if (ShootVolume <= 0f)
        {
            return;
        }

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
