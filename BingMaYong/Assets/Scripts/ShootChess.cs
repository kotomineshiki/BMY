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
    public void Shooteffect()
    {//这个类将发射一个弓箭射向攻击目标
        GameObject temp = Instantiate(arrowPrefab);
        temp.transform.position = this.transform.position + new Vector3(0,0,-5);

        if(direction <= Direction.North)
        {
            int rotateValue = ((int)Direction.North + (int)direction) * 45;
            temp.transform.Rotate(new Vector3(0, 0, 1), rotateValue);
        }
        else
        {
            int rotateValue = (direction - Direction.North) * 45;
            temp.transform.Rotate(new Vector3(0, 0, 1), rotateValue);
        }


        temp.AddComponent<Rigidbody>();//添加刚体属性

        Vector3 velocity = victim.transform.position - gameObject.transform.position;

        temp.GetComponent<Rigidbody>().velocity = velocity.normalized;//添加初始速度
       // temp.GetComponent<Rigidbody>().AddForce(new Vector3(0,0,1));//添加重力

    }
}
