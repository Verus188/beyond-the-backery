using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExperiencePickup : MonoBehaviour
{
    private const string CandyCaneSpritePath = "Assets/Sprites/candy cane.png";
    private const string TakeCandySoundPath = "Assets/Sounds/takeCandySound.mp3";

    [Header("XP")]
    public float xpAmount = 5f;

    [Header("Pickup")]
    public float lifeTime = 15f;
    public float fadeDuration = 5f;
    public float pickupRadius = 0.35f;
    public float attractionRadius = 2f;
    public float attractionSpeed = 4f;
    public float hoverAmplitude = 0.08f;
    public float hoverFrequency = 2.2f;
    public AudioClip takeCandySound;
    private const float TakeCandyVolume = 0.2f;

    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private float spawnedAt;
    private Vector3 basePosition;
    private float hoverPhase;
    private Transform playerTransform;

    public static ExperiencePickup Spawn(Vector3 position, float xpAmount, SpriteRenderer sourceRenderer = null)
    {
        GameObject pickupObject = new GameObject("ExperiencePickup");
        pickupObject.transform.position = position;

        ExperiencePickup pickup = pickupObject.AddComponent<ExperiencePickup>();
        pickup.xpAmount = xpAmount;

        if (sourceRenderer != null && pickup.spriteRenderer != null)
        {
            pickup.spriteRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
            pickup.spriteRenderer.sortingOrder = sourceRenderer.sortingOrder;
        }

        return pickup;
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        CircleCollider2D trigger = GetComponent<CircleCollider2D>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<CircleCollider2D>();
        }
        trigger.isTrigger = true;
        trigger.radius = pickupRadius;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.simulated = true;

        AssignCandyCaneSpriteIfNeeded();
        AssignTakeCandySoundIfNeeded();
    }

    void Start()
    {
        spawnedAt = Time.time;
        baseColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        basePosition = transform.position;
        hoverPhase = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        TryResolvePlayer();
        ApplyAttraction();

        float hoverOffset = Mathf.Sin((Time.time * hoverFrequency) + hoverPhase) * hoverAmplitude;
        transform.position = basePosition + Vector3.up * hoverOffset;

        float elapsed = Time.time - spawnedAt;
        float fadeStart = Mathf.Max(0f, lifeTime - fadeDuration);

        if (spriteRenderer != null && elapsed >= fadeStart)
        {
            float fadeProgress = Mathf.Clamp01((elapsed - fadeStart) / Mathf.Max(0.0001f, fadeDuration));
            float alpha = Mathf.Lerp(baseColor.a, 0f, fadeProgress);
            spriteRenderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        }

        if (elapsed >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void TryResolvePlayer()
    {
        if (playerTransform != null) return;

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    private void ApplyAttraction()
    {
        if (playerTransform == null || attractionRadius <= 0f || attractionSpeed <= 0f)
        {
            return;
        }

        Vector3 targetPosition = playerTransform.position;
        targetPosition.z = basePosition.z;

        float distance = Vector3.Distance(basePosition, targetPosition);
        if (distance > attractionRadius)
        {
            return;
        }

        basePosition = Vector3.MoveTowards(basePosition, targetPosition, attractionSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats == null)
        {
            stats = other.GetComponentInParent<PlayerStats>();
        }

        if (stats == null) return;

        stats.AddXP(xpAmount);
        PlayTakeCandySound();
        Destroy(gameObject);
    }

    private void AssignCandyCaneSpriteIfNeeded()
    {
        if (spriteRenderer == null || spriteRenderer.sprite != null) return;

#if UNITY_EDITOR
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(CandyCaneSpritePath);
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
            return;
        }
#endif

        Debug.LogWarning("[ExperiencePickup] Candy cane sprite not found. Assign a sprite manually.");
    }

    private void AssignTakeCandySoundIfNeeded()
    {
        if (takeCandySound != null) return;

#if UNITY_EDITOR
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(TakeCandySoundPath);
        if (clip != null)
        {
            takeCandySound = clip;
            return;
        }
#endif

        Debug.LogWarning("[ExperiencePickup] Take candy sound not found. Assign an AudioClip manually.");
    }

    private void PlayTakeCandySound()
    {
        if (takeCandySound == null || TakeCandyVolume <= 0f) return;

        GameObject soundObject = new GameObject("TakeCandySound");
        soundObject.transform.position = transform.position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.loop = false;
        source.volume = TakeCandyVolume;
        source.clip = takeCandySound;
        source.Play();

        Destroy(soundObject, takeCandySound.length + 0.05f);
    }
}
