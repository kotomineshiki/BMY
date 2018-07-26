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
        temp.transform.Rotate(new Vector3(0,0,0));//direction
        temp.AddComponent<Rigidbody>();//添加刚体属性
        temp.GetComponent<Rigidbody>().velocity=new Vector3(0,0,0);//添加初始速度
        temp.GetComponent<Rigidbody>().AddForce(new Vector3());//添加重力

    }
}
