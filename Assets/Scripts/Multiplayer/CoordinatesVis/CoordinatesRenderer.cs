using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Positional;
public class CoordinatesRenderer : MonoBehaviour
{
    private PlayerCoordinates playerCord;
    [Tooltip("The transform position to be used for rendering the coordinate system. Assigned Transform will be overriden by playerCoordinate system if active on this object.")]
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

    bool isInitialized;

    // --- ! keep this disabled in the scene and enable it from another script
    public enum CoordinateType
    {
        PlayerCoordinates, MainCameraCoordinates, TrueNorthCoordinates
    }
    public CoordinateType coordinateType = new CoordinateType();
    private void OnEnable()
    {
        switch (coordinateType)
        {
            case CoordinateType.PlayerCoordinates:
                // Handle player coordinates
                coordinate = Player.Instance.gameObject.transform;

                break;
            case CoordinateType.MainCameraCoordinates:
                // Handle main camera coordinates
                coordinate = Player.Instance.mainCamera.transform;
                break;
            case CoordinateType.TrueNorthCoordinates:
                // Handle true north coordinates
                playerCord = GetComponent<PlayerCoordinates>();

                if (playerCord != null)
                {
                    if (playerCord.enabled)
                    {
                        playerCord.SetupOnEnable();
                        coordinate = this.transform;
                        coordinate.position = playerCord.playerCam.position;
                        coordinate.rotation = Player.Instance.Unity2TrueNorth;
                    }// else: playerCoordinates not enabled
                }
                break;
            default:
                // Optional: handle unexpected cases
                break;
        }

        if (!isInitialized)
        {

            // Create LineRenderers
            lineRendererX = CreateLineRenderer(colorX);
            lineRendererY = CreateLineRenderer(colorY);
            lineRendererZ = CreateLineRenderer(colorZ);

            // Create labels
            labelX = CreateLabel(coordinateName + ":X", colorX);
            labelY = CreateLabel(coordinateName + ":Y", colorY);
            labelZ = CreateLabel(coordinateName + ":Z", colorZ);

            isInitialized = true;
        }
        else if (isInitialized)
        {
            lineRendererX.gameObject.SetActive(true);
            lineRendererY.gameObject.SetActive(true);
            lineRendererZ.gameObject.SetActive(true);
            labelX.gameObject.SetActive(true);
            labelY.gameObject.SetActive(true);
            labelZ.gameObject.SetActive(true);
        }


    }
    private void OnDisable()
    {
        lineRendererX.gameObject.SetActive(false);
        lineRendererY.gameObject.SetActive(false);
        lineRendererZ.gameObject.SetActive(false);
        labelX.gameObject.SetActive(false);
        labelY.gameObject.SetActive(false);
        labelZ.gameObject.SetActive(false);
    }

    void Update()
    {
        if (coordinate == null)
        {
            Debug.LogWarning("coordinate transform is null for " + this.gameObject.name);
            return;
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
