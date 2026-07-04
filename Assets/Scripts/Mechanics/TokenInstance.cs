using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;


namespace Platformer.Mechanics
{

    [RequireComponent(typeof(Collider2D))]

    public class TokenInstance : MonoBehaviour
    {
        public AudioClip tokenCollectAudio;
        [Tooltip("If true, animation will start at a random position in the sequence.")]
        public bool randomAnimationStartTime = false;
        [Tooltip("List of frames that make up the animation.")]
        public Sprite[] idleAnimation, collectedAnimation;

        internal Sprite[] sprites = new Sprite[0];

        internal SpriteRenderer _renderer;

        internal int tokenIndex = -1;
        internal TokenController controller;
        internal int frame = 0;
        internal bool collected = false;

        [SerializeField] public GameObject GameController;
        [SerializeField] public Transform tokensParent;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            if (randomAnimationStartTime)
                frame = Random.Range(0, sprites.Length);
            sprites = idleAnimation;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null) OnPlayerEnter(player);
        }

        void OnPlayerEnter(PlayerController player)
        {
            if (collected) return;
            frame = 0;
            sprites = collectedAnimation;
            if (controller != null)
                collected = true;

            if (GameController != null)
            {
                GameController.GetComponent<TokensCounter>().tokens_collected++;
            }
            var ev = Schedule<PlayerTokenCollision>();
            ev.token = this;
            ev.player = player;
        }
    }
}