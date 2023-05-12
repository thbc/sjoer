using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
 using Microsoft.MixedReality.Toolkit;
public class MultiHandTracker : MonoBehaviour  , IMixedRealityPointerHandler  
{
    [SerializeField]
    public FocusProvider fp;

private void Start()
{
}
    private void Update()
    {
       
    }
    
    /* public GameObject clientPointer;
    public GameObject pointerPrefab;
    public ShellHandRayPointer client_shrp;
    public ShellHandRayPointer shrp;


    private Vector3 posHandPointer;
    private Quaternion rotHandPointer;

    public TextMesh textM;
    void Start()
    {       
       

    }
    void Update()
    {
        
        if(shrp == null)
        {
            shrp = FindObjectOfType<ShellHandRayPointer>();
        } 
        
        else if(shrp != null)
        {
            if(clientPointer == null)
            {
                clientPointer = Instantiate(pointerPrefab, this.transform);
            }  
            
            posHandPointer = shrp.transform.position;
            rotHandPointer = shrp.transform.rotation;

            Debug.Log(posHandPointer);
            Debug.Log(rotHandPointer);

            textM.text = posHandPointer.ToString() + "\n" + rotHandPointer.ToString();

            clientPointer.gameObject.transform.position = posHandPointer; 
            clientPointer.gameObject.transform.rotation= rotHandPointer; 

        }
    } */
/* private IMixedRealityPointer currentPointer;
    private FocusDetails focusDetails;

    public MixedRealityToolkit mrtk; */

    
   /*  void Update()
    {
        FocusProvider.GetPointers
        
         if (FocusProvider.TryGetFocusDetails(out focusDetails))
        {
            Vector3 focusPosition = focusDetails.Point;
            // Do something with the focus position
        } 
    } */


   /*  ShellHandRayPointer p;
    private IMixedRealityPointer pointer;

    public FocusProvider fp; */
//MixedRealityToolkit.InputSystem.DetectedInputSources
    /* void Start()
    { */
       /*  p = FindObjectOfType<ShellHandRayPointer>();
        // Get the pointer associated with this game object
        pointer = GetComponent<IMixedRealityPointer>(); */
   /*  } */

/*     void Update()
    { */
        
        /* // Log the position of the pointer every frame
        Debug.Log("Pointer Position: " + pointer?.Position);
        Debug.Log(p.gameObject.name); */
   /*  } */

     // Implement the IMixedRealityPointerHandler interface methods
     public void OnPointerDown(MixedRealityPointerEventData eventData) {
     }
    public void OnPointerUp(MixedRealityPointerEventData eventData) {}
    public void OnPointerDragged(MixedRealityPointerEventData eventData) {}
    public void OnPointerClicked(MixedRealityPointerEventData eventData) {}  


}

   
    
