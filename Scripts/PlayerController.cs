using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 ForwardDirection = new Vector3(0, 0, 1);
    private Vector3 RightDirection = new Vector3(-1, 0, 0);
    public float movementIntensity = 25f;
    public float jumpIntensity = 40f;
    public bool isOnGround = false;
    public bool isOnDoor = false;
    public bool isReady = false;
    public bool tempFreeze = false;
    public int lives = 5;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!GameObject.Find("GameController").GetComponent<GameController>().freezePlayers || lives <= 0)
        {
            tempFreeze = false;
            // Move Forwards
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                rb.AddForce(ForwardDirection * movementIntensity);
            }
            // Move Backwards
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                rb.AddForce(-ForwardDirection * movementIntensity);
            }
            // Move Rightwards
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                rb.AddForce(RightDirection * movementIntensity);
            }
            // Move Leftwards
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                rb.AddForce(-RightDirection * movementIntensity);
            }

            if (Input.GetKey(KeyCode.Space) && isOnGround)
            {
                rb.AddForce(Vector3.up * jumpIntensity);
            }
        }
        else if (!tempFreeze)
        {
            rb.velocity = Vector3.zero;
            tempFreeze = true;
        }

        if (rb.position.y < -30)
        {
            lives--;
            rb.position = lives<=0? new Vector3(Random.Range(-24, -18) * (Random.Range(-1f, 1f) > 0 ? -1 : 1), 16, Random.Range(-3, 13)) : new Vector3(Random.Range(-10f,10f)*(Random.Range(-1f,1f)>0?-1:1), 16, Random.Range(-10f,10f));   
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = false;
        }
    }
}
