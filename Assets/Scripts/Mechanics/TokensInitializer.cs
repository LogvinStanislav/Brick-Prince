using Platformer.Mechanics;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TokensInitializer : MonoBehaviour
{
    [SerializeField] private Transform tokensParent;
    [SerializeField] private GameObject gameController;

    void Awake()
    {
        foreach (Transform child in transform)
        {
            TokenInstance token = child.GetComponent<TokenInstance>();
            if (token != null)
            {
                token.tokensParent = tokensParent;
                token.GameController = gameController;
            }
        }
    }
}