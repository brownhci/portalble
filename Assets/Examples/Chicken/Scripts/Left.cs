﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Left : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Start is called before the first frame update
    bool ispressed = false;
    public GameObject chicken;
    public float speed = 2.0f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (chicken == null)
        {
            chicken = GameObject.FindGameObjectWithTag("Chicken");
        }
        if (ispressed)
        {
            chicken.GetComponent<Animator>().Play("Run");
            chicken.transform.position += new Vector3(chicken.transform.forward.x, chicken.transform.position.y, chicken.transform.forward.z) * speed * Time.deltaTime;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        chicken.transform.rotation = Quaternion.LookRotation(chicken.transform.right);
        ispressed = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        ispressed = false;
        chicken.transform.rotation = Quaternion.LookRotation(chicken.transform.right * -1);
        chicken.GetComponent<Animator>().Play("Idle");
    }
}
