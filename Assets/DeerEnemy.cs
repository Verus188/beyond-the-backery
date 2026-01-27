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

        SpriteRenderer sr = hornsObj.AddComponent<SpriteRenderer>();
        sr.sprite = hornsSprite;
        
        if (spriteRenderer != null)
        {
            sr.sortingLayerID = spriteRenderer.sortingLayerID;
            sr.sortingOrder = spriteRenderer.sortingOrder - 1;
        }
    }
}
