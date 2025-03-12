using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StainGenerator : MonoBehaviour
{
    public GameObject stainPrefab;
    private int minOrderInLayer;
    private int maxOrderInLayer;

    private List<GameObject> stainPool;
    private List<GameObject> tempStains;

    private void Start()
    {
        minOrderInLayer = 1;
        maxOrderInLayer = 9;
        stainPool = new List<GameObject>();
        tempStains = new List<GameObject>();
    }

    /// <summary>
    /// 生成污渍
    /// </summary>
    /// <param name="color"></param>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    /// <param name="scale"></param>
    /// <param name="radius"></param>
    public void Generate(Color color, Vector3 position, Vector3 direction, float scale, float radius)
    {
        // 以position为中心分裂到若干个方向，每个分裂的角度随机
        int splitNum = Random.Range(6, 9);  // 分裂数量随机，数值暂时写死
        Vector3[] splitDirs = new Vector3[splitNum];
        float angleDelta = 360f / splitNum;
        for (int i = 0; i < splitDirs.Length; i++)
        {
            var lastDir = i == 0 ? direction : splitDirs[i - 1];
            var angle = RandomNum(angleDelta, .2f);
            splitDirs[i] = Quaternion.AngleAxis(angle, Vector3.forward) * lastDir;
        }
        // 每个分裂方向生成若干个污渍
        tempStains.Clear();
        foreach (var dir in splitDirs)
        {
            int stainNum = Random.Range(3, 6);
            float stainScale;    // 污渍
            float radiusDelta = radius / 6f;    // 每个污渍间距
            Vector3 stainPos = position;    // 污渍位置
            for (int i = 0; i < stainNum; i++)
            {
                stainScale = scale - (i * scale / stainNum);    // 缩放随距离衰减
                stainPos += dir * RandomNum(radiusDelta, .4f);
                stainPos += (Vector3)Random.insideUnitCircle * RandomNum(radiusDelta * .2f, radiusDelta * .1f); // 位置随机
                var go = GetStain(stainPos, stainScale, dir);
                go.transform.position = stainPos;
                go.transform.right = dir;
                go.transform.localScale = new Vector3(stainScale, stainScale, 1f);
                go.transform.parent = transform;
                go.GetComponent<SpriteRenderer>().color = color;
            }
        }
    }

    /// <summary>
    /// 获取当前污渍对象，若当前位置发生重叠则调整回收
    /// </summary>
    /// <param name="pos">位置</param>
    /// <param name="scale">缩放</param>
    /// <param name="dir">朝向</param>
    /// <returns></returns>
    GameObject GetStain(Vector3 pos, float scale, Vector3 dir)
    {
        int order = minOrderInLayer;   // 当前污渍需要设置的sortingOrder
        var angle = Vector2.SignedAngle(Vector2.right, dir);
        var size = Vector3.one * scale;   // 实际大小
        var cols = Physics2D.OverlapBoxAll(pos, size, angle, LayerMask.GetMask("Stain"));
        if (cols.Length != 0)
        {
            // 若检测到污渍重叠，获取当前最顶层污渍的sortingOrder
            SpriteRenderer spriteRenderer;
            foreach (var item in cols)
            {
                spriteRenderer = item.GetComponent<SpriteRenderer>();
                if (spriteRenderer.sortingOrder > order)
                {
                    order = spriteRenderer.sortingOrder;
                    // 如果即将超出最大叠加层数则直接快进到处理重叠的污渍
                    if (order + 1 > maxOrderInLayer)
                        break;
                }
            }
            if (order + 1 > maxOrderInLayer)
            {
                // 回收最底层的污渍，其余层sortingOrder减1，并标记为已处理，避免被重复处理
                foreach (var item in cols)
                {
                    spriteRenderer = item.GetComponent<SpriteRenderer>();
                    if (spriteRenderer.sortingOrder == minOrderInLayer)
                        item.gameObject.SetActive(false);
                    else if (!tempStains.Contains(item.gameObject))
                    {
                        spriteRenderer.sortingOrder--;
                        tempStains.Add(item.gameObject);
                    }
                }
            }
            order = Mathf.Clamp(order + 1, minOrderInLayer, maxOrderInLayer);
        }

        // 简易对象池
        GameObject go = null;
        foreach (var item in stainPool)
        {
            if (!item.activeInHierarchy)
            {
                go = item;
                go.SetActive(true);
                break;
            }
        }
        if (go == null)
        {
            go = Instantiate(stainPrefab, transform);
            stainPool.Add(go);
        }
        go.GetComponent<SpriteRenderer>().sortingOrder = order;

        return go;
    }

    private float RandomNum(float num, float randomness)
    {
        return num + Random.Range(-num * randomness, num * randomness);
    }
}
