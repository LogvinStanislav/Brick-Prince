using UnityEngine;

namespace Platformer.Mechanics
{

    public partial class PatrolPath : MonoBehaviour
    {
 
        public Vector2 startPosition, endPosition;


        public Mover CreateMover(float speed = 1) => new Mover(this, speed);

        void Reset()
        {
            startPosition = Vector3.left;
            endPosition = Vector3.right;
        }
    }
}