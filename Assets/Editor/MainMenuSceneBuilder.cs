using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEditor.Events;

namespace Platformer.EditorTools
{
    /// <summary>
    /// Создаёт сцену MainMenu.unity со всей нужной иерархией UI:
    /// Canvas, EventSystem, панели Main/LevelSelect/Settings, кнопки.
    /// Запускать через меню Unity: Tools > Brick-Prince > Build Main Menu Scene.
    ///
    /// Это сознательно сделано как генератор, а не готовый .unity файл,
    /// чтобы сцену было легко пересобрать с нуля, если что-то поправили
    /// в структуре, без риска повредить вручную отредактированный YAML.
    /// </summary>
    public static class MainMenuSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/MainMenu.unity";

        [MenuItem("Tools/Brick-Prince/Build Main Menu Scene")]
        public static void BuildScene()
        {
            if (System.IO.File.Exists(ScenePath))
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    "Сцена уже существует",
                    $"Файл {ScenePath} уже существует. Перезаписать его новой сгенерированной версией?\n\nВнимание: все ручные правки в этой сцене будут потеряны.",
                    "Перезаписать",
                    "Отмена");

                if (!overwrite) return;
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // --- Camera ---
            // Canvas в режиме Screen Space - Overlay технически не нуждается
            // в камере для отрисовки UI, но Unity всё равно требует хотя бы
            // одну камеру на сцене, иначе показывает оверлей
            // "No cameras rendering" поверх всего интерфейса.
            GameObject cameraGO = new GameObject("Main Camera");
            cameraGO.tag = "MainCamera";
            Camera camera = cameraGO.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.1f, 0.1f, 0.12f, 1f);
            cameraGO.AddComponent<AudioListener>();

            // --- EventSystem (нужен для работы UI с новым Input System) ---
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<InputSystemUIInputModule>();

            // --- Canvas ---
            GameObject canvasGO = new GameObject("Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGO.AddComponent<GraphicRaycaster>();

            // --- Controller ---
            GameObject controllerGO = new GameObject("MainMenuController");
            var menuController = controllerGO.AddComponent<Platformer.UI.MainMenuController>();

            // --- Панели ---
            GameObject mainPanel = BuildMainPanel(canvasGO.transform, menuController);
            GameObject levelSelectPanel = BuildLevelSelectPanel(canvasGO.transform, menuController, out Button levelButtonPrefabInstance);
            GameObject settingsPanel = BuildSettingsPanel(canvasGO.transform, menuController);

            // Привязываем панели к контроллеру через SerializedObject,
            // чтобы это сработало даже для private [SerializeField] полей.
            SerializedObject so = new SerializedObject(menuController);
            so.FindProperty("mainPanel").objectReferenceValue = mainPanel;
            so.FindProperty("levelSelectPanel").objectReferenceValue = levelSelectPanel;
            so.FindProperty("settingsPanel").objectReferenceValue = settingsPanel;
            so.FindProperty("firstLevelSceneName").stringValue = "Level01";
            so.ApplyModifiedProperties();

            mainPanel.SetActive(true);
            levelSelectPanel.SetActive(false);
            settingsPanel.SetActive(false);

            EditorSceneManager.SaveScene(scene, ScenePath);
            Debug.Log($"MainMenu сцена создана и сохранена: {ScenePath}");
        }

        private static GameObject BuildMainPanel(Transform canvasParent, Platformer.UI.MainMenuController menuController)
        {
            GameObject panel = CreatePanel("MainPanel", canvasParent);

            CreateLabel("Title", panel.transform, "BRICK PRINCE", 64, new Vector2(0, 250), new Vector2(800, 120));

            GameObject playBtn = CreateButton("PlayButton", panel.transform, "Play", new Vector2(0, 60));
            GameObject levelSelectBtn = CreateButton("LevelSelectButton", panel.transform, "Level Select", new Vector2(0, -20));
            GameObject settingsBtn = CreateButton("SettingsButton", panel.transform, "Settings", new Vector2(0, -100));
            GameObject quitBtn = CreateButton("QuitButton", panel.transform, "Quit", new Vector2(0, -180));

            AddPersistentClick(playBtn, menuController, nameof(Platformer.UI.MainMenuController.OnPlayPressed));
            AddPersistentClick(levelSelectBtn, menuController, nameof(Platformer.UI.MainMenuController.ShowLevelSelectPanel));
            AddPersistentClick(settingsBtn, menuController, nameof(Platformer.UI.MainMenuController.ShowSettingsPanel));
            AddPersistentClick(quitBtn, menuController, nameof(Platformer.UI.MainMenuController.OnQuitPressed));

            return panel;
        }

        private static GameObject BuildLevelSelectPanel(Transform canvasParent, Platformer.UI.MainMenuController menuController, out Button levelButtonPrefabInstance)
        {
            GameObject panel = CreatePanel("LevelSelectPanel", canvasParent);

            CreateLabel("Title", panel.transform, "SELECT LEVEL", 48, new Vector2(0, 350), new Vector2(800, 100));

            // Контейнер с вертикальной раскладкой для кнопок уровней.
            GameObject container = new GameObject("ButtonContainer", typeof(RectTransform));
            container.transform.SetParent(panel.transform, false);
            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.sizeDelta = new Vector2(400, 400);
            containerRect.anchoredPosition = new Vector2(0, 20);

            VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // Шаблонная кнопка уровня. Создаётся неактивной в иерархии,
            // используется только как prefab-ссылка для LevelSelectController,
            // который инстанцирует копии в runtime.
            GameObject levelButtonTemplate = CreateButton("LevelButtonTemplate", panel.transform, "Level", Vector2.zero);
            levelButtonTemplate.SetActive(false);
            levelButtonPrefabInstance = levelButtonTemplate.GetComponent<Button>();

            GameObject backBtn = CreateButton("BackButton", panel.transform, "Back", new Vector2(0, -380));

            GameObject controllerGO = new GameObject("LevelSelectController");
            var levelSelectController = controllerGO.AddComponent<Platformer.UI.LevelSelectController>();
            controllerGO.transform.SetParent(panel.transform, false);

            SerializedObject so = new SerializedObject(levelSelectController);
            so.FindProperty("levelButtonPrefab").objectReferenceValue = levelButtonPrefabInstance;
            so.FindProperty("buttonContainer").objectReferenceValue = container.transform;
            so.FindProperty("mainMenuController").objectReferenceValue = menuController;
            so.ApplyModifiedProperties();

            AddPersistentClick(backBtn, levelSelectController, nameof(Platformer.UI.LevelSelectController.OnBackPressed));

            return panel;
        }

        private static GameObject BuildSettingsPanel(Transform canvasParent, Platformer.UI.MainMenuController menuController)
        {
            GameObject panel = CreatePanel("SettingsPanel", canvasParent);

            CreateLabel("Title", panel.transform, "SETTINGS", 48, new Vector2(0, 250), new Vector2(800, 100));
            CreateLabel("VolumeLabel", panel.transform, "Volume", 28, new Vector2(0, 80), new Vector2(400, 60));

            GameObject sliderGO = new GameObject("VolumeSlider", typeof(RectTransform));
            sliderGO.transform.SetParent(panel.transform, false);
            RectTransform sliderRect = sliderGO.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
            sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
            sliderRect.sizeDelta = new Vector2(400, 30);
            sliderRect.anchoredPosition = new Vector2(0, 20);
            Slider slider = sliderGO.AddComponent<Slider>();
            BuildSliderVisuals(sliderGO, slider);
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;

            GameObject backBtn = CreateButton("BackButton", panel.transform, "Back", new Vector2(0, -150));

            GameObject controllerGO = new GameObject("SettingsController");
            var settingsController = controllerGO.AddComponent<Platformer.UI.SettingsController>();
            controllerGO.transform.SetParent(panel.transform, false);

            SerializedObject so = new SerializedObject(settingsController);
            so.FindProperty("volumeSlider").objectReferenceValue = slider;
            so.FindProperty("mainMenuController").objectReferenceValue = menuController;
            so.ApplyModifiedProperties();

            AddPersistentClick(backBtn, settingsController, nameof(Platformer.UI.SettingsController.OnBackPressed));

            return panel;
        }

        // ---------- Вспомогательные методы построения UI ----------

        private static GameObject CreatePanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform));
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.12f, 1f);

            return panel;
        }

        private static GameObject CreateLabel(string name, Transform parent, string text, int fontSize, Vector2 anchoredPos, Vector2 size)
        {
            GameObject labelGO = new GameObject(name, typeof(RectTransform));
            labelGO.transform.SetParent(parent, false);
            RectTransform rect = labelGO.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPos;

            Text label = labelGO.AddComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = fontSize;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;

            return labelGO;
        }

        private static GameObject CreateButton(string name, Transform parent, string label, Vector2 anchoredPos)
        {
            GameObject buttonGO = new GameObject(name, typeof(RectTransform));
            buttonGO.transform.SetParent(parent, false);
            RectTransform rect = buttonGO.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(280, 60);
            rect.anchoredPosition = anchoredPos;

            Image image = buttonGO.AddComponent<Image>();
            image.color = new Color(0.85f, 0.85f, 0.85f, 1f);

            Button button = buttonGO.AddComponent<Button>();
            button.targetGraphic = image;

            GameObject textGO = new GameObject("Text", typeof(RectTransform));
            textGO.transform.SetParent(buttonGO.transform, false);
            RectTransform textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            Text buttonText = textGO.AddComponent<Text>();
            buttonText.text = label;
            buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            buttonText.fontSize = 28;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.black;

            return buttonGO;
        }

        private static void BuildSliderVisuals(GameObject sliderGO, Slider slider)
        {
            GameObject background = new GameObject("Background", typeof(RectTransform));
            background.transform.SetParent(sliderGO.transform, false);
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0.25f);
            bgRect.anchorMax = new Vector2(1, 0.75f);
            bgRect.sizeDelta = Vector2.zero;
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);

            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderGO.transform, false);
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.sizeDelta = Vector2.zero;

            GameObject fill = new GameObject("Fill", typeof(RectTransform));
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0, 0);
            fillRect.anchorMax = new Vector2(1, 1);
            fillRect.sizeDelta = Vector2.zero;
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color(0.3f, 0.6f, 0.9f, 1f);

            GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(sliderGO.transform, false);
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.sizeDelta = Vector2.zero;

            GameObject handle = new GameObject("Handle", typeof(RectTransform));
            handle.transform.SetParent(handleArea.transform, false);
            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 0);
            Image handleImage = handle.AddComponent<Image>();
            handleImage.color = Color.white;

            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;
        }

        /// <summary>
        /// Добавляет persistent-вызов метода без параметров в Button.onClick,
        /// так чтобы он был виден и редактируем в инспекторе Unity, как если
        /// бы кнопка была настроена руками.
        /// </summary>
        private static void AddPersistentClick(GameObject buttonGO, Object target, string methodName)
        {
            Button button = buttonGO.GetComponent<Button>();
            UnityEventTools.AddVoidPersistentListener(button.onClick, (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(
                typeof(UnityEngine.Events.UnityAction), target, methodName));
        }
    }
}
