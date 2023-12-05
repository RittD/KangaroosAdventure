using System;
using UnityEngine;

public class ChickenJump : MonoBehaviour {

    private Rigidbody rb;
    private float distToGround;
    public float minForce;
    public float maxForce;
    private float min;
    private float max;
    public float firstTime = -1f;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
        min = 1f;
        max = 1f;
    }

    void Update()
    {
        if (GameStateHandler.GetGameState() == GameState.WON && IsGrounded())//
        {
            if (firstTime == -1f)
                firstTime = Time.time;

            float jumpForce = UnityEngine.Random.Range(min, max);
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);

            if (Time.time - firstTime > 1f)
            {
                min = minForce;
                max = maxForce;
            }
        }
    }

    private bool IsGrounded()
    {
        //print("Bla");
        return transform.localPosition.y <= 0.49001f; //Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.25f);//
    }
}
