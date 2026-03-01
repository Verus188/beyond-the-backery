using UnityEngine;

public class SnowmanEnemy : Enemy
{
    private static Material explosionParticleMaterial;

    [Header("Snowman Explosion")]
    public float explosionRadius = 1.4f;
    public float explosionDamage = 20f;
    public Color explosionColor = new Color(1f, 0.95f, 0.95f, 1f);
    public int explosionParticles = 20;

    protected override void Die()
    {
        ExplodeOnDeath();
        base.Die();
    }

    private void ExplodeOnDeath()
    {
        DealExplosionDamageToPlayer();
        SpawnExplosionEffect();
    }

    private void DealExplosionDamageToPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (!hit.CompareTag("Player"))
            {
                continue;
            }

            PlayerStats stats = hit.GetComponent<PlayerStats>();
            if (stats == null)
            {
                stats = hit.GetComponentInParent<PlayerStats>();
            }

            if (stats != null)
            {
                stats.TakeDamage(explosionDamage);
                break;
            }
        }
    }

    private void SpawnExplosionEffect()
    {
        if (explosionParticles <= 0)
        {
            return;
        }

        GameObject explosionObject = new GameObject("SnowmanExplosion");
        explosionObject.transform.position = transform.position;

        ParticleSystem ps = explosionObject.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 0.15f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.2f, 0.45f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.8f, 4f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.2f);
        main.startColor = explosionColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = explosionParticles + 10;
        main.gravityModifier = 0.15f;

        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, (short)explosionParticles)
        });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        ParticleSystemRenderer psRenderer = explosionObject.GetComponent<ParticleSystemRenderer>();
        if (psRenderer != null)
        {
            Material material = GetExplosionParticleMaterial();
            if (material != null)
            {
                psRenderer.material = material;
            }
        }

        ps.Play();
        Destroy(explosionObject, 1f);
    }

    private static Material GetExplosionParticleMaterial()
    {
        if (explosionParticleMaterial != null)
        {
            return explosionParticleMaterial;
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
                explosionParticleMaterial = new Material(shader)
                {
                    hideFlags = HideFlags.DontSave
                };
                return explosionParticleMaterial;
            }
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.6f, 0.6f, 0.85f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
