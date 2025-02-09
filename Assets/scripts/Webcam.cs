using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Webカメラ
public class WebCam : MonoBehaviour
{
    private static int INPUT_SIZE = 256;
    private static int FPS = 30;

    // UI
    RawImage rawImage;
    WebCamTexture webCamTexture;

    // スタート時に呼ばれる
    void Start()
    {
        // Webカメラの開始
        this.rawImage = GetComponent<RawImage>();
        WebCamDevice[] webCamDevice = WebCamTexture.devices;
        Debug.Log(webCamDevice[1].name);
        this.webCamTexture = new WebCamTexture("FaceTime HDカメラ", INPUT_SIZE, INPUT_SIZE, FPS);
        this.rawImage.texture = this.webCamTexture;
        this.webCamTexture.Play();
    }
}