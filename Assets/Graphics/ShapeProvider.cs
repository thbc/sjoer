using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using TMPro;
using System.IO;
using Assets.DataManagement;
using Assets.Resources;
using Assets.InfoItems;

namespace Assets.Graphics
{

    class ShapeProvider
    {
        public virtual void Get(InfoItem infoItem)
        {
            throw new NotImplementedException();
        }

        protected GameObject GetShape(string fname)
        {
            GameObject gameObject = GameObject.Instantiate(
                    AssetManager.Instance.objects[fname],
                    Vector3.zero,
                    Quaternion.identity
                );

            // 30/15 = 2, 150/15
            gameObject.transform.localScale = gameObject.transform.localScale * ((float)Config.Instance.conf.UISettings["HorizonPlaneRadius"] / 23);
            //HelperClasses.InfoAreaUtils.Instance.ScaleStick(gameObject, 2f);
            //HelperClasses.InfoAreaUtils.Instance.ScalePin(gameObject, 2f);


            return gameObject;
        }
    }

    class AISHorizonShapeProvider : ShapeProvider
    {
        public override void Get(InfoItem infoItem)
        {
            if (!infoItem.Shape) InjectNewShape(infoItem);

            UpdateShape(infoItem);
        }

        private void UpdateShape(InfoItem infoItem)
        {
            HelperClasses.InfoAreaUtils.Instance.ToggleAISPinOverflowVisible(infoItem.Shape, infoItem.DesiredState);

            // Reset sizing of these things
            HelperClasses.InfoAreaUtils.Instance.ShowAISPinInfo(infoItem.Shape, 0, true);
            HelperClasses.InfoAreaUtils.Instance.ScaleStick(infoItem.Shape, 0, true);
            HelperClasses.InfoAreaUtils.Instance.ToggleHelperStick(infoItem.Shape, false); //4

            switch (infoItem.DesiredState)
            {
                case (ExpandState.Collapsed):
                    // Do nothing, already collapsed
                    break;
                case (ExpandState.Hover):
                    HelperClasses.InfoAreaUtils.Instance.ShowAISPinInfo(infoItem.Shape, (float)Config.Instance.conf.DataSettings["NumItemsOnHover"]); //3
                    break;
                case (ExpandState.Target):
                    HelperClasses.InfoAreaUtils.Instance.ScaleStick(infoItem.Shape, 2f);
                    HelperClasses.InfoAreaUtils.Instance.ShowAISPinInfo(infoItem.Shape, (float)Config.Instance.conf.DataSettings["NumItemsOnHover"] + 1); //4
                    HelperClasses.InfoAreaUtils.Instance.ToggleHelperStick(infoItem.Shape, true); //4

                    break;
            }
        }

        private void InjectNewShape(InfoItem infoItem)
        {
            string prefab = "AISPin";
            infoItem.Shape = GetShape(prefab);

            // A new shape always starts collapsed
            HelperClasses.InfoAreaUtils.Instance.ToggleAISPinOverflowVisible(infoItem.Shape, ExpandState.Collapsed);
        }
    }

    class AISSkyShapeProvider : ShapeProvider
    {
        public override void Get(InfoItem infoItem)
        {
            if (!infoItem.Shape) infoItem.Shape = GetShape("AISPinTarget");

            UpdateShape(infoItem);
        }

        private void UpdateShape(InfoItem infoItem)
        {
            HelperClasses.InfoAreaUtils.Instance.ToggleTargetActive(infoItem.Shape, infoItem.DesiredState);
        }
    }


    // new code:----
    /*   class NAVAIDHorizonShapeProvider : ShapeProvider
    {
        public override void Get(InfoItem infoItem)
        {
            if (!infoItem.Shape) InjectNewShape(infoItem);

            UpdateShape(infoItem);
        }

        private void UpdateShape(InfoItem infoItem)
        {
          //  HelperClasses.InfoAreaUtils.Instance.ToggleAISPinOverflowVisible(infoItem.Shape, infoItem.DesiredState);

            // Reset sizing of these things
         //   HelperClasses.InfoAreaUtils.Instance.ShowAISPinInfo(infoItem.Shape, 0, true);
         //   HelperClasses.InfoAreaUtils.Instance.ScaleStick(infoItem.Shape, 0, true);
        //    HelperClasses.InfoAreaUtils.Instance.ToggleHelperStick(infoItem.Shape, false); //4

        //    switch (infoItem.DesiredState)
        //    {
          //      case (ExpandState.Collapsed):
                    // Do nothing, already collapsed
         //           break;
         //       case (ExpandState.Hover):
          //          HelperClasses.InfoAreaUtils.Instance.ShowAISPinInfo(infoItem.Shape, (float)Config.Instance.conf.DataSettings["NumItemsOnHover"]); //3
           //         break;
           //     case (ExpandState.Target):
            //        HelperClasses.InfoAreaUtils.Instance.ScaleStick(infoItem.Shape, 2f);
             //       HelperClasses.InfoAreaUtils.Instance.ToggleHelperStick(infoItem.Shape, true); //4

               //     break;
          //  }
        }

        private void InjectNewShape(InfoItem infoItem)
        {
            string prefab = "NAVAIDicon";
            infoItem.Shape = GetShape(prefab);

            // A new shape always starts collapsed
           // HelperClasses.InfoAreaUtils.Instance.ToggleAISPinOverflowVisible(infoItem.Shape, ExpandState.Collapsed);
        }
    } */
}