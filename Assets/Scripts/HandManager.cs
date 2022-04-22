using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour {
	//Left hand has priority
	private DataManager dataManager;
	private GestureControl gestureManager;
	private PaintManager paintManager;
	private GameObject grabHolder, palm;
	private bool is_grabbing = false;
	public bool push_enabled = true;
    public bool reEnableGravity = false;
	private IEnumerator coroutine;
	private float palm_collider_delay;

	//Context: objct, paint, menu
	private string context = "object";
	private int context_buff_len = 30;
	private int context_buff_idx;
	private int[] context_buff;
	Dictionary<int, string> context_dict = new Dictionary<int, string>();

	// These methods will be called on the object it hits.
	const string OnRaycastExitMessage = "OnRaycastExit";
	const string OnRaycastEnterMessage = "OnRaycastEnter";
	private GameObject prev_hit;

    private int m_currentMenu = 0;
	private GameObject indexFinger;
	private GameObject thumbFinger;

	Vector3[] indexFingerPos;
	Vector3[] thumbFingerPos;
	Vector3 pIndexFingerPos;
	Vector3 pThumbFingerPos;

	private int indexFingerPosCNT = 0 ;
	private int thumbFingerPosCNT = 0 ;

	private bool pulsationFlag = false;
    private int _handThrowingPowerMultiplier = 10;

	//MA buffer
	private int smoothingBuffer = 1;
	private int smoothingBuffer_idx = 0;
	private Vector3[] objBuffer;

    // For Grabbing Orientation
    private Quaternion inverseGrabRotation;
    private Vector3 grabDeltaPosition;

    public bool bIsLeftHand = true;

	// Use this for initialization
	void Start () {

		dataManager = GameObject.Find ("gDataManager").GetComponent<DataManager> ();
		gestureManager = this.GetComponent<GestureControl> ();
		paintManager = this.GetComponent<PaintManager> ();
		palm = this.transform.GetChild (5).gameObject;
		grabHolder = this.transform.GetChild (5).GetChild (0).gameObject;
		thumbFinger = this.transform.GetChild (0).GetChild (2).gameObject;
		indexFinger = this.transform.GetChild (1).GetChild (2).gameObject;

		indexFingerPos = new Vector3[10];
		thumbFingerPos = new Vector3[10];
		pIndexFingerPos = Vector3.zero;
		pThumbFingerPos = Vector3.zero;

		_handThrowingPowerMultiplier = dataManager.handThrowingPowerMultiplier;

		context_buff = new int[context_buff_len];
		context_dict.Add (0, "object");
		context_dict.Add (1, "paint");
		context_dict.Add (2, "menu");
        context_dict.Add (3, "brush");

		/*initial all user-defined settings*/
		DataManager data_mngr =  GameObject.Find ("gDataManager").GetComponent<DataManager> ();
		palm_collider_delay = data_mngr.getPalmColliderDelay ();
		palm.GetComponent<Rigidbody> ().maxAngularVelocity = 0;
        
        // register new motion
		if(HandActionRecog.getInstance() != null && HandActionRecog.getInstance().BeginMotion("OpenMenu")) {
			HandActionRecog.getInstance ().DefineTransform ("palm|pinch|undefined", "palm|pinch|undefined", Vector3.up, -Vector3.up, 1.0f);
			HandActionRecog.getInstance ().DefineTransform ("palm|pinch|undefined", "palm|pinch|undefined", -Vector3.up, Vector3.up, 1.0f);
			HandActionRecog.getInstance ().EndMotion ();
		}
	}

	// Update is called once per frame
	void Update () {
		if (HandActionRecog.getInstance() != null && HandActionRecog.getInstance().IsMotion("OpenMenu", bIsLeftHand)) {
			contextSwitch ("menu");
		}
	switch (context){
		    case "menu":
			    break;
		    case "paint":
			    break;
		    default:
			GameObject interact_obj = getHandObject ();
			if (interact_obj != null) {
				cleanGuidance ();
                //if hand gesture is grabbing
                string cur_gesture = gestureManager.bufferedGesture();
                 
				if (cur_gesture == "pinch" || cur_gesture == "fist") {
					//grab objbect if hand is not grabbing
					if (!is_grabbing) {
						grabObject (interact_obj);
					} else {
						this.updateObjTransform (interact_obj);
					}
				    //if hand gesture is not grabbing
				} else {
					//but hand is grabbing
                    Transform indextip = transform.Find("index").Find("bone3");
                    Transform thumbtip = transform.Find("thumb").Find("bone3");
                    float index_thumb_dis = Vector3.Distance(indextip.position, thumbtip.position);
                    
					if (is_grabbing && (cur_gesture == "palm" || index_thumb_dis > 0.04f)) {
						//then tell the object to release itself, Here support two version of interaction objects.
						InteractionScriptObject iso = interact_obj.GetComponent<InteractionScriptObject> ();
						if (iso != null && iso.isActiveAndEnabled) {
							iso.releaseSelf ();
						}
						else {
							GrabCollider gc = interact_obj.GetComponentInChildren<GrabCollider> ();
							if (gc != null)
								gc.OnGrabFinished ();
						}
						CancelInvoke();
					}
                    else if (is_grabbing) {
                        this.updateObjTransform(interact_obj);
                    }
				}

			/* nothing in hand (interactable object is null)*/
			} else {
				if (gestureManager.bufferedGesture () == "palm")
					hitObject ();
				else {
					cleanGuidance ();
				}
			}
			break;
		}
		updateHandSpeed ();
	}

	/* pulsation */
	private void VPulse(){
		Vibration.Vibrate (30);
	}
	/* 	hitObject
	*	Input: None
	*	Output: None
	*	Summary: Send msg to any object interesected by the raycast which shoots from palm center to palm.norm direction
	*/
	private void hitObject(){
		RaycastHit hit;
		if (Physics.Raycast (palm.transform.position, palm.transform.forward, out hit, 10f) ) {
			GameObject cur_hit = hit.collider.gameObject;
			if (prev_hit != cur_hit && cur_hit.tag == "InteractableObj") {
				SendMessageTo (OnRaycastExitMessage, prev_hit);
				SendMessageTo (OnRaycastEnterMessage, cur_hit);
				prev_hit = cur_hit;
			}
		} else {
			SendMessageTo (OnRaycastExitMessage, prev_hit);
			prev_hit = null;
		}
		guideToObject ();
	}

	/* 	guideToObject
	*	Input: None
	*	Output: None
	*	Summary: Create a arrow between object and palm
	*/
	private void guideToObject(){
        GameObject arrow = GameObject.Find("arrow");
        if (arrow == null || arrow.activeInHierarchy == false)
            return;

        if (prev_hit) {
			arrow.transform.GetChild (0).gameObject.SetActive(true);
			arrow.transform.position = palm.transform.position + 0.02f * palm.transform.forward;
			arrow.transform.up = palm.transform.forward;
		} else {
			arrow.transform.GetChild (0).gameObject.SetActive(false);
		}
	}
    
    /* 	hitObject
	*	Input: None
	*	Output: None
	*	Summary: Clean 
	*/
	private void cleanGuidance(){
		if (prev_hit != null) {
			SendMessageTo (OnRaycastExitMessage, prev_hit);
			prev_hit = null;
			guideToObject ();
		}
	}

	private void SendMessageTo(string msg, GameObject tar){
		if (tar)
			tar.SendMessage (msg, this.gameObject, SendMessageOptions.DontRequireReceiver);
	}

	/* 	contextSwtitch
	*	Input: String set_to_context
	*	Output: None
	*	Summary: Switch context: 1. 'object' to 'paint' 2. 'paint' to 'object'
	*/
	public void contextSwitch(string set_to_context){
		if (context != set_to_context){
            if (set_to_context == "paint") {
                paintManager.turnOnPaint();
                removeHandObject();
            } else if (set_to_context == "object") {
                paintManager.turnOffPaint();
            } 

			cleanGuidance ();
			context = set_to_context;
		} 
		return;
	}

	/* 	bufferedContext
	*	Input: None
	*	Output: Output mode context in the array
	*	Summary: Build a histogram of context buffer array
	*/
	private string bufferedContext(){
		int[] context_hist = new int[context_dict.Count];
		for (int i = 0; i < context_buff_len; i++) 
			context_hist [context_buff [i]] += 1;

		int modeContext = 0;
		for (int i = 0; i < context_hist.Length; i++) {
			if (context_hist [i] > context_hist [modeContext])
				modeContext = i;
		}

		contextSwitch (context_dict [modeContext]);
		return context_dict [modeContext];
	}

	/* 	contextSwtitch
	*	Input: String set_to_context
	*	Output: None
	*	Summary: Switch context: 1. 'object' to 'paint' 2. 'paint' to 'object'
	*/
	private void contextBuffUpdate(int cur_context){
		context_buff [context_buff_idx++] = cur_context;
		context_buff_idx = context_buff_idx % context_buff_len;
	}

	/* 
	//Method aborted
	private bool isGrabGesture(){
		GameObject thumb_2 = this.transform.GetChild (0).GetChild (2).gameObject;
		GameObject indexfinger_2 = this.transform.GetChild (1).GetChild (2).gameObject;
		float dist_thumb_index = Vector3.Distance(thumb_2.transform.position, indexfinger_2.transform.position);
		if (dist_thumb_index < 0.065){
			return true;
		}
		return false;
	}
	*/

	/* 	grabObject
	*	Input: GameObject obj
	*	Output: None
	*	Summary: 1. Reset and inactivate rigidbody of current obj. Otherwise obj could "magically" move in your hand :) 2. Set obj to move with hand
	*/
	private void grabObject(GameObject obj){
		obj.GetComponent<Collider> ().isTrigger = true;
		/*new feature: push*/
		if (push_enabled) {
			if (coroutine != null)
				StopCoroutine (coroutine);
			palm.GetComponent<Collider> ().isTrigger = true;
		}
		obj.GetComponent<Rigidbody> ().useGravity = false;
		obj.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		obj.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		obj.GetComponent<Rigidbody> ().Sleep ();

        // When grabbing, highlight it.
        GrabCollider grabCol = obj.GetComponentInChildren<GrabCollider>();
        if (grabCol != null)
            grabCol.OnBeingGrabbed();

        inverseGrabRotation = Quaternion.Inverse(grabHolder.transform.rotation);
        grabDeltaPosition = obj.transform.position - grabHolder.transform.position;

		//this was used to make obj a child
		//obj.transform.SetParent(grabHolder.transform);

		//trying this right now:

		//initialize MA array
		objBuffer = new Vector3[smoothingBuffer];
		for (int i = 0; i < smoothingBuffer; i++) {
			objBuffer [i] = obj.transform.position;
		}

		this.updateObjTransform(obj);

		/* pulasation */
		/* before invoke, check 2 things. if it is enabled, if there are existing ones */
		/* FIX!!!*/ 

		//InvokeRepeating ("VPulse", 1.0f, 1.0f);

		is_grabbing = true;
	}

	/* 	releaseObject
	*	Input: GameObject obj
	*	Output: None
	*	Summary: 1. Free current obj from hand 2. Activate rigidbody of current obj
	*/
	private void releaseObject(GameObject obj){
		obj.transform.parent = null;
        obj.GetComponent<Rigidbody>().useGravity = true;
        obj.GetComponent<Collider> ().isTrigger = false;
		obj.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		obj.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		is_grabbing = false;
		//obj.GetComponent<Rigidbody> ().velocity = palm.GetComponent<Rigidbody> ().velocity * 5.0f;


		Vector3 v = calculateVelocity ();
		obj.GetComponent<Rigidbody> ().velocity = new Vector3 (v[0], v[1] / 1.5f,v[2]);

        if (!reEnableGravity)
        {
            obj.GetComponent<Rigidbody>().useGravity = false;
            obj.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
        /*new feature: push*/
        if (push_enabled) {
			coroutine = disablePalmCollider (palm_collider_delay);
			StartCoroutine (coroutine);
		}
	}

	/*new feature: push*/
	public IEnumerator disablePalmCollider(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		palm.GetComponent<Collider> ().isTrigger = false;
	}

	public void setMenu(int menu) {

		m_currentMenu = menu;
	}

	/* 	collectHandSpeed
	*	Input: None
	*	Output: None
	*	Summary: TODO
	*/
	private void collectHandSpeed(){
		return;
	}

	private void updateHandSpeed(){
		indexFingerPos [indexFingerPosCNT] = indexFinger.transform.position - pIndexFingerPos;
		indexFingerPosCNT++;
		if (indexFingerPosCNT > 9) {
			indexFingerPosCNT = 0;
		}


		pIndexFingerPos = indexFinger.transform.position;
	}

	private Vector3 calculateVelocity(){
        // Calculate Speed by the middle point of index and thumb.
        Vector3 fv = Vector3.zero;
        // Use the shorter array as the total length
        int l = indexFingerPos.Length;
        if (thumbFingerPos.Length < l)
            l = thumbFingerPos.Length;

        for (int i = 0; i < l; ++i)
            fv += (indexFingerPos[i] + thumbFingerPos[i]) / 2;
		
		return (fv* _handThrowingPowerMultiplier);
	}

	private bool hand_busy = false;
	private GameObject hand_obj;

	/* 	setHandObject
	*	Input: GameObject obj
	*	Output: None
	*	Summary: 1. Set current interactable object 2. Set busy flag as true
	*/
	public bool setHandObject(GameObject obj){
		if (context != "object") {
			return false;
		}

		hand_obj = obj;
		hand_busy = true;
		return true;
	}

	public bool checkHandBusy(){
		return hand_busy;
	}

	/* 	removeHandObject
	*	Input: None
	*	Output: None
	*	Summary: 1. Remove current obj
	*/
	public void removeHandObject(){
		if (is_grabbing)
			releaseObject (hand_obj);

		hand_obj = null;
		hand_busy = false;
		return;
	}

	public GameObject getHandObject(){
		return hand_obj;
	}

	Vector3 smoothing(Vector3[] myArray, Vector3 pos){
		Vector3 average = new Vector3(0,0,0);

		myArray [smoothingBuffer_idx] = pos;
		smoothingBuffer_idx = (smoothingBuffer_idx + 1) % smoothingBuffer;

		for (int i = 0; i < smoothingBuffer; i++) {
			average += myArray[i];
		}

		return average / smoothingBuffer;
	}

	void updateObjTransform(GameObject obj){
        // Get Delta Rotation
        Quaternion rotation = grabHolder.transform.rotation * inverseGrabRotation;
        // Rotate grabDeltaPositon.
        grabDeltaPosition = rotation * grabDeltaPosition;
        // No buffer
        obj.transform.position = grabHolder.transform.position + grabDeltaPosition;
        obj.transform.rotation = rotation * obj.transform.rotation;
        inverseGrabRotation = Quaternion.Inverse(grabHolder.transform.rotation);
	}

	public bool IsGrabbing {
		get {
			return is_grabbing;
		}
	}
}
