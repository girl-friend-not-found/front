using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine.UI; // UI名前空間を追加
using System.Threading.Tasks;

public class VoiceToText : MonoBehaviour
{
    private AudioClip audioClip;
    private string microphoneName;
    private bool isRecording = false;
    private const string BackendUrl = "http://localhost:8000/transcribe"; // 文字起こしエンドポイント

    [SerializeField] private AivisSpeech aivisSpeech;

    public Text displayText;

    // Start is called before the first frame update
    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneName = Microphone.devices[0];
        }
        else
        {
            Debug.LogError("マイクが見つかりません");
        }
    }

    public void StartRecording()
    {
        if (!isRecording && microphoneName != null)
        {
            audioClip = Microphone.Start(microphoneName, false, 10, 44100);
            isRecording = true;
            Debug.Log("録音開始");
        }
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            Microphone.End(microphoneName);
            isRecording = false;
            Debug.Log("録音終了");
            
            // 音声データを取得してバックエンドに送信
            StartCoroutine(ProcessAndSendAudio());
        }
    }

    private IEnumerator ProcessAndSendAudio()
    {
        if (audioClip == null) yield break;

        float[] samples = new float[audioClip.samples];
        audioClip.GetData(samples, 0);
        byte[] wavData = ConvertToWAV(samples, audioClip.frequency, 1);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", wavData, "audio.wav", "audio/wav");

        using (UnityWebRequest request = UnityWebRequest.Post(BackendUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // JSONレスポンスをパース
                var response = JsonUtility.FromJson<TranscriptionResponse>(request.downloadHandler.text);
                Debug.Log($"文字起こし: {response.transcription}");
                Debug.Log($"AIの応答: {response.reply}");
                
                if (displayText != null)
                {
                    displayText.text = response.reply;
                }
                
                // AIの応答を読み上げる
                if (aivisSpeech != null && response.should_speak && !string.IsNullOrEmpty(response.reply))
                {
                    Debug.Log("音声合成を開始します...");
                    var operation = new WaitForAsyncOperation(aivisSpeech.SpeakText(response.reply));
                    yield return operation;
                    Debug.Log("音声合成が完了しました");
                }
            }
            else
            {
                Debug.LogError("エラー: " + request.error);
            }
        }
    }

    // 非同期処理をコルーチンで待機するためのヘルパークラス
    public class WaitForAsyncOperation : CustomYieldInstruction
    {
        private Task task;

        public WaitForAsyncOperation(Task task)
        {
            this.task = task;
        }

        public override bool keepWaiting => !task.IsCompleted;
    }

    [System.Serializable]
    private class TranscriptionResponse
    {
        public string transcription;
        public string reply;
        public bool should_speak;
    }

    public float[] GetAudioData()
    {
        if (audioClip == null) return null;

        float[] samples = new float[audioClip.samples];
        audioClip.GetData(samples, 0);
        return samples;
    }

    private static byte[] ConvertToWAV(float[] audioData, int sampleRate, int channels)
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + audioData.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)channels);
                writer.Write(sampleRate);
                writer.Write(sampleRate * channels * 2);
                writer.Write((short)(channels * 2));
                writer.Write((short)16);
                writer.Write("data".ToCharArray());
                writer.Write(audioData.Length * 2);

                foreach (float sample in audioData)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return stream.ToArray();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
