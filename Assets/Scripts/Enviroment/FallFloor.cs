using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FallFloor : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Transform upCol;
    public Transform downCol;
    public Transform leftCol;
    public Transform rightCol;

    private Tilemap[] tilemaps;
    private List<Vector3Int> outerRingPositions;
    private BoundsInt bounds;

    private float fallDelTime;
    private float fallTime;

    private float moveDel = 1.6f;

    private void Start()
    {
        fallTime = 10f;

        fallDelTime = 0f;
        outerRingPositions = new List<Vector3Int>();
        tilemaps = GetComponentsInChildren<Tilemap>();
    }

    private void Update()
    {
        fallDelTime += Time.deltaTime;
        if (fallDelTime >= fallTime)
        {
            GetBoundTiles();
            foreach (Vector3Int pos in outerRingPositions)
            {
                foreach (Tilemap tilemap in tilemaps)
                    DestroyTile(1f, tilemap, pos);
            }
            StartCoroutine(MoveColWall(4f));
            fallDelTime = 0f;
        }
    }

    private void GetBoundTiles()
    {
        bounds = floorTilemap.cellBounds;
        outerRingPositions.Clear();

        int x_min = 0;
        int x_max = 0;
        int y_min = 0;
        int y_max = 0;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                if (floorTilemap.GetTile(cellPos) != null)
                {
                    x_min = Mathf.Min(x_min, x);
                    x_max = Mathf.Max(x_max, x);
                    y_min = Mathf.Min(y_min, y);
                    y_max = Mathf.Max(y_max, y);
                }
            }
        }

        for (int x = x_min; x <= x_max; x++)
        {
            outerRingPositions.Add(new Vector3Int(x, y_min, 0));
            outerRingPositions.Add(new Vector3Int(x, y_max, 0));
        }
        for (int y = y_min + 1; y < y_max; y++)
        {
            outerRingPositions.Add(new Vector3Int(x_min, y, 0));
            outerRingPositions.Add(new Vector3Int(x_max, y, 0));
        }
    }

    private void DestroyTile(float t, Tilemap tilemap, Vector3Int pos)
    {

        Matrix4x4 originalMatrix = tilemap.GetTransformMatrix(pos);
        Sequence seq = DOTween.Sequence();

        Vector3 offset = Vector3.zero;
        seq.Join(
            DOTween.Shake(
                getter: () => offset,
                setter: (Vector3 newOffset) =>
                {
                    offset = newOffset;
                    var newMatrix = originalMatrix * Matrix4x4.Translate(offset);
                    tilemap.SetTransformMatrix(pos, newMatrix);
                },
                duration: 3f,
                strength: new Vector3(0.2f, 0.2f, 0f),
                vibrato: 10,
                randomness: 90f,
                fadeOut: true
            )
            .OnComplete(() =>
            {
                float scale = 1f;
                DOTween.To(
                    () => scale,
                    (float newScale) =>
                    {
                        scale = newScale;
                        Matrix4x4 scaledMatrix = originalMatrix * Matrix4x4.Scale(Vector3.one * scale);
                        tilemap.SetTransformMatrix(pos, scaledMatrix);
                    },
                    0f,
                    1f  // duration
                )
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    tilemap.SetTile(pos, null);
                    tilemap.DOKill(true);
                });
            })
        );
    }

    private IEnumerator MoveColWall(float t)
    {
        yield return new WaitForSeconds(t);
        upCol.position += Vector3.down * moveDel;
        downCol.position += Vector3.up * moveDel;
        leftCol.position += Vector3.right * moveDel;
        rightCol.position += Vector3.left * moveDel;
    }
}
