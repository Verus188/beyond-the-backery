using UnityEngine;

public class HornsDamage : MonoBehaviour
{
    public float damage = 10f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats == null)
        {
            stats = other.GetComponentInParent<PlayerStats>();
        }

        if (stats == null) return;

        stats.TakeDamage(damage);
        Destroy(gameObject);
    }
}
