using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.DataManagement;
using UnityEngine;
using Unity;
using Assets.Positional;
using Assets.Graphics;
using Multiplayer.Marking;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit;

namespace Assets.InfoItems
{
    class InfoItem
    {
        protected DTO dto;
        protected GameObject gameObject;
        protected Meta meta;

        private TargettableInfoItem targetHandler;

        public InfoItem(DTO dto, DataType dataType, DisplayArea displayArea)
        {
            this.meta = new Meta(dto.Target, dataType, displayArea);
            this.dto = dto;
        }

        public override bool Equals(System.Object i)
        {
            var other = i as InfoItem;
            return this.Key == other.Key;
        }
        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }

        public bool IsHover
        {
            get { return this.GetTargetHandler() && this.GetTargetHandler().IsHover; }
        }

        public bool IsExpanded
        {
            get { return this.meta.Expanded; }
            set { this.meta.Expanded = value; }
        }

        public bool IsTarget
        {
            get { return this.meta.Target; }
            set { this.meta.Target = value; }
        }

        public int TargetNum
        {
            get { return this.meta.TargetNum; }
            set { this.meta.TargetNum = value; }

        }

        public ExpandState DesiredState
        {
            get { return this.meta.DesiredState; }
            set { this.meta.DesiredState = value; }
        }

        public ExpandState CurrentState
        {
            get { return this.meta.CurrentState; }
            set { this.meta.CurrentState = value; }

        }

        public DataType DataType
        {
            get { return this.meta.DataType; }
        }

        public DisplayArea DisplayArea
        {
            get { return this.meta.DisplayArea; }
        }

        public virtual string Key
        {
            get { return dto.Key; }
        }

        public GameObject Shape
        {
            get { return gameObject; }
            set { gameObject = value; }
        }

        public DTO GetDTO
        {
            get { return dto; }
        }

        public static IEnumerable<InfoItem> Generate(DTO dto, DataType dataType, DisplayArea displayArea)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            //if (GetTargetHandler(true))
            //{
            //    if (GetTargetHandler().IsTarget)
            //    {
            //        Debug.Log("Target on " + Key);

            //    }
            //}
            // First update the target from interactions
            Retarget();
#if UNITY_EDITOR
            //this was added to keep the console clean.
            if (Player.Instance.debugVesselInfoInEditor)
                Debug.Log($"{Key} = {this.meta.DesiredState}");
#endif

            /*  // send MARKED object to other devices
             if(gameObject?.tag == "MARKED-received")
             {
                 Debug.LogWarning("MARKED "+ gameObject.name);
                 GameObject.Find("default").GetComponent<MeshRenderer>().material = Marker.Instance.GetAssignedMaterial();
             } */


            // Get new shape
            Reshape();
            // Fill new shape if necessary
            Refill();
            // Positional shape
            Reposition();

            if (TargetHasChanged())
            {
                if (DisplayArea == DisplayArea.HorizonPlane)
                {
                    OnInfoItemRetargetted();
                }
            }
        }

        protected void UpdateTargetNum()
        {
            if (this.DesiredState == ExpandState.Target) this.meta.TargetNum = HelperClasses.TargetNumberProvider.Instance.GetTargetInt();
            else HelperClasses.TargetNumberProvider.Instance.HandInTargetInt(this.meta.TargetNum);
        }


        // Update target in meta according to selected in scene or selected previously
        protected void Retarget()
        {
            this.meta.DesiredState =
                (this.dto.Target || (GetTargetHandler() && (GetTargetHandler().IsTarget))) ? ExpandState.Target
                : (GetTargetHandler() && (GetTargetHandler().IsHover)) ? ExpandState.Hover
                : ExpandState.Collapsed;

            // Only update the target number if the target has changed. This is the responsibility of the horizon plane
            if (TargetHasChanged())
            {
                if (DisplayArea == DisplayArea.HorizonPlane)
                {


                    UpdateTargetNum();


                    /* Debug.LogWarning(GetTargetHandler().name + "|" + this.DesiredState.ToString());
                    OnInfoItemRetargetted();
 */

                    /* if (this.meta.CurrentState == ExpandState.Target)
                    {
                        
                        // left hand interaction for  Minimizing expanded objects 
                        if (targetHandler.Hand == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Left)
                        {
                            // if the sentMarker is unmarked by the sender (sending-user)
                            if (this.gameObject.tag == "MARKED-sent")
                            {
                                // this is happenning locally on any items that were received via OSC to be marked
                                MarkerMode.Instance.UnmarkSentItem(this.gameObject);
                            }
                        }
                        // right hand interaction for  Minimizing expanded objects 
                        if (targetHandler.Hand == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right)
                        {
                            // if the receivedMarker is unmarked by the receiver (receiving-user)
                            if (this.gameObject.tag == "MARKED-received")
                            {
                                // this is happenning locally on any items that were received via OSC to be marked
                                MarkerMode.Instance.UnmarkReceivedItem(this.gameObject);
                            }
                        }
                    }
                    else if (this.meta.CurrentState == ExpandState.Collapsed)// && this.gameObject.tag != "Untagged") //interaction for  Minimizing expanded objects that are either marked or MARKED-sent
                    {
                        // left hand interaction for Maxmizing collapsed objects
                        if (MarkerMode.Instance.allowMarking && targetHandler.Hand == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Left)
                        {

                            // this calls SendMarker as soon as allowMarking is true. In MarkerMode we check whether connection is established before sending it
                            MarkerMode.Instance.SendMarker(this.dto.Key);
                        }

                        // Optional :
                        // right hand interaction for Maxmizing collapsed objects
                        // currently not implemented since right hand maximizing is handled locally previously in InfoItem
                        //else if (MarkerMode.Instance.allowMarking && targetHandler.Hand == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right)
                        //{ 
                        //}

                    } */

                }


            }
        }

        #region Notifications from InfoItem to its linked target handler

        // Before we did this in TargettableInfoItem.OnClick()
        // .. but we want to make sure that either clicking on Pin or SkyArea panel both trigger this..

        void OnInfoItemRetargetted()
        {
      //      Debug.LogWarning("OnInfoItemRetargetted");
            if (this.meta.DesiredState == ExpandState.Target)// if (IsTarget)
            {
         //   Debug.LogWarning(GetTargetHandler().name + "|" + this.DesiredState.ToString());

    //            Debug.LogWarning("OnInfoItemRetargetted - IsTarget");

                //Debug.Log("is target");
                //interaction for Maxmizing collapsed objects
                if (MarkerMode.Instance.allowMarking)
                {
        //            Debug.LogWarning("OnInfoItemRetargetted - IsAllowMarking");

                    OnMaximize();
                }
       //         else Debug.LogWarning("OnInfoItemRetargetted - IsNOT!!!!!AllowMarking");



            }
            //interaction for Minimizing expanded objects 
            else if (this.meta.DesiredState == ExpandState.Collapsed)  //else if (!IsTarget)
            {
      //          Debug.LogWarning(GetTargetHandler().name + "|" + this.DesiredState.ToString());

       //         Debug.LogWarning("OnInfoItemRetargetted - IsNotTarget");

                // we only unmark tagged objects????
                /*
                    if (this.gameObject.tag == "Untagged")
                        return;
                */
                OnMinimize();
            }
        }

        #endregion



        #region MarkedState Handling
        public MarkedStateFlag markedStateFlag = new MarkedStateFlag();
        // proxy methods to interact with the MarkedStateFlag instance

        /// <summary>
        /// Sets the flag for Marked-Sent items. If  <param name="state"></param> is true, it will add the flag, if false it will remove it.
        /// </summary>
        /// <param name="state"></param>
        public void SetState_MarkedSent(bool state)
        {
            if (state == true)
                markedStateFlag.SetMarkedState(MarkedStateFlag.MarkedState.MarkedSent);
            else
                markedStateFlag.ClearMarkedState(MarkedStateFlag.MarkedState.MarkedSent);

        }
        public void SetState_MarkedReceived(bool state)
        {
            if (state == true)
                markedStateFlag.SetMarkedState(MarkedStateFlag.MarkedState.MarkedReceived);
            else
                markedStateFlag.ClearMarkedState(MarkedStateFlag.MarkedState.MarkedReceived);
        }

        public bool IsMarked_Sent()
        {
            return markedStateFlag.IsMarkedSent();
        }

        public bool IsMarked_Received()
        {
            return markedStateFlag.IsMarkedReceived();
        }
        #endregion
        private void OnMinimize()
        {
            if (GetTargetHandler().Hand == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Left)
            {
                if (IsMarked_Sent())
                {

                    //remove from List<Marked_sent>
                    //set Material
                    //send to client                
                    MarkerMode.Instance.UnmarkSent(this);
                }

            }
            else if (GetTargetHandler().Hand == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right)
            {
                if (IsMarked_Received())
                {
                    //remove from List<Marked_received>
                    //set Material
                    //send to client       
                    MarkerMode.Instance.UnmarkReceived(this);
                }
            }
        }

        private void OnMaximize()
        {
      //      Debug.LogWarning("on max");
            if (GetTargetHandler().Hand == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Left)
            {
      //          Debug.LogWarning("ON MAX- left");

                if (!IsMarked_Sent())
                {
//                    Debug.LogWarning("ON MAX- !IsMarked_Sent");

                    //remove from List<Marked_sent>
                    //set Material
                    //send to client                
                    MarkerMode.Instance.MarkSend(this);
                }
            }
     //       else Debug.LogWarning("on max, but hand is not left");

        }
        public TargettableInfoItem GetTargetHandler(bool forceUpdate = false)
        {
            // If the target changes, a new GameObject is created in ShapeProvider, which forces and update of `targetHandler`
            if (!targetHandler || forceUpdate)
            {
                targetHandler = this.gameObject ? this.gameObject.GetComponent<TargettableInfoItem>() : null;
            }
            return targetHandler;
        }

        // Either we were or became a target, but a change has taken place
        public bool TargetHasChanged()
        {
            return (
                this.meta.DesiredState == ExpandState.Target
                || this.meta.CurrentState == ExpandState.Target)
                && this.meta.DesiredState != this.meta.CurrentState;
        }

        protected virtual void Reshape()
        {
            GraphicFactory.Instance.getShapeProvider(meta.DataType, meta.DisplayArea).Get(this);
        }

        protected virtual void Refill()
        {
            GraphicFactory.Instance.GetFiller(meta.DataType, meta.DisplayArea).Fill(this);
        }

        protected virtual void Reposition()
        {
            GraphicFactory.Instance.getPositioner(meta.DataType, meta.DisplayArea).Position(this);
        }

        // Called on the old InfoItem
        public void InjectNewDTO(DTO dto)
        {
            //this.meta.PreviousTarget = this.meta.Target;
            this.meta.CurrentState = this.meta.DesiredState;
            this.dto = dto;
        }

        // Called on the InfoItem that contains the link
        public void LinkTargetHandler(InfoItem infoItem)
        {
            TargettableInfoItem handler = GetTargetHandler();
            TargettableInfoItem linkedHandler = infoItem.GetTargetHandler();
            handler.SetLink(linkedHandler);
            // Equalize this and linked target status
            handler.IsTarget = linkedHandler.IsTarget;
        }

        public void DestroyMesh()
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

    class AISInfoItem : InfoItem
    {
        public AISInfoItem(DTO dto, DataType dataType, DisplayArea displayArea) : base(dto, dataType, displayArea)
        {
        }

        public static new IEnumerable<InfoItem> Generate(DTO dto, DataType dataType, DisplayArea displayArea)
        {
            AISDTOs aisDTOs = (AISDTOs)dto;

            for (int i = 0; i < aisDTOs.vessels.Length; i++)
            {
                AISDTO aisDTO = aisDTOs.vessels[i];
                if (aisDTO.Valid)
                {
                    yield return new AISInfoItem(aisDTOs.vessels[i], dataType, displayArea);
                }
            }
        }
    }
}

