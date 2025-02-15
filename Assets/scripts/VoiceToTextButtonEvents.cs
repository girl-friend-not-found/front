using UnityEngine;
using UnityEngine.UI;

public class VoiceToTextButtonEvents : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button startRecordingButton;
    public Button stopRecordingButton;

    [Header("VoiceToText Reference")]
    public VoiceToText voiceToText;

    void Awake()
    {
        if (startRecordingButton == null || stopRecordingButton == null)
        {
            Debug.LogError("UIボタンが正しくアサインされていません！");
        }
        if (voiceToText == null)
        {
            Debug.LogError("VoiceToTextオブジェクトが割り当てられていません！");
        }
    }

    void Start()
    {
        // ボタンの OnClick イベントにハンドラを登録する
        startRecordingButton.onClick.AddListener(OnStartButtonClick);
        stopRecordingButton.onClick.AddListener(OnStopButtonClick);
    }

    void OnStartButtonClick()
    {
        Debug.Log("Start Recording Button Clicked (Event Handler)");
        if (voiceToText != null)
        {
            // VoiceToText の録音処理を呼ぶ（ボタンクリックから分離している）
            voiceToText.StartRecording();
        }
    }

    void OnStopButtonClick()
    {
        Debug.Log("Stop Recording Button Clicked (Event Handler)");
        if (voiceToText != null)
        {
            // VoiceToText の停止処理を呼ぶ
            voiceToText.StopRecording();
        }
    }
} 