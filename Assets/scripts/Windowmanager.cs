using UnityEngine;
using SatorImaging.AppWindowUtility;

public class MyTest : MonoBehaviour
{
    void Start()
    {
        AppWindowUtility.Transparent = false;
        // ウィンドウサイズ設定
        int newWidth = 1000;
        int newHeight = 700;
        Screen.SetResolution(newWidth, newHeight, Screen.fullScreen);
    }
}