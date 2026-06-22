using UnityEngine;
using Platformer.UI;
using UnityEngine.SceneManagement;

public class PMActions : MonoBehaviour
{
    [SerializeField] private MetaGameController metaGameController;
    public void ShowDefeatScreen()
    {
        if (metaGameController != null)
            metaGameController.ToggleMainMenu(false);

        if (GameResultUI.Instance != null)
            GameResultUI.Instance.ShowDefeat();
        else
            GoToMainMenu();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

}
