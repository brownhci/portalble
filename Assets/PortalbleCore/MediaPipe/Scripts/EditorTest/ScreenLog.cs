using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ScreenLog : MonoBehaviour {

    public static ScreenLog INSTANCE;
    private float deltaTime = 0.0f;
    private float fps = 30;
    private int max_count = 1;
    private Dictionary<int, string> dictionary_string;

    private void Awake() {
        INSTANCE = this;
        dictionary_string = new Dictionary<int, string>();
        StartCoroutine(log());
    }

    void Update() {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
        float msec = deltaTime * 1000.0f;
        dictionary_string[0] = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
    }

    public IEnumerator log() {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("fpssssssssssssssss = " + fps);
        }
    }

    void OnGUI() {
        int w = Screen.width, h = Screen.height;
        int rect_h = h * 2 / 100;

        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

        for (int i = 0; i < this.max_count; i++) {
            Rect rect = new Rect(0, i * rect_h, w, rect_h);
            GUI.Label(rect, this.dictionary_string[i], style);
        }
    }

    public int RegisterLogID() {
        this.max_count += 1;
        dictionary_string.Add(max_count - 1, "");
        return max_count - 1;
    }

    public void Log(int log_id, string text) {
        dictionary_string[log_id] = text;
    }
}
