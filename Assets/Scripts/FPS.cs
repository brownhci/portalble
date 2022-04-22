using UnityEngine;
using System.Collections;

/* FPS Visualizaiton from https://forum.unity.com/threads/how-can-i-display-fps-on-android-device.386250/ */

public class FPS : MonoBehaviour
{

    private int FramesPerSec;
    private float frequency = 1f;
    private string fps;
    
    void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(FPS2());
  
    }

    private IEnumerator FPS2()
    {
        for (; ; )
        {
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            // Display it

            fps = string.Format("FPS: {0}", Mathf.RoundToInt(frameCount / timeSpan));
        }
    }


    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 100, 10, 150, 20), fps);
        GUI.skin.label.fontSize = 15;
    }
}
