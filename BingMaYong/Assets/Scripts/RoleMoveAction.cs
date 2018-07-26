﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//兵马俑移动动作
public class RoleMoveAction : Action
{
    private float THE_TARGET_RADIUS = 0.05f;                //小于该距离即可算到达目的地
    private float speed = 2f;                              //移动速度
    private Vector2Int destination;                           //移动的目的地
    private RoleMoveAction() { }

    private bool isCar = false;
    private float carRotateSpeed = 2f;
    Vector3 goal;

    private Vector2Int lastPosition;
    int rotateValue;
    /* 
     * 创建一个动作
     * 传入需要移动到的位置pos
     * 返回该移动动作
     * 创建新的动作,设置该动作的目的地
     */
    public static RoleMoveAction GetSSAction(Vector2Int pos)
    {
        RoleMoveAction action = CreateInstance<RoleMoveAction>();
        action.destination = pos;
        return action;
    }

    public override void Update()
    {
        if(isCar)
        {
            // 目标Rotation
            //Quaternion t = Quaternion.LookRotation(goal, new Vector3(0,0,-1));

            // 进行旋转
            //transform.rotation = Quaternion.Slerp(gameobject.transform.rotation, t, Time.deltaTime * carRotateSpeed);

        }
        //移动到某个位置
        transform.position = Vector3.MoveTowards(this.transform.position, MapController.instance.GetWorldPosition(destination), speed * Time.deltaTime);
        
        //前面的路被堵
        if (!MapController.instance.CanWalk(destination))
        {
            //转身回到之前的位置
            Rotate(destination, lastPosition);
            destination = lastPosition;
        }

        //判断是否到达目的地
        float distance = Vector3.Distance(transform.position, MapController.instance.GetWorldPosition(destination));
        if (distance < THE_TARGET_RADIUS)
        {
            this.destroy = true;
            //回调函数
            this.callback.SSActionEvent(this, 1, this.gameobject);
        }
    }
    public override void Start()
    {
        //移动前检测是否是45度角的方向全部转为北方
        gameobject.GetComponent<Chess>().RotateToNorth();
        //播放移动动画,调用了Chess的方法
        gameobject.GetComponent<Chess>().PlayMoveAnimation();
        //旋转
        if ( gameobject.GetComponent<Chess>().chessType != ChessType.Castle)
            Rotate(gameobject.GetComponent<Chess>().GetCurrentPosition(),destination);
        lastPosition = gameobject.GetComponent<Chess>().GetCurrentPosition();
    }

    //移动的旋转，只用选择四个方向
    public void Rotate(Vector2Int currentPosition,Vector2Int nextPos)
    {
        Vector2Int vec = currentPosition - nextPos;
        Direction goalDirection = gameobject.GetComponent<Chess>().direction;
        if(vec == new Vector2Int(-1,0))
        {
            goalDirection = Direction.West;
            goal = new Vector3(-1,0,0);
        }
        else if(vec == new Vector2Int(1, 0))
        {
            goalDirection = Direction.East;
            goal = new Vector3(1, 0, 0);
        }
        else if (vec == new Vector2Int(0, 1))
        {
            goalDirection = Direction.South;
            goal = new Vector3(0, -1, 0);
        }
        else if (vec == new Vector2Int(0, -1))
        {
            goalDirection = Direction.North;
            goal = new Vector3(0, 1, 0);
        }

        rotateValue = -((gameobject.GetComponent<Chess>().direction - goalDirection) / 2) * 90;
        Debug.Log("旋转"+ rotateValue);
       // if (gameobject.GetComponent<Chess>().chessType == ChessType.Car)
           // isCar = true;
       // else
        {
            transform.Rotate(new Vector3(0, 0, 1), rotateValue);//旋转角色
        }
        gameobject.GetComponent<Chess>().direction = goalDirection;
    }
}