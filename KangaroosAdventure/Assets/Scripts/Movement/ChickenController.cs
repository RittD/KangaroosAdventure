using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class ChickenController : MonoBehaviour {
   
    [Range(0.0f, 1.0f)]
    public float rotationSmoothness = 1;

    [Range(0f, 10f)]
    public float stepJumpForce = 1.52f;

    [Range(0.0f, 1.0f)]
    public float movementDuration = 0.25f;

    [Range(0.0f, 5.0f)]
    public float possibleAheadMoving = 1f;

    public static Vector3 desiredPosition;
    public static Vector3 desiredRotation;

    public GameObject chickenCam;

    private Rigidbody rb;

    public static bool chickenResetting = false;


    void Start()
    {
        desiredPosition = new Vector3(5.5f, 0.5f, 5.5f) + FieldHandler.FIELD_OFFSET;
        desiredRotation = new Vector3(0, 0, 0);
        rb = GetComponent<Rigidbody>();
    }

    private void OnGUI()
    {
        if (GameStateHandler.GetGameState() == GameState.GAME)
            ListenForInput(Event.current);
    }

    public void ListenForInput(Event currentEvent)
    {
        Vector3 possNextPos = desiredPosition;

        if (currentEvent.Equals(Event.KeyboardEvent("w")) || currentEvent.Equals(Event.KeyboardEvent(KeyCode.UpArrow.ToString()))) {
            possNextPos += new Vector3(0, 0, 1);
            desiredRotation = new Vector3(0, 0, 0);
        }

        else if (currentEvent.Equals(Event.KeyboardEvent("d")) || currentEvent.Equals(Event.KeyboardEvent(KeyCode.RightArrow.ToString()))) {
            possNextPos += new Vector3(1, 0, 0);
            desiredRotation = new Vector3(0, 90, 0);
        }

        else if (currentEvent.Equals(Event.KeyboardEvent("s")) || currentEvent.Equals(Event.KeyboardEvent(KeyCode.DownArrow.ToString()))) {
            possNextPos += new Vector3(0, 0, -1);
            desiredRotation = new Vector3(0, 180, 0);
        }

        else if (currentEvent.Equals(Event.KeyboardEvent("a")) || currentEvent.Equals(Event.KeyboardEvent(KeyCode.LeftArrow.ToString()))) {
            possNextPos += new Vector3(-1, 0, 0);
            desiredRotation = new Vector3(0, 270, 0);
        }
        else
            return;

        
        //if (InputIsValid(possNextPos)) {
        //    desiredPosition = possNextPos;
        //    HungerScript.GetInstance().DoStep();
        //}
    }

    //private bool InputIsValid(Vector3 possNextPos) {
    //    Vector2 rasteredPos = GetRasterPosition(possNextPos);

    //    Vector3 routeToDo = desiredPosition - transform.position;
    //    routeToDo.y = 0;

    //    bool nextPosVisitable = FieldHandler.GetInstance().FieldIsVisitable(rasteredPos);
    //    bool aheadMovementIsNotToLong = Vector3.Magnitude(routeToDo) < possibleAheadMoving;

    //    return nextPosVisitable && aheadMovementIsNotToLong;
    //}
    public static Vector2 GetGridPosition(Vector3 realPosition)
    {
        Vector3 offset = FieldHandler.FIELD_OFFSET;
        return new Vector2(realPosition.x - 5.5f - offset.x, realPosition.z - 5.5f - offset.z);
    }

    void Update() {

        GameState gameState = GameStateHandler.GetGameState();

        if ((gameState == GameState.GAME || gameState == GameState.WON) && !chickenResetting){
            MoveRigidbodyOfChicken(gameState);
            RotateChicken();
        }
    }

    private void MoveRigidbodyOfChicken(GameState gameState)
    {

        Vector3 remainingRoute = desiredPosition - transform.position;
        remainingRoute.y = 0;

        Vector3 step = Vector3.Normalize(remainingRoute) * (Time.deltaTime / movementDuration);

        float remainingRouteLengthSquared = Vector3.SqrMagnitude(remainingRoute);
        float stepLengthSquared = Vector3.SqrMagnitude(step);

        if (remainingRouteLengthSquared <= stepLengthSquared || stepLengthSquared == 0)
            DoLastStepOrStand(rb);
        else
            DoNormalStep(rb, remainingRoute);
    }

    private void DoNormalStep(Rigidbody rb, Vector3 remainingRoute)
    {
        Vector3 v = Vector3.Normalize(remainingRoute) / movementDuration;

        //jump if grounded and ingame
        if (ChickenIsGrounded(gameObject))
            v.y = stepJumpForce;
        else
            v.y = rb.velocity.y;

        rb.velocity = v;
    }

    private void DoLastStepOrStand(Rigidbody rb)
    {
        transform.position = new Vector3(desiredPosition.x, transform.position.y, desiredPosition.z);

        float vy = rb.velocity.y;
        //jump if won
        if (GameStateHandler.GetGameState() == GameState.WON && ChickenIsGrounded(gameObject))
            vy = Random.Range(1.1f, 1.6f);
        rb.velocity = new Vector3(0, vy, 0);
    }

    private void RotateChicken() {
        Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(desiredRotation), rotationSmoothness);
        transform.rotation = rotation;
        chickenCam.transform.rotation = Quaternion.Euler(90,0,0);
    }

    public static bool ChickenIsGrounded(GameObject chicken) {
        return chicken.transform.localPosition.y <= 0.49001f;
    }
}
