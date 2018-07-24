using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//立射佣
public class ShootChess : Chess
{
    // Use this for initialization
    void Start ()
    {
        action_manager = gameObject.AddComponent<RoleActionManager>();
        mapController = Singleton<MapController>.Instance;
        isAttack = false;
        isMoving = false;

        chessType = ChessType.Shoot;

        blood = 100;
        forCarChessHurt = 10;
        forShootChessHurt = 10;
        forInfantryChessHurt = 10;
        normalAttackHurt = 10;

        attackRange = new List<Vector2Int>() {
        new Vector2Int(-1,1), new Vector2Int(0,1), new Vector2Int(1,1),
        new Vector2Int(-1,0),  new Vector2Int(1,0),
        new Vector2Int(-1,-1),new Vector2Int(0,-1), new Vector2Int(1,-1)};
    }
}
