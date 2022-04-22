using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalman;
using System.Linq;
using System.IO;
using System;
/* Sync only deals with output from MediaPipe */
/* it does not deal with gestures  */

public class Sync : MonoBehaviour {
    
	[SerializeField]
	Mediapipe.HandTracking.Process process;
	public bool fromMediaPipe = true;

	//private WSManager ws;
	//private DataManager dataManager;
	private GameObject l_palm;

	private GameObject l_finger0;
	private GameObject l_finger0_bone0;
	private GameObject l_finger0_bone1;
	private GameObject l_finger0_bone2;

	private GameObject l_finger1;
	private GameObject l_finger1_bone0;
	private GameObject l_finger1_bone1;
	private GameObject l_finger1_bone2;

	private GameObject l_finger2;
	private GameObject l_finger2_bone0;
	private GameObject l_finger2_bone1;
	private GameObject l_finger2_bone2;

	private GameObject l_finger3;
	private GameObject l_finger3_bone0;
	private GameObject l_finger3_bone1;
	private GameObject l_finger3_bone2;

	private GameObject l_finger4;
	private GameObject l_finger4_bone0;
	private GameObject l_finger4_bone1;
	private GameObject l_finger4_bone2;

    private Transform l_arm;
    private GameObject finger;
	private GameObject bone;

    private string actv_hand;

    private IKalmanWrapper kalmanPalm,kalmanIndex,kalmanThumb,kalmanHand;
    private IKalmanWrapper[] mKalmanFilter;
    //moving average filter
    private Vector3[] queuePalm, queueIndex, queueThumb, queueHand;

    private Vector3 palmScale;

    private List<string> frameList = new List<string>();
    private int frame_idx;

    private int smoothingBuffer = 1;
	private int smoothingBuffer_idx;
	private static bool enableKalmanFilter = true;

    // For 1+6
    private Vector3 leapMotionOffset = new Vector3(0f, -0.04f, -0.01f);

    private Vector3 LOffset = new Vector3(0.035f, 0.04f, 0f);
    private Vector3 ROffset = new Vector3(-0.045f, 0.04f, 0f);
    private Vector3 initialLeapMotionOffset;

	// For toggling hand visualization
	public Material transparent_material;
	public Material hand_material;
	private bool m_toggle_hand_visualize = true;
	public bool toggle_hand_visualize{
        get{
			return m_toggle_hand_visualize;
		}
    }
	// Use this for initialization
	void Start () {
		l_palm = this.transform.GetChild (5).gameObject;
		l_finger0 = this.transform.GetChild (0).gameObject;
		l_finger0_bone0 = l_finger0.transform.GetChild (0).gameObject;
        l_finger0_bone1 = l_finger0.transform.GetChild (1).gameObject;
		l_finger0_bone2 = l_finger0.transform.GetChild (2).gameObject;

        l_finger1 = this.transform.GetChild (1).gameObject;
		l_finger1_bone0 = l_finger1.transform.GetChild (0).gameObject;
		l_finger1_bone1 = l_finger1.transform.GetChild (1).gameObject;
		l_finger1_bone2 = l_finger1.transform.GetChild (2).gameObject;

		l_finger2 = this.transform.GetChild (2).gameObject;
		l_finger2_bone0 = l_finger2.transform.GetChild (0).gameObject;
		l_finger2_bone1 = l_finger2.transform.GetChild (1).gameObject;
		l_finger2_bone2 = l_finger2.transform.GetChild (2).gameObject;

		l_finger3 = this.transform.GetChild (3).gameObject;
		l_finger3_bone0 = l_finger3.transform.GetChild (0).gameObject;
		l_finger3_bone1 = l_finger3.transform.GetChild (1).gameObject;
		l_finger3_bone2 = l_finger3.transform.GetChild (2).gameObject;

		l_finger4 = this.transform.GetChild (4).gameObject;
		l_finger4_bone0 = l_finger4.transform.GetChild (0).gameObject;
		l_finger4_bone1 = l_finger4.transform.GetChild (1).gameObject;
		l_finger4_bone2 = l_finger4.transform.GetChild (2).gameObject;

        l_arm = transform.Find("arm");
        

        kalmanPalm = new MatrixKalmanWrapper ();
		kalmanIndex = new MatrixKalmanWrapper ();
		kalmanThumb = new MatrixKalmanWrapper ();
		kalmanHand = new MatrixKalmanWrapper ();

		//moving average queues
		queuePalm = new Vector3[smoothingBuffer];
		queueIndex = new Vector3[smoothingBuffer];
		queueThumb = new Vector3[smoothingBuffer];
		queueHand = new Vector3[smoothingBuffer];
        
		//initialization
		for (int i = 0; i < smoothingBuffer; i++) {
			queuePalm[i] = Vector3.zero;
			queueIndex[i] = Vector3.zero;
			queueThumb[i] = Vector3.zero;
            queueHand[i] = Camera.main.transform.position + Camera.main.transform.rotation * leapMotionOffset;
		}

        palmScale = l_palm.transform.localScale;

        mKalmanFilter = new IKalmanWrapper[22];
        for (int i = 0; i < 22; i++)
            mKalmanFilter[i] = new MatrixKalmanWrapper();
        if (GlobalStates.globalConfigFile.Available)
            initialLeapMotionOffset = GlobalStates.globalConfigFile.HandOffset;
        else
            initialLeapMotionOffset = leapMotionOffset;

	}

    // Update is called once per frame
    void Update () {
		if (fromMediaPipe)
        {
            updateHandSkeletonFromMediaPipe();
        }
        else
        {
        /* from leap motion */
            string msg = "";

			string[] hand_info = msg.Split(new char[] { ',', ':', ';' });


            // check current phone orientation
        #if UNITY_ANDROID
            leapMotionOffset = initialLeapMotionOffset;
            if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft) {
                leapMotionOffset += LOffset;
            }
            else if (Input.deviceOrientation == DeviceOrientation.LandscapeRight) {
                leapMotionOffset += ROffset;
            }
        #endif

            /* return before any hand found */
            if (!hand_info[0].Equals(""))
            {
                updateHandSkeletonFromLeap(hand_info);
            }

            // check current phone orientation
        #if UNITY_ANDROID
            // before this, hand rotation is set to camera rotation. So we can just multiply it
            if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft) {
                Quaternion rot = Quaternion.AngleAxis(90.0f, Camera.main.transform.forward);
                transform.rotation = rot * transform.rotation;
            }
            else if (Input.deviceOrientation == DeviceOrientation.LandscapeRight) {
                Quaternion rot = Quaternion.AngleAxis(-90.0f, Camera.main.transform.forward);
                transform.rotation = rot * transform.rotation;
            }
        #endif
        }
    }

    void updateHandSkeletonFromMediaPipe()
    {
        string[] coords = process.GetCoords();

        Vector3[] vecs = new Vector3[coords.Length];
		Vector3 te = Vector3.zero;
		for (int j = 0; j < coords.Length; j += 1)
		{
			string[] xyz = coords[j].Split(',');
			if (xyz[0] != "")
			{
				te.x = (float)Convert.ToDouble(xyz[0]);
				te.y = (float)Convert.ToDouble(xyz[1]);
				te.z = (float)Convert.ToDouble(xyz[2]);
			}
			vecs[j] = te;
		}

		if (enableKalmanFilter)
        {
			for (int i = 0; i < vecs.Length; i++)
				vecs[i] = mKalmanFilter[i].Update(vecs[i]);
		}

		Vector3 dir00 = vecs[2] - vecs[1];
        Vector3 dir01 = vecs[3] - vecs[2];
        Vector3 dir02 = vecs[4] - vecs[3];
        l_finger0_bone0.transform.localPosition = vecs[2];
        l_finger0_bone0.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir00);
        l_finger0_bone1.transform.localPosition = vecs[3];
        l_finger0_bone1.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir01);
        l_finger0_bone2.transform.localPosition = vecs[4];
        l_finger0_bone2.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir02);

        Vector3 dir10 = vecs[6] - vecs[5];
        Vector3 dir11 = vecs[7] - vecs[6];
        Vector3 dir12 = vecs[8] - vecs[7];
        l_finger1_bone0.transform.localPosition = vecs[6];
        l_finger1_bone0.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir10);
        l_finger1_bone1.transform.localPosition = vecs[7];
        l_finger1_bone1.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir11);
        l_finger1_bone2.transform.localPosition = vecs[8];
        l_finger1_bone2.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir12);

        Vector3 dir20 = vecs[10] - vecs[9];
        Vector3 dir21 = vecs[11] - vecs[10];
        Vector3 dir22 = vecs[12] - vecs[11];
        l_finger2_bone0.transform.localPosition = vecs[10];
        l_finger2_bone0.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir20);
        l_finger2_bone1.transform.localPosition = vecs[11];
        l_finger2_bone1.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir21);
        l_finger2_bone2.transform.localPosition = vecs[12];
        l_finger2_bone2.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir22);

        Vector3 dir30 = vecs[14] - vecs[13];
        Vector3 dir31 = vecs[15] - vecs[14];
        Vector3 dir32 = vecs[16] - vecs[15];
        l_finger3_bone0.transform.localPosition = vecs[14];
        l_finger3_bone0.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir30);
        l_finger3_bone1.transform.localPosition = vecs[15];
        l_finger3_bone1.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir31);
        l_finger3_bone2.transform.localPosition = vecs[16];
        l_finger3_bone2.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir32);

        Vector3 dir40 = vecs[18] - vecs[17];
        Vector3 dir41 = vecs[19] - vecs[18];
        Vector3 dir42 = vecs[20] - vecs[19];
        l_finger4_bone0.transform.localPosition = vecs[18];
        l_finger4_bone0.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir40);
        l_finger4_bone1.transform.localPosition = vecs[19];
        l_finger4_bone1.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir41);
        l_finger4_bone2.transform.localPosition = vecs[20];
        l_finger4_bone2.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir42);

        /* set the palm position */
        l_palm.transform.position = (vecs[0] + vecs[5] + vecs[17]) / 3 ;

        /* set the palm orientation */
        Vector3 vec1 = vecs[5] - vecs[0];
        Vector3 vec2 = vecs[17] - vecs[0];
        Vector3 f = Vector3.Cross(vec1, vec2);
        Vector3 up = vec1 + vec2;
        l_palm.transform.LookAt(l_palm.transform.position + Vector3.Normalize(f), Vector3.Normalize(up));

    }

    /* updating hand skeleton from Leap */
    void updateHandSkeletonFromLeap(string[] hand_info){
        /* checking que will be when getStringMode is called */
        /* check queue */
        int i = 2; //skip hand type
		Vector3 palm_norm = new Vector3();
		Vector3 palm_dir = new Vector3();
		while (i<hand_info.Length){
			string type = hand_info[i++];

             
			if (type.Contains ("palm")) {
				if (type.Contains ("pos")) {
					Vector3 palm_pos = new Vector3 (float.Parse (hand_info [i++]), float.Parse (hand_info [i++]), -float.Parse (hand_info [i++]));
					palm_pos = palm_pos * 0.001f;


					//local position
					l_palm.transform.localPosition = palm_pos;


				} else if (type.Contains ("vel")) {
					i += 3;
				} else if (type.Contains ("norm")) {
					palm_norm = new Vector3 (float.Parse (hand_info [i++]), float.Parse (hand_info [i++]), -float.Parse (hand_info [i++]));
					Quaternion palm_rot_byNorm = Quaternion.FromToRotation (Vector3.forward, palm_norm);

				} else {
					palm_dir = new Vector3 (float.Parse (hand_info [i++]), float.Parse (hand_info [i++]), -float.Parse (hand_info [i++]));
					Quaternion palm_rot_byDir = Quaternion.FromToRotation (Vector3.up, palm_dir);

					l_palm.transform.localRotation = Quaternion.LookRotation(palm_norm, palm_dir);
				}

			} else if (type.Contains ("finger")) {
				int finger_i = int.Parse (hand_info [i++]);
				for (int bone_i = 0; bone_i < 3; bone_i++) {
					for (int vec3_i = 0; vec3_i < 2; vec3_i++) {
						string vec3_type = hand_info [i++];
						finger = getFinger (finger_i);
						bone = getBoneFromFinger (finger_i, bone_i);
						if (vec3_type.Contains ("pos")) {
							Vector3 bone_pos = new Vector3 (float.Parse (hand_info [i++]), float.Parse (hand_info [i++]), -float.Parse (hand_info [i++]));
							bone_pos = bone_pos * 0.001f;
							
							bone.transform.localPosition = bone_pos;
						} else {
							//Quaternion palm_rot_byNorm = Quaternion.FromToRotation (Vector3.forward, palm_norm);
							Vector3 finger_dir = new Vector3 (float.Parse (hand_info [i++]), float.Parse (hand_info [i++]), float.Parse (hand_info [i++]));
							Quaternion palm_rot_byDir = Quaternion.FromToRotation (Vector3.up, finger_dir);
							bone.transform.localRotation = palm_rot_byDir;
						}
					}
				}
			} else if (type.Contains("arm")) {
                // Set arm position
                if (l_arm != null) {
                    if (type.Contains("pos")) {
                        Vector3 arm_pos = new Vector3(float.Parse(hand_info[i++]), float.Parse(hand_info[i++]), -float.Parse(hand_info[i++]));
                        arm_pos = arm_pos * 0.001f;
                        l_arm.localPosition = arm_pos;
                    }
                    else if (type.Contains("dir")) {
                        Vector3 arm_dir = new Vector3(float.Parse(hand_info[i++]), float.Parse(hand_info[i++]), float.Parse(hand_info[i++]));
                        Quaternion arm_rot_byDir = Quaternion.FromToRotation(Vector3.up, arm_dir);
                        l_arm.localRotation = arm_rot_byDir;
                    }
                }
			} else {
                i++;
            }
		}
		/* setting global position
		 *  with vector as offset for Leap point of view
		 *  Apply Kalman filter to this
		*/
		transform.position = smoothing(queueHand, Camera.main.transform.position + Camera.main.transform.rotation * leapMotionOffset);
		transform.rotation = Camera.main.transform.rotation;
	}
	/* Bone Mapping functions, do not modify 
	   unless you are sure what it is */
	GameObject getBoneFromFinger(int fingerIndex, int bone){
		GameObject b = l_finger0_bone0;
		switch (fingerIndex) {
		/* finger 0 */
		case 0:
			switch (bone) {
			case 0:
				b = l_finger0_bone0;
				break;
			case 1:
				b = l_finger0_bone1;
				break;
			case 2:
				b = l_finger0_bone2;
				break;
			}
			break;

			/* finger 0 */
		case 1:
			switch (bone) {
			case 0:
				b = l_finger1_bone0;
				break;
			case 1:
				b = l_finger1_bone1;
				break;
			case 2:
				b = l_finger1_bone2;
				break;
			}
			break;

			/* finger 0 */
		case 2:
			switch (bone) {
			case 0:
				b = l_finger2_bone0;
				break;
			case 1:
				b = l_finger2_bone1;
				break;
			case 2:
				b = l_finger2_bone2;
				break;
			}
			break;

			/* finger 0 */
		case 3:
			switch (bone) {
			case 0:
				b = l_finger3_bone0;
				break;
			case 1:
				b = l_finger3_bone1;
				break;
			case 2:
				b = l_finger3_bone2;
				break;
			}
			break;

			/* finger 0 */
		case 4:
			switch (bone) {
			case 0:
				b = l_finger4_bone0;
				break;
			case 1:
				b = l_finger4_bone1;
				break;
			case 2:
				b = l_finger4_bone2;
				break;
			}
			break;

			/* default */
		default:
			b = l_finger0_bone0;
			break;
		}

		return b;
	}

	GameObject getFinger(int index){
		GameObject f;
		switch (index) {
		case 0:
			f = l_finger0;
			break;
		case 1:
			f =  l_finger1;
			break;
		case 2:
			f = l_finger2;
			break;
		case 3:
			f =  l_finger3;
			break;
		case 4:
			f =  l_finger4;
			break;
		default:
			f = l_finger0;
			break;
		}
		return f;
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

	public static bool ToggleKalmanFilter()
	{
		enableKalmanFilter = !enableKalmanFilter;
		return enableKalmanFilter;
	}

	public Vector3 InitialHandOffset {
        get {
            return initialLeapMotionOffset;
        }
        set {
            initialLeapMotionOffset = value;
        }
    }

	public void ToggleHandVisualize()
	{
		Material temp;
		if (toggle_hand_visualize)
			temp = transparent_material;
		else
			temp = hand_material;
		m_toggle_hand_visualize = !m_toggle_hand_visualize;
		for (int i = 0; i <= 5; i++)
		{
			GameObject finger = transform.GetChild(i).gameObject;
            if (i == 5)
				finger.GetComponent<MeshRenderer>().material = temp;
			for (int j = 0; j < finger.transform.childCount; j++)
			{
				GameObject bone = finger.transform.GetChild(j).gameObject;
				bone.GetComponent<MeshRenderer>().material = temp;
			}
		}
	}

	public void SetHandVisualize(bool view)
	{
		Material temp;
		if (!view)
			temp = transparent_material;
		else
			temp = hand_material;
		for (int i = 0; i <= 5; i++)
		{
			GameObject finger = transform.GetChild(i).gameObject;
			if (i == 5)
				finger.GetComponent<MeshRenderer>().material = temp;
			for (int j = 0; j < finger.transform.childCount; j++)
			{
				GameObject bone = finger.transform.GetChild(j).gameObject;
				bone.GetComponent<MeshRenderer>().material = temp;
			}
		}
	}
}