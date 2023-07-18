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

            Debug.Log($"{Key} = {this.meta.DesiredState}");

            /*  // send MARKED object to other devices
             if(gameObject?.tag == "MARKED")
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
                    if (this.meta.DesiredState == ExpandState.Target)
                    {
                        if (MarkerMode.Instance.allowMarking)
                        {
                            //  this.gameObject.tag = "MARKED";
                            MarkerMode.Instance.SendMarker(this.dto.Key);
                        }
                    }
                    else if(this.meta.DesiredState == ExpandState.Collapsed && this.gameObject.tag == "MARKED")
                    {
                        MarkerMode.Instance.UnmarkItem(this.gameObject);
                    }

                }


            }
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

