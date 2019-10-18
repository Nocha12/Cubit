using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;

    public void Awake()
    {
        if (!instance)
            instance = this;
    }

    public void ShowAd()
    {
        GameManager.instance.SetState(GameManager.State.Ad);

        if (Advertisement.IsReady("video"))
        {
            Advertisement.Show("video");
        }
    }

    public void ShowRewardVideo()
    {
        GameManager.instance.SetState(GameManager.State.Ad);

        if (Advertisement.IsReady("rewardedVideo"))
        {
            ShowOptions options = new ShowOptions { resultCallback = RewardResultCallback };

            Advertisement.Show("rewardedVideo", options);
        }
    }
    public Action ShowFinished;
    public Action ShowFailed;

    private void RewardResultCallback(ShowResult result)
    {
        GameManager.instance.ChangeToBeforeState();

        if (result == ShowResult.Finished)
            ShowFinished();
        else if (result == ShowResult.Skipped || result == ShowResult.Failed)
            ShowFailed();
    }
}
