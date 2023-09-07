using Assets.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.InfoItems
{
    class Meta
    {       
        bool target;
        bool previousTarget;
        bool expanded;
        int targetNum;
        ExpandState desiredState;
        ExpandState currentState;
        protected DataType dataType;
        protected DisplayArea displayArea;

        public Meta(bool target, DataType dataType, DisplayArea displayArea)
        {
            this.target = false;
            this.dataType = dataType;
            this.displayArea = displayArea;
            this.previousTarget = target;
            this.desiredState = ExpandState.Collapsed;
            this.currentState = ExpandState.Collapsed;
        }

        public int TargetNum
        {
            get { return this.targetNum; }
            set { this.targetNum = value; }
        }

        public bool Expanded
        {
            get { return this.expanded; }
            set { this.expanded = value; }
        }

        public ExpandState CurrentState
        {
            get { return this.currentState; }
            set { this.currentState = value; }
        }

        public ExpandState DesiredState
        {
            get { return this.desiredState; }
            set { this.desiredState = value; }
        }

        public bool Target 
        { 
            get { return this.target; } 
            set { this.target = value; }
        }

        public DisplayArea DisplayArea
        {
            get { return this.displayArea; }
        }

        public DataType DataType
        {
            get { return this.dataType; }
        }

        public bool PreviousTarget
        {
            get { return this.previousTarget; }
            set { this.previousTarget = value; }
        }

        // Called on the new Meta object
        public void Merge(Meta oldMeta)
        {
            // Maybe we need to do something here
        }
    }
}