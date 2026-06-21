using UnityEngine;

public class MageController : MonoBehaviour
{
    [Header("Facing")]
    [SerializeField] private bool facingRight = true;

    [Header("Shooting")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float spawnDistance = 1.5f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float fireballSpeed = 6f;
    [SerializeField] private float fireballLifetime = 3f; // настраиваемое время жизни шара

    private float fireTimer;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateFacing();
        fireTimer = fireRate;
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    void Shoot()
    {
        if (fireballPrefab == null) return;

        float direction = facingRight ? 1f : -1f;
        Vector3 spawnPos = transform.position + new Vector3(direction * spawnDistance, 0f, 0f);

        GameObject fireballObj = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        Fireball fireball = fireballObj.GetComponent<Fireball>();
        if (fireball == null)
        {
            fireball = fireballObj.AddComponent<Fireball>();
        }

        fireball.SetLifetime(fireballLifetime);

        Collider2D mageCollider = GetComponent<Collider2D>();
        Collider2D fireballCollider = fireballObj.GetComponent<Collider2D>();
        if (mageCollider != null && fireballCollider != null)
        {
            Physics2D.IgnoreCollision(mageCollider, fireballCollider);
        }

        fireball.Launch(direction * fireballSpeed);
    }

    void UpdateFacing()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !facingRight;
        }
    }
}