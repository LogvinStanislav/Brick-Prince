using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer.UI
{
    /// <summary>
    /// Управляет экранами победы и поражения.
    /// Объект должен быть в корне Hierarchy (не внутри выключенного Canvas),
    /// чтобы Awake() вызвался при старте сцены.
    /// </summary>
    public class GameResultUI : MonoBehaviour
    {
        public static GameResultUI Instance { get; private set; }

        [Header("Экраны")]
        [SerializeField] private GameObject victoryScreen;
        [SerializeField] private GameObject defeatScreen;

        [Header("Текст монет (опционально)")]
        [SerializeField] private TextMeshProUGUI victoryCoinsText;
        [SerializeField] private TextMeshProUGUI defeatCoinsText;

        public static bool IsShowingResult { get; private set; }

        void Awake()
        {
            Instance = this;
            IsShowingResult = false;

            // Убеждаемся что оба экрана выключены при старте
            if (victoryScreen != null) victoryScreen.SetActive(false);
            if (defeatScreen != null) defeatScreen.SetActive(false);
        }

        public void ShowVictory(int coins = 0)
        {
            IsShowingResult = true;
            Time.timeScale = 0f;

            if (victoryCoinsText != null)
                victoryCoinsText.text = $"Coins: {coins}";

            if (victoryScreen != null)
                victoryScreen.SetActive(true);
        }

        public void ShowDefeat(int coins = 0)
        {
            IsShowingResult = true;
            Time.timeScale = 0f;

            if (defeatCoinsText != null)
                defeatCoinsText.text = $"Coins: {coins}";

            if (defeatScreen != null)
                defeatScreen.SetActive(true);
        }

        // Вызывается кнопкой Continue на обоих экранах
        public void GoToMainMenu()
        {
            IsShowingResult = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
