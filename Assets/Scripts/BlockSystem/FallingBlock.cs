using UnityEngine;
using Platformer.Gameplay;
using Platformer.Core;
using static Platformer.Core.Simulation;

public class FallingBlock : MonoBehaviour
{
    private BlockSpawner spawner;
    private bool hasLanded = false;
    private float fallSpeed;
    private float moveSpeed;
    private Vector2 velocity;
    private Rigidbody2D rb;

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
        
        // Отключаем встроенную гравитацию
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void UpdateMovement(float horizontalInput, bool fastFall)
    {
        if (hasLanded) return;

        // Горизонтальное движение
        velocity.x = horizontalInput * moveSpeed;
        
        // Вертикальное движение (своя гравитация)
        velocity.y = fastFall ? -fallSpeed * 5f : -fallSpeed;
        
        rb.linearVelocity = velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверка взаимодействия с врагом
        if (collision.gameObject.CompareTag("Enemy"))
        {
            OnEnemyHit(collision.gameObject);
            return;
        }

        // Проверка взаимодействия с игроком
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerHit(collision.gameObject);
            // Не делаем return, блок продолжает падать
        }

        // Проверка приземления (пол или стена)
        if (hasLanded) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Столкновение снизу (пол)
            if (contact.normal.y > 0.5f)
            {
                LandBlock();
                break;
            }
            // Столкновение сбоку (стена)
            else if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                LandBlock();
                break;
            }
        }
    }

    void OnEnemyHit(GameObject enemy)
    {
        Debug.Log($"Block hit enemy: {enemy.name}");
        
        // Уничтожаем врага
        Destroy(enemy);
        
        // Уничтожаем блок
        Destroy(gameObject);
        
        // Уведомляем spawner что блок больше не активен
        if (spawner != null)
        {
            spawner.OnBlockLanded();
        }
    }

    void OnPlayerHit(GameObject player)
    {
        Debug.Log($"Block touched player: {player.name}");
        
        // Наносим урон игроку если блок падает (не приземлился)
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
        rb.bodyType = RigidbodyType2D.Static;
        
        spawner.OnBlockLanded();
        
        // Визуальная индикация приземления
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = sprite.color * 0.85f;
        }
    }
}