using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    private Vector3 originalPos;
    private Quaternion originalRot;

    public static Vector3 targetPos;
    public static Quaternion targetRot;
    private Vector2 finalPos;

    public Animator animator;
    public float timeToMove = 0.2f;

    public static bool chickenResetting = false;

    public GameObject chickenCam;

    private void Start()
    {
        
    }

    private void OnGUI()
    {
        TryMoving(Event.current);
    }

    public void TryMoving(Event currentEvent)
    {
        GameState gameState = GameStateHandler.GetGameState();
        if (gameState == GameState.GAME && !chickenResetting && !animator.GetBool("isMoving"))
        {
            ListenForInput(currentEvent);
        }
    }

    private void ListenForInput(Event currentEvent)
    {

        if (currentEvent.Equals(Event.KeyboardEvent("w"))){
            StartCoroutine(MovePlayer(new Vector3(0, 0, 1), 0));
        }
        else if (currentEvent.Equals(Event.KeyboardEvent("a")))
        {
            StartCoroutine(MovePlayer(new Vector3(-1, 0, 0), 270));
        }
        else if (currentEvent.Equals(Event.KeyboardEvent("s")))
        {
            StartCoroutine(MovePlayer(new Vector3(0, 0, -1), 180));
        }
        else if (currentEvent.Equals(Event.KeyboardEvent("d")))
        {
            StartCoroutine(MovePlayer(new Vector3(1, 0, 0), 90));
        }
    }


    private IEnumerator MovePlayer(Vector3 direction, float rotation)
    {
        Vector3 desiredPos = transform.position + direction;
        if (!InputIsValid(desiredPos))
            yield break;
        targetPos = desiredPos;

        animator.SetBool("isMoving", true);
        StaminaScript.GetInstance().DoStep(targetPos);

        animator.speed = 1f / timeToMove;
        float elapsedTime = 0;
        originalPos = transform.position;
        originalRot = transform.rotation;

        float fieldSize = FieldHandler.GetInstance().fields.Length;
        targetRot = Quaternion.Euler(0, rotation, 0);

        while(elapsedTime < timeToMove)
        {
            if (GameStateHandler.GetGameState() == GameState.LOSS)
            {
                animator.SetBool("isMoving", false);
                yield break;
            }

            transform.position = Vector3.Lerp(originalPos, targetPos, elapsedTime / timeToMove);
            transform.rotation = Quaternion.Lerp(originalRot, targetRot, elapsedTime / timeToMove);
            chickenCam.transform.rotation = Quaternion.Euler(90, 0, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;

        animator.SetBool("isMoving", false);
    }

    private bool InputIsValid(Vector3 possNextPos)
    {
        return FieldHandler.GetInstance().FieldIsVisitable(GetGridPosition(possNextPos));
    }


    public static Vector2 GetGridPosition(Vector3 realPosition)
    {
        Vector3 offset = FieldHandler.FIELD_OFFSET;
        return new Vector2(realPosition.x - 5.5f - offset.x, realPosition.z - 5.5f - offset.z);
    }
}
