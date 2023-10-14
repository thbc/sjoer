using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.HelperClasses;
using Assets.Resources;
using Assets.InfoItems;

namespace Multiplayer.Marking
{
    [System.Serializable]
    public class MarkedStateFlag
    {
        [System.Flags]
        public enum MarkedState
        {
            Unmarked = 0,
            MarkedSent = 1 << 0,
            MarkedReceived = 1 << 1
        }

        public MarkedState markedState = MarkedState.Unmarked;

        public MarkedState GetMarkedState()
        {
            return markedState;
        }

        public void SetMarkedState(MarkedState newState)
        {
            if (newState == MarkedState.Unmarked)
            {
                markedState = MarkedState.Unmarked;
            }
            else
            {
                markedState |= newState;
            }

            AutoUnmark();
            Debug.Log("MarkedState changed to: " + markedState);
        }

        public void ClearMarkedState(MarkedState stateToClear)
        {
            markedState &= ~stateToClear;

            AutoUnmark();
            Debug.Log("MarkedState changed to: " + markedState);
        }

        private void AutoUnmark()
        {
            if (markedState != MarkedState.MarkedSent && markedState != MarkedState.MarkedReceived && markedState != (MarkedState.MarkedSent | MarkedState.MarkedReceived))
            {
                markedState = MarkedState.Unmarked;
            }
        }

        public bool IsMarkedSent()
        {
            return (markedState & MarkedState.MarkedSent) != 0;
        }

        public bool IsMarkedReceived()
        {
            return (markedState & MarkedState.MarkedReceived) != 0;
        }
    }
}