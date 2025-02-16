using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class VoiceToText : MonoBehaviour
{
    private AudioClip audioClip;
    private string microphoneName;
    private bool isRecording = false;
    private const string BackendUrl = "http://localhost:8000/transcribe";

    [SerializeField] private AivisSpeech aivisSpeech;
    public Text displayText;

    // Webカメラから画像を取得するための参照
    [SerializeField] private WebCamCapture webCamCapture;

    [SerializeField] private MyAnimationController AnimationControl;

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

        // 1) 音声をWAV化
        float[] samples = new float[audioClip.samples];
        audioClip.GetData(samples, 0);
        byte[] wavData = ConvertToWAV(samples, audioClip.frequency, 1);

        // 2) カメラ画像（Base64）を取得
        string base64Image = null;
        if (webCamCapture != null)
        {
            base64Image = webCamCapture.GetCamImageBase64();
        }

        // 3) multipart/form-data で送る
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", wavData, "audio.wav", "audio/wav");
        
        // 画像が取得できている場合のみ
        if (!string.IsNullOrEmpty(base64Image))
        {
            form.AddField("img", base64Image);  // ＜ポイント：ここで Base64文字列を送信
        }

        using (UnityWebRequest request = UnityWebRequest.Post(BackendUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"受信したレスポンス: {request.downloadHandler.text}");  // 生のJSONを確認
                
                var response = JsonUtility.FromJson<TranscriptionResponse>(request.downloadHandler.text);
                Debug.Log($"文字起こし: {response.transcription}");
                Debug.Log($"AIの応答: {response.reply}");
                
                if (response.emotion != null)
                {
                    Debug.Log("感情分析結果:");
                    Debug.Log($"- 怒り: {response.emotion.angry}%");
                    Debug.Log($"- 嫌悪: {response.emotion.disgust}%");
                    Debug.Log($"- 恐れ: {response.emotion.fear}%");
                    Debug.Log($"- 幸福: {response.emotion.happy}%");
                    Debug.Log($"- 悲しみ: {response.emotion.sad}%");
                    Debug.Log($"- 驚き: {response.emotion.surprise}%");
                    Debug.Log($"- 中立: {response.emotion.neutral}%");
                    string dominantEmotion = response.GetDominantEmotion();
                    Debug.Log($"最も強い感情: {dominantEmotion}");

                    // アニメーション制御スクリプトに渡す
                    MyAnimationController animController = FindObjectOfType<MyAnimationController>();
                    if (animController != null)
                    {
                        animController.SetEmotion(dominantEmotion);
                    }
                }
                else
                {
                    Debug.LogWarning("感情データが含まれていません");
                }

                if (displayText != null)
                {
                    displayText.text = response.reply;
                }

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
                Debug.LogError($"エラー: {request.error}");
                Debug.LogError($"レスポンス: {request.downloadHandler.text}");
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
    private class EmotionData
    {
        public float angry;
        public float disgust;
        public float fear;
        public float happy;
        public float sad;
        public float surprise;
        public float neutral;
    }

    [System.Serializable]
    private class TranscriptionResponse
    {
        public string transcription;
        public string reply;
        public bool should_speak;
        public EmotionData emotion;

        public string GetDominantEmotion()
        {
            if (emotion == null) return null;

            var emotions = new Dictionary<string, float>
            {
                { "angry", emotion.angry },
                { "disgust", emotion.disgust },
                { "fear", emotion.fear },
                { "happy", emotion.happy },
                { "sad", emotion.sad },
                { "surprise", emotion.surprise },
                { "neutral", emotion.neutral }
            };

            return emotions.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }
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
}
