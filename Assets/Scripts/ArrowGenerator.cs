using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{
    public GameObject[] arrowPrefab;
    private const float GenerateAxisY = 20f;

    public int[] arrowPrefabIndex;
    private MapMarker mapMarker;

    private List<int> randomGenerateOrder = new List<int>();
    private Queue<int> posQueue = new Queue<int>();
    public int selfAddOrderIndex
    {
        get
        {
            int tmp = _generateOrderIndex;
            if (++_generateOrderIndex >= randomGenerateOrder.Count)
            {
                ShuffleList(randomGenerateOrder);
                _generateOrderIndex = 0;
            }
            return tmp;
        }
        set
        {
            _generateOrderIndex = value;
        }
    }
    private int _generateOrderIndex = 0;

    void Start()
    {
        mapMarker = GetComponent<MapMarker>();
        for (int i = 0; i < arrowPrefabIndex.Length; i++)
        {
            GenerateArrow(transform.position + mapMarker.markerPositions[i], arrowPrefabIndex[i], false);
        }

        for (int i = 1; i < arrowPrefab.Length; i++)
        {
            randomGenerateOrder.Add(i);
        }
        ShuffleList(randomGenerateOrder);
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
            pos = GetAvailablePos();
            StartCoroutine(GenerateAfterDestroy(pos));
        }
        else
            StartCoroutine(GenerateAfterDestroy(pos));
    }

    private IEnumerator GenerateAfterDestroy(Vector3 pos, bool initY = true)
    {
        yield return new WaitForSeconds(1f);
        GenerateArrow(pos, randomGenerateOrder[selfAddOrderIndex], false);
        posQueue.Dequeue();
    }

    private Vector3 GetAvailablePos()
    {
        for (int i = 0; i < mapMarker.markerPositions.Length; i++)
        {
            Vector3 pos = transform.position + mapMarker.markerPositions[i];
            Collider2D isAvailable = Physics2D.OverlapCircle(pos, .25f, LayerMask.GetMask("Arrow"));
            if (isAvailable == null && (posQueue.Count == 0 || !posQueue.Contains(i)))
            {
                posQueue.Enqueue(i);
                return pos;
            }
        }
        return transform.position + Vector3.zero;
    }

    private void ShuffleList<T>(IList<T> list)
    {
        int count = list.Count;
        for (int i = 0; i < count - 1; i++)
        {
            int randIndex = Random.Range(i, count);
            T temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }
}
