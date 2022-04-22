using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionScriptObject : MonoBehaviour {
	Dictionary<string, int> hand_nearby = new Dictionary<string, int>();
	private bool isInteracted, notified = false;
	private bool useFingerBuffer = false;
	private DataManager dataManager;
	private HandManager hand_l, hand_r;

	private bool shortVibrTriggered;
	//inserted hand finger buffers
	private int fingers_buff_len = 15;
	private int fingers_buff_idx;
	private int[] hand_l_fingers_buff, hand_r_fingers_buff;

	//Highlight variables
	public Material[] primaryMaterial;
	public Material secondaryMaterial;

	// Use this for initialization
	void Start () {
		//Initialize dictionary for calculating num of fingers inserted into this.gameObject
		hand_nearby.Add("Hand_l",0);
		hand_nearby.Add("Hand_r",0);

		shortVibrTriggered = false;

		//Initialize all managers
		dataManager = GameObject.Find ("gDataManager").GetComponent<DataManager> ();
		hand_l = GameObject.Find ("Hand_l").GetComponent<HandManager> ();
		hand_r = GameObject.Find ("Hand_r").GetComponent<HandManager> ();

		fingers_buff_len = dataManager.unCollidingBuffer;
		//Initialize highlight main texture

		if (this.GetComponent<Renderer> () != null) {
			primaryMaterial = new Material[1];
			primaryMaterial[0] = this.GetComponent<Renderer> ().material;
		} else {
			primaryMaterial = new Material[transform.childCount];

			int i = 0;
			foreach (Transform child in transform) {
				primaryMaterial [i] = child.GetComponent<Renderer> ().material;
				i++;
			}
		}

		//Initialize hand finger buffers
		hand_l_fingers_buff = new int[fingers_buff_len];
		hand_r_fingers_buff = new int[fingers_buff_len];

		/* check if need finger buffer */
		useFingerBuffer = dataManager.useFingerBuffer;
	}

	// Update is called once per frame
	void Update () {
		//Update hand finger buffer
		int hand_l_fingers,hand_r_fingers;
		hand_nearby.TryGetValue ("Hand_l", out hand_l_fingers);
		hand_nearby.TryGetValue ("Hand_r", out hand_r_fingers);
		hand_l_fingers_buff [fingers_buff_idx] = hand_l_fingers;
		hand_r_fingers_buff [fingers_buff_idx] = hand_r_fingers;
		fingers_buff_idx = (fingers_buff_idx + 1) % fingers_buff_len;


		//If this is not interacted with any hand, check if this object could be set as left/right hand's only interactable object
		if (!isInteracted) {
			if (bufferedLeftHandFingers () > 0) {
				if (!hand_l.checkHandBusy ()) {
					//Set left hand object if it is not in painting
					if (hand_l.setHandObject (this.gameObject)){
						isInteracted = true;
						highlightSelf ();
						/* vibration */
						if (!shortVibrTriggered) {
							
							Vibration.Vibrate (30);
							shortVibrTriggered = true;
						}
					}
				}
			} else if (hand_nearby.TryGetValue ("Hand_r", out hand_r_fingers) && hand_r_fingers > 0) {
				//TODO: finish it if we need right hand
			}
		}
		//If this is current being interacted with any hand, remove itself from interactable list of left/right hand whenever left/right hand leaves object
		else {
			if (bufferedLeftHandFingers () == 0 && useFingerBuffer) {
				releaseSelf();
			}
		}
	}

	/* 	releaseSelf
	*	Input: None
	*	Output: None
	*	Summary: Reset self as not interactable any more
	*/
	public void releaseSelf(){
		isInteracted = notified = false;
		unhighlightSelf ();
		hand_l.removeHandObject ();
		shortVibrTriggered = false;
	}

	/* 	notifyLeave
	*	Input: None
	*	Output: None
	*	Summary: Use for hand manager to notify this object to leave 
	*/
	public void notifyLeave(){
		notified = true;	
	}

	private int bufferedLeftHandFingers (){
		int avg_fingers = 0;
		for (int i = 0; i < fingers_buff_len; i++)
			avg_fingers += hand_l_fingers_buff [i];
		return (int)(Math.Ceiling((double)avg_fingers / (double)fingers_buff_len));
	}

	private int bufferedRightHandFingers (){
		int avg_fingers = 0;
		for (int i = 0; i < fingers_buff_len; i++)
			avg_fingers += hand_r_fingers_buff [i];
		return (int)(Math.Ceiling((double)avg_fingers / (double)fingers_buff_len));
	}

	private void highlightSelf(){
		if (this.GetComponent<Renderer> () != null) {
			secondaryMaterial.mainTexture = primaryMaterial [0].mainTexture;
			secondaryMaterial.color = primaryMaterial[0].color;
			this.GetComponent<Renderer> ().material = secondaryMaterial;
		} else {
			int i = 0;
			foreach (Transform child in transform) {
				secondaryMaterial.mainTexture = primaryMaterial [i].mainTexture;
				child.gameObject.GetComponent<Renderer> ().material = secondaryMaterial;
			}
		}

	}

	private void unhighlightSelf(){
		if (this.GetComponent<Renderer> () != null) {
			this.GetComponent<Renderer> ().material = primaryMaterial[0];
		} else {
			int i = 0;
			foreach (Transform child in transform) {
				child.gameObject.GetComponent<Renderer> ().material = primaryMaterial [i];
			}
		}
	}

	private void OnRaycastEnter(GameObject sender){

	}

	private void OnRaycastExit(GameObject sender){

	}

	private void updateHighlight(GameObject sender){
		float highlight_threshold = 0.35f;
		float dist_obj_cam = Vector3.Distance (sender.transform.position, this.transform.position);
		if (dist_obj_cam < highlight_threshold){
			/*Set the _emissionColor according to dist from sender to this.gameObject*/
			Color add_color = new Vector4 (0.8f, 0.8f, 0.8f, 0f);
			Color cur_color_max = add_color*((highlight_threshold - dist_obj_cam)/highlight_threshold);
			this.GetComponent<Renderer> ().material.SetColor("_EmissionColor", cur_color_max);
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.transform.parent != null){
			if (other.transform.parent.parent != null) {
				if (other.transform.parent.parent.name == "Hand_l" || other.transform.parent.parent.name == "Hand_r")
					hand_nearby [other.transform.parent.parent.name] += 1;
			}else {
				if (other.transform.parent.name == "Hand_l" || other.transform.parent.name == "Hand_r")
					hand_nearby [other.transform.parent.name] += 1;
			}
		}
	}

	void OnTriggerExit(Collider other){
		if (other.transform.parent != null) {
			if (other.transform.parent.parent != null) {
				if (other.transform.parent.parent.name == "Hand_l" || other.transform.parent.parent.name == "Hand_r")
					hand_nearby [other.transform.parent.parent.name] -= 1;
			}else {
				if (other.transform.parent.name == "Hand_l" || other.transform.parent.name == "Hand_r")
					hand_nearby [other.transform.parent.name] -= 1;
			}
		} 

	}
}
