using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractionScript : MonoBehaviour {
	/**
	 * Child indexes of hand represent:
	 * 0 - thumb
	 * 1 - index
	 * 2 - middle
	 * 3 - pinky
	 * 4 - ring
	 * 5 - palm for another hand
	 * 6- forearm
	 */
	private GameObject hand_l,hand_r;

	//Left hand finger declare
	private GameObject thumb_l, indexfinger_l, middlefinger_l, ringfinger_l, palm_l;
	private GameObject thumb_l_2, indexfinger_l_2, middlefinger_l_2, ringfinger_l_2;
	//Right hand finger declare
	private GameObject thumb_r, indexfinger_r, middlefinger_r, ringfinger_r, palm_r;
	private GameObject thumb_r_2, indexfinger_r_2, middlefinger_r_2, ringfinger_r_2;

	private GameObject grabHolder;
	private DataManager dataManager;

	//Boolean var to record current grab motion
	bool grabbed = false;

	private int sizeOfSpeedQ = 5; 
	private Queue<Vector3> speedList = new Queue<Vector3>();

	private int sizeOfPosQ = 6; 
	private Queue<Vector3> posList_l = new Queue<Vector3>();
	private Queue<Vector3> posList_r = new Queue<Vector3>();

	private float colliderReenableTime = 1.5f;
	private float add;
	static float restoreColliderTimer;
		
	private Vector3 prePos;

	//Temp method for pose pointing gesture (delete)
	private float dist_thumb_index_r_initial;

	// Use this for initialization
	void Start () {
		//0.21 finger to palm
		dataManager = GameObject.Find ("gDataManager").GetComponent<DataManager> ();
		hand_l = GameObject.Find ("Hand_l").gameObject;
		hand_r = GameObject.Find ("Hand_r").gameObject;

		for (int i=0; i < sizeOfSpeedQ; i++)
			speedList.Enqueue(new Vector3(0,0,0));

		thumb_l = hand_l.transform.GetChild (0).gameObject;
		indexfinger_l = hand_l.transform.GetChild (1).gameObject;
		middlefinger_l = hand_l.transform.GetChild (2).gameObject;
		ringfinger_l = hand_l.transform.GetChild (3).gameObject;
		palm_l = hand_l.transform.GetChild (5).gameObject;

		thumb_l_2 = thumb_l.transform.GetChild (2).gameObject;
		indexfinger_l_2 = indexfinger_l.transform.GetChild (2).gameObject;
		middlefinger_l_2 = middlefinger_l.transform.GetChild (2).gameObject;
		ringfinger_l_2 = ringfinger_l.transform.GetChild (2).gameObject;

		thumb_r = hand_r.transform.GetChild (0).gameObject;
		indexfinger_r = hand_r.transform.GetChild (1).gameObject;
		middlefinger_r = hand_r.transform.GetChild (2).gameObject;
		ringfinger_r = hand_r.transform.GetChild (3).gameObject;
		palm_r = hand_r.transform.GetChild (5).gameObject;

		thumb_r_2 = thumb_r.transform.GetChild (2).gameObject;
		indexfinger_r_2 = indexfinger_r.transform.GetChild (2).gameObject;
		middlefinger_r_2 = middlefinger_r.transform.GetChild (2).gameObject;
		ringfinger_r_2 = ringfinger_r.transform.GetChild (2).gameObject;

		grabHolder = palm_l.transform.GetChild (0).gameObject;

		for (int i = 0; i < sizeOfPosQ; i++) {
			posList_l.Enqueue(new Vector3(0,0,0));
			posList_r.Enqueue(new Vector3(0,0,0));
		}

		prePos = new Vector3 (0, 0, 0);
	}

	bool isMoving(float thredValue){
		//check movement on 3 axis
		Vector3 tmp = this.transform.position - prePos;
		prePos = this.transform.position;
		if ((Math.Abs (tmp [0]) < thredValue) &&
		   (Math.Abs (tmp [1]) < thredValue) &&
		   (Math.Abs (tmp [2]) < thredValue))
			return true;
		return false;
	}

	// Update is called once per frame
	void Update () {

		//Grab Detection: 
		//With rigidbody
		Collider c = this.GetComponent<Collider>();
		float dist_thumb_index_l = Vector3.Distance(thumb_l_2.transform.position, indexfinger_l_2.transform.position);
		float dist_thumb_index_r = Vector3.Distance(thumb_r_2.transform.position, indexfinger_r_2.transform.position);

		Vector3 indexfinger_0_palm_r = indexfinger_r.transform.GetChild(0).position - palm_r.transform.position;
		Vector3 indexfinger_2_thumb1_r = indexfinger_r.transform.GetChild(2).position - palm_r.transform.position;
		float curve_indexfinger_r = Vector3.Angle(indexfinger_2_thumb1_r, indexfinger_0_palm_r);

		//Determine whethere a grab happnens. Left hand has priority here.
		if ((dist_thumb_index_l < 0.065f) && 
			c.bounds.Intersects (thumb_l_2.GetComponent<Collider> ().bounds) &&
			(c.bounds.Intersects (indexfinger_l_2.GetComponent<Collider> ().bounds) ||
				c.bounds.Intersects (middlefinger_l_2.GetComponent<Collider> ().bounds) ||
				c.bounds.Intersects (ringfinger_l_2.GetComponent<Collider> ().bounds)) && 
			!dataManager.checkLeftHandBusy() ) {

			//Record current velocity and delete the oldest velocity
			Vector3 initial_v = new Vector3 ();
			initial_v = palm_l.GetComponent<Rigidbody> ().velocity;
			speedList.Dequeue ();
			speedList.Enqueue (initial_v);
			grabbed = true;
			restoreColliderTimer = Time.time;
			this.GetComponent<Rigidbody> ().isKinematic = true;
			dataManager.setLeftHandBusyOn ();
			disableFingersCollider ();
			this.transform.SetParent(grabHolder.transform);
		} else if (dist_thumb_index_r < 0.060 && (curve_indexfinger_r>15) &&
			(c.bounds.Intersects (thumb_r_2.GetComponent<Collider> ().bounds) || c.bounds.Contains (thumb_r_2.transform.position)) &&
			((c.bounds.Intersects (indexfinger_r_2.GetComponent<Collider> ().bounds) ||
					c.bounds.Intersects (middlefinger_r_2.GetComponent<Collider> ().bounds) ||
					c.bounds.Intersects (ringfinger_r_2.GetComponent<Collider> ().bounds)))) {
		} 
		else if (grabbed == true && dist_thumb_index_l > 0.065f) {
			grabbed = false;
			this.transform.parent = null;
			this.GetComponent<Rigidbody> ().isKinematic = false;
			dataManager.setLeftHandBusyOff();

			int num_speed = speedList.Count;
			Vector3 average = new Vector3 (0, 0, 0);
			for (int i = 0; i < sizeOfSpeedQ; i++) {
				average += speedList.Dequeue ();
				speedList.Enqueue (new Vector3 (0, 0, 0));
			}
			this.GetComponent<Rigidbody> ().velocity = palm_l.GetComponent<Rigidbody> ().velocity* 0.8f;
			restoreColliderTimer = Time.time;
		} else {
			float diff = Time.time - restoreColliderTimer;

			//Wait a certain interval to re-enable the collider of the hand
			if (diff > colliderReenableTime) {
				for (int i = 0; i < 2; i++) {
					palm_l.GetComponent<Collider> ().isTrigger = false;

					palm_r.GetComponent<Collider> ().isTrigger = false;

					enableFingersCollider ();
				}
			}
		}


		//Hover feature 2
		float dist_obj_palm_l = Mathf.Abs(Vector3.Distance(this.transform.position, palm_l.transform.position));
		float dist_obj_palm_r = Mathf.Abs(Vector3.Distance(this.transform.position, palm_r.transform.position));
		float threshold = 0.25f;
		if ((dist_obj_palm_l < threshold || dist_obj_palm_r < threshold)) {
			//Map distance to color
			float percent = Mathf.Pow(Mathf.Min((threshold - Mathf.Min (dist_obj_palm_l, dist_obj_palm_r)) / (threshold/2), 1), 2);
			float max_brightness = 0.35f;
			float min_brightness = 0f;

			if (this.GetComponent<Renderer> ().material.GetColor ("_EmissionColor").r > max_brightness) {
				add = -0.03f;
			} else if (this.GetComponent<Renderer> ().material.GetColor ("_EmissionColor").r <= min_brightness){
				add = 0.03f;
			}
			Color add_color = new Vector4 (add, add, add, 0f);
			Color cur_color =  this.GetComponent<Renderer> ().material.GetColor("_EmissionColor") + percent*add_color;
			this.GetComponent<Renderer> ().material.SetColor("_EmissionColor", cur_color);
		}else{
			Color cur_color =  new Vector4 (0f, 0f, 0f, 0f);
			this.GetComponent<Renderer> ().material.SetColor("_EmissionColor", cur_color);
		}
	}

	private void disableFingersCollider(){
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 1; j++) {
				hand_l.transform.GetChild (i).GetChild (j).GetComponent<Rigidbody> ().isKinematic = true;
			}
		}
		return;
	}

	private void enableFingersCollider(){
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 1; j++) {
				hand_l.transform.GetChild (i).GetChild (j).GetComponent<Rigidbody> ().isKinematic = false;
			}
		}
		return;
	}

	private void OnDrawGizmosSelected () {
		if(hand_l != null) {
			// Draws a blue line from this transform to the target
			Gizmos.color = Color.blue;
			Gizmos.DrawLine (transform.position, hand_l.transform.position);
		}
	}


	/* 	Add force function 
	*	Input: GameObject obj
	*	Output: None
	*/
	private void addForce(GameObject obj){ 
		/* Add force */
		Collider c = obj.GetComponent<Collider>();
		if (c.bounds.Intersects (palm_l.GetComponent<Collider> ().bounds)) {
			Vector3 average = new Vector3 (0, 0, 0);
			Vector3 last_pos = posList_l.Dequeue ();
			posList_l.Enqueue (new Vector3 (0, 0, 0));
			for (int i = 0; i < sizeOfSpeedQ; i++) {
				average += posList_l.Dequeue () - last_pos;
				posList_l.Enqueue (new Vector3 (0, 0, 0));
			}
			average = average / posList_l.Count;
			obj.GetComponent<Rigidbody> ().AddForce (average*2f);
		}

		if (c.bounds.Intersects (palm_r.GetComponent<Collider> ().bounds)) {
			Vector3 average = new Vector3 (0, 0, 0);
			Vector3 last_pos = posList_r.Dequeue ();
			posList_r.Enqueue (new Vector3 (0, 0, 0));
			for (int i = 0; i < sizeOfSpeedQ; i++) {
				average += posList_r.Dequeue () - last_pos;
				posList_r.Enqueue (new Vector3 (0, 0, 0));
			}
			average = average / posList_r.Count;
			obj.GetComponent<Rigidbody> ().AddForce (average*2f);
		}

	}

}
