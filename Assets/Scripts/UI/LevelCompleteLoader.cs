using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Platformer.UI
{
    /// <summary>
    /// Небольшой статический помощник для отложенной загрузки главного меню
    /// после победы на уровне. Используется из PlayerEnteredVictoryZone,
    /// который сам не MonoBehaviour и не может запускать корутины —
    /// поэтому этот класс создаёт временный служебный GameObject в сцене.
    /// </summary>
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
