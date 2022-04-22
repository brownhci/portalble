using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mediapipe.HandTracking
{
    public class InitializeProcess : MonoBehaviour
    {
        public ARCore.ARCoreDepthSetting depthSetting;
        [SerializeField]
        private GameObject process;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine("SetDepth");
        }

        IEnumerator SetDepth()
        {
            // Start process (i.e. start getting predictions), but default to
            process.SetActive(true);

            // Keep trying to get depth estimate
            while (true)
            {
                yield return new WaitForSeconds(0.25f);
                if (depthSetting.SetDepth()) break;
            }
        }
    }
}