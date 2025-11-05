using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using CesiumForUnity;    // Cesium types (CesiumGeoreference, CesiumEllipsoid)
using Unity.Mathematics; // double3
using TMPro;
using Unity.XR.CoreUtils;
public class CesiumGeoSpawner : MonoBehaviour
{
    public CesiumGeoreference georeference; // assign in Inspector
    [System.Obsolete]
    public XROrigin sessionOrigin;   // optional: for AR alignment
    public GameObject prefab;               // prefab to spawn
    public TMP_InputField latInput, lonInput, altInput; // optional UI

    void Start()
    {
        if (georeference == null) georeference = FindObjectOfType<CesiumGeoreference>();
        if (sessionOrigin == null) sessionOrigin = FindObjectOfType<ARSessionOrigin>();
    }

    public void OnSpawnFromUI()
    {
        if (!double.TryParse(latInput.text, out double lat) ||
            !double.TryParse(lonInput.text, out double lon))
        {
            Debug.LogWarning("Invalid lat/lon");
            return;
        }
        double alt = 0.0;
        double.TryParse(altInput.text, out alt);

        SpawnAt(lon, lat, alt);
    }

    public void SpawnAt(double lon, double lat, double alt)
    {
        if (georeference == null)
        {
            Debug.LogError("No CesiumGeoreference found.");
            return;
        }

        // ensure georeference initialized so matrices are valid
        georeference.Initialize();

        // 1) Convert lon/lat/height -> ECEF (ellipsoid.LongitudeLatitudeHeightToCenteredFixed expects (lon, lat, height))
        double3 ecef = georeference.ellipsoid.LongitudeLatitudeHeightToCenteredFixed(new double3(lon, lat, alt));

        // 2) Convert ECEF -> Cesium Unity local coordinates (double3)
        double3 unityD = georeference.TransformEarthCenteredEarthFixedPositionToUnity(ecef);

        // 3) Cast to float Vector3 for Unity world usage
        Vector3 unityPos = new Vector3((float)unityD.x, (float)unityD.y, (float)unityD.z);

        // 4) If you want ARSessionOrigin mapping (usually not needed if Cesium georeference sits under your ARSessionOrigin),
        Vector3 worldPos = (sessionOrigin != null) ? sessionOrigin.transform.TransformPoint(unityPos) : unityPos;

        // 5) Spawn and optionally add AR anchor
        var anchorGO = new GameObject("CesiumSpawn");
        anchorGO.transform.position = worldPos;
        anchorGO.transform.rotation = Quaternion.identity;

        // Try to add ARAnchor so AR tracking will attempt to keep it stable
        var arAnchor = anchorGO.AddComponent<ARAnchor>(); // requires AR Foundation reference
        Instantiate(prefab, anchorGO.transform);
    }
}
