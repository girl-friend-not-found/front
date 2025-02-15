using UnityEngine;
using VoicevoxBridge;
using System.Threading.Tasks;

public class AivisSpeech : MonoBehaviour
{
    [SerializeField] private VOICEVOX voicevox;
    private int speaker = 888753760; // ずんだもん あまあま

    void Awake()
    {
        // VOICEVOXコンポーネントが設定されていない場合は、自動的に取得を試みる
        if (voicevox == null)
        {
            voicevox = GetComponent<VOICEVOX>();
            if (voicevox == null)
            {
                Debug.LogError("VOICEVOXコンポーネントが見つかりません。VOICEVOXコンポーネントをアタッチしてください。");
            }
        }
    }

    public async Task SpeakText(string text)
    {
        if (voicevox == null)
        {
            Debug.LogError("VOICEVOXコンポーネントが設定されていません");
            return;
        }

        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("読み上げるテキストが空です");
            return;
        }

        try
        {
            await voicevox.PlayOneShot(speaker, text);
            Debug.Log($"テキスト「{text}」の読み上げが完了しました");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"音声合成中にエラーが発生しました: {e.Message}");
        }
    }
}
