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


        direction = Direction.North;
    }
    public GameObject arrowPrefab;
    void shooteffect(Vector2Int toAttackPos)
    {//这个类将发射一个弓箭射向攻击目标
        GameObject temp = Instantiate(arrowPrefab);
        temp.transform.position = this.transform.position;
        temp.transform.Rotate(new Vector3(0,0,0));//direction
        temp.AddComponent<Rigidbody>();//添加刚体属性
        temp.GetComponent<Rigidbody>().velocity=new Vector3(0,1,0);//添加初始速度
        temp.GetComponent<Rigidbody>().AddForce(new Vector3(0,0,1));//添加重力

    }

    public override void Move()
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
                shooteffect(new Vector2Int(1,1));
                action_manager.Attack(gameObject, victim, hurt);
            }
            else
            {
                //自动攻击
                AutoAttacks();
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
                nextDestination = mapController.GetNextStep(GetCurrentPosition(), destination);
            Debug.Log("下一个移动到的位置" + nextDestination);
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
                isAttack = false;
                StopMoveAnimation();
            }
        }
    }
}
