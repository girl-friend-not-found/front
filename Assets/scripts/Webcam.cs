using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class WebCamCapture : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    private WebCamTexture webCamTexture;

    private const int INPUT_SIZE = 256;
    private const int FPS = 30;

    void Start()
    {
        // Webカメラデバイスの存在確認
        WebCamDevice[] devices = WebCamTexture.devices;
        
        // デバッグ情報の出力
        Debug.Log($"検出されたカメラ数: {devices.Length}");
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log($"カメラ {i}: {devices[i].name}");
        }

        if (devices.Length > 0)
        {
            try
            {
                // Webカメラのセットアップ
                webCamTexture = new WebCamTexture(devices[0].name, INPUT_SIZE, INPUT_SIZE, FPS);
                
                // RawImageの確認
                if (rawImage == null)
                {
                    Debug.LogError("RawImageが設定されていません");
                    return;
                }

                rawImage.texture = webCamTexture;
                webCamTexture.Play();

                Debug.Log("Webカメラの初期化に成功しました");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Webカメラの初期化中にエラーが発生しました: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("利用可能なWebカメラが見つかりません");
        }
    }

    // カメラの現在フレームをPNG化して Base64 文字列を返す例
    public string GetCamImageBase64()
    {
        if (webCamTexture == null || !webCamTexture.isPlaying)
        {
            Debug.LogError("Webカメラが初期化されていないか、再生されていません");
            return null;
        }

        try
        {
            Texture2D texture = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
            texture.SetPixels(webCamTexture.GetPixels());
            texture.Apply();

            byte[] pngData = texture.EncodeToPNG();
            string base64 = System.Convert.ToBase64String(pngData);

            // メモリリーク防止のためテクスチャを破棄
            Destroy(texture);

            return base64;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"画像のBase64エンコード中にエラーが発生しました: {e.Message}");
            return null;
        }
    }

    void OnDestroy()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}
