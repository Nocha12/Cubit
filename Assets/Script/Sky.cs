using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using System;

public class Sky : MonoBehaviour
{
    float duration = 4f;

    public enum Skin
    {
        Normal,
        Red,
        Rabbit
    }
    [Serializable]
    public class SkySkin
    {
        public Skin s;
        public Material skyBox;
    }

    public List<SkySkin> skySkins;
    
    public void Reset()
    {
        StopAllCoroutines();
        StartCoroutine(SkyRot());
        RenderSettings.skybox.SetFloat("_Rotation", 0);
    }

    public void ChangeSkin(Skin s)
    {
        RenderSettings.skybox = skySkins.Find(q => q.s == s).skyBox;
    }

    public IEnumerator SkyRot()
    {
        yield return new WaitForSeconds(0.01f);
        while (true)
        {
            Action<ITween<float>> updateSkyRotation = (t) =>
            {
                RenderSettings.skybox.SetFloat("_Rotation", t.CurrentValue);
            };

            gameObject.Tween("RotateSky", 0, 360, duration, TweenScaleFunctions.QuadraticEaseInOut, updateSkyRotation);
            
            yield return new WaitForSeconds(duration);
        }
    }
}
