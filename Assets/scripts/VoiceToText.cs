using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections;

public class VoiceToText : MonoBehaviour
{
    private AudioClip audioClip;
    private string microphoneName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 使用可能なマイクデバイスを取得
        if (Microphone.devices.Length > 0)
        {
            microphoneName = Microphone.devices[0];
            audioClip = Microphone.Start(microphoneName, true, 10, 44100);
        }
        else
        {
            Debug.LogError("マイクが見つかりません");
        }
    }

    public float[] GetAudioData()
    {
        if (audioClip == null) return null;

        float[] samples = new float[audioClip.samples];
        audioClip.GetData(samples, 0);
        return samples;
    }

    public static byte[] ConvertToWAV(float[] audioData, int sampleRate, int channels)
    {
        int byteRate = sampleRate * channels * 2; // 16ビット（2バイト）
        MemoryStream stream = new MemoryStream();

        // WAVヘッダーを書き込む
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write("RIFF".ToCharArray());
            writer.Write(36 + audioData.Length * 2);
            writer.Write("WAVE".ToCharArray());
            writer.Write("fmt ".ToCharArray());
            writer.Write(16); // Subchunk1Size (16 for PCM)
            writer.Write((short)1); // AudioFormat (1 for PCM)
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write((short)(channels * 2)); // BlockAlign
            writer.Write((short)16); // BitsPerSample

            // データチャンク
            writer.Write("data".ToCharArray());
            writer.Write(audioData.Length * 2);

            foreach (float sample in audioData)
            {
                short value = (short)(sample * short.MaxValue);
                writer.Write(value);
            }
        }

        return stream.ToArray();
    }

    private const string ApiUrl = "https://api.openai.com/v1/audio/transcriptions";
    private const string ApiKey = "YOUR_API_KEY"; // OpenAIのAPIキー

    public IEnumerator SendAudioToWhisper(byte[] audioData)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "audio.wav", "audio/wav");
        form.AddField("model", "whisper-1");

        UnityWebRequest request = UnityWebRequest.Post(ApiUrl, form);
        request.SetRequestHeader("Authorization", "Bearer " + ApiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("認識結果: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("エラー: " + request.error);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
