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
        //判断攻击时候对象是否已经被摧毁
        GameObject tempGo = victim ?? null;
        if (tempGo == null)
        {
            this.destroy = true;
            //回调函数
            this.callback.SSActionEvent(this, 3,gameobject);
        }
        else
        {
            //播放攻击动画,调用了Chess的方法
            gameobject.GetComponent<Chess>().PlayAttackAnimation();
            //受害者扣血,调用了Chess的方法
            victim.GetComponent<Chess>().ReduceBoold(hurt);
            Debug.Log("执行攻击");
            this.destroy = true;
            //回调函数
            this.callback.SSActionEvent(this, 2, this.gameobject, victim);
        }
    }
    public override void Start()
    {
        RotateToVictim();
    }

    //攻击时朝向对方
    public void RotateToVictim()
    {

        Vector2Int victimPos = victim.GetComponent<Chess>().GetCurrentPosition();
        Vector2Int myPos = gameobject.GetComponent<Chess>().GetCurrentPosition();
        if (victimPos - myPos == new Vector2Int(0,1))
        {
            int rotateValue = -(gameobject.GetComponent<Chess>().direction - Direction.North) * 90;
            transform.Rotate(new Vector3(0, 0, 1), rotateValue);//旋转角色
            gameobject.GetComponent<Chess>().direction = Direction.North;
        }
        else if(victimPos - myPos == new Vector2Int(0, -1))
        {
            int rotateValue = -(gameobject.GetComponent<Chess>().direction - Direction.South) * 90;
            transform.Rotate(new Vector3(0, 0, 1), rotateValue);//旋转角色
            gameobject.GetComponent<Chess>().direction = Direction.South;
        }
        else if(victimPos - myPos == new Vector2Int(-1, 0))
        {
            int rotateValue = -(gameobject.GetComponent<Chess>().direction - Direction.East) * 90;
            transform.Rotate(new Vector3(0, 0, 1), rotateValue);//旋转角色
            gameobject.GetComponent<Chess>().direction = Direction.East;
        }
        else if(victimPos - myPos == new Vector2Int(1, 0))
        {
            int rotateValue = -(gameobject.GetComponent<Chess>().direction - Direction.West) * 90;
            transform.Rotate(new Vector3(0, 0, 1), rotateValue);//旋转角色
            gameobject.GetComponent<Chess>().direction = Direction.West;
        }
       
    }
    //攻击结束转回
    public void ReturnRotate()
    {

    }
}
