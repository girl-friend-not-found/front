using UnityEngine;
using UnityEngine.UI;

public class textdisplay : MonoBehaviour
{
    private GameObject textObj;

    void Start()
    {
        // このButtonオブジェクトの親(Canvas)から "Text (TMP)" という名前のTextオブジェクトを検索
        if (transform.parent != null)
        {
            Transform textTransform = transform.parent.Find("Text (TMP)");
            if (textTransform != null)
            {
                textObj = textTransform.gameObject;
                Debug.Log("Textオブジェクトが見つかりました: " + textObj.name);
            }
            else
            {
                Debug.LogError("Canvasの子オブジェクトに 'Text (TMP)' が見つかりません。");
            }
        }
        else
        {
            Debug.LogError("このオブジェクトは親を持っていません。");
        }

        // 自身のButtonコンポーネントを取得してOnClickイベントにToggleTextDisplay()を登録
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(ToggleTextDisplay);
        }
        else
        {
            Debug.LogError("Buttonコンポーネントが見つかりません。");
        }
    }

    // ButtonのOnClickイベントで呼ばれるメソッド
    public void ToggleTextDisplay()
    {
        if (textObj != null)
        {
            Debug.Log("ToggleTextDisplay呼ばれました。現在のactiveSelf: " + textObj.activeSelf);
            textObj.SetActive(!textObj.activeSelf);
            Debug.Log("Toggle後のactiveSelf: " + textObj.activeSelf);
        }
        else
        {
            Debug.LogError("Textオブジェクトが設定されていません。");
        }
    }
}
