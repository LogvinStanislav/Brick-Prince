using UnityEngine;

namespace Platformer.UI
{
    /// <summary>
    /// Контроллер сцены главного меню. Управляет тем, какая панель
    /// (Main / LevelSelect / Settings) видна, и обрабатывает кнопки
    /// верхнего уровня: Play и Quit.
    /// LevelSelect делегирован отдельному LevelSelectController,
    /// чтобы этот класс не разрастался при добавлении новых уровней.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Панели меню")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject levelSelectPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Настройки")]
        [Tooltip("Сцена, которая загрузится по кнопке Play (обычно первый уровень)")]
        [SerializeField] private string firstLevelSceneName = "Level01";

        /// <summary>
        /// Если true, при следующей загрузке сцены MainMenu сразу откроется
        /// панель выбора уровня, а не главный экран. Используется для
        /// возврата сюда после победы на уровне. Флаг статический, чтобы
        /// пережить смену сцены, и сбрасывается сразу после использования.
        /// </summary>
        public static bool OpenLevelSelectOnNextLoad = false;

        void Start()
        {
            if (OpenLevelSelectOnNextLoad)
            {
                OpenLevelSelectOnNextLoad = false;
                ShowLevelSelectPanel();
            }
            else
            {
                ShowMainPanel();
            }
        }

        public void ShowMainPanel()
        {
            SetActivePanel(mainPanel);
        }

        public void ShowLevelSelectPanel()
        {
            SetActivePanel(levelSelectPanel);
        }

        public void ShowSettingsPanel()
        {
            SetActivePanel(settingsPanel);
        }

        public void OnPlayPressed()
        {
            SceneLoader.Load(firstLevelSceneName);
        }

        public void OnQuitPressed()
        {
            SceneLoader.Quit();
        }

        private void SetActivePanel(GameObject panelToShow)
        {
            if (mainPanel != null) mainPanel.SetActive(mainPanel == panelToShow);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(levelSelectPanel == panelToShow);
            if (settingsPanel != null) settingsPanel.SetActive(settingsPanel == panelToShow);
        }
    }
}
