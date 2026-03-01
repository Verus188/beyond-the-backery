using UnityEngine;

public class DeerEnemy : Enemy
{
    [Header("Deer Specifics")]
    [Tooltip("The sprite to spawn when horns are dropped.")]
    public Sprite hornsSprite;

    [Tooltip("The name of the boolean parameter in the Animator to switch to the no-horns animation.")]
    public string noHornsParameter = "NoHorns";

    private bool hasDroppedHorns = false;

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (health == 1 && !hasDroppedHorns)
        {
            DropHorns();
        }
    }

    private void DropHorns()
    {
        hasDroppedHorns = true;

        if (animator != null)
        {
            animator.SetBool(noHornsParameter, true);
        }

        SpawnHorns();
    }

    private void SpawnHorns()
    {
        if (hornsSprite == null)
        {
            Debug.LogWarning("[DeerEnemy] Horns Sprite is not assigned!");
            return;
        }

        GameObject hornsObj = new GameObject("DroppedHorns");
        hornsObj.transform.position = transform.position;
        hornsObj.transform.localScale = Vector3.one * 2.25f;

        SpriteRenderer sr = hornsObj.AddComponent<SpriteRenderer>();
        sr.sprite = hornsSprite;

        CircleCollider2D trigger = hornsObj.AddComponent<CircleCollider2D>();
        trigger.isTrigger = true;

        Rigidbody2D rb = hornsObj.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        rb.gravityScale = 0f;
        
        if (spriteRenderer != null)
        {
            sr.sortingLayerID = spriteRenderer.sortingLayerID;
            sr.sortingOrder = spriteRenderer.sortingOrder - 1;
        }

        HornsDamage hornsDamage = hornsObj.AddComponent<HornsDamage>();
        hornsDamage.damage = 10f;

        AutoFadeHorns fadeScript = hornsObj.AddComponent<AutoFadeHorns>();
        fadeScript.totalLifetime = 20f;
        fadeScript.fadeDuration = 5f;
    }
}
