using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DigitalRuby.Tween;

public class Customization : MonoBehaviour
{
    public class UnlockC
    {
        public bool isUnlocked;
    }

    [Serializable]
    public class PlayerSkinUnlock : UnlockC
    {
        public string name;
        public Player.Skin s;
    }
    [Serializable]
    public class BoxSkinUnlock : UnlockC
    {
        public string name;
        public Box.Skin s;
    }
    [Serializable]
    public class BgmUnlock : UnlockC
    {
        public string name;
        public SoundManager.BgmType t;
    }
    [Serializable]
    public class SkyUnlock : UnlockC
    {
        public string name;
        public Sky.Skin s;
    }

    public List<PlayerSkinUnlock> playerSkinUnlocks;
    public List<BoxSkinUnlock> boxSkinUnlocks;
    public List<BgmUnlock> bgmUnlocks;
    public List<SkyUnlock> skyUnlocks;

    public Text musicText;
    public Text skyText;
    public Text boxText;
    public Text playerText;

    public Text musicNextText;
    public Text skyNextText;
    public Text boxNextText;
    public Text playerNextText;

    public Player player;
    public BoxManager boxManager;
    public Sky sky;

    public List<Transform> musicBtns;
    public List<Transform> skyBtns;
    public List<Transform> boxBtns;
    public List<Transform> playerBtns;

    int FindSkin(List<UnlockC> li, int index, bool reverse = false)
    {
        if (!reverse)
        {
            for (int i = index + 1; i < li.Count; i++)
                if (li[i].isUnlocked)
                    return i;

            for (int i = 0; i < index; i++)
                if (li[i].isUnlocked)
                    return i;
        }
        else
        {
            for (int i = index - 1; i >= 0; i--)
                if (li[i].isUnlocked)
                    return i;

            for (int i = li.Count - 1; i > index; i--)
                if (li[i].isUnlocked)
                    return i;
        }
        return index;
    }

    void SetPlayerWithIndex(int index)
    {
        var item = playerSkinUnlocks[index];

        player.ChangeSkin(item.s);
        playerText.text = item.name;
        PlayerPrefs.SetInt("PlayerSkin", (int)item.s);
    }
    void SetBoxWithIndex(int index)
    {
        var item = boxSkinUnlocks[index];

        foreach (var b in boxManager.boxes)
        {
            b.ChangeSkin(item.s);
            b.ChangeColor();
        }
        boxText.text = item.name;
        PlayerPrefs.SetInt("BoxSkin", (int)item.s);
    }
    void SetSkyWithIndex(int index)
    {
        var item = skyUnlocks[index];
        sky.ChangeSkin(item.s);
        skyText.text = item.name;
        PlayerPrefs.SetInt("SkySkin", (int)item.s);
    }
    void SetBgmWithIndex(int index, bool playMusic = true)
    {
        var item = bgmUnlocks[index];
        if(playMusic)
            SoundManager.Instance.ChangeBGM(item.t);
        musicText.text = item.name;
        PlayerPrefs.SetInt("BgmType", (int)item.t);
    }

    public List<UnlockC> ChangeList(List<PlayerSkinUnlock> beforeList)
    {
        List<UnlockC> li = new List<UnlockC>();

        for (int i = 0; i < beforeList.Count; i++)
            li.Add(beforeList[i]);

        return li;
    }

    public List<UnlockC> ChangeList(List<BoxSkinUnlock> beforeList)
    {
        List<UnlockC> li = new List<UnlockC>();

        for (int i = 0; i < beforeList.Count; i++)
            li.Add(beforeList[i]);

        return li;
    }

    public List<UnlockC> ChangeList(List<SkyUnlock> beforeList)
    {
        List<UnlockC> li = new List<UnlockC>();

        for (int i = 0; i < beforeList.Count; i++)
            li.Add(beforeList[i]);

        return li;
    }

    public List<UnlockC> ChangeList(List<BgmUnlock> beforeList)
    {
        List<UnlockC> li = new List<UnlockC>();

        for (int i = 0; i < beforeList.Count; i++)
            li.Add(beforeList[i]);

        return li;
    }

    public void PlayerLeft()
    {
        gameObject.Tween("PlayerLeft", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, ActionEffect.instance.ScaleEffect(playerBtns[0]));
        
        int index = FindSkin(ChangeList(playerSkinUnlocks), PlayerPrefs.GetInt("PlayerSkin", 0), true);
        SetPlayerWithIndex(index);
    }

    public void PlayerRight()
    {
        gameObject.Tween("PlayerRight", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, ActionEffect.instance.ScaleEffect(playerBtns[1]));

        int index = FindSkin(ChangeList(playerSkinUnlocks), PlayerPrefs.GetInt("PlayerSkin", 0));
        SetPlayerWithIndex(index);
    }

    public void BoxLeft()
    {
        gameObject.Tween("BoxLeft", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, ActionEffect.instance.ScaleEffect(boxBtns[0]));

        int index = FindSkin(ChangeList(boxSkinUnlocks), PlayerPrefs.GetInt("BoxSkin", 0), true);
        SetBoxWithIndex(index);
    }

    public void BoxRight()
    {
        gameObject.Tween("BoxRight", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, ActionEffect.instance.ScaleEffect(boxBtns[1]));

        int index = FindSkin(ChangeList(boxSkinUnlocks), PlayerPrefs.GetInt("BoxSkin", 0));
        SetBoxWithIndex(index);
    }

    public void SkyLeft()
    {
        gameObject.Tween("SkyLeft", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, ActionEffect.instance.ScaleEffect(skyBtns[0]));

        int index = FindSkin(ChangeList(skyUnlocks), PlayerPrefs.GetInt("SkySkin", 0), true);
        SetSkyWithIndex(index);
    }

    public void SkyRight()
    {
        gameObject.Tween("SkyRight", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, ActionEffect.instance.ScaleEffect(skyBtns[1]));

        int index = FindSkin(ChangeList(skyUnlocks), PlayerPrefs.GetInt("SkySkin", 0));
        SetSkyWithIndex(index);
    }

    public void BgmLeft()
    {
        gameObject.Tween("BgmLeft", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, ActionEffect.instance.ScaleEffect(musicBtns[0]));

        int index = FindSkin(ChangeList(bgmUnlocks), PlayerPrefs.GetInt("BgmType", 0), true);
        SetBgmWithIndex(index);
    }

    public void BgmRight()
    {
        gameObject.Tween("BgmRight", new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, ActionEffect.instance.ScaleEffect(musicBtns[1]));

        int index = FindSkin(ChangeList(bgmUnlocks), PlayerPrefs.GetInt("BgmType", 0));
        SetBgmWithIndex(index);
    }

    string ChangeSaveString(List<UnlockC> li)
    {
        var saveStr = "";
        for (int i = 0; i < li.Count; i++)
        {
            if (li[i].isUnlocked)
                saveStr += '1';
            else
                saveStr += '0';
        }
        return saveStr;
    }

    public void Save()
    {
        PlayerPrefs.SetString("PlayerSkinSave", ChangeSaveString(ChangeList(playerSkinUnlocks)));
        PlayerPrefs.SetString("BoxSkinSave", ChangeSaveString(ChangeList(boxSkinUnlocks)));
        PlayerPrefs.SetString("BgmSave", ChangeSaveString(ChangeList(bgmUnlocks)));
        PlayerPrefs.SetString("SkySave", ChangeSaveString(ChangeList(skyUnlocks)));
    }

    public void Load()
    {
        var playerSave = PlayerPrefs.GetString("PlayerSkinSave", "1");
        for (int i = 0; i < playerSave.Length; i++)
            if (playerSave[i] == '1')
                playerSkinUnlocks[i].isUnlocked = true;
            else
                playerSkinUnlocks[i].isUnlocked = false;

        var boxSave = PlayerPrefs.GetString("BoxSkinSave", "1");
        for (int i = 0; i < boxSave.Length; i++)
            if (boxSave[i] == '1')
                boxSkinUnlocks[i].isUnlocked = true;
            else
                boxSkinUnlocks[i].isUnlocked = false;

        var bgmSave = PlayerPrefs.GetString("BgmSave", "1");
        for (int i = 0; i < bgmSave.Length; i++)
            if (bgmSave[i] == '1')
                bgmUnlocks[i].isUnlocked = true;
            else
                bgmUnlocks[i].isUnlocked = false;

        var skySave = PlayerPrefs.GetString("SkySave", "1");
        for (int i = 0; i < playerSave.Length; i++)
            if (skySave[i] == '1')
                skyUnlocks[i].isUnlocked = true;
            else
                skyUnlocks[i].isUnlocked = false;
    }

    private void Start()
    {
        Load();

        SetPlayerWithIndex(PlayerPrefs.GetInt("PlayerSkin", 0));
        SetBoxWithIndex(PlayerPrefs.GetInt("BoxSkin", 0));
        SetSkyWithIndex(PlayerPrefs.GetInt("SkySkin", 0));
        SetBgmWithIndex(PlayerPrefs.GetInt("BgmType", (int)SoundManager.BgmType.Normal), false);
    }
}
