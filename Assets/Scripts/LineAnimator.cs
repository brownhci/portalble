using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAnimator : MonoBehaviour {
    public float moveSpeed = 1.0f;
    private Material m_material;

	// Use this for initialization
	void Start () {
        m_material = GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
		if (m_material != null) {
            Vector2 newoffset = m_material.mainTextureOffset;
            newoffset.x += moveSpeed * Time.deltaTime;
            if (newoffset.x >= 1.0f)
                newoffset.x -= 1.0f;
            m_material.mainTextureOffset = newoffset;
        }
	}
}
