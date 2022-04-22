using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextController : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
        transform.forward = new Vector3(-1, -1, -1);
    }

    // Update is called once per frame
    void Update()
    {
        var lookPos = cam.position - transform.position;
        lookPos.y = 0;
        lookPos.x = -1 * lookPos.x;
        lookPos.z = -1 * lookPos.z;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 0.9f);
    }
}
