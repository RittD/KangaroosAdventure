using UnityEngine;

public class GameButtonHandler : MonoBehaviour
{

    public void SetStandardView()
    {
        CameraController.SetCameraViewAngle(CameraViewAngle.STANDARD);
    }


    public void SetBirdviewView()
    {
        CameraController.SetCameraViewAngle(CameraViewAngle.BIRDVIEW);
    }


    public void SetOverviewView()
    {
        CameraController.SetCameraViewAngle(CameraViewAngle.OVERVIEW);
    }

    public void MoveRight()
    {
        Move(KeyCode.D);
    }
    public void MoveUp()
    {
        Move(KeyCode.W);
    }
    public void MoveLeft()
    {
        Move(KeyCode.A);
    }
    public void MoveDown()
    {
        Move(KeyCode.S);
    }

    private void Move(KeyCode movement)
    {
        GameManager.GetInstance().chickenObj.GetComponent<GridMovement>().TryMoving(Event.KeyboardEvent(movement.ToString()));
    }
}
