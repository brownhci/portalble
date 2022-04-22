using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class distHighlightFeature : MonoBehaviour {
	private GameObject palm;
	const string updateHighlightMessage = "updateHighlight";
	public bool isEnable = true;

	// Use this for initialization
	void Start () {
		palm = GameObject.Find ("Hand_l").transform.GetChild(5).gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (isEnable)
			closestObject ();
	}

	/* 	closestObject
	*	Input: None
	*	Output: None
	*	Summary: Send msg to any object interesected by the raycast which shoots from palm center to palm.norm direction
	*/
	private void closestObject(){
		Collider[] collider_group = Physics.OverlapSphere (palm.transform.position, 0.35f);
		foreach (Collider c in collider_group) {
			if (c.tag == "InteractableObj")
				SendMessageTo (updateHighlightMessage, c.gameObject);
		}
	}

	private void SendMessageTo(string msg, GameObject tar){
		if (tar)
			tar.SendMessage (msg, this.gameObject, SendMessageOptions.DontRequireReceiver);
	}
}
