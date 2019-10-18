using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRuby.Tween;
using System;

public class UI : MonoBehaviour
{
    public CanvasGroup mainC;
    public CanvasGroup inGameC;
    public CanvasGroup endGameC;
    public CanvasGroup how2PlayC;
    public CanvasGroup alertC;

    public CanvasGroup mainInC;
    public CanvasGroup customC;

    public CanvasGroup[] turorialTextC;

    public Text bestT;
    public Text bestScoreT;
    public Text touch2StartT;

    public Image scoreUI;
    public Text bestScoreResultT;
    public Text scoreResultT;
    public Text touch2RestartT;

    public BoxManager boxManager;

    public Transform rotateButton;
    public Transform customButton;
    public Transform hardModeButton;
    public Transform adButton;
    public Transform adYesButton;

    public Transform coffeeButton;

    public Transform cloasAlertButton;

    public List<AlertUI> alertUIs;
    [Serializable]
    public class AlertUI
    {
        public List<GameObject> UIs;
        public AlertType type;
    }

    public enum AlertType
    {
        RandonBox,
        AdFailed,
        Coffee
    }

    Coroutine effectC;

    public void Reset()
    {
        rotateButton.localScale = Vector3.one;

        mainC.blocksRaycasts = true;
        mainC.alpha = 1;

        endGameC.alpha = 0;

        var c = touch2RestartT.color;
        c.a = 0;
        touch2RestartT.color = c;
        c = scoreResultT.color;
        c.a = 0;
        scoreResultT.color = c;

        StopAllCoroutines();

        Start();
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("HardMode", 0) == 1)
        {
            GameManager.instance.isHardMode = true;

            Color textC = Color.HSVToRGB(0, 0.14f, 1);

            bestT.color = textC;
            bestScoreT.color = textC;
            touch2StartT.color = textC;

            bestScoreT.text = PlayerPrefs.GetInt("HardModeBestScore", 0).ToString();
            touch2StartT.text = "WELCOME TO HARDMODE!";
        }
        else
            bestScoreT.text = PlayerPrefs.GetInt("BestScore", 0).ToString();

        if (PlayerPrefs.GetInt("CustomUnlocked", 0) != 1)
            customButton.gameObject.SetActive(false);
        if (PlayerPrefs.GetInt("HardModeUnlocked", 0) != 1)
            hardModeButton.gameObject.SetActive(false);

        effectC = StartCoroutine(MainTextEffect());

        AdsManager.instance.ShowFailed = ShowFailed;
    }

    public void GameStart()
    {
        StopCoroutine(effectC);

        mainC.blocksRaycasts = false;

        var eff = ActionEffect.instance;

        gameObject.Tween("CanvasEff1", 1, 0, 0.5f, TweenScaleFunctions.CubicEaseOut, eff.AlphaEffect(mainC));
        gameObject.Tween("CanvasEff2", 0, 1, 0.5f, TweenScaleFunctions.CubicEaseOut, eff.AlphaEffect(inGameC));
    }

    public void GameOver()
    {
        if (GameManager.instance.isHardMode)
            bestScoreResultT.text = "BEST " + PlayerPrefs.GetInt("HardModeBestScore", 0).ToString();
        else
            bestScoreResultT.text = "BEST " + PlayerPrefs.GetInt("BestScore", 0).ToString();

        void updateCanvas2(ITween<float> t)
        {
            endGameC.alpha = t.CurrentValue;
            scoreUI.transform.localScale = new Vector3(t.CurrentValue, t.CurrentValue, 1);
        }
        void updateCanvas2End(ITween<float> t)
        {
            StartCoroutine(ScoreResultTextEffect());
        }

        var eff = ActionEffect.instance;

        gameObject.Tween("CanvasEff", 1, 0, 1f, TweenScaleFunctions.CubicEaseOut, eff.AlphaEffect(inGameC))
            .ContinueWith(new FloatTween().Setup(0, 1, 1f, TweenScaleFunctions.CubicEaseOut, updateCanvas2, updateCanvas2End))
            .ContinueWith(new FloatTween().Setup(0, 1, 1f, TweenScaleFunctions.CubicEaseOut, eff.AlphaEffect(scoreResultT)));
    }

    IEnumerator ScoreResultTextEffect()
    {
        for (int i = 0; i < boxManager.score; i++)
        {
            scoreResultT.text = (i + 1).ToString();

            float rNum = UnityEngine.Random.Range(1f, 1.1f);
            Vector3 scaledVec = new Vector3(rNum, rNum, 1);

            var eff = ActionEffect.instance;

            gameObject.Tween("ScoreResultTextEff", scaledVec, Vector3.one, 1.5f / boxManager.score, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(scoreResultT.transform));
            yield return new WaitForSeconds(1.5f / boxManager.score);
        }

        if (boxManager.score >= 20 && PlayerPrefs.GetInt("NoAds", 0) == 0)
            AdsManager.instance.ShowAd();

        GameManager.instance.SetState(GameManager.State.ReadyToRestart);
        StartCoroutine(FinalCanvasEffect());
    }

    IEnumerator FinalCanvasEffect()
    {
        var eff = ActionEffect.instance;

        gameObject.Tween("FinalTextAlphaEff", 0, 1, 0.5f, TweenScaleFunctions.CubicEaseOut, eff.AlphaEffect(touch2RestartT));

        while (true)
        {
            List<Transform> transforms = new List<Transform>
            {
                touch2RestartT.transform,
                scoreUI.transform
            };

            gameObject.Tween("FinalCanvasScaleEff", new Vector3(1, 1, 1), new Vector3(1.05f, 1.05f, 1), 0.4f, TweenScaleFunctions.CubicEaseIn, eff.ScaleEffect(transforms))
                    .ContinueWith(new Vector3Tween().Setup(new Vector3(1.05f, 1.05f, 1), new Vector3(1, 1, 1), 0.4f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(transforms)));

            yield return new WaitForSeconds(0.8f);
        }
    }

    public void PressCustomButton()
    {
        var gM = GameManager.instance;
        var eff = ActionEffect.instance;

        if (gM.state == GameManager.State.Ready)
        {
            SoundManager.Instance.ChangeBGM((SoundManager.BgmType)PlayerPrefs.GetInt("BgmType", (int)SoundManager.BgmType.Normal));

            gM.SetState(GameManager.State.Move);

            gameObject.Tween("CostomButtonScaleEff", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(customButton));
            gameObject.Tween("updateCustomCanvas1", 0, 1, 1, TweenScaleFunctions.CubicEaseOut, eff.AlphaEffect(customC));
            gameObject.Tween("updateCustomCanvas2", 1, 0, 1, TweenScaleFunctions.CubicEaseOut, eff.AlphaEffect(mainInC));

            customC.blocksRaycasts = true;
            mainInC.blocksRaycasts = false;

            System.Action<ITween<Vector3>> updateEnd = (t) =>
            {
                gM.SetState(GameManager.State.Custom);
            };

            gameObject.Tween("ChangeCamPos", new Vector3(5, 8.5f, -5), new Vector3(2, 1.55f, -2), 1f, TweenScaleFunctions.CubicEaseOut, eff.PosEffect(gM.camT));
            gameObject.Tween("ChangeCamRot", new Vector3(45, -45, 0), new Vector3(10, -45, 0), 1, TweenScaleFunctions.CubicEaseOut, eff.RotEffect(gM.camT), updateEnd);
        }
        else if (gM.state == GameManager.State.Custom)
        {
            SoundManager.Instance.PlayBGM(SoundManager.Instance.mainBGM);

            gM.SetState(GameManager.State.Move);

            gameObject.Tween("CostomButtonScaleEff", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(customButton));
            gameObject.Tween("updateCustomCanvas1", 1, 0, 1, TweenScaleFunctions.CubicEaseOut, eff.AlphaEffect(customC));
            gameObject.Tween("updateCustomCanvas2", 0, 1, 1, TweenScaleFunctions.CubicEaseOut, eff.AlphaEffect(mainInC));

            customC.blocksRaycasts = false;
            mainInC.blocksRaycasts = true;

            void updateEnd(ITween<Vector3> t)
            {
                gM.SetState(GameManager.State.Ready);
            }

            gameObject.Tween("ChangeCamPos", new Vector3(2, 1.55f, -2), new Vector3(5, 8.5f, -5), 1f, TweenScaleFunctions.CubicEaseOut, eff.PosEffect(gM.camT));
            gameObject.Tween("ChangeCamRot", new Vector3(10, -45, 0), new Vector3(45, -45, 0), 1, TweenScaleFunctions.CubicEaseOut, eff.RotEffect(gM.camT), updateEnd);
        }
    }

    public void PressRotateButton()
    {
        var gM = GameManager.instance;

        if (gM.state == GameManager.State.Play)
        {
            var eff = ActionEffect.instance;
            gameObject.Tween("RotButtonScaleEff", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(rotateButton));

            gM.RotateGame(gM.game.transform.eulerAngles.y, gM.game.transform.eulerAngles.y + 90, 0.5f);
        }
    }

    public void PressHardModeButton()
    {
        var gM = GameManager.instance;
        var eff = ActionEffect.instance;

        if (gM.state != GameManager.State.Ready)
            return;

        gM.SetState(GameManager.State.Move);

        StopCoroutine(effectC);

        List<Transform> transforms = new List<Transform>
        {
            bestT.transform,
            bestScoreT.transform,
            touch2StartT.transform
        };

        gameObject.Tween("HardButtonScaleEff", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(hardModeButton));

        void effectEnd(ITween<Vector3> t)
        {
            gM.SetState(GameManager.State.Ready);
            effectC = StartCoroutine(MainTextEffect());
        }

        if (gM.isHardMode)
        {
            PlayerPrefs.SetInt("HardMode", 0);
            gM.isHardMode = false;

            void updateMainTextsEnd(ITween<Vector3> t)
            {
                Color textC = Color.HSVToRGB(235 / 360f, 0.14f, 1);

                bestT.color = textC;
                bestScoreT.color = textC;
                touch2StartT.color = textC;

                bestScoreT.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
                touch2StartT.text = "TOUCH THE SCREEN TO PLAY";
            }

            gameObject.Tween("MainTextEff", new Vector3(1, 1, 1), new Vector3(0, 0, 0), 0.5f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(transforms), updateMainTextsEnd)
                .ContinueWith(new Vector3Tween().Setup(new Vector3(0, 0, 0), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(transforms), effectEnd));
        }
        else
        {
            PlayerPrefs.SetInt("HardMode", 1);
            gM.isHardMode = true;

            void updateMainTextsEnd(ITween<Vector3> t)
            {
                Color textC = Color.HSVToRGB(0, 0.14f, 1);

                bestT.color = textC;
                bestScoreT.color = textC;
                touch2StartT.color = textC;

                bestScoreT.text = PlayerPrefs.GetInt("HardModeBestScore", 0).ToString();
                touch2StartT.text = "WELCOME TO HARDMODE!";
            }

            gameObject.Tween("MainTextEff", new Vector3(1, 1, 1), new Vector3(0, 0, 0), 0.5f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(transforms), updateMainTextsEnd)
                .ContinueWith(new Vector3Tween().Setup(new Vector3(0, 0, 0), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(transforms), effectEnd));
        }
    }

    IEnumerator MainTextEffect()
    {
        while (true)
        {
            List<Transform> transforms = new List<Transform>
            {
                bestScoreT.transform,
                touch2StartT.transform
            };

            gameObject.Tween("MainTextEff", new Vector3(1, 1, 1), new Vector3(1.2f, 1.2f, 1), 0.4f, TweenScaleFunctions.CubicEaseIn, ActionEffect.instance.ScaleEffect(transforms))
                .ContinueWith(new Vector3Tween().Setup(new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.4f, TweenScaleFunctions.CubicEaseOut, ActionEffect.instance.ScaleEffect(transforms)));
            yield return new WaitForSeconds(0.8f);
        }
    }

    public void PressAdButton()
    {
        var eff = ActionEffect.instance;

        if (GameManager.instance.state == GameManager.State.Custom && GameManager.instance.state != GameManager.State.Alert)
        {
            gameObject.Tween("AdButtonScaleEff", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(adButton));

            ShowAlert(AlertType.RandonBox);
        }
    }

    public void PressYesAdButton()
    {
        var eff = ActionEffect.instance;

        CloseAlert();

        if(PlayerPrefs.GetInt("NoAds", 0) == 0)
            AdsManager.instance.ShowRewardVideo();
        else
            AdsManager.instance.ShowFinished();

        gameObject.Tween("AdYesButtonScaleEff", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(adYesButton));
    }

    public void PressCoffeeButton()
    {
        var eff = ActionEffect.instance;

        if (GameManager.instance.state == GameManager.State.Custom && GameManager.instance.state != GameManager.State.Alert)
        {
            gameObject.Tween("CoffeeButtonScaleEff", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(adButton));

            ShowAlert(AlertType.Coffee);
        }
    }

    public void ShowFailed()
    {
        ShowAlert(AlertType.AdFailed);
    }

    public void ShowAlert(AlertType t)
    {
        var eff = ActionEffect.instance;

        GameManager.instance.SetState(GameManager.State.Alert);

        gameObject.Tween("AlertCanvasScaleEff", new Vector3(0, 0, 0), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(alertC.transform));
        alertC.blocksRaycasts = true;

        SetAlert(t);
    }

    public void CloseAlert()
    {
        var eff = ActionEffect.instance;

        GameManager.instance.ChangeToBeforeState();

        gameObject.Tween("CloseAlertButtonScaleEff", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(cloasAlertButton));
        gameObject.Tween("AlertCanvasScaleEff", new Vector3(1, 1, 1), new Vector3(0, 0, 0), 0.5f, TweenScaleFunctions.CubicEaseOut, eff.ScaleEffect(alertC.transform));
        alertC.blocksRaycasts = false;
    }

    public void SetAlert(AlertType t)
    {
        foreach(var alertUI in alertUIs)
            foreach (var ui in alertUI.UIs)
                ui.SetActive(false);

        var showingUI = alertUIs.Find(u => u.type == t);
        foreach (var ui in showingUI.UIs)
            ui.SetActive(true);
    }
}