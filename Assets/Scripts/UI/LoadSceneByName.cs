using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer.UI
{

    public class LoadSceneByName : MonoBehaviour
    {
        [SerializeField] private string sceneName;

        public void LoadLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
    }
}