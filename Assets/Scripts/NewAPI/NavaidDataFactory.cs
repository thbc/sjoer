using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.DataManagement.Navaids;
using System.Linq;
using Assets.Positional;
namespace Assets.DataManagement.Navaids
{
    public class NavaidDataFactory : MonoBehaviour
    {
        public GameObject seamarkerPrefab;
        public Transform seamarkerContainer;
        public Dictionary<string, NavaidData.Feature> currentNavaids = new Dictionary<string, NavaidData.Feature>();

        void Start()
        {
            Player.Instance.OnPlayerPositionChanged += UpdateNavaidPositions;
        }
        void Update()
        {
            UpdateNavaidPositions();
        }
        
        public void UpdateFeatures(NavaidData.FeatureCollection newFeatures)
        {
            // Step 1: Mark navaids for removal
            HashSet<string> newFeatureIds = new HashSet<string>(newFeatures.features.Select(f => f.properties.GlobalID));
            List<string> navaidsToRemove = currentNavaids.Keys.Where(id => !newFeatureIds.Contains(id)).ToList();

            // Step 2: Remove the marked navaids and related GameObjects
            foreach (var featureId in navaidsToRemove)
            {
                if (currentNavaids.TryGetValue(featureId, out NavaidData.Feature oldFeature))
                {
                    if (oldFeature.gameObject != null)
                    {
                        Destroy(oldFeature.gameObject);
                    }
                    currentNavaids.Remove(featureId);
                    Debug.Log($"Removed feature with GlobalID: {featureId}");
                }
            }

            // Step 3: Add or update remaining navaids
            foreach (var newFeature in newFeatures.features)
            {
                if (currentNavaids.ContainsKey(newFeature.properties.GlobalID))
                {
                    // Update existing feature if necessary
                    // You can update the properties of the existing feature or replace the feature entirely
                    // You might also want to update the associated GameObject here
                }
                else
                {
                    // Add new feature
                    currentNavaids[newFeature.properties.GlobalID] = newFeature;
                    newFeature.gameObject = CreateNavaidGameObject(newFeature);
                    Debug.Log($"Added new feature with GlobalID: {newFeature.properties.GlobalID}");

                    UpdateNavaidPosition(newFeature);

                }
            }
        }

        // Utility method to create a GameObject for a Feature
        private GameObject CreateNavaidGameObject(NavaidData.Feature feature)
        {

            GameObject navaidObject = Instantiate(seamarkerPrefab, seamarkerContainer);
            navaidObject.name = feature.properties.GlobalID;
            //navaidObject.transform.position = new Vector3((float)feature.properties.Long, 0, (float)feature.properties.Lat);
            // Add any required components to the GameObject
            return navaidObject;
        }

        public void UpdateNavaidPositions()
        {
            foreach (var item in currentNavaids)
            {
                UpdateNavaidPosition(item.Value);
            }
        }
        public void UpdateNavaidPosition(NavaidData.Feature _navaid)
        {
            Vector3 position = Player.Instance.GetWorldTransform(_navaid.properties.Lat, _navaid.properties.Lat);
            _navaid.gameObject.transform.position = HelperClasses.InfoAreaUtils.Instance.UnityCoordsToHorizonPlane(position, Player.Instance.mainCamera.transform.position);
            _navaid.gameObject.transform.rotation = HelperClasses.InfoAreaUtils.Instance.FaceUser(_navaid.gameObject.transform.position, Player.Instance.mainCamera.transform.position);

        
        }
        
        /* 
            infoItem.Shape.transform.position =HelperClasses.InfoAreaUtils.Instance.UnityCoordsToHorizonPlane(position, aligner.mainCamera.transform.position);
            infoItem.Shape.transform.rotation =HelperClasses.InfoAreaUtils.Instance.FaceUser(infoItem.Shape.transform.position, aligner.mainCamera.transform.position);
        */
    }
}