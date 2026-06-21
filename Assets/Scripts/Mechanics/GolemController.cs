using Platformer.Core;
using Platformer.Gameplay;
using Platformer.Mechanics;
using System;
using UnityEngine;
using static Platformer.Core.Simulation;

[RequireComponent(typeof(AnimationController), typeof(Collider2D))]
public class GolemController : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    public PatrolPath path;
    internal PatrolPath.Mover mover;
    internal AnimationController control;

    [Header("Top Detection")]
    [SerializeField] private float topTolerance = 0.1f;

    private Collider2D golemCollider;
    private Rigidbody2D playerRb = null;
    private Vector3 previousPosition;

    void Awake()
    {
        control = GetComponent<AnimationController>();
        golemCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        if (path != null)
        {
            if (mover == null) mover = path.CreateMover(control.maxSpeed * patrolSpeed);
            control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
        }
    }

    void FixedUpdate()
    {
        MovePlayerWithGolem();
        previousPosition = transform.position;
    }

    void MovePlayerWithGolem()
    {
        if (playerRb == null) return;

        Vector3 deltaMovement = transform.position - previousPosition;
        playerRb.position += (Vector2)deltaMovement;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckPlayerCollision(collision);
        CheckBlockCollision(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        CheckPlayerCollision(collision);
        CheckBlockCollision(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = null;
        }
    }

    void CheckPlayerCollision(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Collider2D playerCollider = collision.collider;
        if (playerCollider == null || golemCollider == null) return;

        bool isOnTop = playerCollider.bounds.min.y >= golemCollider.bounds.max.y - topTolerance;

        if (!isOnTop)
        {
            Schedule<PlayerDeath>();
            playerRb = null;
        }
        else
        {
            playerRb = collision.rigidbody;
        }
    }

    void CheckBlockCollision(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Block")) return;

        FallingBlock block = collision.gameObject.GetComponent<FallingBlock>();
        if (block != null)
        {
            block.DestroyByGolem();
        }
    }

    public void OnBlockDetected(GameObject blockObject)
    {
        FallingBlock block = blockObject.GetComponentInParent<FallingBlock>();
        if (block != null)
        {
            block.DestroyByGolem();
        }
    }
}