using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuGameManager : MonoBehaviour
{
    public void StartGameAction()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void ExitGameAction()
    {
        Application.Quit();
    }
}
