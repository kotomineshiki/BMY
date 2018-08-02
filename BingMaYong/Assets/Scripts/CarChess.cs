using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//车佣
public class CarChess : Chess
{

    public float runningAccumulate=0;//冲锋加成 用来标识自从上次走之后连续走了多少格，转向和暂停都会清空这个值

    //public Vector2Int currentDirection=new Vector2Int(0,1);//当前朝向

    // Use this for initialization
    void Awake()
    {
        action_manager = gameObject.AddComponent<RoleActionManager>();
        mapController = Singleton<MapController>.Instance;
        willAttack = false;
        isMoving = false;

        chessType = ChessType.Car;

        blood = 100;

        forInfantryChessHurt = 100;
        normalAttackHurt = 10;

        attackRange = new List<Vector2Int>() {new Vector2Int(0,1)};

        direction = Direction.North;
    }
    public override Vector2Int GetAttackTargetLocations(Vector2Int victimPos)
    {
        Vector2Int delta = victimPos - currentPosition;
        Vector2Int toward = GetNextStep(currentPosition, victimPos)-currentPosition;
        Debug.Log(delta+"Toward" + toward);
        if (toward.x == 0&&delta.x==0)//此时要么在上要么在下
        {
            if (delta.y > 0) return new Vector2Int(0, -1) + victimPos;
            if (delta.y <0 ) return new Vector2Int(0, 1) + victimPos;
        }
        if (toward.y == 0&&delta.y==0)//此时要么在左要么在右
        {
            if (delta.x > 0) return new Vector2Int(0, -1) + victimPos;
            if (delta.x < 0) return new Vector2Int(0, 1) + victimPos;
        }
        if (toward.x == 0 && delta.x != 0)//此时要么在上要么在下
        {
            if (delta.x > 0) return new Vector2Int(-1, 0) + victimPos;
            if (delta.x < 0) return new Vector2Int(1, 0) + victimPos;
        }
        if (toward.y == 0 && delta.y != 0)//此时要么在左要么在右
        {
            if (delta.y > 0) return new Vector2Int(0, -1) + victimPos;
            if (delta.y < 0) return new Vector2Int(0, 1) + victimPos;
        }

        Debug.Log("有妖孽");
        return new Vector2Int(-99, -99);
    }
    /*
     * 车寻找下一个寻路的位置
     * 传入当前位置和目的地
     * 返回下一个到达的位置
     */
    public override Vector2Int GetNextStep(Vector2Int currentPos, Vector2Int destination)//全新版本:寻路只能走直线
    {
        Vector2Int nextPosition = new Vector2Int(0,0);
        Vector2Int delta = destination - currentPos;//当前距离
        if (delta.x > 0) nextPosition.x = 1;
        if (delta.y > 0) nextPosition.y = 1;
        if (delta.x < 0) nextPosition.x = -1;
        if (delta.y < 0) nextPosition.y = -1;
        if (nextPosition.magnitude > 1)
        {


            /*   if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                   nextPosition.y = 0;//此处随意了
               else
                   nextPosition.x = 0;*/
            if (delta.x != 0) nextPosition.y = 0;
        }
        
        //如果下一个方向等价于当前方向
        if(nextPosition==new Vector2Int(0, 1) && direction == Direction.North)
        {
     //       Debug.Log("y");
            runningAccumulate++;
        }else
        if (nextPosition == new Vector2Int(0, -1) && direction == Direction.South)
        {
    //        Debug.Log("y");
            runningAccumulate++;
        }else
        if (nextPosition == new Vector2Int(1, 0) && direction == Direction.East)
        {
    //        Debug.Log("y");
            runningAccumulate++;
        }else
        if (nextPosition == new Vector2Int(-1, 0) && direction == Direction.West)
        {
     //       Debug.Log("y");
            runningAccumulate++;
        }
        else
        {
     //       Debug.Log("C");
            runningAccumulate = 0;//否则清零
        }

        Debug.Log(nextPosition + currentPos);
        return nextPosition+currentPos;
    }
    /*
    public override void AutoAttacks()
    {
        if (!isMoving && !willAttack && chessType != ChessType.Castle)
        {
            float MinBlood = 9999;
            //检测周围是否有棋子
            GameObject victim = null;

            List<Vector2Int> chioceRange = attackRange;
            List<Vector2Int> tempList = new List<Vector2Int>();
            //对战车进行特殊处理,攻击目标只能是方向的正前方
            if (chessType == ChessType.Car)
            {
                if (direction == Direction.North)
                    tempList.Add(new Vector2Int(0, 1));
                else if (direction == Direction.South)
                    tempList.Add(new Vector2Int(0, -1));
                else if (direction == Direction.East)
                    tempList.Add(new Vector2Int(-1, 0));
                else
                    tempList.Add(new Vector2Int(1, 0));
                chioceRange = tempList;
            }

            foreach (Vector2Int pos in chioceRange)//?
            {
                Vector2Int temp = currentPosition + pos;
                if (temp.x >= 0 && temp.y >= 0 && temp.x <= 9 && temp.y <= 13)
                {
                    Tile tile = mapController.GetTileWithPosition(temp);
                    if (tile.tileState == TileState.Occupied && tile.side == Side.playerB)
                    {
                        //检测可以攻击的棋子是否已经死亡
                        GameObject tempGo = tile.occupyChess ?? null;
                        if (tempGo == null) { return; }

                        if (MinBlood > tile.occupyChess.GetComponent<Chess>().GetBlood())
                        {
                            victim = tile.occupyChess;
                            MinBlood = tile.occupyChess.GetComponent<Chess>().GetBlood();
                        }
                    }
                }
            }
            if (victim != null)
            {
                Singleton<PlayerController>.Instance.Attack(gameObject, victim);
                runningAccumulate = 0;//战车特有：重置冲锋加成
            }
        }
    }
    */
    /*
    public override void Move()
    {
        //判断是否到达终点
        if (GetCurrentPosition() == destination)
        {
            Debug.Log("到达终点");
            isMoving = false;
            StopMoveAnimation();

            //之后可以智能判断周边是否需要攻击
            if (willAttack)
            {
                //如果是攻击状态在移动结束后需要攻击
                //得到伤害
                isAttacking = true;
                float hurt = GetChessHurt(victim);
                //攻击
                action_manager.Attack(gameObject, victim, hurt);
            }
            else
            {
                isAttacking = false;
            }
        }
        else
        {
            //得到下一个位置
            if (chessType == ChessType.Car)
            {
                nextDestination = gameObject.GetComponent<CarChess>().GetNextStep(GetCurrentPosition(), destination);
            }
            else
                nextDestination = GetNextStep(GetCurrentPosition(), destination);
            Debug.Log("下一个移动到的位置$$$$$$$$$$$$$$" + nextDestination);
            //如果该位置是合法的，走向该位置
            if (MapController.instance.CanWalk(nextDestination))
            {
                //移动到该位置
                MoveToPosition(nextDestination);
            }
            else
            {
                Debug.Log("该位置不合法，应该停在当前位置");
                isMoving = false;
                willAttack = false;
                isAttacking = false;
                StopMoveAnimation();
            }
        }
    }
    */
    public bool IsFront(Vector2Int pos)//传入一个位置，并判断这个位置是否是车正前方
    {
        if (direction == Direction.East)
        {
            if(pos==currentPosition+new Vector2Int(1, 0))
                return true;
        }

        if (direction == Direction.West)
        {
            if (pos == currentPosition + new Vector2Int(-1, 0))
                return true;
        }
        if (direction == Direction.North)
        {
            if (pos == currentPosition + new Vector2Int(0, 1))
                return true;
        }
        if (direction == Direction.East)
        {
            if (pos == currentPosition + new Vector2Int(0, -1))
                return true;
        }
        return false;
    }

    /*public override Vector2Int GetNextStep(Vector2Int currentPos, Vector2Int destination)
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
    }*/

}
