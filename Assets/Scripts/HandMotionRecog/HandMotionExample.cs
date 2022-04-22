using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMotionExample : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log (HandActionRecog.getInstance ());
		if(HandActionRecog.getInstance ().BeginMotion ("Basic")) {
			HandActionRecog.getInstance ().DefineTransform ("palm|pinch|undefined", "palm|pinch|undefined", -Vector3.up, Vector3.up, 1.0f);
			HandActionRecog.getInstance ().DefineTransform ("palm|pinch|undefined", "palm|pinch|undefined", Vector3.up, -Vector3.up, 1.0f);
			HandActionRecog.getInstance ().EndMotion ();
			Debug.Log ("defined gesture Basic");
		}
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("Action Current:" + GameObject.Find ("Hand_l").GetComponent<GestureControl> ().bufferedGesture ());
		Debug.Log ("Action Recog:" + HandActionRecog.getInstance ().IsMotion ("Basic"));
	}
}
