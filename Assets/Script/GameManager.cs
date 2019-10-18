using UnityEngine;
using System.Collections;
using DigitalRuby.Tween;
using UnityEngine.EventSystems;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public enum State
    {   
        Unlock,
        Ready,
        Move,
        Custom,
        Alert,
        Play,
        Ad,
        GameOver,
        ReadyToRestart
    }

    public static GameManager instance = null;

    public State state = State.Ready;
    public State beforeState = State.Ready;

    public Transform camT;
    public Transform playerT;

    public int tutorialPhase;
    public bool continueTutorial;

    public BoxManager boxManager;
    public UI ui;

    public GameObject game;
    public bool isRotating;
    public bool isHardMode;

    public Sky sky;
    public UnlockManager unlockManager;

    void Awake()
    {
        Application.targetFrameRate = 60;

        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("BestScore", 20);

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Reset();
    }

    void Reset()
    {
        state = State.Ready;
        beforeState = State.Ready;
        boxManager.Reset();
        ObjectPooler.instance.Reset();
        sky.Reset();
        ui.Reset();
        StopAllCoroutines();
        playerT.GetComponent<Player>().Reset();
        camT.localPosition = new Vector3(5, 8.5f, -5);
        unlockManager.UnlockWithBestScore();
    }

    public void SetState(State t)
    {
        beforeState = state;

        state = t;
    }

    public void ChangeToBeforeState()
    {
        state = beforeState;
    }

    void Update()
    {
        if (state == State.ReadyToRestart && ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Space)))
        {
            SetState(State.Ready);

            Reset();

            SoundManager.Instance.PlayBGM(SoundManager.Instance.mainBGM);

            Time.timeScale = 1;
            TweenFactory.Clear();
        }

        if (state == State.Ready
            && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began
            && !IsPointerOverUIObject(Input.GetTouch(0).position) || Input.GetKeyDown(KeyCode.B))
        {
            SetState(State.Play);

            SoundManager.Instance.ChangeBGM((SoundManager.BgmType)PlayerPrefs.GetInt("BgmType", (int)SoundManager.BgmType.Normal));

            if (PlayerPrefs.GetInt("IsFirstPlayNew", 1) == 1)
            {
                StartCoroutine(FirstStart());
                PlayerPrefs.SetInt("IsFirstPlayNew", 0);
            }
            else
            {
                ui.how2PlayC.gameObject.SetActive(false);
                boxManager.GameStart();
            }
            ui.GameStart();

            System.Action<ITween<Vector3>> updateCamPos = (t) =>
            {
                camT.localPosition = t.CurrentValue;
            };
            gameObject.Tween("ChangeCamPos", new Vector3(5, 8.5f, -5), new Vector3(4, 7, -4), 0.5f, TweenScaleFunctions.CubicEaseInOut, updateCamPos);
        }

        if (!isRotating && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            RotateGame(game.transform.eulerAngles.y, game.transform.eulerAngles.y + 90, 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            ui.PressCustomButton();
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ui.PressHardModeButton();
        }
    }
    
    IEnumerator FirstStart()
    {
        var textT = ui.turorialTextC[tutorialPhase].transform.GetComponentsInChildren<Transform>();
        var canvasT = ui.turorialTextC[tutorialPhase].transform;

        var effectPhase = 0;

        System.Action<ITween<float>> updateTutorialTextScale = (t) =>
        {
            foreach(var trans in textT)
            {
                if(trans != canvasT)
                    trans.localScale = new Vector3(t.CurrentValue, t.CurrentValue, 1);
            }
        };

        System.Action<ITween<float>> updateTutorialCanvasAlpha = (t) =>
        {
            ui.turorialTextC[effectPhase].alpha = t.CurrentValue;
        };

        gameObject.Tween("TutorialTextScale0", 0, 1, 0.5f, TweenScaleFunctions.CubicEaseOut, updateTutorialTextScale);
        gameObject.Tween("TutorialCanvasAlpha0", 0, 1, 0.5f, TweenScaleFunctions.CubicEaseOut, updateTutorialCanvasAlpha);

        for (int i = 0; i < ui.turorialTextC.Length; i++)
        {
            yield return new WaitForSeconds(0.5f);

            if (i == ui.turorialTextC.Length - 1)
            {
                System.Action<ITween<float>> updateHow2CanvasAlpha = (t) =>
                {
                    ui.how2PlayC.alpha = t.CurrentValue;
                };
                gameObject.Tween("How2CanvasAlpha", 1, 0, 1, TweenScaleFunctions.CubicEaseOut, updateHow2CanvasAlpha);
                yield return new WaitForSeconds(1f);
                continue;
            }

            continueTutorial = true;

            while (tutorialPhase == i)
                yield return null;

            gameObject.Tween("TutorialTextScale" + i.ToString(), 1, 1.2f, 0.25f, TweenScaleFunctions.CubicEaseIn, updateTutorialTextScale)
                .ContinueWith(new FloatTween().Setup(1.2f, 0, 0.25f, TweenScaleFunctions.CubicEaseOut, updateTutorialTextScale));
            gameObject.Tween("TutorialCanvasAlpha" + i.ToString(), 1, 0, 0.5f, TweenScaleFunctions.CubicEaseOut, updateTutorialCanvasAlpha);

            yield return new WaitForSeconds(0.5f);

            textT = ui.turorialTextC[i + 1].GetComponentsInChildren<Transform>();
            canvasT = ui.turorialTextC[i + 1].transform;
            effectPhase += 1;

            gameObject.Tween("TutorialTextScale" + (i + 1).ToString(), 0, 1, 0.5f, TweenScaleFunctions.CubicEaseOut, updateTutorialTextScale);
            gameObject.Tween("TutorialCanvasAlpha" + (i + 1).ToString(), 0, 1, 0.5f, TweenScaleFunctions.CubicEaseOut, updateTutorialCanvasAlpha);
        }

        boxManager.GameStart();
    }

    public void RotateGame(float f, float e, float time)
    {
        if (tutorialPhase == 2 && continueTutorial)
        {
            tutorialPhase = 3;
            continueTutorial = false;
        }

        isRotating = true;

        System.Action<ITween<float>> updateRotateGame = (t) =>
        {
            game.transform.eulerAngles = new Vector3(0, t.CurrentValue, 0);
        };

        System.Action<ITween<float>> rotateGameEnd = (t) =>
        {
            isRotating = false;
        };

        gameObject.Tween("RotateGame", f, e, time, TweenScaleFunctions.CubicEaseOut, updateRotateGame, rotateGameEnd);
    }
    
    public void GameOver()
    {
        SetState(State.GameOver);

        ui.GameOver();

        RotateGame(game.transform.eulerAngles.y, 0, 0.8f);
        playerT.GetComponent<Player>().ani.speed = 0;

        System.Action<ITween<float>> updateTimeScale = (t) =>
        {
            Time.timeScale = t.CurrentValue;
        };

        System.Action<ITween<Vector3>> updateCamPos = (t) =>
        {
            camT.localPosition = t.CurrentValue;
        };

        gameObject.Tween("ChangeTimeScale", Time.timeScale, 0.1f, 0.4f, TweenScaleFunctions.CubicEaseOut, updateTimeScale)
            .ContinueWith(new FloatTween().Setup(Time.timeScale, 1, 1f, TweenScaleFunctions.CubicEaseIn, updateTimeScale));
        gameObject.Tween("ChangeCamPos", camT.localPosition, new Vector3(2.5f + playerT.position.x, camT.position.y - 3, -2.5f + playerT.position.z), 0.5f, TweenScaleFunctions.CubicEaseInOut, updateCamPos);
    }

    public static bool IsPointerOverUIObject(Vector2 touchPos)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = touchPos;

        var results = new List<RaycastResult>();
        
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }
}