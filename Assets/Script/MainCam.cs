using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{
    private GameObject player;
    public float offset;
    private void Start()
    {
        player = FindObjectOfType<Player>().gameObject;
        offset = 7 - 0.5f;
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;

        if (GameManager.instance.state != GameManager.State.Play)
                return;

        float d = player.transform.position.y + offset;
        float s = Mathf.Lerp(pos.y, d, 0.1f);

        transform.position = new Vector3(pos.x, s, pos.z);
    }
}
