using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Positional;
using Assets.HelperClasses;
using System.Threading;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Assets.DataManagement.Navaids
{
    public class NavaidDataFactory : MonoBehaviour
    {
        public GameObject seamarkerPrefab;
        public Transform seamarkerContainer;
        public NavaidData.NavaidDict currentNavaids = new NavaidData.NavaidDict();
        private CancellationTokenSource cancellationTokenSource;


        void Start()
        {
            //  Player.Instance.OnPlayerPositionChanged += UpdateNavaidPositions;
            cancellationTokenSource = new CancellationTokenSource();

        }
        void Update()
        {
            UpdateNavaidPositions();
        }

        public void UpdateNavaids(List<NavaidData.Navaid> newFeatures)
        {
            // Step 1: Mark navaids for removal
            HashSet<string> newFeatureIds = new HashSet<string>(newFeatures.Select(f => f.GlobalID));
            List<string> navaidsToRemove = currentNavaids.navaids.Keys.Where(id => !newFeatureIds.Contains(id)).ToList();

            // Step 2: Remove the marked navaids and related GameObjects
            foreach (var featureId in navaidsToRemove)
            {
                if (currentNavaids.navaids.TryGetValue(featureId, out NavaidData.Navaid oldFeature))
                {
                    if (oldFeature.Shape != null)
                    {
                        Destroy(oldFeature.Shape);
                    }
                    currentNavaids.navaids.Remove(featureId);
                    Debug.Log($"Removed feature with GlobalID: {featureId}");
                }
            }

            // Step 3: Add or update remaining navaids
            foreach (var newFeature in newFeatures)
            {
                if (currentNavaids.navaids.ContainsKey(newFeature.GlobalID))
                {
                    // Update existing feature if necessary
                    // You can update the properties of the existing feature or replace the feature entirely
                    // You might also want to update the associated GameObject here
                }
                else
                {
                    // Add new feature
                    currentNavaids.navaids[newFeature.GlobalID] = newFeature;
                    Debug.Log($"Added new feature with GlobalID: {newFeature.GlobalID}");
                    //newFeature.Shape = CreateNavaidGameObject(newFeature);
                    //UpdateNavaidPosition(newFeature);  

                }

            }
        }

        public NavaidsGroup currentlyDisplayedNavaidGroup;

        public void SetCurrentNavaidGroup(NavaidsGroup _currGroup)
        {
            currentlyDisplayedNavaidGroup = _currGroup;
            DisplayNavaids();
        }

        public void DisplayNavaids()//(List<NavaidData.Navaid> newFeatures)
        {
            foreach (var newFeature in currentNavaids.navaids.Values)
            {
                Debug.LogWarning("display navaid: " + newFeature.GlobalID);
                if (newFeature.IsPartOfGroup(currentlyDisplayedNavaidGroup))
                {
                    if (newFeature.Shape == null)
                        newFeature.Shape = CreateNavaidGameObject(newFeature);

                    //  UpdateNavaidPosition(newFeature);
                }
                else
                {
                    Debug.LogWarning(newFeature.GlobalID + " is not part of group: " + currentlyDisplayedNavaidGroup.ToString());

                    // the ones that were displayed before but are not part of current group anymore:
                    if (newFeature.navaidInstance != null)
                    {
                        Destroy(newFeature.Shape);
                        newFeature.Shape = null;
                        newFeature.navaidInstance = null;
                        Debug.Log("Destroyed NavaidInstance: " + newFeature.GlobalID);
                    }

                }
            }
        }

        private GameObject CreateNavaidGameObject(NavaidData.Navaid feature)
        {

            GameObject navaidObject = Instantiate(seamarkerPrefab, seamarkerContainer);
            navaidObject.name = feature.GlobalID;
            feature.navaidInstance = navaidObject.GetComponent<NavaidMonoBehaviour>();
            feature.navaidInstance.setTypeText(feature.NavaidType.ToString());
            //navaidObject.transform.position = new Vector3((float)feature.properties.Long, 0, (float)feature.properties.Lat);
            // Add any required components to the GameObject
            Debug.LogWarning("created GObj for " + feature.GlobalID);
            return navaidObject;
        }

        public void UpdateNavaidPositions()
        {
            foreach (var item in currentNavaids.navaids)
            {
                UpdateNavaidPosition(item.Value);
            }
        }
        public void UpdateNavaidPosition(NavaidData.Navaid _navaid)
        {
            if (_navaid.Shape == null)
                return;
            Vector3 position = Player.Instance.GetWorldTransform(_navaid.Latitude, _navaid.Longitude);
            _navaid.Shape.transform.position = HelperClasses.InfoAreaUtils.Instance.UnityCoordsToHorizonPlane(position, Player.Instance.mainCamera.transform.position, .7f);
            _navaid.Shape.transform.rotation = HelperClasses.InfoAreaUtils.Instance.FaceUser(_navaid.Shape.transform.position, Player.Instance.mainCamera.transform.position);
            float dist = position.magnitude;
            _navaid.navaidInstance.setDistanceText(dist);
            // experimental:
            
            _navaid.navaidInstance.DisplayContent(true);

            foreach (var item in currentNavaids.navaids)
            {
                if(/* item.Value.Shape != null &&  */item.Value != _navaid)
                    CheckAndHandleOverlap(_navaid, item.Value);
            }
            

        }
        /* private void UpdateVisibility()
            {
                // Reset visibility for all navaids
                foreach (var navaid in currentNavaids.navaids)
                {
                    if (navaid.Value.Shape != null)
                        navaid.Shape.GetComponent<Renderer>().enabled = true;
                }

                // Check and handle overlaps in one pass
                for (int i = 0; i < navaids.Count; i++)
                {
                    for (int j = i + 1; j < navaids.Count; j++)
                    {
                        CheckAndHandleOverlap(navaids[i], navaids[j]);
                    }
                }
            } */

        private void CheckAndHandleOverlap(NavaidData.Navaid navaid1, NavaidData.Navaid navaid2)
        {
            if (navaid1.Shape == null || navaid2.Shape == null)
                return;

            Collider collider1 = navaid1.Shape.GetComponent<Collider>();
            Collider collider2 = navaid2.Shape.GetComponent<Collider>();

            if (collider1.bounds.Intersects(collider2.bounds))
            {
                Vector3 referencePosition = Player.Instance.transform.position; // Define the reference position
                HandleOverlapVisibility(navaid1, navaid2, referencePosition);
            }
        }

        private void HandleOverlapVisibility(NavaidData.Navaid navaid1, NavaidData.Navaid navaid2, Vector3 referencePosition)
        {
            float distance1 = Vector3.Distance(navaid1.Shape.transform.position, referencePosition);
            float distance2 = Vector3.Distance(navaid2.Shape.transform.position, referencePosition);

            if (distance1 <= distance2)
            {
                navaid2.navaidInstance.DisplayContent(false);
            }
            else
            {
                navaid1.navaidInstance.DisplayContent(false);
            }
        }

        /* 
            infoItem.Shape.transform.position =HelperClasses.InfoAreaUtils.Instance.UnityCoordsToHorizonPlane(position, aligner.mainCamera.transform.position);
            infoItem.Shape.transform.rotation =HelperClasses.InfoAreaUtils.Instance.FaceUser(infoItem.Shape.transform.position, aligner.mainCamera.transform.position);
        */
    }
}