using Platformer.Core;
using Platformer.Gameplay;
using Platformer.Mechanics;
using UnityEngine;
using static Platformer.Core.Simulation;

public class FallingBlock : MonoBehaviour
{
    private BlockSpawner spawner;
    private bool hasLanded = false;
    private float fallSpeed;
    private float moveSpeed;
    private Vector2 velocity;
    private Rigidbody2D rb;

    [Header("Shatter Effect")]
    [SerializeField] private int shatterRows = 10;
    [SerializeField] private int shatterCols = 10;
    [SerializeField] private float shardForce = 1f;
    [SerializeField] private float shardLifetime = 1.5f;

    public AudioClip destroySound;

    [Header("Rotation")]
    [SerializeField] private float rotationDuration = 0.15f;
    private bool isRotating = false;

    public void Rotate90()
    {
        if (hasLanded || isRotating) return;
        StartCoroutine(RotateRoutine());
    }

    private System.Collections.IEnumerator RotateRoutine()
    {
        isRotating = true;

        float startAngle = transform.eulerAngles.z;
        float targetAngle = startAngle - 90f; // по часовой стрелке
        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rotationDuration;
            float currentAngle = Mathf.LerpAngle(startAngle, targetAngle, t);
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        isRotating = false;
    }

    public void Initialize(BlockSpawner blockSpawner, float fall, float move)
    {
        spawner = blockSpawner;
        fallSpeed = fall;
        moveSpeed = move;
        velocity = new Vector2(0, -fallSpeed);

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void UpdateMovement(float horizontalInput, bool fastFall)
    {
        if (hasLanded) return;

        velocity.x = horizontalInput * moveSpeed;
        velocity.y = fastFall ? -fallSpeed * 5f : -fallSpeed;

        rb.linearVelocity = velocity;
    }

    public void DestroyByGolem()
    {
        if (hasLanded)
        {
            PlayDestroySound();
            ShatterSprite();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {


        if (collision.gameObject.CompareTag("Golem"))
        {
            Debug.Log($"Block touch");
            if (!hasLanded)
            {
                LandBlock();
            }
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            OnEnemyHit(collision.gameObject);
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerHit(collision.gameObject);
        }

        if (hasLanded) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                LandBlock();
                break;
            }
            else if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                LandBlock();
                break;
            }
        }
    }

    void OnEnemyHit(GameObject enemy)
    {
        if (!hasLanded)
        {
            Schedule<EnemyDeath>().enemy = enemy.GetComponent<EnemyController>();

            PlayDestroySound();
            ShatterSprite();

            if (spawner != null)
            {
                spawner.OnBlockLanded();
            }
        }
    }

    void OnPlayerHit(GameObject player)
    {

        if (!hasLanded)
        {
            Schedule<PlayerDeath>();
        }
    }

    void LandBlock()
    {
        if (hasLanded) return;

        hasLanded = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Dynamic;

        spawner.OnBlockLanded();

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = sprite.color * 0.85f;
        }
    }

    void PlayDestroySound()
    {
        if (destroySound == null) return;

        GameObject tempAudio = new GameObject("DestroySound");
        tempAudio.transform.position = transform.position;
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.clip = destroySound;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
        audioSource.Play();
        Destroy(tempAudio, destroySound.length);
    }

    void ShatterSprite()
    {
        SpriteRenderer originalSprite = GetComponent<SpriteRenderer>();
        if (originalSprite == null || originalSprite.sprite == null)
        {
            Destroy(gameObject);
            return;
        }

        Texture2D texture = originalSprite.sprite.texture;
        Rect spriteRect = originalSprite.sprite.rect;

        float pieceWidth = spriteRect.width / shatterCols / 5;
        float pieceHeight = spriteRect.height / shatterRows / 5;
        float pixelsPerUnit = originalSprite.sprite.pixelsPerUnit;

        // Размер всего спрайта в мировых единицах
        Vector2 fullSize = originalSprite.bounds.size;
        Vector3 originalScale = transform.localScale;

        for (int y = 0; y < shatterRows; y++)
        {
            for (int x = 0; x < shatterCols; x++)
            {
                Rect rect = new Rect(
                    spriteRect.x + x * pieceWidth,
                    spriteRect.y + y * pieceHeight,
                    pieceWidth,
                    pieceHeight
                );

                Sprite pieceSprite = Sprite.Create(
                    texture,
                    rect,
                    new Vector2(0.5f, 0.5f),
                    pixelsPerUnit
                );

                // Смещение куска относительно центра исходного блока
                float offsetX = (x - (shatterCols - 1) / 2f) * (fullSize.x / shatterCols);
                float offsetY = (y - (shatterRows - 1) / 2f) * (fullSize.y / shatterRows);
                Vector3 piecePos = transform.position + new Vector3(offsetX, offsetY, 0);

                GameObject piece = new GameObject("Shard");
                piece.transform.position = piecePos;
                piece.transform.localScale = originalScale;

                SpriteRenderer sr = piece.AddComponent<SpriteRenderer>();
                sr.sprite = pieceSprite;
                sr.sortingLayerID = originalSprite.sortingLayerID;
                sr.sortingOrder = originalSprite.sortingOrder;

                Rigidbody2D rbPiece = piece.AddComponent<Rigidbody2D>();
                Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(0.3f, 1f)).normalized;
                rbPiece.AddForce(dir * Random.Range(shardForce * 0.5f, shardForce), ForceMode2D.Impulse);
                rbPiece.AddTorque(Random.Range(-200f, 200f));

                Destroy(piece, shardLifetime);
            }
        }

        Destroy(gameObject);
    }
}