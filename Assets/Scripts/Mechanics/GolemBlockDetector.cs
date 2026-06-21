using UnityEngine;

public class GolemBlockDetector : MonoBehaviour
{
    private GolemController golem;

    void Start()
    {
        golem = GetComponentInParent<GolemController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            golem.OnBlockDetected(other.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            golem.OnBlockDetected(other.gameObject);
        }
    }
}