using System;
using UnityEngine;
using Assets.Resources;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.HelperClasses
{
    public class TargetNumberProvider : CSSingleton<TargetNumberProvider>
    {
        HashSet<int> ProvidedTargetNums = new HashSet<int>();

        public int GetTargetInt()
        {
            int i = 1;
            while (ProvidedTargetNums.Contains(i))
            {
                i++;
            }
            ProvidedTargetNums.Add(i);
            return i;
        }

        public void HandInTargetInt(int i)
        {
            ProvidedTargetNums.Remove(i);
        }
    }
}
