using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//步兵佣
public class InfantryChess : Chess
{
    // Use this for initialization
    void Start ()
    {
        action_manager = gameObject.AddComponent<RoleActionManager>();
        mapController = Singleton<MapController>.Instance;
        isAttack = false;
        isMoving = false;

        chessType = ChessType.Infantry;

        blood = 100;
        forCarChessHurt = 100;
        forShootChessHurt = 20;
        forInfantryChessHurt = 10;
        normalAttackHurt = 10;

        attackRange = new List<Vector2Int>(){
        new Vector2Int(0,1),
        new Vector2Int(-1,0),  new Vector2Int(1,0),
        new Vector2Int(0,-1)};
    }
}
