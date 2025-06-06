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
    private bool isFall;
    private void Start()
    {
        player = GetComponentInParent<Player>();
        isFall = false;
    }

    private void Update()
    {
        Vector3Int cell = tilemap.WorldToCell(player.transform.position);

        if (tilemap.GetTile(cell) == null && !isFall)
        {
            Vector2 footPosL = player.GetComponent<Collider2D>().bounds.min;
            Vector2 footPosR = player.GetComponent<Collider2D>().bounds.max;
            Collider2D groundCheckL = Physics2D.OverlapBox(footPosL, new Vector2(0.8f, 0.1f), 0f, floorLayer);
            Collider2D groundCheckR = Physics2D.OverlapBox(footPosR, new Vector2(-0.8f, -0.1f), 0f, floorLayer);
            if (groundCheckL == null && groundCheckR == null && player.stateMachine.currentState != player.dashingState
                && player.stateMachine.currentState != player.dashInState
                && player.stateMachine.currentState != player.dashOutState)
            {
                player.stateMachine.ChangeState(player.fallState);
                isFall = true;
            }
        }
    }

    //void OnDrawGizmos()
    //{
    //    // ��ȡ�Ų�λ�ã�Collider2D �����½ǣ�
    //    Vector2 footPos = GetComponent<Collider2D>().bounds.min;
    //    Vector2 boxSize = new Vector2(0.8f, 0.1f);

    //    // ���� Gizmo ����ɫΪ��ɫ
    //    Gizmos.color = Color.blue;

    //    // �ڽŲ�λ�û��Ƽ�������WireCube �ǿ�ܶ��������ľ��Σ�
    //    Gizmos.DrawWireCube(footPos + boxSize / 2, boxSize); // ����ƫ������ʹ�����ȷ��ʾ
    //}
}
