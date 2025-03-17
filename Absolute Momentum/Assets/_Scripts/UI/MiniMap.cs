using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public static MiniMap Instance;
    [SerializeField] GameObject plane;
    [SerializeField] RectTransform scrollViewRectTransform;
    [SerializeField] RectTransform contentRectTransform;
    [SerializeField] MiniMapIcon miniMapIconPrefab;
    Dictionary<MiniMapWorldObject, MiniMapIcon> miniMapWorldObjectsLookup = new Dictionary<MiniMapWorldObject, MiniMapIcon>();

    Matrix4x4 transformationMatrix;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CalculateTransformationMatrix();
    }
    
    public void RegisterMiniMapWorldObject(MiniMapWorldObject miniMapWorldObject) {
        var miniMapIcon = Instantiate(miniMapIconPrefab);
        miniMapIcon.transform.SetParent(contentRectTransform);
        miniMapIcon.SetIcon(miniMapWorldObject.Icon);
        miniMapIcon.SetColor(miniMapWorldObject.IconColor);
        miniMapIcon.SetText(miniMapWorldObject.Text);
        miniMapIcon.SetTextSize(miniMapWorldObject.TextSize);
        miniMapWorldObjectsLookup[miniMapWorldObject] = miniMapIcon;
    }

    private void Update()
    {
        UpdateMiniMapIcons();
    }

    void UpdateMiniMapIcons() {
        foreach (var kvp in miniMapWorldObjectsLookup) {
            var miniMapWorldObject = kvp.Key;
            var miniMapIcon = kvp.Value;

            var mapPosition = WorldPositionTomapPosition(miniMapWorldObject.transform.position);
            Debug.Log("Map Position " + mapPosition);
            miniMapIcon.RectTransform.anchoredPosition = mapPosition;
            var rotation = miniMapWorldObject.transform.rotation.eulerAngles;
            miniMapIcon.IconRectTransform.localRotation = Quaternion.AngleAxis(-rotation.y, Vector3.forward);
        }
    }

    Vector2 WorldPositionTomapPosition (Vector3 worldPos) {
        var pos = new Vector2(worldPos.x, worldPos.z);
        return transformationMatrix.MultiplyPoint3x4(pos);
    }

    void CalculateTransformationMatrix()
    {
        // Get the size of the minimap UI
        var miniMapDimensions = contentRectTransform.rect.size;

        // Get the size of the Plane (default Unity Plane is 10x10 units)
        float baseSizeX = 10f;
        float baseSizeZ = 10f;

        // Calculate the scaled dimensions of the Plane
        float scaledSizeX = baseSizeX * plane.transform.lossyScale.x;
        float scaledSizeZ = baseSizeZ * plane.transform.lossyScale.z;

        Vector2 planeDimensions = new Vector2(scaledSizeX, scaledSizeZ);

        // Calculate the scale ratio between the minimap and the plane
        var scaleRatio = new Vector2(
            miniMapDimensions.x / planeDimensions.x,
            miniMapDimensions.y / planeDimensions.y
        );

        // No translation needed since the origin (0,0,0) is the center of the plane
        var translation = Vector2.zero;

        // Create the transformation matrix
        transformationMatrix = Matrix4x4.TRS(translation, Quaternion.identity, scaleRatio);
    }
}
