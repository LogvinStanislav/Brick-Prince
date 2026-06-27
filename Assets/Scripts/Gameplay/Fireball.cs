using Platformer.Core;
using Platformer.Gameplay;
using Unity.Cinemachine;
using UnityEngine;
using static Platformer.Core.Simulation;

[RequireComponent(typeof(Rigidbody2D))]
public class Fireball : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] private float lifetime = 3f; // врем€ жизни по умолчанию (можно мен€ть в инспекторе префаба)

    [Header("Explosion")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private AudioClip explosionSound;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool hasExploded = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // ѕозвол€ет переопределить врем€ жизни программно (например, из MageController)
    public void SetLifetime(float newLifetime)
    {
        lifetime = newLifetime;
    }

    public void Launch(float velocityX)
    {
        rb.linearVelocity = new Vector2(velocityX, 0f);

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = velocityX < 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject);
    }

    void HandleHit(GameObject hitObject)
    {
        if (hasExploded) return;

        if (hitObject.name == "CinemachineConfiner")
        {
            return;
        }

        if (hitObject.CompareTag("Player"))
        {
            Schedule<PlayerDeath>();
            Explode();
            return;
        }
        if (!hitObject.CompareTag("Enemy") && !hitObject.CompareTag("Fireball"))
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Vector3 explodePos = transform.position;

        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, explodePos, Quaternion.identity);
            Destroy(effect, 2f);
        }

        if (explosionSound != null)
        {
            GameObject tempAudio = new GameObject("ExplosionSound");
            tempAudio.transform.position = explodePos;
            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
            audioSource.clip = explosionSound;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 1f;
            audioSource.Play();
            Destroy(tempAudio, explosionSound.length);
        }

        Destroy(gameObject);
    }
}