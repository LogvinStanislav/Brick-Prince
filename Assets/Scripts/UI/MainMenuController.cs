using UnityEngine;

namespace Platformer.UI
{

    public class MainMenuController : MonoBehaviour
    {
        [Header("Панели меню")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject levelSelectPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Настройки")]
        [Tooltip("Сцена, которая загрузится по кнопке Play (обычно первый уровень)")]
        [SerializeField] private string firstLevelSceneName = "Level01";


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
