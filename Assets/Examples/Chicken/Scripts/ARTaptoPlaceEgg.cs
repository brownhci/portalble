using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]

public class ARTaptoPlaceEgg : MonoBehaviour
{
    public GameObject gameObjectToInstantiate;
    private GameObject spawnedObject;
    private ARRaycastManager _arRaycastManager;
    private Vector2 touchPosition;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private GameObject DC;

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }

    private void Start()
    {
        DC = GameObject.Find("DemoController");
    }
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }
    // Update is called once per frame
    /* Detect the touch position on the screen and place the egg only once*/
    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }
        if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            if (spawnedObject == null & DC.GetComponent<ChickenDemoController>().chickens.Count == 0)
            {
                spawnedObject = Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
                DC.GetComponent<ChickenDemoController>().eggs.Add(spawnedObject);
            }
        }
    }
}
