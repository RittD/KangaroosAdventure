using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    MENU, OPTIONS, HELP, CONTACT, TUTORIAL, GAME, WON, LOSS
}

public class GameStateHandler : MonoBehaviour
{
    public static Toggle tutorialToggle;
    private static GameState gameState = GameState.LOSS;


    public static GameState GetGameState()
    {
        return gameState;
    }

    public bool revived = false;

    public static void SetGameState(GameState newGameState)
    {
        GameState oldGameState = gameState;
        gameState = newGameState;
        GameManager gameManager = GameManager.GetInstance();
        AudioManager audioManager = AudioManager.GetInstance();

        CanvasHandler.GetInstance().SetCurrentCanvas();
        Camera groundCamera = gameManager.chickenObj.GetComponent<GridMovement>().chickenCam.GetComponent<Camera>();

        switch (gameState)
        {
            case GameState.WON:
                groundCamera.enabled = false;
                CameraController.SetCameraViewAngle(CameraViewAngle.STANDARD);
                gameManager.fenceDoorObj.GetComponent<Animation>().Play();
                //ChickenController.desiredRotation = new Vector3(0, 45, 0);
                GridMovement.targetRot = Quaternion.Euler(0, 45, 0);
                audioManager.PlayWinSounds();
                gameManager.HideReviveButton();
                break;
            case GameState.LOSS:
                groundCamera.enabled = false;
                CameraController.SetCameraViewAngle(CameraViewAngle.STANDARD);
                audioManager.PlayLossSounds();
                gameManager.TryToShowReviveButton();
                break;
            case GameState.MENU:
                groundCamera.enabled = false;
                CameraController.SetCameraViewAngle(CameraViewAngle.OVERVIEW);
                audioManager.PlayMenuMusic();
                //only refresh when game was over
                if (oldGameState == GameState.WON || oldGameState == GameState.LOSS)
                    gameManager.RefreshScene();
                break;
            case GameState.GAME:
                print(tutorialToggle);
                if (tutorialToggle.isOn)
                    PlayerPrefs.SetInt("TutorialSeen", 1);
                groundCamera.enabled = true;
                CameraController.SetCameraViewAngle(CameraViewAngle.STANDARD);
                gameManager.stdViewToggle.GetComponent<Toggle>().isOn = true;
                audioManager.PlayGameMusic();
                StaminaScript.GetInstance().RefreshStamina();
                break;
            case GameState.TUTORIAL:
                groundCamera.enabled = true;
                CameraController.SetCameraViewAngle(CameraViewAngle.STANDARD);
                gameManager.stdViewToggle.GetComponent<Toggle>().isOn = true;
                audioManager.PlayGameMusic();
                StaminaScript.GetInstance().RefreshStamina();
                break;
        }
    }
}
