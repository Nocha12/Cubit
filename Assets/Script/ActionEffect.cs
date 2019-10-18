using UnityEngine;
using System.Collections;
using DigitalRuby.Tween;
using System.Collections.Generic;
using UnityEngine.UI;

public class ActionEffect
{
    public static ActionEffect instance = new ActionEffect();

    public System.Action<ITween<Vector3>> PosEffect(Transform t)
    {
        void update(ITween<Vector3> value)
        {
            t.localPosition = value.CurrentValue;
        }

        return update;
    }

    public System.Action<ITween<Vector3>> ScaleEffect(List<Transform> transforms)
    {
        void update(ITween<Vector3> value)
        {
            foreach (var t in transforms)
                t.localScale = value.CurrentValue;
        }

        return update;
    }

    public System.Action<ITween<Vector3>> ScaleEffect(Transform transform)
    {
        void update(ITween<Vector3> value)
        {
            transform.localScale = value.CurrentValue;
        }

        return update;
    }

    public System.Action<ITween<float>> AlphaEffect(CanvasGroup canvas)
    {
        void update(ITween<float> value)
        {
            canvas.alpha = value.CurrentValue;
        }

        return update;
    }

    public System.Action<ITween<float>> AlphaEffect(Text text)
    {
        void update(ITween<float> value)
        {
            var c = text.color;
            c.a = value.CurrentValue;
            text.color = c;
        }

        return update;
    }

    public System.Action<ITween<Vector3>> RotEffect(Transform t)
    {
        void update(ITween<Vector3> value)
        {
            t.eulerAngles = value.CurrentValue;
        }

        return update;
    }
}
