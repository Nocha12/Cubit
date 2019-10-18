using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using UnityEngine.UI;

public class BoxManager : MonoBehaviour
{
    public GameObject box;
    public GameObject map;
    public float boxDropTime;
    public float dropCount;
    public Text scoreText;
    public int score;
    public float highestBoxY;
    
    public List<Box> boxes = new List<Box>();

    public int[,] floor = new int[3, 3];
    public bool[,] isDroped = new bool[3, 3];

    public void Reset()
    {
        boxes = new List<Box>();
        floor = new int[3, 3];
        isDroped = new bool[3, 3];

        highestBoxY = 0;
        score = 0;
        scoreText.text = "0";
        dropCount = 0;
        boxDropTime = 1;
        StopAllCoroutines();
    }

    public void GameStart()
    {
        StartCoroutine(DropBox());
    }

    private void MakeBox(Vector2Int pos)
    {
        var beforeBox = boxes.Find(x => x.boxPos == new Vector2Int(pos.x + 1, pos.y + 1));
        boxes.Remove(beforeBox);

        System.Action<ITween<Vector3>> updateBoxScale = (t) =>
        {
            beforeBox.warningObj.transform.localScale = t.CurrentValue;
        };

        System.Action<ITween<float>> updateBoxLaser = (t) =>
        {
            for (int i = 0; i < 4; i++)
            {
                var p = beforeBox.laserR[i].GetPosition(1);
                beforeBox.laserR[i].SetPosition(1, new Vector3(p.x, t.CurrentValue, p.z));
            }
        };

        System.Action<ITween<Vector3>> BoxScaleCompleted = (t) =>
        {
            floor[pos.x + 1, pos.y + 1] += 1;
            float boxY = 0.25f + 0.5f * floor[pos.x + 1, pos.y + 1] + 10;
            Vector3 boxP = new Vector3(pos.x * 0.55f, boxY, pos.y * 0.55f);

            var b = ObjectPooler.instance.SpawnFromPool(boxP, Quaternion.Euler(0, 0, 0));
            
            b.GetComponent<Box>().boxPosY = boxY - 10;
            if (highestBoxY < boxY - 10)
                highestBoxY = boxY - 10;
        };

        Vector3 currentScale = beforeBox.warningObj.transform.localScale;
        Vector3 endScale = new Vector3(1, 1, 1);

        float warningTime = 0.8f;
        float dropTime = 0.7f;
        if (GameManager.instance.isHardMode)
        {
            warningTime = 0.6f;
            dropTime = 0.35f;
        }
        beforeBox.warningObj.Tween((dropCount += 1).ToString(), currentScale, endScale, warningTime, TweenScaleFunctions.CubicEaseOut, updateBoxScale, BoxScaleCompleted)
            .ContinueWith(new Vector3Tween().Setup(endScale, currentScale, dropTime, TweenScaleFunctions.CubicEaseIn, updateBoxScale));

        gameObject.Tween((dropCount += 1).ToString() + "Laser", 0, 1f, warningTime, TweenScaleFunctions.CubicEaseIn, updateBoxLaser)
           .ContinueWith(new FloatTween().Setup(1f, 0, dropTime, TweenScaleFunctions.CubicEaseOut, updateBoxLaser));
    }

    private Vector2Int CheckFloor()
    {
        int min = 99999999, max = 0;
        Vector2Int minPos = new Vector2Int();

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                if (floor[i, j] < min && !isDroped[i, j])
                {
                    min = floor[i, j];
                    minPos = new Vector2Int(i - 1, j - 1);
                }
                if (floor[i, j] > max)
                {
                    max = floor[i, j];
                }
            }
        if ((max - min) >= 4)
            return minPos;
        else
            return new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2));
    }

    private IEnumerator DropBox()
    {
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            if (GameManager.instance.state == GameManager.State.GameOver)
                break;

            int cnt = 0;
            int boxCount = 1;
            if (GameManager.instance.isHardMode)
                boxCount = 2;

            int randBoxCount = Random.Range(1, boxCount + 1);

            for (int i = 0; i < randBoxCount; i++)
            {
                Vector2Int p = CheckFloor();
                if (!isDroped[p.x + 1, p.y + 1])
                {
                    MakeBox(p);
                    isDroped[p.x + 1, p.y + 1] = true;
                }
                else {
                    i--;
                    cnt++;

                    if (cnt > 50)
                        break;
                }
            }

            if(GameManager.instance.isHardMode)
                Time.timeScale += 0.001f;
            Time.timeScale += 0.0001f;
            if (boxDropTime > 0.4f)
            {
                boxDropTime -= 0.01f;
                if (GameManager.instance.isHardMode)
                    boxDropTime -= 0.01f;
            }
            yield return new WaitForSeconds(boxDropTime);
        }
    }
}
