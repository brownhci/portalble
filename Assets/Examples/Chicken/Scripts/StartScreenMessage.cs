using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class StartScreenMessage : MonoBehaviour
{

    private ARPlaneManager planeManager;

    private Text message;

    /* timer set to close the screen message */
    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        planeManager = GameObject.Find("AR Session Origin").GetComponent<ARPlaneManager>();
        message = this.transform.GetChild(0).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (planeManager.trackables.count > 0)
        {
            message.text = "Tap the screen to generate an Object";
            if(GameObject.Find("Chicken") != null | GameObject.Find("GrabbingCube") != null | GameObject.FindGameObjectsWithTag("InteractableObj").Length != 0)
            {
                message.text = "Start Grab!";
                timer += Time.deltaTime;
                if (timer >= 3)
                    Destroy(gameObject);
            }
        }
    }
}
