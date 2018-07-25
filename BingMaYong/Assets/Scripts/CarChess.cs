using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//车佣
public class CarChess : Chess
{
    public Vector2Int currentDirection=new Vector2Int(0,1);//当前朝向
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
    }
    public override  void Move()
    {
        //判断是否到达终点
        if (GetCurrentPosition() == destination)
        {
            Debug.Log("到达终点");
            isMoving = false;
            StopMoveAnimation();

            //之后可以智能判断周边是否需要攻击
            if (isAttack)
            {
                //如果是攻击状态在移动结束后需要攻击
                //得到伤害
                float hurt = GetChessHurt(victim);
                //攻击
                action_manager.Attack(gameObject, victim, hurt);
            }
        }
        else
        {
            //得到下一个位置
            Debug.Log("当前位置" + GetCurrentPosition() + "目标位置" + destination);
            nextDestination = GetNextStep(GetCurrentPosition(), destination);
            Debug.Log("下一个移动到的位置" + nextDestination);
            //如果该位置是合法的，走向该位置
            if (MapController.instance.CanWalk(nextDestination))
            {
                /*if (this.OnWalk != null)
                {
                    this.OnWalk(currentPosition);//表示占领当前position
                }*/

                ReleaseCurrentPosition(); //释放当前占领
                OccupyPosition(nextDestination);  //占领新的

                //移动到该位置
                MoveToPosition(nextDestination);
            }
            else
            {
                Debug.Log("该位置不合法，应该停在当前位置");
                isMoving = false;
                isAttack = false;
                StopMoveAnimation();
            }
        }
    }
    Vector2Int GetNextStep(Vector2Int currentPos, Vector2Int destination)
    {//车兵特殊的寻路函数，需要了解其当前朝向才能寻路
     /*   Vector2Int delta = destination - currentPos;
        Vector2Int test = currentPos;
        List<Vector2Int> result = new List<Vector2Int>();
        int westCount=0;
        int eastCount=0;
        int southCount = 0;
        int northCount = 0;
        if (delta.x > 0)
            eastCount = delta.x;
        if (delta.x < 0)
            westCount = -delta.x;
        if (delta.y > 0)
            northCount = delta.y;
        if (delta.y < 0)
            southCount = -delta.y;
        if (currentDirection == Direction.East)
        {
            for(int i = 0; i < eastCount; ++i)
            {
                if(mapController.CanWalk(test+new Vector2Int(1, 0)) == false)//如果该方位不可走，则应该结束当前方向的行走，
                {
                    break;
                }
                else//如果可走，则加入到寻路队列中
                {
                    test += new Vector2Int(1, 0);
                    result.Add(test);
                }
            }
            for (int i = 0; i < northCount; ++i)
            {
                if (mapController.CanWalk(test + new Vector2Int(0, 1)) == false)//如果该方位不可走，则应该结束当前方向的行走，
                {
                    break;
                }
                else//如果可走，则加入到寻路队列中
                {
                    test += new Vector2Int(0, 1);
                    result.Add(test);
                }
            }
            for (int i = 0; i < southCount; ++i)
            {
                if (mapController.CanWalk(test + new Vector2Int(0, 1)) == false)//如果该方位不可走，则应该结束当前方向的行走，
                {
                    break;
                }
                else//如果可走，则加入到寻路队列中
                {
                    test += new Vector2Int(0, 1);
                    result.Add(test);
                }
            }
        }
     */
        return new Vector2Int(0, 0);
    }

}
