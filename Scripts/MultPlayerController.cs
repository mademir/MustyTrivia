using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultPlayerController : NetworkBehaviour
{
    private Rigidbody rb;
    private GameObject menu;
    private Vector3 ForwardDirection = new Vector3(0, 0, 1);
    private Vector3 RightDirection = new Vector3(-1, 0, 0);
    public float movementIntensity = 25f;
    public float jumpIntensity = 40f;
    public bool isOnGround = false;
    public bool isOnDoor = false;
    public bool isReady = false;
    public bool tempFreeze = false;
    [SyncVar] public int lives = 5;
    [SyncVar(hook = nameof(SetColour))] public Color colour;
    [SyncVar(hook = nameof(SetNameTag))] public string nameTag;
    Color tmpColour;

    float deltaTime;/////////////////////////////////////////////////////////////////////

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        menu = GameObject.Find("CanvasUI").transform.GetChild(7).transform.GetChild(0).gameObject;
        tmpColour = colour;
        if (hasAuthority)
        {
            nameTag = SteamFriends.GetPersonaName();
            CmdSetNameTag(nameTag);
        }
    }

    void SetNameTag(string oldName, string newName) => transform.GetChild(0).GetComponent<TextMeshPro>().text = newName;

    void SetColour(Color oldColour, Color newColour) => GetComponent<Renderer>().material.color = newColour;

    [Command] private void CmdSetNameTag(string name) => RpcSetNameTag(name);
    [ClientRpc] private void RpcSetNameTag(string name) => transform.GetChild(0).GetComponent<TextMeshPro>().text = name;

    [Command] public void CmdSetLevel(int level) => GameObject.Find("GameController").GetComponent<GameController>().Level = level;

    [Command] private void CmdReduceLives(int l) => RpcReduceLives(l);
    [ClientRpc] private void RpcReduceLives(int l) => lives = l;

    private void Update()
    {
        if (!hasAuthority) return;
        ////////////////////////////////////////////////////////////////////////////
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        GameObject.Find("FPS").GetComponent<Text>().text = "FPS: " + Mathf.Ceil(fps).ToString();
        ////////////////////////////////////////////////////////////////////////////
    }

    private void FixedUpdate()
    {
        if (!hasAuthority) return;

        if (tmpColour != colour)
        {
            GetComponent<Renderer>().material.color = colour;
            tmpColour = colour;
        }

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
            CmdReduceLives(lives);
            rb.position = lives <= 0 ? new Vector3(Random.Range(-24, -18) * (Random.Range(-1f, 1f) > 0 ? -1 : 1), 16, Random.Range(-3, 13)) : new Vector3(Random.Range(-10f, 10f) * (Random.Range(-1f, 1f) > 0 ? -1 : 1), 16, Random.Range(-10f, 10f));
        }

        if (Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.P)) //Set level cheat code
        {
            menu.transform.GetChild(5).gameObject.SetActive(true);
            if (Input.GetKey(KeyCode.Keypad1)) CmdSetLevel(10);
            if (Input.GetKey(KeyCode.Keypad2)) CmdSetLevel(20);
            if (Input.GetKey(KeyCode.Keypad3)) CmdSetLevel(30);
            if (Input.GetKey(KeyCode.Keypad4)) CmdSetLevel(40);
            if (Input.GetKey(KeyCode.Keypad5)) CmdSetLevel(50);
            if (Input.GetKey(KeyCode.Keypad6)) CmdSetLevel(60);
            if (Input.GetKey(KeyCode.Keypad7)) CmdSetLevel(70);
            if (Input.GetKey(KeyCode.Keypad8)) CmdSetLevel(80);
            if (Input.GetKey(KeyCode.Keypad9)) CmdSetLevel(90);
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
