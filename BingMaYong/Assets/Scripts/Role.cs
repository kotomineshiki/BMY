﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role : MonoBehaviour
{
    private RoleActionManager action_manager;                       //运动管理器
    private Stack<Vector3> path = new Stack<Vector3>();             //兵马俑会移动的路径

    public void Start()
    {
        //初始化运动管理器单例
        action_manager = Singleton<RoleActionManager>.Instance;
    }

    /*
     * 兵马俑移动
     * 无参数
     * 无返回值
     * 根据设置的移动路径,一次拿出一个位置去移动
     */
    public void Move()
    {
        if (path.Count != 0)
        {
            Vector3 nextPos = path.Peek();
            MoveToPosition(nextPos);
            path.Pop();
        }
    }
    /*
     * 设置兵马俑移动路线
     * 传入路径的容器pos
     * 无返回值
     */
    public void SetPath(Stack<Vector3> pos)
    {
        path = pos;
    }

    /*
     * 播放移动动作
     * 无参数,返回值
     * 寻找第一个子对象,子对象上有Animator
     */ 
    public void PlayMoveAnimation()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Animator>().SetBool("move", true);
            break;
        }
    }
    /*
     * 停止移动动作,返回idle状态
     * 无参数,返回值
     * 寻找第一个子对象,子对象上有Animator
     */
    public void StopMoveAnimation()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Animator>().SetBool("move", false);
            break;
        }
    }

    /*
     * 根据位置去移动兵马俑
     * 传入移动的下一个位置pos
     * 无返回值
     * 调用运动管理器方法
     */
    private void MoveToPosition(Vector3 pos)
    {
        action_manager.Move(gameObject, pos);
    }
}
