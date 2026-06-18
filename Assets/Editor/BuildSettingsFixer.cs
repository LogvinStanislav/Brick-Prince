using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Platformer.EditorTools
{
    /// <summary>
    /// Приводит список сцен в Build Settings к порядку, который нужен игре:
    /// MainMenu первой (запускается при старте билда), затем Level01.
    /// Старая ссылка на несуществующий SampleScene удаляется.
    ///
    /// Запускать через меню Unity: Tools > Brick-Prince > Fix Build Settings Scenes.
    /// </summary>
    public static class BuildSettingsFixer
    {
        [MenuItem("Tools/Brick-Prince/Fix Build Settings Scenes")]
        public static void FixScenes()
        {
            string mainMenuPath = "Assets/Scenes/MainMenu.unity";
            string level01Path = "Assets/Scenes/Level01.unity";

            bool mainMenuExists = System.IO.File.Exists(mainMenuPath);
            bool level01Exists = System.IO.File.Exists(level01Path);

            if (!mainMenuExists)
            {
                Debug.LogWarning($"BuildSettingsFixer: {mainMenuPath} не найден. " +
                    "Сначала запусти Tools > Brick-Prince > Build Main Menu Scene.");
            }
            if (!level01Exists)
            {
                Debug.LogWarning($"BuildSettingsFixer: {level01Path} не найден.");
            }

            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();

            if (mainMenuExists)
                scenes.Add(new EditorBuildSettingsScene(mainMenuPath, true));
            if (level01Exists)
                scenes.Add(new EditorBuildSettingsScene(level01Path, true));

            EditorBuildSettings.scenes = scenes.ToArray();

            Debug.Log("BuildSettingsFixer: список сцен в Build Settings обновлён -> " +
                string.Join(", ", scenes.Select(s => s.path)));
        }
    }
}
