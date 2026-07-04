using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer.UI
{

    public class GameResultUI : MonoBehaviour
    {
        public static GameResultUI Instance { get; private set; }

        [Header("Экраны")]
        [SerializeField] private GameObject victoryScreen;
        [SerializeField] private GameObject defeatScreen;

        [Header("Текст монет (опционально)")]
        [SerializeField] private TextMeshProUGUI victoryCoinsText;
        [SerializeField] private TextMeshProUGUI defeatCoinsText;
        [SerializeField] private GameObject GameController;

        public static bool IsShowingResult { get; private set; }

        void Awake()
        {
            Instance = this;
            IsShowingResult = false;
        }

        public void ShowVictory()
        {
            IsShowingResult = true;
            Time.timeScale = 0f;


            if (victoryCoinsText != null)
                victoryCoinsText.text = $"Coins: + {GameController.GetComponent<TokensCounter>().tokens_collected} / {GameController.GetComponent<TokensCounter>().tokens_number}";

            if (victoryScreen != null)
                victoryScreen.SetActive(true);
        }

        public void ShowDefeat()
        {
            IsShowingResult = true;
            Time.timeScale = 0f;

            if (defeatCoinsText != null)
                defeatCoinsText.text = $"Coins: - {GameController.GetComponent<TokensCounter>().tokens_collected}";

            if (defeatScreen != null)
                defeatScreen.SetActive(true);
        }

        public void GoToMainMenu()
        {
            IsShowingResult = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
