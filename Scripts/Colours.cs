using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colours : MonoBehaviour
{   
    enum Side
    {
        Top,
        Right,
        Bottom,
        Left
    };

    public Color start, end;
    [SerializeField]
    Side side;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player") return;
        float x = other.GetComponent<Rigidbody>().position.x;
        float z = other.GetComponent<Rigidbody>().position.z;
        switch (side)
        {
            case Side.Top:
                if (x < 0) other.GetComponent<MultPlayerController>().colour = new Color(0f, 1f, -((-x / 14.5f)-1f));
                if (x > 0) other.GetComponent<MultPlayerController>().colour = new Color(0f, -((x / 14.5f) - 1f), 1f);
                break;

            case Side.Right:
                if (z > 0) other.GetComponent<MultPlayerController>().colour = new Color(-((z / 14.5f) - 1f), 0f, 1f);
                if (z < 0) other.GetComponent<MultPlayerController>().colour = new Color(1f, 0f, -((-z / 14.5f) - 1f));
                break;

            case Side.Left:
                if (z > 0) other.GetComponent<MultPlayerController>().colour = new Color(-((z / 14.5f) - 1f), 1f, 0f);
                if (z < 0) other.GetComponent<MultPlayerController>().colour = new Color(1f, -((-z / 14.5f) - 1f), 0f);
                break;

            case Side.Bottom:
                if (x < 0) other.GetComponent<MultPlayerController>().colour = new Color(1f, -((-x / 14.5f) - 1f), 0f);
                if (x > 0) other.GetComponent<MultPlayerController>().colour = new Color(1f, -((x / 14.5f) - 1f), 0f);
                break;
            default:
                break;
        }
    }
}
