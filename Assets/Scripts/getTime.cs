using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class getTime : MonoBehaviour {

    private float m_time;
	// Use this for initialization
	void Start () {
        m_time = 0;
	}
	
	// Update is called once per frame
	void Update () {
        m_time = Time.time;
	}

    public float getCurrentTime() {
        return m_time;
    }
}

