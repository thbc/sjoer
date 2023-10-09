using System;
using UnityEngine;
using Assets.Resources;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Positional;
using Assets.Graphics;
using Assets.DataManagement;

namespace Assets.HelperClasses
{
    public class InfoAreaUtils : CSSingleton<InfoAreaUtils>
    {
        public Vector3 UnityCoordsToHorizonPlane(Vector3 obj, Vector3 player)
        {
            Vector3 dir = (obj - player).normalized;
            Vector3 newPosition = player + dir * (float)Config.Instance.conf.UISettings["HorizonPlaneRadius"];
            return new Vector3(
                    newPosition.x,
                    (float)Config.Instance.conf.VesselSettingsD["BridgeHeight"] - (float)Config.Instance.conf.DataSettings["UIElementHeight"], // newPosition.y > align with the horizon. TODO: Get layer number
                    newPosition.z
                );

        }

        public Vector3 UnityCoordsToSkyArea(Vector3 obj, ExpandState expandState, Player player)
        {
            Vector3 tmp = UnityCoordsToHorizonPlane(obj, player.mainCamera.transform.position);
            return new Vector3(
                    tmp.x,
                    expandState == ExpandState.Target ? tmp.y + (float)Config.Instance.conf.DataSettings["SkyAreaHeightTarget"] : tmp.y + (float)Config.Instance.conf.DataSettings["SkyAreaHeightHover"],
                    tmp.z
                );
        }

        public float Vector3ToCircleT(Vector3 pt, Vector3 center)
        {
            Vector3 up = center + Vector3.forward + new Vector3(0, pt.y, 0);
            float angle = Vector3.Angle(up, pt);
            return angle;
        }

        // t in radians
        public Vector3 CircleTToVector3(float t, float height, Vector3 center)
        {
            Vector3 up = center + Vector3.forward + new Vector3(0, height, 0);
            Vector3 ret = (Quaternion.AngleAxis(t, Vector3.up) * up).normalized * (float)Config.Instance.conf.UISettings["HorizonPlaneRadius"];

            return ret;
        }

        public static Vector2 rotate(Vector2 v, float delta)
        {
            return new Vector2(
                v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
                v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
        }

        public Vector2 DegreesToWorldVec(float deg, Quaternion toWorld)
        {
            float rad = DegreesToRadians(deg);
            return toWorld * new Vector2(
                    (float)Math.Cos(rad), (float)Math.Sin(rad)
                );
        }

        // BCR in meters if result > 0. If -1: no collision
        public double CalculateBCR(Vector2 ast, Vector2 bst, Vector2 ad, Vector2 bd)
        {
            Vector2 intersect = Vector2.zero;
            if (DoRaysIntersect(ast, bst, ad, bd))
            {
                intersect = GetPointOfIntersection(ast, bst, ad, bd);
            }

            return (ast - intersect).magnitude;
        }

        bool DoRaysIntersect(Vector2 ast, Vector2 bst, Vector2 ad, Vector2 bd)
        {
            float dx, dy, det, u, v;
            dx = bst.x - ast.x;
            dy = bst.y - ast.y;
            det = bd.x * ad.y - bd.y * ad.x;
            u = (dy * bd.x - dx * bd.y) / det;
            v = (dy * ad.x - dx * ad.y) / det;
            return u > 0 && v > 0;
        }

        Vector2 GetPointOfIntersection(Vector2 p1, Vector2 p2, Vector2 n1, Vector2 n2)
        {
            Vector2 p1End = p1 + n1; // another point in line p1->n1
            Vector2 p2End = p2 + n2; // another point in line p2->n2

            float m1 = (p1End.y - p1.y) / (p1End.x - p1.x); // slope of line p1->n1
            float m2 = (p2End.y - p2.y) / (p2End.x - p2.x); // slope of line p2->n2

            float b1 = p1.y - m1 * p1.x; // y-intercept of line p1->n1
            float b2 = p2.y - m2 * p2.x; // y-intercept of line p2->n2

            float px = (b2 - b1) / (m1 - m2); // collision x
            float py = m1 * px + b1; // collision y

            return new Vector2(px, py); // return statement
        }

        public Vector3 MoveAlongCircle(Vector3 pt, float step, Vector3 center)
        {
            Vector2 problem2D = new Vector2(pt.x, pt.z);
            problem2D = rotate(problem2D, step);

            return new Vector3(
                problem2D.x,
                pt.y,
                problem2D.y);


            float t = Vector3ToCircleT(pt, center);
            return CircleTToVector3(t + step, pt.y, center);
        }


        // Calculates the new position on the SkyArea/HoriznPlane after moving by xDist
        public Vector3 MoveAlongX(Vector3 obj, float dist, Vector3 player)
        {
            float angle = (float)(Math.PI - 2 * Math.Acos(
                (dist / 2) / Config.Instance.conf.UISettings["HorizonPlaneRadius"]));
            return Quaternion.Euler(0, RadiansToDegrees(angle), 0) * (obj - player);
        }

        float RadiansToDegrees(float radians)
        {
            return radians * (float)(180 / Math.PI);
        }

        float DegreesToRadians(float degrees)
        {
            return degrees * (float)(Math.PI / 180);
        }

        public Quaternion FaceUser(Vector3 position, Vector3 player)
        {
            return Quaternion.LookRotation(player - position);
        }

        public void ShowAISPinInfo(GameObject target, float numInfo, bool def = false)
        {
            BoxCollider boxCollider = target.GetComponent<BoxCollider>();
            GameObject pin = target.transform.Find($"StickAnchor/Stick/PinAnchor").gameObject;
            pin.transform.localScale = new Vector3(pin.transform.localScale.x, def ? 1 : pin.transform.localScale.y * numInfo, pin.transform.localScale.z);
            GameObject icon = target.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/ShipIconAnchor").gameObject;
            icon.transform.localScale = new Vector3(icon.transform.localScale.x, icon.transform.localScale.y, def ? 1 : icon.transform.localScale.z / numInfo);
            GameObject labels = target.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/TopPinAnchor").gameObject;
            labels.transform.localScale = new Vector3(labels.transform.localScale.x, labels.transform.localScale.y, def ? 1 : labels.transform.localScale.z / numInfo);

            boxCollider.size = new Vector3(boxCollider.size.x, def ? 3 : boxCollider.size.y + (numInfo - 1) * boxCollider.size.y, boxCollider.size.z);
            boxCollider.center = new Vector3(boxCollider.center.x, def ? 1.5f : boxCollider.size.y / 2, boxCollider.center.z);

            if (!def)
            {
                GameObject defT = target.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject;
                if (defT.tag == "MARKED-sent" || defT.tag == "MARKED-received")
                    defT.GetComponent<MeshRenderer>().material = MarkerMode.Instance.GetAssignedMaterial();

            }
        }

        public void ToggleHelperStick(GameObject g, bool enable)
        {
            GameObject helperStick = g.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/StickConnection").gameObject;
            helperStick.SetActive(enable);





        }

        public void ToggleAISPinOverflowVisible(GameObject g, ExpandState expandState)
        {
            int n = (int)Config.Instance.conf.DataSettings["NumItemsOnHover"];

            GetAISPinComponent(g, "1Label").enabled = n > 1 && expandState != ExpandState.Collapsed;
            GetAISPinComponent(g, "1Value").enabled = n > 1 && expandState != ExpandState.Collapsed;
            GetAISPinComponent(g, "2Label").enabled = n > 2 && expandState != ExpandState.Collapsed;
            GetAISPinComponent(g, "2Value").enabled = n > 2 && expandState != ExpandState.Collapsed;

            GetAISPinComponent(g, "TargetNum").enabled = expandState == ExpandState.Target;

            // Lastly enable/disable the target image
            g.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/TopPinAnchor/TopPinAnchor2/CanvasTxt/Target").GetComponent<Image>().enabled = expandState == ExpandState.Target; ;


        }

        public void ToggleTargetActive(GameObject g, ExpandState expandState)
        {
            g.transform.Find($"PinAnchor/AISPinTarget/Canvas/TargetButtonA").GetComponent<Image>().enabled = expandState == ExpandState.Target;
            g.transform.Find($"PinAnchor/AISPinTarget/Canvas/TargetButtonP").GetComponent<Image>().enabled = expandState != ExpandState.Target;
        }

        private TextMeshProUGUI GetAISPinComponent(GameObject g, string fname)
        {
            GameObject obj = g.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/TopPinAnchor/TopPinAnchor2/CanvasTxt/{fname}").gameObject;
            return obj.GetComponent<TextMeshProUGUI>(); ;
        }

        public void ScaleStick(GameObject target, float scale, bool def = false)
        {
            BoxCollider boxCollider = target.GetComponent<BoxCollider>();
            GameObject stick = target.transform.Find($"StickAnchor").gameObject;
            GameObject pin = target.transform.Find($"StickAnchor/Stick/PinAnchor").gameObject;
            pin.transform.localPosition = def ? Vector3.up * 1.25f : Vector3.up * 1.25f * scale;
            GameObject distanceRuler = target.transform.Find($"StickAnchor/DistanceRuler").gameObject;
            //stick.transform.localScale = new Vector3(stick.transform.localScale.x, def ? 1 : stick.transform.localScale.y * scale, stick.transform.localScale.y);
            //pin.transform.localScale = new Vector3(pin.transform.localScale.x, def ? 1 : pin.transform.localScale.y * (1/scale), pin.transform.localScale.z);
            //distanceRuler.transform.localScale = new Vector3(def ? 0.1f : distanceRuler.transform.localScale.x * (1 / scale), distanceRuler.transform.localScale.y, distanceRuler.transform.localScale.z);

            boxCollider.size = new Vector3(boxCollider.size.x, def ? 3.25f : boxCollider.size.y + (scale / boxCollider.size.y) * boxCollider.size.y, boxCollider.size.z);
            boxCollider.center = new Vector3(boxCollider.center.x, def ? 1.625f : boxCollider.center.y + (scale / boxCollider.center.y) * boxCollider.center.y, boxCollider.center.z);
        }

        public float GetStickScale(GameObject target)
        {
            GameObject stick = target.transform.Find($"StickAnchor").gameObject;
            return stick.transform.localScale.y;
        }

        public float GetYPosOfDistanceRuler(GameObject target, float distanceToVessel)
        {
            float maxDistance = 2f; // HelperClasses.InfoAreaUtils.Instance.GetStickScale(target);
            float maxDistanceR = (float)Config.Instance.conf.DataSettings["MaxRulerDistance"];
            return (Math.Min(distanceToVessel, maxDistanceR) / maxDistanceR) * maxDistance;
        }

        public void ScalePin(GameObject target, float scale)
        {
            BoxCollider boxCollider = target.GetComponent<BoxCollider>();
            GameObject pin = target.transform.Find($"StickAnchor/Stick/PinAnchor").gameObject;
            pin.transform.localScale = new Vector3(pin.transform.localScale.x * scale, pin.transform.localScale.y * scale, pin.transform.localScale.z);

            boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y + (scale / boxCollider.size.y) * boxCollider.size.y, boxCollider.size.z);
            boxCollider.center = new Vector3(boxCollider.center.x, boxCollider.center.y + (scale / boxCollider.center.y) * boxCollider.center.y, boxCollider.center.z);
        }

        public void PinToLayerOne(GameObject target)
        {
            HelperClasses.InfoAreaUtils.Instance.ScaleStick(target, 1f);
            //HelperClasses.InfoAreaUtils.Instance.ScalePin(gameObject, 2f);
        }

        public void PinToLayerTwo(GameObject target)
        {
            HelperClasses.InfoAreaUtils.Instance.ScaleStick(target, 2f);

        }

    }
}
