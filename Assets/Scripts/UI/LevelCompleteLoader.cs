using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Platformer.UI
{

    public static class LevelCompleteLoader
    {
        public static void LoadMainMenuAfterDelay(float delaySeconds)
        {
            GameObject runner = new GameObject("LevelCompleteLoaderRunner");
            var behaviour = runner.AddComponent<LevelCompleteLoaderBehaviour>();
            behaviour.StartCoroutine(behaviour.WaitAndLoad(delaySeconds));
        }

        private class LevelCompleteLoaderBehaviour : MonoBehaviour
        {
            public IEnumerator WaitAndLoad(float delaySeconds)
            {
                yield return new WaitForSeconds(delaySeconds);
                Time.timeScale = 1f;
                MainMenuController.OpenLevelSelectOnNextLoad = true;
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
