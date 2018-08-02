using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//步兵佣
public class InfantryChess : Chess
{

    // Use this for initialization
    void Awake()
    {
        action_manager = gameObject.AddComponent<RoleActionManager>();
        mapController = Singleton<MapController>.Instance;
        willAttack = false;
        isMoving = false;

        chessType = ChessType.Infantry;

        blood = 100;


        attackRange = new List<Vector2Int>(){
        new Vector2Int(0,1),
        new Vector2Int(-1,0),  new Vector2Int(1,0),
        new Vector2Int(0,-1)};

        direction = Direction.North;
    }
}
