using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class MapMarker : MonoBehaviour
{
    public Color gizmoColor = Color.red;
    public float radius = 0.3f;
    public Vector3[] markerPositions;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        for (int i = 0; i < markerPositions.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(markerPositions[i]);
            Gizmos.DrawSphere(worldPos, radius);
#if UNITY_EDITOR
            Handles.Label(worldPos + Vector3.up * 0.3f, $"Point {i}");
#endif
        }
    }
}
