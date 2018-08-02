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
        if(role.GetComponent<Chess>().chessType != ChessType.Castle)
        {
            Vector2Int delta = endPosition - role.GetComponent<Chess>().GetCurrentPosition();

            if ((delta.x != 0 && delta.y != 0) && role.GetComponent<Chess>().chessType == ChessType.Car)//特殊逻辑
            {
                return;
            }
            bool isAttacking = role.GetComponent<Chess>().GetAttackStatus();
            if (isAttacking)
                return;
            //设置目的地
            role.GetComponent<Chess>().SetDestination(endPosition);
            //停止攻击状态
            role.GetComponent<Chess>().StopAttackStatus();
            //之前没有在移动,没有在攻击则可移动
            if (!role.GetComponent<Chess>().GetMoving() && !isAttacking)
                role.GetComponent<Chess>().Move();
        }
    }
    /*
     * 攻击一个目标兵马俑
     * 传入攻击者role,传入被攻击者victim
     * 无返回值
     * 调用寻路管理器获取一个路径容器,赋值给角色的路径容器,调用角色的Move方法,走到被攻击者前面,然后再攻击
     */
    public void Attack(GameObject role,GameObject victim)
    {

        GameObject tempGo = role ?? null;
        if (tempGo == null) { return; }

        if (role.GetComponent<Chess>().chessType != ChessType.Castle)
        {
            bool isAttacking = role.GetComponent<Chess>().GetAttackStatus();
            if (isAttacking)
                return;
            Vector2Int victimPos = victim.GetComponent<Chess>().GetCurrentPosition();
            //设置目的地

            Vector2Int pos = role.GetComponent<Chess>().GetAttackTargetLocations(victimPos);
            Debug.Log(victimPos+"dfaswe"+pos);
            MapController.instance.OrderPosition(pos);//预定位置
            role.GetComponent<Chess>().SetDestination(pos);
            Debug.Log("攻击位置"+pos);
            //设置被攻击者
            role.GetComponent<Chess>().SetAttack(victim);
            //之前没有在移动,没有在攻击则可移动到被攻击者旁
            if (!role.GetComponent<Chess>().GetMoving() && !isAttacking)
                role.GetComponent<Chess>().Move();
        }
     
    }
}

