using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRebindTester : MonoBehaviour {
    public GameObject m_HandPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setHand() {
        if (m_HandPrefab != null) {
            GameObject gobj = Instantiate(m_HandPrefab);
            GameObject r_hand = GameObject.Find("Hand_r");
            Transform palm = r_hand.transform.Find("palm");
            gobj.transform.position = palm.position;
            gobj.transform.rotation = palm.rotation;
            gobj.transform.Rotate(0f, 90f, 0f, Space.Self);
            gobj.transform.Rotate(0f, 0f, 90f, Space.Self);
            gobj.transform.Rotate(180f, 0f, 0f, Space.Self);
            gobj.transform.SetParent(r_hand.transform);

            Portalble.HandMeshMapping hmm = gobj.GetComponent<Portalble.HandMeshMapping>();
            if (hmm != null) {
                hmm.Init();
                hmm.ResetBindPose();
            }
        }
    }
}
