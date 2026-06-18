using UnityEngine;
using UnityEngine.SceneManagement;

public class PMActions : MonoBehaviour
{

    public void GoToMainMenu()
    {
        Debug.Log("Going to Main Menu...");
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }

}
