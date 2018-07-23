using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role : MonoBehaviour
{
    private RoleActionManager action_manager;                       //运动管理器
    private GameObject victim;                                      //被攻击者
    private bool isAttack;                                          //是否需要攻击
    private bool isMoving;                                          //是否正在移动
    private Vector2Int destination;                                 //最终目的地
    private MapController mapController;                            //地图控制器
    private Vector2Int nextDestination;                             //下一个要到达的位置

    public void Start()
    {
        //初始化运动管理器单例
        action_manager = Singleton<RoleActionManager>.Instance;
        mapController = Singleton<MapController>.Instance;
        isAttack = false;
        isMoving = false;
    }

    /*
     * 兵马俑移动
     * 无参数
     * 无返回值
     * 根据设置的移动路径,一次拿出一个位置去移动
     */
    public void Move()
    {
        //判断是否到达终点
        if (gameObject.GetComponent<Chess>().GetCurrentPosition() == destination)
        {
            isMoving = false;
            gameObject.GetComponent<Role>().StopMoveAnimation();
            if(isAttack)
            {
                //如果是攻击状态在移动结束后需要攻击
                //得到伤害
                float hurt = gameObject.GetComponent<Chess>().GetHurt();
                //攻击
                action_manager.Attack(gameObject, victim, hurt);
            }
        }
        else 
        {
            Debug.Log("Role move"+destination);
            //得到下一个位置
            nextDestination = mapController.GetNextStep(gameObject.GetComponent<Chess>().GetCurrentPosition(), destination);
            if (MapController.instance.CanWalk(nextDestination))//如果该位置是合法的，走向该位置
            {
                Vector3 pos = mapController.GetWorldPosition(nextDestination);
                Debug.Log(nextDestination);
                //移动到该位置
                MoveToPosition(pos);
            }
            else
            {
                Debug.Log("该位置不合法，应该停在当前位置");
                destination = gameObject.GetComponent<Chess>().GetCurrentPosition();//令目的地为当前位置
            }
        }
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
     * 设置攻击目标,转变为攻击状态
     * 传入攻击目标vic
     * 无返回值
     */
    public void SetAttack(GameObject vic)
    {
        victim = vic;
        isAttack = true;
    }
    /*
     * 减少自身的血量
     * 传入攻击者的伤害hurt
     * 无返回值
     * 调用了Chess的方法
     */
    public void ReduceBoold(float hurt)
    {
        Debug.Log("扣血" + hurt);
        float boold = gameObject.GetComponent<Chess>().GetBoold();
        if (boold - hurt >= 0)
        {
            gameObject.GetComponent<Chess>().SetBoold(boold - hurt);
        }
    }
    /*
     * 播放攻击动画
     * 无参数,无返回值
     * 获取子对象的动画组件设置trigger
     */
    public void PlayAttackAnimation()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Animator>().SetTrigger("IsChessAttack");
            break;
        }
    }

    /*
     * 设置兵马俑的目的地
     * 传入目的地pos
     * 无返回值
     */
    public void SetDestination(Vector2Int pos)
    {
        destination = pos;
    }
    /*
     * 得到兵马俑的下一个位置
     * 返回nextDestination
     */
    public Vector2Int GetNextDestination()
    {
        return nextDestination;
    }

    /*
     * 根据位置去移动兵马俑
     * 传入移动的下一个位置pos
     * 无返回值
     * 调用运动管理器方法
     */
    private void MoveToPosition(Vector3 pos)
    {
        isMoving = true;
        action_manager.Move(gameObject, pos);
    }

    /*
     * 得到兵马俑是否正在移动
     * 返回bool类型
     */
    public bool GetMoving()
    {
        return isMoving;
    }
    /*
     * 取消兵马俑的攻击状态
     */
    public void StopAttackStatus()
    {
        isAttack = false;
    }
    /*
     * 得到兵马俑攻击状态
     * 返回bool类型
     */
    public bool GetAttackStatus()
    {
        return isAttack;
    }
}
