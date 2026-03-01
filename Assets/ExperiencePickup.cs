using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExperiencePickup : MonoBehaviour
{
    private const string CandyCaneSpritePath = "Assets/Sprites/candy cane.png";

    [Header("XP")]
    public float xpAmount = 5f;

    [Header("Pickup")]
    public float lifeTime = 15f;
    public float fadeDuration = 5f;
    public float pickupRadius = 0.35f;

    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private float spawnedAt;

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
    }

    void Start()
    {
        spawnedAt = Time.time;
        baseColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
    }

    void Update()
    {
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
}
