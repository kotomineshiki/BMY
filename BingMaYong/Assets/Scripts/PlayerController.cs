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
    public void Move(GameObject role, Vector2Int endPosition)
    {
        bool isAttacking = role.GetComponent<Role>().GetAttackStatus();
        //设置目的地
        role.GetComponent<Role>().SetDestination(endPosition);
        //停止攻击状态
        role.GetComponent<Role>().StopAttackStatus();
        //之前没有在移动,没有在攻击则可移动
        if (!role.GetComponent<Role>().GetMoving() && !isAttacking)
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
        bool isAttacking = role.GetComponent<Role>().GetAttackStatus();
        //设置目的地
        role.GetComponent<Role>().SetDestination(victim.GetComponent<Chess>().GetCurrentPosition() + new Vector2Int(0,-1)) ;
        //设置被攻击者
        role.GetComponent<Role>().SetAttack(victim);
        //之前没有在移动,没有在攻击则可移动到被攻击者旁
        if (!role.GetComponent<Role>().GetMoving() && !isAttacking)
            role.GetComponent<Role>().Move();
    }
}

