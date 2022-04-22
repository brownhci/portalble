using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARPlaneManager))]
public class ARPlaneController : MonoBehaviour
{

    public GameObject Hide_button;
    private ARPlaneManager planeManager;
    [SerializeField]
    private bool hide = true;

    private void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
    }

   
    // Start is called before the first frame update
    void Start()
    {
        Hide_button.GetComponent<Button>().onClick.AddListener(delegate { TogglePlaneDetection(); });
    }

    private void SetAllPlanesActive(bool value)
    {
        foreach(var plane in planeManager.trackables)
        {
            plane.GetComponent<ChangeMaterial>().Toggle(value);
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (hide)
            SetAllPlanesActive(false);
        else
            SetAllPlanesActive(true);
    }

    public void TogglePlaneDetection()
    {
        hide = !hide;
    }
}
