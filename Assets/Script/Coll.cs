using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coll : MonoBehaviour
{
    public bool isCol = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cube"))
            isCol = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isCol = false;
    }
}