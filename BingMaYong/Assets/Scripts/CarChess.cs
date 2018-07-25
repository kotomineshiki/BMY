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

                Vector3 pos = mapController.GetWorldPosition(nextDestination);
                //移动到该位置
                MoveToPosition(pos);
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
        Vector2Int delta = destination - currentPos;//其delta值
        if((delta-currentDirection).sqrMagnitude<delta.sqrMagnitude)//应该优先沿着当前方向前行，然后当前方向如果再走就会距离目标更远的时候，转向，然后继续走
            return currentPos += currentDirection;//返回当前面向的下一格
        else
        {//设置新的方向
            currentDirection.x = 0;
            currentDirection.y = 0;
            if (delta.x > 0) currentDirection.x = 1;
            if (delta.x < 0) currentDirection.x = -1;
            if (delta.y > 0) currentDirection.y = 1;
            if (delta.y < 0) currentDirection.y = -1;
            
            //todo :转向函数
            Debug.Log(currentDirection);
            return currentPos += currentDirection;
        }
    }

}
