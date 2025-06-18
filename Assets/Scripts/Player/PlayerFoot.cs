using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerFoot : MonoBehaviour
{
    public Tilemap tilemap;
    public LayerMask floorLayer;
    private Player player;
    private Collider2D trapFloorCollider;
    public bool isFall;
    public float fallTime;
    private void Start()
    {
        player = GetComponentInParent<Player>();
        isFall = false;
        fallTime = 0.0f;
    }

    private void Update()
    {
        Vector3Int cell = tilemap.WorldToCell(transform.position);
        if (tilemap.GetTile(cell) == null && !isFall)
        {
            fallTime += Time.deltaTime;
            if (fallTime >= 0.3f)
            {
                fallTime = 0.0f;
                Vector2 footPosL = player.GetComponent<Collider2D>().bounds.min;
                //Vector2 footPosR = player.GetComponent<Collider2D>().bounds.max;
                Collider2D groundCheckL = Physics2D.OverlapBox(footPosL, new Vector2(0.8f, 0.1f), 0f, floorLayer);
                //Collider2D groundCheckR = Physics2D.OverlapBox(footPosR, new Vector2(-0.8f, -0.1f), 0f, floorLayer);
                if (groundCheckL == null && player.stateMachine.currentState != player.dashingState
                    && player.stateMachine.currentState != player.dashInState
                    && player.stateMachine.currentState != player.dashOutState)
                {
                    player.stateMachine.ChangeState(player.fallState);
                    isFall = true;
                }
            }
        }
    }

    //void OnDrawGizmos()
    //{
    //    // 获取脚部位置（Collider2D 的左下角）
    //    Vector2 footPos = GetComponent<Collider2D>().bounds.min;
    //    Vector2 boxSize = new Vector2(0.8f, 0.8f);

    //    // 设置 Gizmo 的颜色为绿色
    //    Gizmos.color = Color.blue;

    //    // 在脚部位置绘制检测区域框（WireCube 是框架而不是填充的矩形）
    //    Gizmos.DrawWireCube(footPos + boxSize / 2, boxSize); // 算上偏移量，使框架正确显示
    //}
}
