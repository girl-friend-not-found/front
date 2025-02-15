using UnityEngine;
using VoicevoxBridge;

public class AivisSpeech : MonoBehaviour
{
    [SerializeField] VOICEVOX voicevox;

    async void Start()
    {
        int speaker = 888753760; // ずんだもん あまあま
        string text = "ずんだもんなのだ";
        await voicevox.PlayOneShot(speaker, text);
        Debug.Log("ボイス再生終了");
    }
}
