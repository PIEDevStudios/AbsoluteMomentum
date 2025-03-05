using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public static MiniMap Instance;
    [SerializeField] Plane terrain;
    [SerializeField] RectTransform scrollViewRectTransform;
    [SerializeField] RectTransform contentRectTransform;
    [SerializeField] MiniMapIcon miniMapIconPrefab;

    Matrix4x4 transformationMatrix;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CalculateTransformationMatrix();
    }
    Dictionary<MiniMapWorldObject, MiniMapIcon> miniMapWorldObjectsLookup = new Dictionary<MiniMapWorldObject, MiniMapIcon>();
    public void RegisterMiniMapWorldObject(MiniMapWorldObject miniMapWorldObject) {
        var miniMapIcon = Instantiate(miniMapIconPrefab);
        miniMapIcon.transform.SetParent(contentRectTransform);
        miniMapIcon.SetIcon(miniMapWorldObject.Icon);
        miniMapIcon.setColor(miniMapWorldObject.IconColor);
        miniMapIcon.setText(miniMapWorldObject.Text);
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
            
            miniMapIcon.RectTransform.anchoredPosition = mapPosition;
            var rotation = miniMapWorldObject.transform.rotation.eulerAngles;
            miniMapIcon.IconRectTransform.localRotation = Quaternion.AngleAxis(-rotation.y, Vector3.forward);
        }
    }

    Vector2 WorldPositionTomapPosition (Vector3 worldPos) {
        var pos = new Vector2(worldPos.x, worldPos.z);
        return transformationMatrix.MultiplyPoint3x4(pos);
    }

    void CalculateTransformationMatrix() {
        var miniMapDimensions = contentRectTransform.rect.size;
        var terrainDimensions = new Vector2(terrain.terrainData.size.x, terrain.terrainData.size.z);

        var scaleRatio = miniMapDimensions / terrainDimensions;

        var translation = -miniMapDimensions;

        transformationMatrix = Matrix4x4.TRS(translation, Quaternion.identity, scaleRatio);
    }
}
