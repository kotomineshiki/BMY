using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//兵马俑的动作管理器
public class RoleActionManager : ActionManager
{
    private RoleMoveAction roleMove;                //兵马俑移动的一个动作

    /* 
     * 兵马俑移动
     * 传入需要移动的兵马俑GameObject,以及兵马俑要移动的位置pos
     * 无返回值
     * 初始化动作然后开始这个动作
     */
    public void Move(GameObject patrol,Vector3 pos)
    {
        roleMove = RoleMoveAction.GetSSAction(pos);
        this.RunAction(patrol, roleMove, this);
    }

    //停止所有动作
    public void DestroyAllAction()
    {
        DestroyAll();
    }
}

