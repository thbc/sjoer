using Assets.InfoItems;
using Assets.Positional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Graphics
{
    class Solution
    {
        public List<Tuple<InfoItem, float>> Moves;
        public float Goodness;
        public Solution(float e = 0)
        {
            Moves = new List<Tuple<InfoItem, float>>();
            Goodness = e;
        }

        public void AddMove(InfoItem i, float o)
        {
            Moves.Add(new Tuple<InfoItem, float>(i, o));
        }

        public void Evaluate()
        {
            foreach (Tuple<InfoItem, float> item in Moves)
            {
                Goodness += (float) Math.Pow((1 + item.Item2), 2);
            }
        }
    }

    class PostProcessor
    {
        protected List<InfoItem> infoItems;
        protected Player aligner;

        public List<InfoItem> PostProcess(List<InfoItem> infoItems)
        {
            this.infoItems = infoItems;
            //Process();
            return infoItems;
        }

        public void SetAligner(Player aligner)
        {
            this.aligner = aligner;
        }

        protected virtual void Process() { }
    }

    class AISSkyPostProcessor : PostProcessor
    {
        float rectWidth = 2f;
        protected override void Process()
        {
            if (infoItems.Count < 2) return;

            foreach (InfoItem infoItem in infoItems)
            {
                Collider myCol = infoItem.Shape.GetComponent<Collider>();
                List<InfoItem> colliders = new List<InfoItem>() { infoItem };
                foreach (InfoItem other in infoItems)
                {
                    if (infoItem == other) continue;
                    Collider theirCol = other.Shape.GetComponent<Collider>();

                    if (myCol.bounds.Intersects(theirCol.bounds))
                    {
                        colliders.Add(other);
                    }
                }

                if (colliders.Count <= 1) continue; 

                Debug.Log($"Before sort: " + string.Join(" > ", colliders.Select(a => a.Key)));

                colliders.Sort((a, b) => a.TargetNum.CompareTo(b.TargetNum));
                //    InfoItem[] arr = colliders.ToArray();

                //    QuickSort(arr, 0, colliders.Count - 1);

                Debug.Log($"After sort: " + string.Join(" > ", colliders.Select(a => a.Key)));

                for (int k = 0; k < colliders.Count - 1; k++)
                {
                    if (Overlaps(colliders[k], colliders[k + 1]))
                    {
                        colliders[k + 1].Shape.transform.position = HelperClasses.InfoAreaUtils.Instance.MoveAlongCircle(
                            colliders[k + 1].Shape.transform.position,
                            (float)(-Math.PI / 8),
                            aligner.transform.position);
                        colliders[k + 1].Shape.transform.rotation = HelperClasses.InfoAreaUtils.Instance.FaceUser(
                            colliders[k + 1].Shape.transform.position,
                            aligner.transform.position);
                    }
                }





                //colliders.Sort(
                //    (a, b) =>
                //        Vector3.Dot(a.Shape.transform.position, b.Shape.transform.position)
                //        //HelperClasses.InfoAreaUtils.Instance.Vector3ToCircleT(b.Shape.transform.position, aligner.mainCamera.transform.position)
                //        //.CompareTo(
                //        //    HelperClasses.InfoAreaUtils.Instance.Vector3ToCircleT(a.Shape.transform.position, aligner.mainCamera.transform.position)
                //        //)
                //    );

                //colliders.Sort(
                //    (a, b) =>
                //        Vector.Dot(a.Shape.transform.position
                //        HelperClasses.InfoAreaUtils.Instance.Vector3ToCircleT(b.Shape.transform.position, aligner.mainCamera.transform.position)
                //        .CompareTo(
                //            HelperClasses.InfoAreaUtils.Instance.Vector3ToCircleT(a.Shape.transform.position, aligner.mainCamera.transform.position)
                //        )
                //    );

                //for (int i = 0; i < colliders.Count - 1; i++)
                //{
                //    //while (Overlaps(colliders[i], colliders[i + 1]))
                //    //{
                //    float dist = (colliders[i].Shape.transform.position - colliders[i + 1].Shape.transform.position).magnitude;
                //    double omtrek = 2 * 5 * Math.PI;
                //    float rad = rectWidth - (dist / (float)omtrek);

                //    colliders[i + 1].Shape.transform.SetPositionAndRotation(HelperClasses.InfoAreaUtils.Instance.MoveAlongCircle(
                //        colliders[i + 1].Shape.transform.position, rad,
                //        aligner.mainCamera.transform.position
                //        ), HelperClasses.InfoAreaUtils.Instance.FaceUser(colliders[i + 1].Shape.transform.position,
                //        aligner.mainCamera.transform.position));
                //    //}
                //}

                //}

                //Solution s = new Solution(float.PositiveInfinity);

                //foreach (InfoItem infoItem in infoItems)
                //{
                //    Solution newS = GetSolution(infoItem);

                //    if (newS.Goodness < s.Goodness) s = newS;

                //        //o.Item.Shape.transform.position =
                //        //    HelperClasses.InfoAreaUtils.Instance
                //        //    .MoveAlongX(
                //        //        o.Item.Shape.transform.position,
                //        //        o.OffsetVector,
                //        //        aligner.mainCamera.transform.position);
                //        //o.Item.Shape.transform.rotation =
                //        //    HelperClasses.InfoAreaUtils.Instance
                //        //    .FaceUser(
                //        //        o.Item.Shape.transform.position,
                //        //        aligner.mainCamera.transform.position);
                //}
                //ExecuteSolution(s);
            }
        }

        private void QuickSort(InfoItem[] arr, int start, int end)
        {
            int i;
            if (start < end)
            {
                i = Partition(arr, start, end);

                QuickSort(arr, start, i - 1);
                QuickSort(arr, i + 1, end);
            }
        }

        private int Partition(InfoItem[] arr, int start, int end)
        {
            InfoItem temp;
            InfoItem p = arr[end];
            int i = start - 1;

            for (int j = start; j <= end - 1; j++)
            {
                if (IsLeft(arr[j].Shape.transform.position, p.Shape.transform.position, aligner.mainCamera.transform.position))
                {
                    i++;
                    temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                }
            }

            temp = arr[i + 1];
            arr[i + 1] = arr[end];
            arr[end] = temp;
            return i + 1;
        }

        // Find out if A is on the left of B
        private bool IsLeft(Vector3 a, Vector3 b, Vector3 player)
        {
            Vector3 delta = b - a;
            Vector3 dir = player - a;
            Vector3 cross = Vector3.Cross(delta, dir);
            return Math.Sign(cross.y) >= 0;
        } 

        private void ExecuteSolution(Solution s)
        {
            foreach (Tuple<InfoItem, float> pair in s.Moves)
            {
                InfoItem i = pair.Item1;
                float d = pair.Item2;

                i.Shape.transform.position =
                    HelperClasses.InfoAreaUtils.Instance
                    .MoveAlongX(
                        i.Shape.transform.position,
                        d,
                        aligner.mainCamera.transform.position);
                i.Shape.transform.rotation =
                    HelperClasses.InfoAreaUtils.Instance
                    .FaceUser(
                        i.Shape.transform.position,
                        aligner.mainCamera.transform.position);
            }
        }

        private Solution GetSolution(InfoItem i)
        {
            Solution s = new Solution();

            foreach (InfoItem o in infoItems)
            {
                // Skip yourself
                if (o == i) continue;

                if (Overlaps(i, o))
                {
                    s.AddMove(i, CalculateOffsetVector(i, o));
                }
            }

            s.Evaluate();

            return s;
        }

        // How far does Y need to move to not be in X
        private float CalculateOffsetVector(InfoItem x, InfoItem y)
        {
            Vector3 offset = (y.Shape.transform.position - x.Shape.transform.position);
            Vector3 xToPlayer = aligner.mainCamera.transform.position - x.Shape.transform.position;
            return offset.magnitude * Math.Sign(Vector3.Dot(xToPlayer, offset));
        }
        private bool Overlaps(InfoItem x, InfoItem y)
        {
            Collider xC = x.Shape.GetComponent<Collider>();
            Collider yC = y.Shape.GetComponent<Collider>();

            return xC.bounds.Intersects(yC.bounds);
        }
    }

    class AISHorizonPostProcessor : PostProcessor
    {
        protected override void Process()
        {

        }
    }

    class NAVAIDHorizonPostProcessor : PostProcessor
    {
        protected override void Process()
        {

        }
    }
}
