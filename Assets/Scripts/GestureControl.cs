using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.UI;
/* This is the gesture detection used for Portal-ble
 * Our default engine uses an open-source Accord.net
   If you prefer to integrate with Unity-OpenCV, please
   purchase it on the Unitystore*/

    /* merged Android and Ios */
public class GestureControl : MonoBehaviour {

    //Left hand finger declare
    private GameObject palm;

	//poseDetector buffer
	int[] gesture_buff;
	int gesture_buff_len = 5;
	int gesture_buff_idx = 0; 
	GameObject dataMgr;

    //Flag if it's left hand
    private bool bIsLeftHand;

	//Gesture dictionary
	Dictionary<int, string> gesture_dict = new Dictionary<int, string>();

    // test svm manual
    private SVMCalculator svc;
    // Use this for initialization
    void Start () {
        svc = this.GetComponent<SVMCalculator>();
		dataMgr = GameObject.Find ("gDataManager");
		gesture_buff_len = dataMgr.GetComponent<DataManager> ().gestBuffer;

        palm = this.transform.GetChild (5).gameObject;
		gesture_buff = new int[gesture_buff_len];

		//Gesture dicitonary establishes
		gesture_dict.Add(0, "palm");
		gesture_dict.Add(1, "pinch");
		gesture_dict.Add(2, "paint");
		gesture_dict.Add(3, "fist");
		gesture_dict.Add(4, "undefined");

       HandManager hm = GetComponent<HandManager>();
        if (hm != null)
            bIsLeftHand = hm.bIsLeftHand;
        else
            bIsLeftHand = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (svc.isSVMReady())
        {
            gesture_buff [gesture_buff_idx++] = gestureDetectorMLpredict ();
            gesture_buff_idx = (gesture_buff_idx) % gesture_buff_len;
		}
     }

    /* 	gestureDetectorMLpredict
	*	Input: None
	*	Output: label indicate left or right hand
    *	three: "RIGHT_HAND", "LEFT_HAND", "NO_HAND"
	*/
    public string getActiveHand() {
        return "RIGHT_HAND";
    }

    /* prediction android */
    private int gestureDetectorMLpredict()
    {
        /* initial satte */
        #if UNITY_IOS && !UNITY_EDITOR
            if (!ios_svm_model_ready)
                return 0;
        #else
            //if (svm_model == null)
            if (!svc.isSVMReady())
                return 0;
        #endif
        /* the second joints on every finger */
        Vector3[] vec_bone2 = new Vector3[5];
        /* the finger tips on every finger */
        Vector3[] vec_bone1 = new Vector3[5];
        float[] cur_data_array = new float[30];

        Vector3 palm_plane_norm = palm.transform.forward;
        Vector3 palm_plane_up = palm.transform.up;
        Vector3 palm_plane_right = palm.transform.right;

        for (int i = 0; i < 5; i++)
        {
            Vector3 vec_palm_bone2 = this.transform.GetChild(i).GetChild(2).position - palm.transform.position;
            Vector3 vec_palm_bone1 = this.transform.GetChild(i).GetChild(1).position - palm.transform.position;
            vec_bone2[i].x = Vector3.ProjectOnPlane(vec_palm_bone2, palm_plane_right).magnitude;
            vec_bone2[i].y = Vector3.ProjectOnPlane(vec_palm_bone2, palm_plane_norm).magnitude;
            vec_bone2[i].z = Vector3.ProjectOnPlane(vec_palm_bone2, palm_plane_up).magnitude;
            vec_bone1[i].x = Vector3.ProjectOnPlane(vec_palm_bone1, palm_plane_right).magnitude;
            vec_bone1[i].y = Vector3.ProjectOnPlane(vec_palm_bone1, palm_plane_norm).magnitude;
            vec_bone1[i].z = Vector3.ProjectOnPlane(vec_palm_bone1, palm_plane_up).magnitude;
            cur_data_array[i * 6] = vec_bone2[i].x;
            cur_data_array[i * 6 + 1] = vec_bone2[i].y;
            cur_data_array[i * 6 + 2] = vec_bone2[i].z;
            cur_data_array[i * 6 + 3] = vec_bone1[i].x;
            cur_data_array[i * 6 + 4] = vec_bone1[i].y;
            cur_data_array[i * 6 + 5] = vec_bone1[i].z;
        }

#if UNITY_IOS && !UNITY_EDITOR
        int result = PredictSVM(mat_n, cur_data_array);
#else

        /*Our new svm library function need further twick for this ARM64 version because errors in the order that applies weights
         * in the histogram intersection kernal of the svm model*/
        //int result = svc.decide(Array.ConvertAll(cur_data_array, x => (double)x));

        /*svc bruteforce_distance is using distance between fingers to predict */
        Vector3 thumb_bone3 = this.transform.GetChild(6).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).position;
        Vector3 index_bone3 = this.transform.GetChild(6).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).position;
        int result = svc.bruteforce_distance(thumb_bone3, index_bone3);
#endif
        return result;
    }


    /* 	[important] Used to store the buffered gesture
	*	Input: None
	*	Output: current active gesture (palm, paint, pinch, fist, idle)
	*	Summary: Output mode gesture in last detector_buff_len frames to reduce noise
	*/
    public string bufferedGesture(){
		int[] gesture_hist = new int[gesture_dict.Count];
        if (gesture_dict.Count == 0){
            return "";
        }
        for (int i = 0; i < gesture_buff_len; i++) {
                gesture_hist [gesture_buff [i]] += 1;
        }

		int modeGesture = 0;
		for (int i = 0; i < gesture_hist.Length; i++){
			if (gesture_hist[i] >= gesture_hist[modeGesture])
				modeGesture = i;
		}
		return gesture_dict [modeGesture];
	}
  
    /* 	OBSOLETE DO NOT USE
	*	Input: None
	*	Output: None
	*	Summary: Output current gesture data into a .txt file
	*/
    private void handDataGenerator()
    {
        Vector3[] vec_bone2 = new Vector3[5];
        Vector3 palm_plane_norm, palm_plane_up, palm_plane_right;
        palm_plane_norm = palm.transform.forward;
        palm_plane_up = palm.transform.up;
        palm_plane_right = palm.transform.right;

        string temp = "";
        for (int i = 0; i < 5; i++)
        {
            Vector3 vec_palm_bone2 = this.transform.GetChild(i).GetChild(2).position - palm.transform.position;
            vec_bone2[i].x = Vector3.ProjectOnPlane(vec_palm_bone2, palm_plane_right).magnitude;
            vec_bone2[i].y = Vector3.ProjectOnPlane(vec_palm_bone2, palm_plane_norm).magnitude;
            vec_bone2[i].z = Vector3.ProjectOnPlane(vec_palm_bone2, palm_plane_up).magnitude;
            temp += vec_bone2[i].x.ToString("F10") + "," + vec_bone2[i].y.ToString("F10") + "," + vec_bone2[i].z.ToString("F10");
            if (i < 4)
                temp += ",";
            else
                temp += "\n";
        }
    }
}
