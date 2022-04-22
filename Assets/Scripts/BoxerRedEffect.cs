using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerRedEffect : MonoBehaviour {
    public float fadeTime = 0.5f;
    public Color destColor;
    private Color originColor;
    private float fadeCount = 0.5f;

    void Start() {
        originColor = GetComponent<Renderer>().material.color;    
    }
    // Update is called once per frame
    void Update () {
        if (fadeCount < fadeTime) {
            float t = 1.0f - fadeCount / fadeTime;
            Color final = Color.Lerp(originColor, destColor, t);
            GetComponent<Renderer>().material.color = final;

            fadeCount += Time.deltaTime;
            if (fadeCount > fadeTime)
                fadeCount = fadeTime;
        }
	}

    public void Flash (float second) {
        if (second < 0f)
            second = 0.5f;
        fadeTime = second;
        fadeCount = 0f;
    }
}
