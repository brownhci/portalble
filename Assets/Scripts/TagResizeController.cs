using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagResizeController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GameObject parent = transform.parent.gameObject;

		transform.localScale = (transform.parent.worldToLocalMatrix * parent.GetComponent<Renderer>().bounds.size);
	}
}
