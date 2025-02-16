using UnityEngine;
using System.Collections;

public class MyAnimationController : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetEmotion(string emotion)
    {
        if (anim == null) return;

        // すべての感情をリセット
        anim.SetBool("angry", false);
        anim.SetBool("disgust", false);
        anim.SetBool("fear", false);
        anim.SetBool("happy", false);
        anim.SetBool("sad", false);
        anim.SetBool("surprise", false);
        anim.SetBool("neutral", false);

        // 受け取った感情に基づいてアニメーションを設定
        switch (emotion)
        {
            case "angry":
                anim.SetBool("angry", true);
                break;
            case "disgust":
                anim.SetBool("disgust", true);
                break;
            case "fear":
                anim.SetBool("fear", true);
                break;
            case "happy":
                anim.SetBool("happy", true);
                break;
            case "sad":
                anim.SetBool("sad", true);
                break;
            case "surprise":
                anim.SetBool("surprise", true);
                break;
            case "neutral":
                anim.SetBool("neutral", true);
                break;
        }

        StartCoroutine(ResetEmotionsAfterDelay());
    }

    private IEnumerator ResetEmotionsAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        anim.SetBool("angry", false);
        anim.SetBool("disgust", false);
        anim.SetBool("fear", false);
        anim.SetBool("happy", false);
        anim.SetBool("sad", false);
        anim.SetBool("surprise", false);
        anim.SetBool("neutral", false);
    }
}