using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{
    public GameObject arrowPrefab;
    private const float GenerateAxisY = 20;

    void Start()
    {
        Vector3 pos = new Vector3(-10, GenerateAxisY, 0);
        GenerateArrow(pos);
        pos.x = 10;
        GenerateArrow(pos);
        pos.x = 0;
        GenerateArrow(pos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateArrow(Vector3 pos)
    {
        pos.y = GenerateAxisY;
        Instantiate(arrowPrefab, pos, Quaternion.Euler(new Vector3(0, 0, -90)));
    }

    public void DestroyArrow(GameObject obj, Vector3 pos)
    {
        Destroy(obj);
        StartCoroutine(GenerateAfterDestroy(pos));
    }

    private IEnumerator GenerateAfterDestroy(Vector3 pos)
    {
        yield return new WaitForSeconds(1f);
        GenerateArrow(pos);
    }
}
