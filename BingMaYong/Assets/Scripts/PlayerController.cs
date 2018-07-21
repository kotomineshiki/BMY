using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//玩家管理器
public class PlayerController : MonoBehaviour
{
    /*
     * 移动一个兵马俑
     * 传入移动的兵马俑GameObject,传入移动的Vector2位置
     * 无返回值
     * 调用寻路管理器获取一个路径容器,赋值给角色的路径容器,调用角色的Move方法
     */ 
    public void Move(GameObject role,Vector2 endPosition)
    {
        Stack<Vector3> positions = new Stack<Vector3>();
        //调用寻路管理，返回一个路径容器
        //不知道用什么先用stack
        positions.Push(new Vector3(2.25f, 5.46f, -0.54f));
        positions.Push(new Vector3(1.16f, 5.46f, -0.54f));
        positions.Push(new Vector3(0.025f, 5.46f, -0.54f));
        positions.Push(new Vector3(0.025f, 4.39f, -0.54f));
        positions.Push(new Vector3(0.025f, 3.36f, -0.54f));

        role.GetComponent<Role>().SetPath(positions);
        role.GetComponent<Role>().Move();
    }
    /*
     * 攻击一个目标兵马俑
     * 传入攻击者role,传入被攻击者victim
     * 无返回值
     * 调用寻路管理器获取一个路径容器,赋值给角色的路径容器,调用角色的Move方法,走到被攻击者前面,然后再攻击
     */
    public void Attack(GameObject role,GameObject victim)
    {
        Stack<Vector3> positions = new Stack<Vector3>();
        //调用寻路管理，返回一个路径容器
        //不知道用什么先用stack
        positions.Push(new Vector3(2.25f, 5.46f, -0.54f));
        positions.Push(new Vector3(1.16f, 5.46f, -0.54f));
        positions.Push(new Vector3(0.025f, 5.46f, -0.54f));
        positions.Push(new Vector3(0.025f, 4.39f, -0.54f));
        positions.Push(new Vector3(0.025f, 3.36f, -0.54f));
        //设置路线
        role.GetComponent<Role>().SetPath(positions);
        //设置被攻击者
        role.GetComponent<Role>().SetAttack(victim);
        //移动
        role.GetComponent<Role>().Move();
    }
}

