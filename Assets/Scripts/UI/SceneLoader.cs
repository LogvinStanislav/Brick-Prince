using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer.UI
{

    public static class SceneLoader
    {
        public static void Load(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("SceneLoader: попытка загрузить сцену с пустым именем.");
                return;
            }
            
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
