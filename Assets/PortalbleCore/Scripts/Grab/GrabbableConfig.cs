using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portalble.Functions.Grab {
    /// <summary>
    /// Grab Configuration Data Structure
    /// </summary>
    public class GrabbableConfig {
        public static readonly int POS_LOCK_X = 0x1;
        public static readonly int POS_LOCK_Y = 0x2;
        public static readonly int POS_LOCK_Z = 0x4;
        public static readonly int ROT_LOCK_X = 0x8;
        public static readonly int ROT_LOCK_Y = 0x16;
        public static readonly int ROT_LOCK_Z = 0x32;

        public GrabbableConfig(int initialLock = 0) {
            m_data = initialLock;
        }

        /// <summary>
        /// data field. Use each bit to represent lock.
        /// </summary>
        int m_data;

        public int GetRawLockData() {
            return m_data;
        }
        /// <summary>
        /// Mark if this object can be thrown
        /// </summary>
        bool m_throwable;
        public bool Throwable {
            get {
                return m_throwable;
            }
            set {
                m_throwable = value;
            }
        }
        /// <summary>
        /// If given lock is locked.
        /// </summary>
        /// <param name="loc">lock</param>
        /// <returns>true for locked, else is false</returns>
        public bool isLocked(int loc) {
            return ((m_data & loc) == loc);
        }

        /// <summary>
        /// Set a given lock locked or not.
        /// </summary>
        /// <param name="loc">lock</param>
        /// <param name="flag">true for lock, false for else.</param>
        /// <returns></returns>
        public bool setLock(int loc, bool flag) {
            if (flag == true) {
                m_data = m_data | loc;
            }
            else {
                m_data = m_data & (~loc);
            }

            return true;
        }

        /// <summary>
        /// Return a special vector representing three dimension position lock
        /// For example, if there's no position lock, it returns Vector.one
        /// if it has x axis lock, it returns (0,1,1)
        /// </summary>
        /// <returns>position lock vector</returns>
        public Vector3 getPositionLockVector() {
            Vector3 ret = Vector3.one;
            if (isLocked(POS_LOCK_X)) {
                ret.x = 0;
            }
            if (isLocked(POS_LOCK_Y)) {
                ret.y = 0;
            }
            if (isLocked(POS_LOCK_Z)) {
                ret.z = 0;
            }
            return ret;
        }
    }
}
