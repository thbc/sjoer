using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Positional;
public class CoordinatesRenderer : MonoBehaviour
{
    public bool trueNorthCoordinates;
    public bool usePlayerMainCam;
    public Player player;
    float trueNC_initialY;
    public Transform playerCam;

    public Transform coordinate;  // Assign your playspace transform here
    public Material lineMaterial;
    private LineRenderer lineRendererX;
    private LineRenderer lineRendererY;
    private LineRenderer lineRendererZ;
    public string coordinateName;
    public GameObject LabelPrefab;
    private TextMesh labelX;
    private TextMesh labelY;
    private TextMesh labelZ;
    public float rayLength = 5f;

    public Color colorX;
    public Color colorY;
    public Color colorZ;

    public float lineWidth = 0.1f;

    void Start()
    {
            if (trueNorthCoordinates)
            {
                if (usePlayerMainCam)
                    playerCam = player.mainCamera.transform;

                coordinate = this.transform;
                coordinate.position = playerCam.position;
                coordinate.rotation = player.Unity2TrueNorth;
                trueNC_initialY = coordinate.position.y;
            }
        
        // Create LineRenderers
        lineRendererX = CreateLineRenderer(colorX);
        lineRendererY = CreateLineRenderer(colorY);
        lineRendererZ = CreateLineRenderer(colorZ);

        // Create labels
        labelX = CreateLabel(coordinateName+":X", colorX);
        labelY = CreateLabel(coordinateName + ":Y", colorY);
        labelZ = CreateLabel(coordinateName + ":Z", colorZ);

        
    }

    void Update()
    {
        if (trueNorthCoordinates)
        {
            coordinate.position = new Vector3(playerCam.position.x, trueNC_initialY, playerCam.position.z);
            coordinate.rotation = player.Unity2TrueNorth;
        }

        // Calculate rays for each axis
        Vector3 origin = coordinate.position;

        // Set LineRenderer positions for each axis
        SetLineRendererPositions(lineRendererX, origin, coordinate.right);
        SetLineRendererPositions(lineRendererY, origin, coordinate.up);
        SetLineRendererPositions(lineRendererZ, origin, coordinate.forward);

        // Set label positions
        SetLabelPosition(labelX, origin + coordinate.right * rayLength);
        SetLabelPosition(labelY, origin + coordinate.up * rayLength);
        SetLabelPosition(labelZ, origin + coordinate.forward * rayLength);

       


    }

    // Helper method to create a LineRenderer with a specified color
    private LineRenderer CreateLineRenderer(Color color)
    {
        var lineRenderer = new GameObject().AddComponent<LineRenderer>();
        lineRenderer.transform.SetParent(transform);
        lineRenderer.positionCount = 2;
        lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, 0.01f), new Keyframe(1, 0.01f));
        lineRenderer.material = lineMaterial;
        lineRenderer.material.color = color;
        return lineRenderer;
    }

    // Helper method to set the positions of a LineRenderer
    private void SetLineRendererPositions(LineRenderer lineRenderer, Vector3 origin, Vector3 direction)
    {
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, origin + direction * rayLength);
        // Set the width of the LineRenderer
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

    }
        // Helper method to create a label with a specified text and color
        // Helper method to create a label with a specified text and color
        private TextMesh CreateLabel(string text, Color color)
    {
        GameObject textObj = Instantiate(LabelPrefab);
        textObj.name = text + "Label";
        textObj.transform.SetParent(transform);

        TextMesh textMesh = textObj.GetComponent<TextMesh>();
        textMesh.text = text;
        textMesh.color = color;

        textObj.SetActive(true);    
     
        return textMesh;
    }


    // Helper method to set the position of a label
    private void SetLabelPosition(TextMesh label, Vector3 position)
    {
        label.transform.position = position;
    }
}
