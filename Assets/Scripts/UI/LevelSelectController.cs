using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer.UI
{

    public class LevelSelectController : MonoBehaviour
    {
        [Header("Данные уровней")]
        [SerializeField] private List<LevelInfo> levels = new List<LevelInfo>
        {
            new LevelInfo { displayName = "Level 1", sceneName = "Level01", isAvailable = true },
            new LevelInfo { displayName = "Level 2", sceneName = "Level02", isAvailable = true },
            new LevelInfo { displayName = "Level 3", sceneName = "Level03", isAvailable = false },
            new LevelInfo { displayName = "Level 4", sceneName = "Level04", isAvailable = false },
            new LevelInfo { displayName = "Level 5", sceneName = "Level05", isAvailable = false },
        };

        [Header("Ссылки")]
        [Tooltip("Префаб кнопки уровня. Должен содержать компонент Button и дочерний Text.")]
        [SerializeField] private Button levelButtonPrefab;

        [Tooltip("Родитель, в который будут добавлены кнопки уровней (например, объект с Vertical/Grid Layout Group)")]
        [SerializeField] private Transform buttonContainer;

        [Tooltip("Кнопка 'Назад' для возврата в главное меню")]
        [SerializeField] private MainMenuController mainMenuController;

        private readonly List<GameObject> spawnedButtons = new List<GameObject>();

        void OnEnable()
        {
            BuildButtons();
        }

        private void BuildButtons()
        {
            ClearButtons();

            if (levelButtonPrefab == null || buttonContainer == null)
            {
                Debug.LogWarning("LevelSelectController: не назначен levelButtonPrefab или buttonContainer.");
                return;
            }

            foreach (var level in levels)
            {
                Button buttonInstance = Instantiate(levelButtonPrefab, buttonContainer);
                spawnedButtons.Add(buttonInstance.gameObject);

                Text label = buttonInstance.GetComponentInChildren<Text>();
                if (label != null) label.text = level.displayName;

                buttonInstance.interactable = level.isAvailable;


                LevelInfo capturedLevel = level;
                buttonInstance.onClick.AddListener(() => SceneLoader.Load(capturedLevel.sceneName));
            }
        }

        private void ClearButtons()
        {
            foreach (var go in spawnedButtons)
            {
                if (go != null) Destroy(go);
            }
            spawnedButtons.Clear();
        }

        public void OnBackPressed()
        {
            if (mainMenuController != null)
            {
                mainMenuController.ShowMainPanel();
            }
        }
    }
}
