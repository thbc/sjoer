using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.HelperClasses
{
    class Timer : MonoBehaviour
    {
        private float time;
        private float totalTime;
        private bool done = false;
        private Action onComplete;

        public Timer(float time, Action onComplete)
        {
            this.time = time;
            this.totalTime = time;
            this.onComplete = onComplete;
        }

        void Start()
        {

        }

        public void Update()
        {
            if (!done)
            {
                if (time > 0)
                {
                    time -= Time.deltaTime;

                    if (time < 0) time = 0;
                }

                if (time == 0)
                {
                    done = true;
                    this.onComplete();
                }
            }

        }

        public double GetSecondsRemaining()
        {
            return Math.Round(time);
        }

        public bool hasFinished()
        {
            return done;
        }

        public void restart()
        {
            done = false;
            time = totalTime;
        }

    }
}
