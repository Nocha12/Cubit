using DigitalRuby.Tween;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UnlockManager : MonoBehaviour
{
    [Serializable]
    public class UnlockItem
    {
        public bool isUnlocked;
        public int score;
        public bool isSkin;
        public string title;
        public string drip;
        public string name;
        public GameObject obj;
        public Sprite sprite;
        public UnlockType type;
        public AllType unlockItemType;
    }
    [Serializable]
    public class AllType
    {
        public Player.Skin playerS;
        public Box.Skin boxS;
        public Sky.Skin skyS;
        public SoundManager.BgmType bgmType;
    }

    public enum UnlockType
    {
        Custom,
        HardMode,
        PlayerSkin,
        BoxSkin,
        Bgm,
        Background
    }

    public List<UnlockItem> unlockItems;
    public Queue<UnlockItem> unlock = new Queue<UnlockItem>();

    public CanvasGroup unlockC;
    public Text titleText;
    public Text dripText;
    public Text nameText;
    public Transform imgBackground;
    public Transform skinImg;
    public Image iconImg;

    private GameObject beforeObj;

    public Customization customization;

    void LoadData()
    {
        string defaultData = "";
        for (int i = 0; i < unlockItems.Count; i++)
            defaultData += "0";

        string unlockString = PlayerPrefs.GetString("UnlockString", defaultData);
        var length = unlockString.Length;

        for (int i = 0; i < length; i++)
            unlockItems[i].isUnlocked = unlockString[i] != '0';
        for (int i = 0; i < unlockItems.Count - length; i++)
            unlockItems[length + i].isUnlocked = false;
    }

    void SaveData()
    {
        string saveDataString = "";

        for (int i = 0; i < unlockItems.Count; i++)
            saveDataString += unlockItems[i].isUnlocked ? "1" : "0";

        PlayerPrefs.SetString("UnlockString", saveDataString);
    }

    void Awake()
    {
        LoadData();
    }

    private void Start()
    {
        AdsManager.instance.ShowFinished = UnlockRandomSkin;
    }

    public void UnlockWithBestScore()
    {
        for (int i = 0; i < unlockItems.Count; i++)
        {
            if (!unlockItems[i].isUnlocked && unlockItems[i].score <= PlayerPrefs.GetInt("BestScore", 0))
            {
                Unlock(unlockItems[i]);

                unlock.Enqueue(unlockItems[i]);

                unlockItems[i].isUnlocked = true;
            }
        }

        SaveData();

        ResetNextUnlockTexts();

        if (unlock.Count > 0)
        {
            GameManager.instance.SetState(GameManager.State.Unlock);

            StartCoroutine(UnlockQueue());
        }
    }

    private void ResetNextUnlockTexts()
    {
        bool isPlayerSkinLeft = false;
        bool isBoxSkinLeft = false;
        bool isBgmLeft = false;
        bool isSkyLeft = false;

        var orderedUnlockItems = unlockItems.OrderBy(x => x.score).ToList();

        for (int i = 0; i < unlockItems.Count; i++)
        {
            if (orderedUnlockItems[i].isUnlocked)
                continue;

            if (orderedUnlockItems[i].type == UnlockType.PlayerSkin && !isPlayerSkinLeft)
            {
                isPlayerSkinLeft = true;
                customization.playerNextText.text = orderedUnlockItems[i].score.ToString();
            }
            if (orderedUnlockItems[i].type == UnlockType.BoxSkin && !isBoxSkinLeft)
            {
                isBoxSkinLeft = true;
                customization.boxNextText.text = orderedUnlockItems[i].score.ToString();
            }
            if (orderedUnlockItems[i].type == UnlockType.Bgm && !isBgmLeft)
            {
                isBgmLeft = true;
                customization.musicNextText.text = orderedUnlockItems[i].score.ToString();
            }
            if (orderedUnlockItems[i].type == UnlockType.Background && !isSkyLeft)
            {
                isSkyLeft = true;
                customization.skyNextText.text = orderedUnlockItems[i].score.ToString();
            }
        }
        if (!isPlayerSkinLeft)
            customization.playerNextText.text = "NONE";
        if (!isBoxSkinLeft)
            customization.boxNextText.text = "NONE";
        if (!isBgmLeft)
            customization.musicNextText.text = "NONE";
        if (!isSkyLeft)
            customization.skyNextText.text = "NONE";
    }

    public List<UnlockItem> GetLockedSkins()
    {
        List<UnlockItem> lockedSkins = new List<UnlockItem>();

        foreach (var item in unlockItems)
            if (!item.isUnlocked && item.type != UnlockType.HardMode && item.type != UnlockType.Custom)
                lockedSkins.Add(item);

        return lockedSkins;
    }

    public void UnlockRandomSkin()
    {
        GameManager.instance.SetState(GameManager.State.Unlock);

        List<UnlockItem> lockedSkins = GetLockedSkins();

        if (lockedSkins.Count == 0)
        {
            GameManager.instance.ChangeToBeforeState();
            return;
        }

        int rIndex = UnityEngine.Random.Range(0, lockedSkins.Count);

        Unlock(lockedSkins[rIndex]);

        lockedSkins[rIndex].isUnlocked = true;

        SaveData();
        ResetNextUnlockTexts();

        unlock.Enqueue(lockedSkins[rIndex]);
        StartCoroutine(UnlockQueue());
    }

    void Unlock(UnlockItem item)
    {
        if (item.type == UnlockType.Custom)
        {
            PlayerPrefs.SetInt("CustomUnlocked", 1);
            GameManager.instance.ui.customButton.gameObject.SetActive(true);
        }
        else if (item.type == UnlockType.HardMode)
        {
            PlayerPrefs.SetInt("HardModeUnlocked", 1);
            GameManager.instance.ui.hardModeButton.gameObject.SetActive(true);
        }
        else if (item.type == UnlockType.PlayerSkin)
            customization.playerSkinUnlocks.Find(q => q.s == item.unlockItemType.playerS).isUnlocked = true;
        else if (item.type == UnlockType.BoxSkin)
            customization.boxSkinUnlocks.Find(q => q.s == item.unlockItemType.boxS).isUnlocked = true;
        else if (item.type == UnlockType.Bgm)
            customization.bgmUnlocks.Find(q => q.t == item.unlockItemType.bgmType).isUnlocked = true;
        else if (item.type == UnlockType.Background)
            customization.skyUnlocks.Find(q => q.s == item.unlockItemType.skyS).isUnlocked = true;

        customization.Save();
    }

    void SetUnlockCanvas(UnlockItem item)
    {
        if(item.isSkin)
        {
            if (beforeObj != null)
                beforeObj.SetActive(false);

            iconImg.gameObject.SetActive(false);
            skinImg.gameObject.SetActive(true);
            item.obj.SetActive(true);

            beforeObj = item.obj;
        }
        else
        {
            skinImg.gameObject.SetActive(false);
            iconImg.gameObject.SetActive(true);
            iconImg.sprite = item.sprite;
        }

        titleText.text = item.title;
        dripText.text = item.drip;
        nameText.text = item.name;
    }

    IEnumerator UnlockQueue()
    {
        var item = unlock.Dequeue();

        SetUnlockCanvas(item);

        Action<ITween<float>> updateCanvas = (t) =>
        {
            unlockC.transform.localScale = new Vector3(t.CurrentValue, t.CurrentValue, 1);
        };
        gameObject.Tween("UpdateCanvas", 0, 1, 0.5f, TweenScaleFunctions.CubicEaseOut, updateCanvas);

        yield return new WaitForSeconds(0.5f);

        while (unlock.Count > 0)
        {
            if (unlock.Count != 0)
                while (true)
                {
                    if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
                        break;

                    yield return null;
                }

            item = unlock.Dequeue();

            Action<ITween<float>> updateScale = (t) =>
            {
                titleText.transform.localScale = new Vector3(t.CurrentValue, t.CurrentValue, 1);
                dripText.transform.localScale = new Vector3(t.CurrentValue, t.CurrentValue, 1);
                nameText.transform.localScale = new Vector3(t.CurrentValue, t.CurrentValue, 1);
                imgBackground.transform.localScale = new Vector3(t.CurrentValue, t.CurrentValue, 1);
                skinImg.transform.localScale = new Vector3(t.CurrentValue, t.CurrentValue, 1);
                iconImg.transform.localScale = new Vector3(t.CurrentValue, t.CurrentValue, 1);
            };
            void updateScaleEnd(ITween<float> t)
            {
                SetUnlockCanvas(item);
            }
            gameObject.Tween("updateScale", 1, 0, 0.25f, TweenScaleFunctions.CubicEaseOut, updateScale, updateScaleEnd)
                .ContinueWith(new FloatTween().Setup(0, 1, 0.25f, TweenScaleFunctions.CubicEaseOut, updateScale));

            yield return new WaitForSeconds(0.5f);
        }
       
        while (true)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                gameObject.Tween("UpdateCanvas", 1, 1.2f, 0.25f, TweenScaleFunctions.CubicEaseIn, updateCanvas)
                    .ContinueWith(new FloatTween().Setup(1.2f, 0, 0.25f, TweenScaleFunctions.CubicEaseOut, updateCanvas));
                yield return new WaitForSeconds(0.5f);
                GameManager.instance.ChangeToBeforeState();
                break;
            }

            yield return null;
        }
    }
}
