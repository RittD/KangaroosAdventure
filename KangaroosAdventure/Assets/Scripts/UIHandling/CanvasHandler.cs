using UnityEngine;
using UnityEngine.UI;

public class CanvasHandler : MonoBehaviour
{
    private static CanvasHandler instance;
    public GameObject menuCanvas;
    public GameObject optionsCanvas;
    public GameObject helpCanvas;
    public GameObject contactCanvas;
    public GameObject tutorialCanvas;
    public GameObject gameCanvas;
    public GameObject staminaTip;
    public GameObject groundCamera;
    public GameObject groundCameraTip;
    public GameObject joystick;
    public Button[] buttons;
    public GameObject joystickTip;
    public GameObject viewControl;
    public GameObject viewControlTip;
    public GameObject gameEndCanvas;
    public Text gameEndMessage;


    public static CanvasHandler GetInstance()
    {
        return instance;
    }


    public void Init()
    {
        instance = this;
        
    }

    public void SetCurrentCanvas()
    {
        switch (GameStateHandler.GetGameState())
        {
            case GameState.MENU:
                SetMenuCanvas();
                break;
            case GameState.OPTIONS:
                SetOptionsCanvas();
                break;
            case GameState.HELP:
                SetHelpCanvas();
                break;
            case GameState.CONTACT:
                SetContactCanvas();
                break;
            case GameState.TUTORIAL:
                SetTutorialCanvas();
                break;
            case GameState.GAME:
                SetGameCanvas();
                break;
            case GameState.WON:
                SetGameEndCanvas(true);
                break;
            case GameState.LOSS:
                SetGameEndCanvas(false);
                break;
        }
    }

    private void SetMenuCanvas()
    {
        gameCanvas.SetActive(false);
        //healthBarCanvas.SetActive(false);
        gameEndCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        helpCanvas.SetActive(false);
        contactCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    private void SetOptionsCanvas()
    {
        gameCanvas.SetActive(false);
        //healthBarCanvas.SetActive(false);
        gameEndCanvas.SetActive(false);
        menuCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    private void SetHelpCanvas()
    {
        menuCanvas.SetActive(false);
        helpCanvas.SetActive(true);
    }

    private void SetContactCanvas()
    {
        menuCanvas.SetActive(false);
        contactCanvas.SetActive(true);
    }

    private void SetTutorialCanvas()
    {
        gameEndCanvas.SetActive(false);
        menuCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        tutorialCanvas.SetActive(true);
        gameCanvas.SetActive(true);
        ShowGameInterface(true);
        ShowToolTips(true);
    }

    private void SetGameCanvas()
    {
        gameEndCanvas.SetActive(false);
        menuCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        tutorialCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        ShowGameInterface(true);
        ShowToolTips(false);
    }

    private void SetGameEndCanvas(bool won)
    {
        menuCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        gameEndCanvas.SetActive(true);
        ShowGameInterface(false);

        if (won)
            gameEndMessage.GetComponent<Text>().text = "Reunited!";

        else
            gameEndMessage.GetComponent<Text>().text = "Game over!";
    }

    public void SetMenuCanvasActive(bool shallBeActive)
    {
        menuCanvas.SetActive(shallBeActive);
    }

    private void ShowGameInterface(bool state)
    {
        joystick.SetActive(state);
        if(!state)
            foreach (Button button in buttons)
            {
                button.GetComponent<PressedHandler>().buttonPressed = false;
            }

        viewControl.SetActive(state);
        groundCamera.SetActive(state);
    }

    private void ShowToolTips(bool state)
    {
        staminaTip.SetActive(state);
        groundCameraTip.SetActive(state);
        joystickTip.SetActive(state);
        viewControlTip.SetActive(state);
    }
}
