using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour {
    public Toggle tutorialToggle;

    public void StartTutorialOrGame()
    {
        //PlayerPrefs.DeleteAll();
        GameStateHandler.tutorialToggle = tutorialToggle;
        if (PlayerPrefs.GetInt("TutorialSeen") == 1)
            GameStateHandler.SetGameState(GameState.GAME);
        else {
            GameStateHandler.SetGameState(GameState.TUTORIAL);
        }

    }

    public void StartGame()
    {
        GameStateHandler.SetGameState(GameState.GAME);
    }


    public void OpenMenu()
    {
        GameStateHandler.SetGameState(GameState.MENU);
    }


    public void OpenOptions()
    {
        GameStateHandler.SetGameState(GameState.OPTIONS);
    }

    public void OpenHelp()
    {
        GameStateHandler.SetGameState(GameState.HELP);
    }

    public void OpenContact()
    {
        GameStateHandler.SetGameState(GameState.CONTACT);
    }

    public void ExitGame()
    {
        Application.Quit(0);
    }


    public void RestartGame()
    {
        GameManager.GetInstance().RestartGame();
    }
}
