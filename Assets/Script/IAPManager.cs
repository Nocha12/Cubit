using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPManager : MonoBehaviour
{
    public void BuyCoffe()
    {
        PlayerPrefs.SetInt("NoAds", 1);
    }
}
