using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Mechanics
{

    public class KinematicObject : MonoBehaviour
    {

        public float minGroundNormalY = .65f;

        public float gravityModifier = 1f;


        public Vector2 velocity;


        public bool IsGrounded { get; private set; }

        protected Vector2 targetVelocity;
        protected Vector2 groundNormal;
        protected Rigidbody2D body;
        protected ContactFilter2D contactFilter;
        protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

        protected const float minMoveDistance = 0.001f;
        protected const float shellRadius = 0.01f;



        public void Bounce(float value)
        {
            velocity.y = value;
        }


        /// <param name="dir"></param>
        public void Bounce(Vector2 dir)
        {
            velocity.y = dir.y;
            velocity.x = dir.x;
        }


        /// <param name="position"></param>
        public void Teleport(Vector3 position)
        {
            body.position = position;
            velocity *= 0;
            body.linearVelocity *= 0;
        }

        protected virtual void OnEnable()
        {
            body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
        }

        protected virtual void OnDisable()
        {
            body.bodyType = RigidbodyType2D.Dynamic;
        }

        protected virtual void Start()
        {
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            contactFilter.useLayerMask = true;
        }

        protected virtual void Update()
        {
            targetVelocity = Vector2.zero;
            ComputeVelocity();
        }

        protected virtual void ComputeVelocity()
        {

        }

        protected virtual void FixedUpdate()
        {
            if (velocity.y < 0)
                velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
            else
                velocity += Physics2D.gravity * Time.deltaTime;

            velocity.x = targetVelocity.x;

            IsGrounded = false;

            var deltaPosition = velocity * Time.deltaTime;

            var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

            var move = moveAlongGround * deltaPosition.x;

            PerformMovement(move, false);

            move = Vector2.up * deltaPosition.y;

            PerformMovement(move, true);

        }

        void PerformMovement(Vector2 move, bool yMovement)
        {
            var distance = move.magnitude;

            if (distance > minMoveDistance)
            {
                var count = body.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
                for (var i = 0; i < count; i++)
                {
                    var currentNormal = hitBuffer[i].normal;

                    if (currentNormal.y > minGroundNormalY)
                    {
                        IsGrounded = true;
                        if (yMovement)
                        {
                            groundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    }
                    if (IsGrounded)
                    {
                        var projection = Vector2.Dot(velocity, currentNormal);
                        if (projection < 0)
                        {
                            velocity = velocity - projection * currentNormal;
                        }
                    }
                    else
                    {
                        bool isCeiling = currentNormal.y < -0.1f;
                        bool isWall = Mathf.Abs(currentNormal.x) > 0.1f && !isCeiling;

                        if (isCeiling)
                        {
                            velocity.y = Mathf.Min(velocity.y, 0);
                        }
                        if (isWall)
                        {
                            velocity.x = 0;
                        }
                    }
                    var modifiedDistance = hitBuffer[i].distance - shellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }
            body.position = body.position + move.normalized * distance;
        }

    }
}