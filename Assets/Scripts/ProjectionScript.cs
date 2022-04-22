using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProjectionScript : MonoBehaviour {
	public GameObject thumb;
	public GameObject thumb1;
	public GameObject indexfinger;
	public GameObject middlefinger;
	public GameObject ringfinger;
	public GameObject palm;
	public GameObject thumb_r;
	public GameObject thumb1_r;
	public GameObject indexfinger_r;

	public GameObject idxfinger_r;
	public GameObject thmb_r;
	public GameObject midfinger_r;

	public GameObject middlefinger_r;
	public GameObject ringfinger_r;
	public GameObject palm_r;
	public Material mat;
	public Color color;
	public Queue<Vector3> speedList = new Queue<Vector3>();
	public GameObject Hand_L, Hand_R;
	bool grabbed = false;
	static float restoreColliderTimer;
	//public Camera cam;
	// Use this for initialization
	void Start () {
		float dist_thumb_fore = Vector3.Distance(thumb.transform.position, indexfinger.transform.position);
		//0.21 finger to palm
		for (int i=0; i < 10; i++)
			speedList.Enqueue(new Vector3(0,0,0));
	}

	//Simplified version of code


	// Update is called once per frame
	void Update () {
		//Grab Detection: 

		//With rigidbody
		Collider c = this.GetComponent<Collider>();
		float dist_palm_index = Vector3.Distance(palm.transform.position, indexfinger.transform.position);
		float dist_palm_middle = Vector3.Distance(palm.transform.position, middlefinger.transform.position);
		float dist_palm_ring = Vector3.Distance(palm.transform.position, ringfinger.transform.position);
		float dist_palm_thumb = Vector3.Distance(palm.transform.position, thumb.transform.position);
		float dist_thumb_index = Vector3.Distance(thumb.transform.position, indexfinger.transform.position);
		float dist_thumb_index_r = Vector3.Distance(thumb_r.transform.position, indexfinger_r.transform.position);

		//Method 3(w/o rigidBoy of object): The fingers' collider interesect with the object
		float grav = -0.01f;
		float v_y = grav;
		
		//use vector to help detect grab
		Vector3 index_thumb1 = indexfinger.transform.position - thumb1.transform.position;
		Vector3 thumb_thumb1 = thumb.transform.position - thumb1.transform.position;
		float angle = Vector3.Angle(index_thumb1, thumb_thumb1);
		Vector3 index_thumb1_r = indexfinger_r.transform.position - thumb1_r.transform.position;
		Vector3 thumb_thumb1_r = thumb_r.transform.position - thumb1_r.transform.position;
		float angle_r = Vector3.Angle(index_thumb1_r, thumb_thumb1_r);
		Vector3 Velocity = new Vector3(0,0,0);
		if ((angle < 20 || (angle > 30 && angle < 45) || dist_thumb_index < 0.085) &&
		     c.bounds.Intersects (thumb.GetComponent<Collider> ().bounds) &&
		     (c.bounds.Intersects (indexfinger.GetComponent<Collider> ().bounds) ||
		     c.bounds.Intersects (middlefinger.GetComponent<Collider> ().bounds) ||
		     c.bounds.Intersects (ringfinger.GetComponent<Collider> ().bounds))) {
			
			this.transform.parent = palm.transform;
			Rigidbody palm_rb = palm.GetComponent<Rigidbody> ();
			Vector3 initial_v = new Vector3 ();
			initial_v = palm_rb.velocity;
			speedList.Dequeue ();
			speedList.Enqueue (initial_v);
			Velocity = initial_v;
			grabbed = true;
			restoreColliderTimer = Time.time;

		} else if (dist_thumb_index_r < 0.085 &&
		           (c.bounds.Intersects (thumb_r.GetComponent<Collider> ().bounds) || c.bounds.Contains (thumb_r.transform.position)) &&
		           ((c.bounds.Contains (indexfinger_r.transform.position) ||
		           c.bounds.Contains (middlefinger_r.transform.position) ||
		           c.bounds.Contains (ringfinger_r.transform.position)) ||

		           (c.bounds.Intersects (indexfinger_r.GetComponent<Collider> ().bounds) ||
		           c.bounds.Intersects (middlefinger_r.GetComponent<Collider> ().bounds) ||
		           c.bounds.Intersects (ringfinger_r.GetComponent<Collider> ().bounds)))) {
			this.GetComponent<Rigidbody> ().isKinematic = true;
			this.transform.parent = palm_r.transform;
			Rigidbody palm_rb = palm_r.GetComponent<Rigidbody> ();
			Vector3 initial_v = new Vector3 ();
			initial_v = palm_rb.velocity;
			speedList.Dequeue ();
			speedList.Enqueue (initial_v);
			Velocity = initial_v;
			grabbed = true;
			for (int i = 0; i < 2; i++) {
				idxfinger_r.transform.GetChild (i).GetComponent<Collider> ().isTrigger = true;
				thmb_r.transform.GetChild (i).GetComponent<Collider> ().isTrigger = true;
				midfinger_r.transform.GetChild (i).GetComponent<Collider> ().isTrigger = true;
				palm_r.GetComponent<Collider> ().isTrigger = true;
			}
			restoreColliderTimer = Time.time;

		} else if (grabbed == true) {
			grabbed = false;
			this.transform.parent = null;
			this.GetComponent<Rigidbody> ().isKinematic = false;
			int num_speed = speedList.Count;
			Vector3 average = new Vector3 (0, 0, 0);
			for (int i = 0; i < 10; i++) {
				average += speedList.Dequeue ();
				speedList.Enqueue (new Vector3 (0, 0, 0));
			}
			average = 0.8f * (average / num_speed);
			this.GetComponent<Rigidbody> ().velocity = average;
			restoreColliderTimer = Time.time;

		} else {
			float diff = Time.time - restoreColliderTimer;
			if (diff > 1.5f) {
				for (int i = 0; i < 2; i++) {
					idxfinger_r.transform.GetChild (i).GetComponent<Collider> ().isTrigger = false;
					thmb_r.transform.GetChild (i).GetComponent<Collider> ().isTrigger = false;
					midfinger_r.transform.GetChild (i).GetComponent<Collider> ().isTrigger = false;
					palm_r.GetComponent<Collider> ().isTrigger = false;
				}
			}
		}


		//Hover feature
		float dist_obj_palm = Vector3.Distance(this.transform.position, palm.transform.position);
		float dist_obj_palm_r = Vector3.Distance(this.transform.position, palm_r.transform.position);
		float threshold = 0.25f;
		if (dist_obj_palm < threshold || dist_obj_palm_r < threshold) {
			this.GetComponent<Renderer> ().material.SetFloat ("_Metallic", (threshold - Mathf.Min (dist_obj_palm, dist_obj_palm_r)) / threshold);
		}
		else{
			this.GetComponent<Renderer> ().material.SetFloat ("_Metallic", 0);
		}
	}
	private void disableCollider(GameObject finger){
		for (int i = 0; i < 2; i++) {
			finger.transform.GetChild (i).GetComponent<Collider> ().isTrigger = true;
		}
		return;
	}

	private void enableCollider(GameObject finger){
		for (int i=0; i<2; i++)
			finger.transform.GetChild (i).GetComponent<Collider>().isTrigger = false;
		return;
	}


}
