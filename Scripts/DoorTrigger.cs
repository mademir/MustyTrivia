using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public bool correct;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (gameObject.name != "S") other.GetComponent<MultPlayerController>().isOnDoor = true;
            else other.GetComponent<MultPlayerController>().isReady = true;
        }
    }
    /*private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (gameObject.name != "S") other.GetComponent<MultPlayerController>().isOnDoor = true;
            else other.GetComponent<MultPlayerController>().isReady = true;
        }
    }*/
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (gameObject.name != "S") other.GetComponent<MultPlayerController>().isOnDoor = false;
            else other.GetComponent<MultPlayerController>().isReady = false;
        }
    }
}
