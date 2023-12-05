using UnityEngine;


public class CameraController : MonoBehaviour {
    public static CameraViewAngle cameraView;

    public Transform chickenTransform;
    private static float smoothing;

    private Vector3 standardOffset = new Vector3(0, 2.7f, -3.3f);
    private Vector3 standardRotation = new Vector3(30, 0, 0);
    private Vector3 birdviewOffset = new Vector3(0, 6, 0);
    private Vector3 birdviewUnderTreeOffset = new Vector3(0, 2f, 0);
    private Vector3 birdviewRotation = new Vector3(90, 0, 0);

    private Vector3 overviewPosition = new Vector3(15, 28, 15);

    private bool startingSequenceFinished = false;

    void FixedUpdate()
    {
        GameState gameState = GameStateHandler.GetGameState();

        Vector3 desiredPos = GridMovement.targetPos;
        bool isHiddenByTree = GameManager.GetInstance().GetAllDestroyed() ?
            false : FieldHandler.GetInstance().FieldIsNotViewableFromBirdView(GridMovement.GetGridPosition(desiredPos));
        Vector3 cameraOffset = standardOffset;
        Vector3 cameraRotation = standardRotation;


        switch (cameraView)
        {
            case CameraViewAngle.OVERVIEW:
                if (!startingSequenceFinished)
                    return;

                Vector3 desiredCameraPos = overviewPosition + FieldHandler.FIELD_OFFSET;
                transform.position = Vector3.Lerp(transform.position, desiredCameraPos, smoothing);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(birdviewRotation), smoothing);
                return;


            case CameraViewAngle.BIRDVIEW:
                cameraOffset = isHiddenByTree ? birdviewUnderTreeOffset : birdviewOffset;
                cameraRotation = birdviewRotation;
                break;

            default:
                cameraOffset = standardOffset;
                cameraRotation = standardRotation;
                break;
        }
        Vector3 chickenPos = chickenTransform.position;
        //do not follow chicken jumps with camera
        chickenPos.y = 0.5f + FieldHandler.FIELD_OFFSET.y;

        transform.position = Vector3.Lerp(transform.position, chickenPos + cameraOffset, smoothing);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(cameraRotation), smoothing);
    }

    public static void SetCameraViewAngle(CameraViewAngle newCameraView)
    {
        smoothing = newCameraView == CameraViewAngle.OVERVIEW ? 0.08f : 0.1f;
        cameraView = newCameraView;
    }

    public void EndStartSequence()
    {
        startingSequenceFinished = true;
        Destroy(GetComponent<Animator>());
        CanvasHandler.GetInstance().SetMenuCanvasActive(true);
        FieldHandler.GetInstance().DestroyAllObjectsWithTag("StartSequenceOnly");
    }
}

public enum CameraViewAngle
{
    STANDARD, BIRDVIEW, OVERVIEW
}