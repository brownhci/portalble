using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portalble
{
    public class Funcs
    {

        public static float clamp(float v, float l, float u)
        {
            if (v < l)
                return l;
            if (v > u)
                return u;
            return v;
        }

        public static bool idleHandManager(GameObject hand_l, GameObject hand_r, string ACTIVE_HAND) {

            if (ACTIVE_HAND == "LEFT_HAND" && hand_r != null)
            {
                hand_r.transform.position = new Vector3(0, 0, 9999);
                return true;
            }
            else if (ACTIVE_HAND == "RIGHT_HAND" && hand_l != null)
            {
                hand_l.transform.position = new Vector3(0, 0, 9999);
                return true;

            }
            else if (ACTIVE_HAND == "NO_HAND" && hand_l != null && hand_r != null)
            {
                hand_l.transform.position = new Vector3(0, 0, 9999);
                hand_r.transform.position = new Vector3(0, 0, 9999);
                return true;
            }
            /* if none of conditions met, it should be false */
            return false;
        }
    }
}