using UnityEngine;

public class GameEndButtonHandler : MonoBehaviour
{

    public void OnReviveButtonPressed()
    {
        GameManager gameManager = GameManager.GetInstance();
        if (gameManager.ad.IsLoaded())
            gameManager.ad.Show();

        gameManager.HideReviveButton();
    }
}
