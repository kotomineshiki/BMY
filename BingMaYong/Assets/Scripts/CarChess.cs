using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//车佣
public class CarChess : Chess
{
    // Use this for initialization
    void Start ()
    {
        action_manager = gameObject.AddComponent<RoleActionManager>();
        mapController = Singleton<MapController>.Instance;
        isAttack = false;
        isMoving = false;

        chessType = ChessType.Car;

        blood = 100;
        forCarChessHurt = 10;
        forShootChessHurt = 100;
        forInfantryChessHurt = 100;
        normalAttackHurt = 10;

        attackRange = new List<Vector2Int>() {new Vector2Int(0,1)};

        direction = Direction.North;
    }

    /*
     * 车寻找下一个寻路的位置
     * 传入当前位置和目的地
     * 返回下一个到达的位置
     */ 
    public override Vector2Int GetNextStep(Vector2Int currentPos, Vector2Int destination)
    {//车兵特殊的寻路函数，需要了解其当前朝向才能寻路
        if (currentPos == destination)
            return currentPos;
        List<Vector2Int> range;
        //根据方向不同放入List中的顺序不同(从而达到同方向优先目的)
        if(direction == Direction.North)
        {
            range = new List<Vector2Int>(){
            new Vector2Int(0,1),
            new Vector2Int(-1,0),  new Vector2Int(1,0),
            new Vector2Int(0,-1)};
        }
        else if(direction == Direction.East)
        {
            //对于车来说是东，但正常方向应该是西才对
            range = new List<Vector2Int>(){
            new Vector2Int(-1,0),  new Vector2Int(0,1), new Vector2Int(0,-1),new Vector2Int(1,0)
            };
        }
        else if (direction == Direction.West)
        {
            //对于车来说是西，但正常方向应该是东才对
            range = new List<Vector2Int>(){
            new Vector2Int(1,0), new Vector2Int(0,1),new Vector2Int(0,-1),new Vector2Int(-1,0)
            };
        }
        else
        {
            range = new List<Vector2Int>(){
           new Vector2Int(0,-1), new Vector2Int(-1,0), new Vector2Int(1,0) , new Vector2Int(0,1)};
        }
        //排除了不可到达和超出范围的格子
        List<Vector2Int> mayMoveTo = new List<Vector2Int>();
        foreach (Vector2Int vec in range)
        {
            Vector2Int tempPos = currentPos + vec;
            if (tempPos.x >= 0 && tempPos.y >= 0 && tempPos.x <= 9 && tempPos.y <= 13 &&
                MapController.instance.tiles[tempPos.x, tempPos.y].tileState != TileState.Occupied && MapController.instance.tiles[tempPos.x, tempPos.y].tileState != TileState.Obstacle)
            {
                mayMoveTo.Add(tempPos);
            }
        }
        //找寻最佳到达的位置
        Vector2Int temp = destination - currentPos;
        foreach (Vector2Int vec in mayMoveTo)
        {
            if(temp.x == 0 && temp.y > 0 && vec == currentPos + new Vector2Int(0, 1))
            {
                return vec;
            }
            else if(temp.x == 0 && temp.y < 0 && vec == currentPos + new Vector2Int(0, -1))
            {
                return vec;
            }
            else if(temp.x > 0 && temp.y == 0 && vec == currentPos + new Vector2Int(1, 0))
            {
                return vec;
            }
            else if(temp.x < 0 && temp.y == 0 && vec == currentPos + new Vector2Int(-1, 0))
            {
                return vec;
            }
            else if(temp.x>0 && temp.y>0 && (vec == currentPos + new Vector2Int(0,1) || vec == currentPos + new Vector2Int(1, 0)))
            {
                return vec;
            }
            else if(temp.x > 0 && temp.y < 0 && (vec == currentPos + new Vector2Int(1, 0) || vec == currentPos + new Vector2Int(0, -1)))
            {
                return vec;
            }
            else if(temp.x < 0 && temp.y < 0 && (vec == currentPos + new Vector2Int(0, -1) || vec == currentPos + new Vector2Int(-1, 0)))
            {
                return vec;
            }
            else if(temp.x < 0 && temp.y > 0 && (vec == currentPos + new Vector2Int(-1, 0) || vec == currentPos + new Vector2Int(0, 1)))
            {
                return vec;
            }
        }
        if (mayMoveTo.Count == 0)
            return currentPos;
        else
            return mayMoveTo[0];
    }

}
