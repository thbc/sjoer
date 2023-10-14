using UnityEngine;
using Assets.Positional;
using Assets.HelperClasses;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Physics;


public class TestPlacement : MonoBehaviour, IMixedRealitySourceStateHandler
{
    IMixedRealityPointer p;

    public Player aligner;
    public double dtoLatitude;
    public double dtoLongitude;

    public enum approaches
    {
       a1, a2, a3, a4, a5, a6, a7, a8, a9
    }
    public approaches Approach;

        // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        // Subscribe to input source events
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
        if (eventData.Controller != null && eventData.Controller.ControllerHandedness == Handedness.Right)
        {
            p = eventData.Controller.InputSource.Pointers.FirstOrDefault();
            SetupPointer();
        }


    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        if (p != null && eventData.Controller != null && eventData.Controller.ControllerHandedness == Handedness.Right)
        {
            p = null;
        }
    }
    // Update is called once per frame

    public Transform playerTransform;
    void Update()
    {
        switch (Approach)
        {
            case approaches.a1:
                Vector3 position = aligner.GetWorldTransform(dtoLatitude, dtoLongitude);
                //infoItem.Shape.transform.position = HelperClasses.InfoAreaUtils.Instance.UnityCoordsToHorizonPlane(position, aligner.mainCamera.transform.position);
                this.transform.position = InfoAreaUtils.Instance.UnityCoordsToHorizonPlane(position, aligner.mainCamera.transform.position);
                this.transform.rotation = InfoAreaUtils.Instance.FaceUser(this.transform.position, aligner.mainCamera.transform.position);
                break;
            case approaches.a2:
                 Vector3 tmp = InfoAreaUtils.Instance.UnityCoordsToHorizonPlane(this.transform.position, aligner.mainCamera.transform.position);
               var v1= new Vector3(
                tmp.x,
                tmp.y,
                tmp.z
                );
                this.transform.position =v1;



                break;
            case approaches.a3:

                if (p != null && !p.IsFocusLocked) // trying out IsFocusLocked for now
                {
                    RayStep rayStep = p.Rays[0];
                    Vector3 rayOrigin = rayStep.Origin;
                    Vector3 rayDirection = rayStep.Direction;
                    Vector3 positionUnitsAway = rayOrigin + rayDirection * 5.0f;

                    Vector3 obj = positionUnitsAway;

                    Vector3 tmp__ = InfoAreaUtils.Instance.UnityCoordsToHorizonPlane(positionUnitsAway, aligner.mainCamera.transform.position);
                    var v1__ = new Vector3(                     tmp__.x,                     tmp__.y,                     tmp__.z
                     );
                    this.transform.position = v1__;





                   
                        Vector3 dir = (obj - aligner.mainCamera.transform.position).normalized;
                        Vector3 newPosition = aligner.mainCamera.transform.position + dir * 5f; //5 is radius atm
                        var horV = new Vector3(
                                newPosition.x,newPosition.y, newPosition.z);

                    

                  
                        var finV = new Vector3(
                                horV.x,
                               horV.y,
                                horV.z
                            );
                    this.transform.position = finV;
                    


                }


                    break;
            case approaches.a4:
                RayStep rayStep_ = p.Rays[0];
                //Vector3 rayOrigin_ = rayStep_.Origin;

                Vector3 rayDirectionWorld = rayStep_.Direction;

                Vector3 rayDirectionLocal = playerTransform.InverseTransformDirection(rayDirectionWorld);

                Vector3 rayOriginWorld = rayStep_.Origin;
                Vector3 rayOriginLocal = playerTransform.InverseTransformPoint(rayOriginWorld);

                Vector3 positionUnitsAway_ = rayOriginLocal + rayDirectionLocal * 5.0f;
                Vector3 obj_ = positionUnitsAway_;


                Debug.LogWarning("RAY origin:" + rayOriginLocal);

                //Vector3 tmp__ = InfoAreaUtils.Instance.UnityCoordsToHorizonPlane(positionUnitsAway, aligner.mainCamera.transform.position);
                //var v1__ = new Vector3(tmp__.x, tmp__.y, tmp__.z
                // );
                //this.transform.position = v1__;






                Vector3 dir_ = (obj_ - aligner.mainCamera.transform.position).normalized;
                Vector3 newPosition_= aligner.mainCamera.transform.position + dir_ * 5f; //5 is radius atm
                var horV_ = new Vector3(
                        newPosition_.x, newPosition_.y, newPosition_.z);




                var finV_ = new Vector3(
                        horV_.x,
                       horV_.y,
                        horV_.z
                    );
                // ... do stuff with rayOriginLocal ...
                Vector3 rayOriginWorldAgain = playerTransform.TransformPoint(positionUnitsAway_);
                transform.position = rayOriginWorldAgain;
               // Vector3 rayDirectionWorldAgain = playerTransform.TransformDirection(rayDirectionLocal);


                break;
                case approaches.a5:
                RayStep raySt = p.Rays[0];

                Vector3 rayOrig = raySt.Origin;

                // Get the vector from the player's position to the target
                Vector3 direction = rayOrig - aligner.mainCamera.transform.position;

                    // Align this direction with True North
                    direction = aligner.Unity2TrueNorth * direction;

                    var pointInPlayer1Frame = direction.normalized;
                var playerp = aligner.mainCamera.transform.position;

                // Transform the point to world coordinates
                Vector3 pointInWorldFrame = aligner.Unity2TrueNorth * pointInPlayer1Frame + playerTransform.position;

                // Then transform to player2's local coordinates
                Vector3 pointInPlayer2Frame = Quaternion.Inverse(aligner.Unity2TrueNorth) * (pointInWorldFrame - playerp);
                this.transform.position = pointInPlayer2Frame;
                break;
                case approaches.a6:


                //  transform.position = p.Position;    // = finger position
                //CursorFocus = name for cursor from pointer
                transform.position = GameObject.Find("CursorFocus").transform.position;
                
                break;
                case approaches.a7:

                // On Player1's side:
                GameObject worldPointGO = GameObject.Find("CursorFocus");
                if (worldPointGO != null)
                {
                    
                    // The world point has already been obtained.
                    Vector3 worldPoint = worldPointGO.transform.position;

                    // Transform the world point to player2's local space.
                    Vector3 localPointPlayer2 = player2.transform.InverseTransformPoint(worldPoint);

                    // Finally, transform the local point in player2's local space back to world space.
                    Vector3 worldPointFinal = player2.transform.TransformPoint(localPointPlayer2);

                    // Update this object's position to worldPointFinal.
                    this.transform.position = worldPointFinal;


                }
                break;

                case approaches.a8:         //this option is valid an working!!!°!

                GameObject worldPointGO_ = GameObject.Find("CursorFocus");
                if (worldPointGO_ != null)
                {
                    Vector3 obj = worldPointGO_.transform.position;
                    float distance = Vector3.Distance(obj, player1.transform.position);

                    // Get a vector from player to object
                    Vector3 dir = (obj - player1.transform.position).normalized;

                    // Calculate azimuth and elevation
                    float azimuth = (Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg);
                    if (azimuth < 0) azimuth += 360; // Ensure azimuth is between 0 and 360

                    float elevation = Mathf.Asin(dir.y) * Mathf.Rad2Deg;

                    // Convert angles from degrees to radians
                    float azimuthRad = azimuth * Mathf.Deg2Rad;
                    float elevationRad = elevation * Mathf.Deg2Rad;

                    // Convert spherical coordinates to Cartesian coordinates
                    float x = distance * Mathf.Cos(elevationRad) * Mathf.Sin(azimuthRad);
                    float y = distance * Mathf.Sin(elevationRad);
                    float z = distance * Mathf.Cos(elevationRad) * Mathf.Cos(azimuthRad);

                    // Return the position relative to the player's position
                    this.transform.position = player2.transform.position + new Vector3(x, y, z);
                }

                break;
            case approaches.a9:             //this option is valid an working!!!°!
               
                if (p != null && !p.IsFocusLocked) // trying out IsFocusLocked for now
                {
                    RayStep rayStep = p.Rays[0];
                    Vector3 rayOrigin = rayStep.Origin;
                    Vector3 rayDirection = rayStep.Direction;
                    Vector3 positionUnitsAway = rayOrigin + rayDirection * 5.0f;


                    Vector3 obj = positionUnitsAway;
                    float distance = Vector3.Distance(obj, player1.transform.position);

                    // Get a vector from player to object
                    Vector3 dir = (obj - player1.transform.position).normalized;

                    // Calculate azimuth and elevation
                    float azimuth = (Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg);
                    if (azimuth < 0) azimuth += 360; // Ensure azimuth is between 0 and 360

                    float elevation = Mathf.Asin(dir.y) * Mathf.Rad2Deg;

                    // Convert angles from degrees to radians
                    float azimuthRad = azimuth * Mathf.Deg2Rad;
                    float elevationRad = elevation * Mathf.Deg2Rad;

                    // Convert spherical coordinates to Cartesian coordinates
                    float x = distance * Mathf.Cos(elevationRad) * Mathf.Sin(azimuthRad);
                    float y = distance * Mathf.Sin(elevationRad);
                    float z = distance * Mathf.Cos(elevationRad) * Mathf.Cos(azimuthRad);

                    // Return the position relative to the player's position
                    this.transform.position = player2.transform.position + new Vector3(x, y, z);
                }

                break;
            default:
                break;
        }
    }

    public Transform player1;
    public Transform player2;



    void SetupPointer()
    {
        Debug.LogWarning("setup pointer");
    }

    private void OnPrimaryPointerChanged(IMixedRealityPointer oldPointer, IMixedRealityPointer newPointer)
    {
        // this is for only right hand tracking

        if (newPointer != null
            && newPointer.Controller != null
            && newPointer.Controller.ControllerHandedness == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right)
        {
            p = newPointer;
            SetupPointer();
        }
        else
        {
            p = null;
        }

    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.FocusProvider?.UnsubscribeFromPrimaryPointerChanged(OnPrimaryPointerChanged);
        OnPrimaryPointerChanged(null, null);
    }

}
