using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour {


    private Vector3 hand_l_position;
	private bool hand_l_busy, hand_r_busy;
	private GameObject hand_l_obj, hand_r_obj;
	public int gestBuffer = 3;
	public int unCollidingBuffer = 1;
	public bool useFingerBuffer = true;
	public int handThrowingPowerMultiplier = 20;
	public float palm_collider_delay = 1.0f;

    // Use this for initialization
    void Start () {
		hand_l_position = new Vector3 (0, 0, 0);	
	}

	// Update is called once per frame
	void Update () {

	}

	public void setPalmColliderDelay(float set_to){
		palm_collider_delay = set_to;
	}

	public float getPalmColliderDelay(){
		return palm_collider_delay;
	}

	public void setLeftHandPosition(Vector3 v){
		hand_l_position = v;
	}

	public void setLeftHandVelocity(Vector3 v){
	}

	public void setLeftHandBusyOn(){
		hand_l_busy = true;
	}

	public void setLeftHandBusyOff(){
		hand_l_obj = null;
		hand_l_busy = false;
	}

	public void setRightHandBusyOn(){
		hand_r_busy = true;
	}

	public void setRightHandBusyOff(){
		hand_r_obj = null;
		hand_r_busy = false;
	}

	public bool checkLeftHandBusy(){
		return hand_l_busy;
	}


	public bool checkRightHandBusy(){
		return hand_r_obj;
	}

	public void setLeftHandObject(GameObject obj){
		hand_l_busy = true;
		hand_l_obj = obj;
		return;
	}

	public void setRightHandObject(GameObject obj){
		hand_r_obj = obj;
		return;
	}

	public GameObject getLeftHandObject(){
		return hand_l_obj;
	}

	public Vector3 getLeftHandPosition (){
		return hand_l_position;
	}

}
