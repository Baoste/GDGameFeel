using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{
    public GameObject[] arrowPrefab;
    private const float GenerateAxisY = 20f;

    public int[] arrowPrefabIndex;
    private MapMarker mapMarker;

    void Start()
    {
        mapMarker = GetComponent<MapMarker>();
        for (int i = 0; i < arrowPrefabIndex.Length; i++)
        {
            GenerateArrow(transform.position + mapMarker.markerPositions[i], arrowPrefabIndex[i], false);
        }
    }

    private void GenerateArrow(Vector3 pos, int idx = 0, bool initY = true)
    {
        if (initY)
            pos.y = GenerateAxisY;
        else
            pos.y += GenerateAxisY;
        Instantiate(arrowPrefab[idx], pos, Quaternion.Euler(new Vector3(0, 0, -90)));
    }

    public void DestroyArrow(GameObject obj, Vector3 pos, bool initY = true)
    {
        Destroy(obj, 1f);
        if (obj.GetComponent<Arrow>().isOutFloor)
        {
            pos = mapMarker.markerPositions[Random.Range(0, arrowPrefabIndex.Length)];
            StartCoroutine(GenerateAfterDestroy(pos));
        }
        else
            StartCoroutine(GenerateAfterDestroy(pos));
    }

    private IEnumerator GenerateAfterDestroy(Vector3 pos, bool initY = true)
    {
        yield return new WaitForSeconds(1f);
        GenerateArrow(pos, Random.Range(1, 4), initY);
    }
}
