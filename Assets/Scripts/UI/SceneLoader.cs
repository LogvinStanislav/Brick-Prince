using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer.UI
{
    /// <summary>
    /// Тонкая обёртка над SceneManager, чтобы вся логика загрузки сцен
    /// (и связанные с ней мелочи, например сброс Time.timeScale) была в одном месте.
    /// </summary>
    public static class SceneLoader
    {
        public static void Load(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("SceneLoader: попытка загрузить сцену с пустым именем.");
                return;
            }

            // На случай, если переход происходит из меню паузы, где Time.timeScale = 0.
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
