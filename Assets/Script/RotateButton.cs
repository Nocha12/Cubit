using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateButton : MonoBehaviour, IDragHandler
{
    void Start()
    {
        var x = PlayerPrefs.GetFloat("RotateButtonX", 150);
        var y = PlayerPrefs.GetFloat("RotateButtonY", 200);

        transform.position = new Vector3(x, y, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.instance.tutorialPhase == 3 && GameManager.instance.continueTutorial)
        {
            GameManager.instance.tutorialPhase = 4;
            GameManager.instance.continueTutorial = false;
        }

        transform.position = Input.mousePosition;
        PlayerPrefs.SetFloat("RotateButtonX", transform.position.x);
        PlayerPrefs.SetFloat("RotateButtonY", transform.position.y);
    }
}
