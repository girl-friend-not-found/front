using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Inspectorまたは自動取得
    public Text statusText;         // 表示するテキスト
    public Button button;           // Button

    void Start()
    {
        // videoPlayerが未設定の場合、"Cube" からVideoPlayerを取得
        if (videoPlayer == null)
        {
            GameObject vpObject = GameObject.Find("Cube");
            if (vpObject != null)
            {
                videoPlayer = vpObject.GetComponent<VideoPlayer>();
            }
            else
            {
                Debug.LogError("VideoPlayerが見つかりません。");
            }
        }

        // buttonが未設定の場合、子オブジェクトを検索して取得 (例)
        if (button == null)
        {
            Button btnObj = GetComponent<Button>();
            if (btnObj != null)
            {
                button = btnObj.GetComponent<Button>();
            }
        }

        if (button != null)
        {
            button.onClick.AddListener(ToggleVideo);
        }
        else
        {
            Debug.LogError("ButtonがInspectorに設定されていません。");
        }
    }

    void ToggleVideo()
    {
        Text text = GetComponent<Text>();
        Debug.Log("粉バナナ");
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            // text.text = "動画は一時停止中";
        }
        else
        {
            videoPlayer.Play();
            // text.text = "動画再生中";
        }
    }
}
