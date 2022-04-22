using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* This class is used to define dynamic gestures, 
 * for example, a series of gestures like flip-n-flop hands to trigger an action
 * we are NOT USING this for now */
public class HandActionRecog : MonoBehaviour {
	private GestureControl LeftHandGC;
	private GestureControl RightHandGC;
	private Transform LeftHandPalm;
	private Transform RightHandPalm;

	public float obsoleteTime = 4.0f;		// 4 seconds
	//public float disturbanceThreshold = 0.1f;		// pretty short time, 0.1 seconds action will be ignored in the queue

	private LinkedList<HandActionItem> leftHandList;
	private LinkedList<HandActionItem> rightHandList;

	private float newItemDelta = 0.01f;				// the delta score that two gestures' differences are reached.

	private Dictionary<string, HandMotion>	handMotionList;		// the list that contains Hand Motions
	private bool isDefiningNewMotion = false;
	private HandMotion currentEditMotion;

    private bool isEnabled = true;                  // Wether this system is active.

	private static HandActionRecog _instance;

	public static HandActionRecog getInstance() {
		return _instance;
	}

    void Awake() {
        // Singleton.
        if (_instance != null)
        {
            Debug.LogError("Trying to initialize two or more instances of HandActionRecog");
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
		GameObject hHand = GameObject.Find ("Hand_l");
		if (hHand != null) {
			LeftHandGC = hHand.GetComponent<GestureControl> ();
			LeftHandPalm = hHand.transform.Find ("palm");
		}
		hHand = GameObject.Find ("Hand_r");
		if (hHand != null) {
			RightHandGC = hHand.GetComponent<GestureControl> ();
			RightHandPalm = hHand.transform.Find ("palm");
		}

		leftHandList = new LinkedList<HandActionItem> ();
		rightHandList = new LinkedList<HandActionItem> ();
		handMotionList = new Dictionary<string, HandMotion> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(LeftHandGC != null && RightHandGC != null && LeftHandPalm != null && RightHandPalm != null) {
			UpdateQueue ();
		}
	}

	// Update the queue, assuming that Left and Right Hand Gesture Control isn't Null
	private void UpdateQueue() {

		// pop out obsolete items
		// --------------------- A potentialbug ---------------------------
		// if we use pause or something, since here it's Time.time that isn't affected by the time scale.
		// A short inresponse may happen (because previous items are pop out)
		// ----------------------------------------------------------------
		float currentTime = Time.time;
		while (leftHandList.Count >= 2 && currentTime - leftHandList.First.Value.InsertTime > obsoleteTime)
			leftHandList.RemoveFirst ();
		while (rightHandList.Count >= 2 && currentTime - rightHandList.First.Value.InsertTime > obsoleteTime)
			rightHandList.RemoveFirst ();

		// insert new gesture
		HandActionItem lhai = new HandActionItem(LeftHandGC.bufferedGesture(), currentTime, LeftHandPalm.forward, LeftHandPalm.position);
		HandActionItem rhai = new HandActionItem (RightHandGC.bufferedGesture (), currentTime, RightHandPalm.forward, RightHandPalm.position);

		if (leftHandList.Count == 0 || HandActionItem.CompareTwoHandActionItem (leftHandList.Last.Value, lhai) > newItemDelta) {
			leftHandList.AddLast (lhai);
		}
		if (rightHandList.Count == 0 || HandActionItem.CompareTwoHandActionItem (rightHandList.Last.Value, rhai) > newItemDelta) {
			rightHandList.AddLast (rhai);
		}
	}

	/// <summary>
	/// Begins the motion. You must call this before you defines a new motion that you're interested in.
	/// </summary>
	/// <returns><c>true</c>, if this motion name is available, <c>false</c> otherwise.</returns>
	/// <param name="motionName">The new motion name.</param>
	public bool BeginMotion(string motionName) {
		if (handMotionList.ContainsKey (motionName))
			return false;

		if (isDefiningNewMotion) {
			Debug.LogWarning ("Try to define a new motion when last motion define hasn't finished.");
			return false;
		}

		// now start new motion
		currentEditMotion = new HandMotion(motionName);
		isDefiningNewMotion = true;
		return true;
	}

	/// <summary>
	/// Ends the motion define and add this motion to the dictionary.
	/// </summary>
	/// <returns><c>true</c>, if motion was ended and added successfully, <c>false</c> otherwise.</returns>
	public bool EndMotion() {
		if (isDefiningNewMotion && currentEditMotion != null) {
			handMotionList.Add (currentEditMotion.MotionName, currentEditMotion);
			currentEditMotion = null;
			isDefiningNewMotion = false;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Defines a gesture.
	/// </summary>
	/// <returns><c>true</c>, if gesture was defined successfully, <c>false</c> otherwise.</returns>
	/// <param name="gestureName">Gesture name.</param>
	/// <param name="duration">Duration for a still gesture.</param>
	/// <param name="palmVector">Palm vector for this gesture.</param>
	public bool DefineGesture(string gestureName, Vector3 palmNorm, float duration = .0f, bool posLock = true) {
		if (!isDefiningNewMotion || currentEditMotion == null)
			return false;
		HandMatchGesture item = new HandMatchGesture (gestureName, palmNorm, duration, posLock);
		currentEditMotion.AddMatchItem (item);
		return true;
	}

	/// <summary>
	/// Defines the transform.
	/// </summary>
	/// <returns><c>true</c>, if transform was defined, <c>false</c> otherwise.</returns>
	/// <param name="fromg">From which gestures? (| or operation is supported)</param>
	/// <param name="tog">To which gestures? (| or operation is supported)</param>
	/// <param name="fromn">From which normal? (Vector3.zero means this feature isn't important)</param>
	/// <param name="ton">To which normal?</param>
	/// <param name="timespan">Timespan. In how long time this transform should be finished</param>
	public bool DefineTransform(string fromg, string tog, Vector3 fromn, Vector3 ton, float timespan = 0.0f) {
		if (!isDefiningNewMotion || currentEditMotion == null)
			return false;
		HandMatchTransform item = new HandMatchTransform (fromg, tog, fromn, ton, timespan);
		currentEditMotion.AddMatchItem (item);
		return true;
	}

	/// <summary>
	/// Determines whether a motion is satisfied.
	/// </summary>
	/// <returns><c>true</c> if the motion with motionName is satisfied; otherwise, <c>false</c>.</returns>
	/// <param name="motionName">Motion name.</param>
	public bool IsMotion (string motionName, bool isLeftHand = true) {
        // If not enabled, just return false.
        if (!isEnabled)
            return false;

		if (handMotionList.ContainsKey (motionName)) {
			HandMotion hm = handMotionList [motionName];

            if (isLeftHand)
                return hm.Match(leftHandList);
            else
                return hm.Match(rightHandList);
		}
		return false;
	}

    /// <summary>
    /// Debug function, print lefthand action list.
    /// </summary>
	public void LogAllList () {
		LinkedListNode<HandActionItem> lln = leftHandList.Last;
		while(lln != null && lln.Value != null) {
			lln = lln.Previous;
		}
	}

    /// <summary>
    /// Make the system work or stop working.
    /// </summary>
    /// <param name="e">true:Start working. false:Stop working</param>
    public void SetEnabled (bool e) {
        isEnabled = e;
    }

    public bool SystemEnabled {
        get {
            return isEnabled;
        }
        set {
            SetEnabled(value);
        }
    }
}
