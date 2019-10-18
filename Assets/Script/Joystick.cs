using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour
{
    public CanvasGroup joystickC;
    public GameObject handle;
    public float handleRange;
    public Vector3 result;

    void Update()
    {
        if (GameManager.instance.state == GameManager.State.GameOver)
        {
            return;
        }

        //#if UNITY_EDITOR
        //        CheckMouse();
        //#else
       //CheckTouch();
        CheckMouse();
//#endif
    }

    void CheckTouch()
    {
        if (Input.touchCount == 0)
        {
            joystickC.alpha -= 0.04f;
            return;
        }

        var touch = Input.GetTouch(0);

        if (GameManager.IsPointerOverUIObject(touch.position))
            return;

        Vector3 pos = touch.position;

        if (touch.phase == TouchPhase.Began)
        {
            joystickC.alpha = 1;

            transform.position = pos;
            handle.transform.position = pos;
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            if (GameManager.instance.tutorialPhase == 0 && GameManager.instance.continueTutorial)
            {
                GameManager.instance.tutorialPhase = 1;
                GameManager.instance.continueTutorial = false;
            }

            result = transform.position - handle.transform.position;

            if (Vector2.Distance(transform.position, pos) < handleRange)
                handle.transform.position = pos;
            else
                handle.transform.position = (pos - transform.position).normalized * handleRange + transform.position;
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            handle.transform.position = transform.position;
            result = new Vector3(0, 0, 0);
        }
    }

    void CheckMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            joystickC.alpha = 1;

            transform.position = Input.mousePosition;
            handle.transform.position = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            if (GameManager.instance.tutorialPhase == 0 && GameManager.instance.continueTutorial)
            {
                GameManager.instance.tutorialPhase = 1;
                GameManager.instance.continueTutorial = false;
            }

            result = transform.position - handle.transform.position;

            if (Vector2.Distance(transform.position, Input.mousePosition) < handleRange)
                handle.transform.position = Input.mousePosition;
            else
                handle.transform.position = (Input.mousePosition - transform.position).normalized * handleRange + transform.position;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            handle.transform.position = transform.position;
            result = new Vector3(0, 0, 0);
        }
        else
        {
            joystickC.alpha -= 0.01f;
        }
    }
}
