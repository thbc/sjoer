using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class drawLine : NetworkBehaviour
{

    private LineRenderer lr;

    // private Vector3 forwrdVec = new Vector3(0,0,0);

    [SerializeField] private Button rotateRightBtn;
    [SerializeField] private Button rotateLeftBtn;
    [SerializeField] private Button rotateUpBtn;
    [SerializeField] private Button rotateDownBtn;
    [SerializeField] private Button moveForwardBtn;
    [SerializeField] private Button moveBackwardBtn;
    [SerializeField] private Button moveLeftBtn;
    [SerializeField] private Button moveRightBtn;
    [SerializeField] private Slider yAxisSlider;
    [SerializeField] private Slider xAxisSlider;

    float lineLength = 30f;
    void Start()
    {

        transform.position = new Vector3(0, 0, 0);

        lr = GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));

        // Set some positions
        /*    Vector3[] positions = new Vector3[3];
           positions[0] = new Vector3(-2.0f, -2.0f, 0.0f);
           positions[1] = new Vector3(0.0f, 2.0f, 0.0f);
           positions[2] = new Vector3(2.0f, -2.0f, 0.0f);
           lr.positionCount = positions.Length;
           lr.SetPositions(positions); */

        // Set to face its Transform Component
        //  lr.alignment = LineAlignment.TransformZ;


        /* Color c1 = Color.white;
        Color c2 = new Color(1, 1, 1, 0);

        lr.SetColors(c1,c2); */
        if (IsHost)
            lr.endColor = Color.green;
        if (IsClient)
            lr.endColor = Color.red;

        Debug.Log("checking if is local player ? " + IsLocalPlayer);

        if (!IsLocalPlayer) return;

       /*  rotateRightBtn = GameObject.Find("rotateRightBtn").GetComponent<Button>();

        rotateRightBtn.onClick.AddListener(() =>
        {
            Debug.Log("rotate right");
            transform.Rotate(0, 10, 0);
        });

        rotateLeftBtn = GameObject.Find("rotateLeftBtn").GetComponent<Button>();

        rotateLeftBtn.onClick.AddListener(() =>
        {
            Debug.Log("rotate left");

            transform.Rotate(0, -10, 0);
        });

        rotateUpBtn = GameObject.Find("rotateUpBtn").GetComponent<Button>();

        rotateUpBtn.onClick.AddListener(() =>
        {
            Debug.Log("rotate up");
            transform.Rotate(10, 0, 0);
        });

        rotateDownBtn = GameObject.Find("rotateDownBtn").GetComponent<Button>();

        rotateDownBtn.onClick.AddListener(() =>
        {
            Debug.Log("rotate down");

            transform.Rotate(-10, 0, 0);
        });



        moveForwardBtn = GameObject.Find("moveForwardBtn").GetComponent<Button>();

        moveForwardBtn.onClick.AddListener(() =>
        {

            transform.Translate(0, 0, 1);
        });

        moveBackwardBtn = GameObject.Find("moveBackwardBtn").GetComponent<Button>();

        moveBackwardBtn.onClick.AddListener(() =>
        {

            transform.Translate(0, 0, -1);
        });

        moveLeftBtn = GameObject.Find("moveLeftBtn").GetComponent<Button>();

        moveLeftBtn.onClick.AddListener(() =>
        {

            transform.Translate(-1, 0, 0);
        });

        moveRightBtn = GameObject.Find("moveRightBtn").GetComponent<Button>();

        moveRightBtn.onClick.AddListener(() =>
        {

            transform.Translate(1, 0, 0);
        }); */



        /*          yAxisSlider = GameObject.Find("yAxisSlider").GetComponent<Slider>();

                    yAxisSlider.onValueChanged.AddListener((val) =>
                    {
                        if(!overwriteForwardVect)
                        overwriteForwardVect = true;

                        actualForward.y = val;

                    });
                 xAxisSlider = GameObject.Find("xAxisSlider").GetComponent<Slider>();

                    xAxisSlider.onValueChanged.AddListener((val) =>
                    {
                        if(!overwriteForwardVect)
                        overwriteForwardVect = true;

                        actualForward.x = val;

                    });
         */
    }

    public bool overwriteForwardVect;
    public Vector3 actualForward = new Vector3();

    // Update is called once per frame
    void Update()
    {



        //    if (!IsOwner) return;
        var positions = new Vector3[2];
        positions[0] = this.transform.position;
        //var startPoint = this.transform.position;
        if (!overwriteForwardVect)
        {
            var forward = this.transform.forward;//Camera.main.transform.forward;
            actualForward = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
        }

        //var endpoint = startPoint + actualForward * lineLength;
        positions[1] = positions[0] + actualForward * lineLength;
        lr.positionCount = 2; //positions.Length;
        lr.SetPositions(positions);



    }

    void dbgMesh()
    {

    }

}
