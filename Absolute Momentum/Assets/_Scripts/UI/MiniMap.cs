using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public static MiniMap Instance;
    [SerializeField] GameObject plane;
    [SerializeField] RectTransform scrollViewRectTransform;
    [SerializeField] RectTransform contentRectTransform;
    [SerializeField] MiniMapIcon miniMapIconPrefab;
    [SerializeField] Vector2 worldSize;
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

    private void UpdateMiniMapIcons() {
        // float iconScale = 1 / contentRectTransform.transform.localScale.x;
        // foreach (var kvp in miniMapWorldObjectsLookup)
        // {
        //     var miniMapWorldObject = kvp.Key;
        //     var miniMapIcon = kvp.Value;
        //     var mapPosition = WorldPositionToMapPosition(miniMapWorldObject.transform.position);
        //     miniMapIcon.RectTransform.anchoredPosition = mapPosition;
        //     var rotation = miniMapWorldObject.transform.rotation.eulerAngles;
        //     miniMapIcon.IconRectTransform.localRotation = Quaternion.AngleAxis(-rotation.y, Vector3.forward);
        //     miniMapIcon.IconRectTransform.localScale = Vector3.one * iconScale;
        // }

        foreach (var kvp in miniMapWorldObjectsLookup)
        {
            var miniMapWorldObject = kvp.Key;
            var miniMapIcon = kvp.Value;

            // POSITION
            var mapPosition = WorldPositionToMapPosition(miniMapWorldObject.transform.position);
            miniMapIcon.RectTransform.anchoredPosition = mapPosition;

            // ROTATION DEBUG
            Vector3 forward = miniMapWorldObject.transform.forward;
            float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            Debug.Log($"Object: {miniMapWorldObject.name} | " +
                    $"Forward: {forward} | " +
                    $"Calculated Angle: {angle} | " +
                    $"Transform Rotation: {miniMapWorldObject.transform.eulerAngles.y}");

            miniMapIcon.IconRectTransform.localEulerAngles = new Vector3(0, 0, -angle);
        }
    }

    private Vector2 WorldPositionToMapPosition (Vector3 worldPos) {
        var pos = new Vector2(worldPos.x, worldPos.z);
        return transformationMatrix.MultiplyPoint3x4(pos);
    }

    private void CalculateTransformationMatrix()
    {
        var minimapSize = contentRectTransform.rect.size;
        var worldSize = new Vector2(this.worldSize.x, this.worldSize.y);

        var translation = minimapSize * 0;
        var scaleRatio = minimapSize / worldSize;

        transformationMatrix = Matrix4x4.TRS(translation, Quaternion.identity, scaleRatio);
    }
}
