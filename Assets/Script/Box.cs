using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DigitalRuby.Tween;

public class Box : MonoBehaviour, IPooledObject
{
    public enum Skin
    {
        Normal,
        DotHeart,
        Heart,
        Rabbit
    }

    [Serializable]
    public class SkinMaterial
    {
        public Skin skin;
        public Material material;
    }

    public Renderer inBoxR;
    public Renderer outBoxR;
    public Renderer warning;
    public Renderer[] trailR;
    public LineRenderer[] laserR;
    public GameObject warningObj;
    public Vector2Int boxPos;
    public float boxPosY = 0.25f;
    public bool isFalling = true;
    public bool isDisabled;
    public bool isBasicBox;
    public Renderer boxRenderer;
    
    public List<SkinMaterial> skinMaterials;

    private Rigidbody rig;
    public BoxManager manager;

    private const float boxInterval = 0.55f;
    private const int sideEffLen = 4;

    public void ChangeSkin(Skin skinE)
    {
        boxRenderer.material = skinMaterials.Find(e => e.skin == skinE).material;
    }

    public void ChangeColor()
    {
        float r = UnityEngine.Random.Range(0f, 1f);

        string cStr = "_Color"; 
        string eColorStr = "_EmissionColor";

        Color outBoxC = Color.HSVToRGB(r, 0.8f, 0.75f);
        Color inBoxC = Color.HSVToRGB(r, 0.4f, 0.9f);
        Color trailC = Color.HSVToRGB(r, 0.4f, 0.8f);
        Color laserC = Color.HSVToRGB(r, 0.3f, 1);
        Color waningC = Color.HSVToRGB(r, 0.5f, 0.9f);

        outBoxR.material.SetColor(cStr, outBoxC);
        outBoxR.material.SetColor(eColorStr, outBoxC);

        inBoxR.material.SetColor(cStr, inBoxC);
        inBoxR.material.SetColor(eColorStr, inBoxC);

        for (int i = 0; i < sideEffLen; i++)
        {
            trailR[i].material.SetColor(cStr, trailC);
            trailR[i].material.SetColor(eColorStr, trailC);
        }
        for (int i = 0; i < sideEffLen; i++)
        {
            laserR[i].material.SetColor(cStr, laserC);
            laserR[i].material.SetColor(eColorStr, laserC);
        }

        warning.material.SetColor(cStr, waningC);
        warning.material.SetColor(eColorStr, waningC);
    }

    void Start()
    {
        manager = FindObjectOfType<BoxManager>();
    }

    public void Reset()
    {
        isDisabled = false;

        warningObj.transform.localScale = new Vector3(0, 1, 0);

        for (int i = 0; i < sideEffLen; i++)
        {
            var p = laserR[i].GetPosition(1);
            laserR[i].SetPosition(1, new Vector3(p.x, 0, p.z));
        }
    }

    public void ResetBasicBox()
    {
        gameObject.SetActive(true);
        
        ChangeSkin((Skin)PlayerPrefs.GetInt("BoxSkin", 0));
        ChangeColor();

        SetBoxPos(transform.position);

        manager.boxes.Add(this);
    }

    private void SetBoxPos(Vector3 pos)
    {
        boxPos.x = (int)((pos.x + boxInterval) / boxInterval);
        boxPos.y = (int)((pos.z + boxInterval) / boxInterval);
    }

    public void OnObjectSpawn()
    {
        if (GameManager.instance.state == GameManager.State.GameOver)
            return;

        Reset();

        isFalling = true;

        ChangeSkin((Skin)PlayerPrefs.GetInt("BoxSkin", 0));
        ChangeColor();

        rig = GetComponent<Rigidbody>();
        rig.constraints = RigidbodyConstraints.FreezeRotation
                        | RigidbodyConstraints.FreezePositionX
                        | RigidbodyConstraints.FreezePositionZ;
         
        float dropPower = -5;
        if (GameManager.instance.isHardMode)
            dropPower = -10;

        rig.AddForce(new Vector3(0, dropPower, 0), ForceMode.Impulse);

        SetBoxPos(transform.position);

        StartCoroutine(ActiveLineRenderer());

        manager = FindObjectOfType<BoxManager>();

        manager.boxes.Add(this);
    }

    IEnumerator ActiveLineRenderer()
    {
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < sideEffLen; i++)
            trailR[i].gameObject.SetActive(true);
    }

    private void Update()
    {
        if (isDisabled)
            return;
        StandardShaderUtils.ChangeRenderMode(inBoxR.material, StandardShaderUtils.BlendMode.Opaque);

        if (GameManager.instance.state == GameManager.State.GameOver)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            return;
        }

        int removeBoxOffset = 4;
        if (manager.highestBoxY - transform.position.y > removeBoxOffset)
        {
            for (int i = 0; i < sideEffLen; i++)
                trailR[i].gameObject.SetActive(false);

            gameObject.SetActive(false);
        }

        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material.EnableKeyword("_EMISSION");
            Color c = renderer.material.color;
            c.a += 0.05f;
            if (c.a > 1)
                c.a = 1;
            renderer.material.color = c;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isFalling)
        {
            if (GameManager.instance.state == GameManager.State.GameOver)
                return;

            SoundManager.Instance.PlaySingle(SoundManager.Instance.boxSound, transform.position);

            manager.scoreText.text = (manager.score += 1).ToString();

            var scaleEff = ActionEffect.instance.ScaleEffect(manager.scoreText.transform);
            manager.scoreText.gameObject.Tween("ScoreTextEffect", new Vector3(1.5f, 1.5f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.CubicEaseOut, scaleEff);

            EZCameraShake.CameraShaker.Instance.ShakeOnce(1, 1, 0.01f, 0.1f);

            isFalling = false;
            rig.velocity = Vector3.zero;
            rig.constraints = RigidbodyConstraints.FreezeAll;

            manager.isDroped[boxPos.x, boxPos.y] = false;

            Vector3 pos = transform.position;
            if (!collision.gameObject.CompareTag("Player"))
                transform.position = new Vector3(pos.x, boxPosY, pos.z);
            else
            {
                GameManager.instance.GameOver();

                if (GameManager.instance.isHardMode)
                {
                    if (manager.score > PlayerPrefs.GetInt("HardModeBestScore"))
                        PlayerPrefs.SetInt("HardModeBestScore", manager.score);
                }
                else
                {
                    if (manager.score > PlayerPrefs.GetInt("BestScore"))
                        PlayerPrefs.SetInt("BestScore", manager.score);
                }

                var playerR = collision.transform.GetComponent<Rigidbody>();
                playerR.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }
}
