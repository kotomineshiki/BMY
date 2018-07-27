using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//兵马俑的动作管理器
public class RoleActionManager : ActionManager
{
    private RoleMoveAction roleMove;                //兵马俑移动的一个动作
    private RoleAttackAction roleAttack;            //兵马俑攻击的一个动作
    /* 
     * 兵马俑移动
     * 传入需要移动的兵马俑GameObject,以及兵马俑要移动的位置pos
     * 无返回值
     * 初始化动作然后开始这个动作
     */
    public void Move(GameObject role,Vector2Int pos)
    {
        roleMove = RoleMoveAction.GetSSAction(pos);
        this.RunAction(role, roleMove, this);
    }

    /* 
     * 兵马俑攻击
     * 传入攻击者role,被攻击者victim,攻击者的伤害hurt
     * 无返回值
     * 初始化动作然后开始攻击动作
     */
    public void Attack(GameObject role, GameObject victim, float hurt)
    {
        if (role.GetComponent<Chess>().chessType == ChessType.Infantry)
        {
            role.transform.GetChild(2).GetComponent<Animation>().Play();
        }
        roleAttack = RoleAttackAction.GetSSAction(victim,hurt);
        this.RunAction(role, roleAttack, this);
    }

    //停止所有动作
    public void DestroyAllAction()
    {
        DestroyAll();
    }
}

