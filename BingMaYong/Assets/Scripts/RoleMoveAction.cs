using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//兵马俑移动动作
public class RoleMoveAction : Action
{
    private float THE_TARGET_RADIUS = 0.05f;                //小于该距离即可算到达目的地
    private float speed = 2f;                              //移动速度
    private Vector3 destination;                           //移动的目的地
    private RoleMoveAction() { }

    /* 
     * 创建一个动作
     * 传入需要移动到的位置pos
     * 返回该移动动作
     * 创建新的动作,设置该动作的目的地
     */
    public static RoleMoveAction GetSSAction(Vector3 pos)
    {
        RoleMoveAction action = CreateInstance<RoleMoveAction>();
        action.destination = pos;
        return action;
    }

    public override void Update()
    {
        //移动到某个位置
        transform.position = Vector3.MoveTowards(this.transform.position, destination, speed * Time.deltaTime);
        //转向目的地
        //this.transform.LookAt(destination);

        //判断是否到达目的地
        float distance = Vector3.Distance(transform.position, destination);
        if (distance < THE_TARGET_RADIUS)
        {
            this.destroy = true;
            //回调函数
            this.callback.SSActionEvent(this, 1, this.gameobject);
        }
    }
    public override void Start()
    {
        //播放移动动画,调用了Role的方法
        gameobject.GetComponent<Role>().PlayMoveAnimation();
    }
}