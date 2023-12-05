using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Direction
{
    RIGHT, DOWN, LEFT, UP
}

public class PressedHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool buttonPressed = false;
    private GridMovement movement;
    public Direction direction;
    private Event keyboardEvent; 

    void Start()
    {
        KeyCode keyCode;

        switch (direction)
        {
            case Direction.RIGHT:
                keyCode = KeyCode.D;
                break;
            case Direction.DOWN:
                keyCode = KeyCode.S;
                break;
            case Direction.LEFT:
                keyCode = KeyCode.A;
                break;
            case Direction.UP:
                keyCode = KeyCode.W;
                break;
            default:
                keyCode = KeyCode.Delete;
                break;
        }
        keyboardEvent = Event.KeyboardEvent(keyCode.ToString());

    }


    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonPressed = false;
    }


    void Update()
    {
        if (buttonPressed)
            GameManager.GetInstance().chickenObj.GetComponent<GridMovement>().TryMoving(keyboardEvent);
    }
}
