using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portalble {
    public class PortalbleConfigItems {
        public static PortalbleConfigDefinition[] list =
        {
            new PortalbleConfigDefinition("hand_offset", Vector3.zero, typeof(Vector3)),
            new PortalbleConfigDefinition("meshhand_scale", Vector3.one, typeof(Vector3))
        };
    }
}
