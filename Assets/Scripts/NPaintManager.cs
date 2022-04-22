using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Portalble;

public class NPaintManager : MonoBehaviour
{
    private Transform last_hand;
    private TubeRenderer tr;
    private GameObject currTube;

    private bool is_painting = false;
    private bool clean_trail = false;

    [SerializeField]
    private Material m_mat;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PortalbleGeneralController pgc = PortalbleGeneralController.main;
        if (pgc == null)
        {
            Debug.LogWarning("Cannot find PortalbleGeneralController");
            return;
        }
        Transform thand = pgc.ActiveHandTransform;
        if (pgc.ActiveHandGesture != "pinch" || thand != last_hand || thand == null)
            last_hand = thand;

        if (pgc.ActiveHandGesture == "pinch") {
            /* user just started painting */
            if (!is_painting)
            {
                currTube = makeTube();


                is_painting = true;
                clean_trail = false;

                /* user is painting */
            }
            else
            {
                Vector3 newPoint = new Vector3();
                is_painting = true;
                newPoint = pgc.ActiveHandIndexFingerTransform.position;
                // makeSphere(newPoint);
                currTube.GetComponent<TubeRenderer>().vecs.Add(newPoint);
                //add here
                int interval = 20;
                float nextTime = 0;

                List<Vector3> vecs = currTube.GetComponent<TubeRenderer>().vecs;

                if (Time.time >= nextTime)
                {
                    // record, update, render new point
                    nextTime = Time.time + interval;
                    vecs.Add(newPoint);
                    currTube.GetComponent<TubeRenderer>().UpdatePos();
                    //touchPaint_helper((newPoint));
                }

                // Calculate new velocity
                int n_prev_positions_max = 5;
                int len_vecs = vecs.Count;
                int num_prev_positions = vecs.Count;
                if (num_prev_positions >= n_prev_positions_max)
                {
                    num_prev_positions = n_prev_positions_max;
                }
                Vector3 velocity_acc = Vector3.zero;
                for (int i = 1; i < num_prev_positions; i++)
                {
                    Vector3 prevVelocity1 = vecs[len_vecs - i];
                    Vector3 prevVelocity2 = vecs[len_vecs - i - 1];
                    Vector3 difference = prevVelocity2 - prevVelocity1;
                }
            }
        }
       
    }

    private GameObject makeTube()
    {
        GameObject t = new GameObject("tube");
        tr = t.AddComponent<TubeRenderer>();
        t.GetComponent<Renderer>().material = m_mat;
        tr.material = m_mat;
        t.SetActive(true);
        t.GetComponent<MeshRenderer>().enabled = true;
        tr._radiusOne = 0.002f;
        tr._sides = 7;
        tr._radiusTwo = 0.002f;
        return t;
    }
}
