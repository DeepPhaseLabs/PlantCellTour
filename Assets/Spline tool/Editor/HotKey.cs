using UnityEngine;
using System.Collections;

namespace SplineTool
{
    public class HotKey
    {
        public KeyCode keyCode;
        public bool alt;
        public bool ctrl;
        public bool shift;

        public HotKey(KeyCode kc, bool useAlt, bool useCtrl, bool useShift)
        {
            keyCode = kc;
            alt = useAlt;
            ctrl = useCtrl;
            shift = useShift;
        }
    }
}
