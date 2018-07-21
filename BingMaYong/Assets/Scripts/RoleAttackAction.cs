using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleAttackAction : Action
{
    private GameObject victim;
    private float hurt;
    private RoleAttackAction() { }

    /* 
     * 创建一个攻击动作
     * 传入被攻击者vic,攻击者的伤害hurt
     * 返回该攻击动作
     * 创建新的动作,设置该动作的被攻击者和受到的伤害
     */
    public static RoleAttackAction GetSSAction(GameObject vic,float hurt)
    {
        RoleAttackAction action = CreateInstance<RoleAttackAction>();
        action.victim = vic;
        action.hurt = hurt;
        return action;
    }

    public override void Update()
    {
        //受害者扣血,调用了Role的方法
        victim.GetComponent<Role>().ReduceBoold(hurt);
        this.destroy = true;
        //回调函数
        this.callback.SSActionEvent(this);
    }
    public override void Start()
    {
        //播放攻击动画,调用了Role的方法
        gameobject.GetComponent<Role>().PlayAttackAnimation();
    }
}
