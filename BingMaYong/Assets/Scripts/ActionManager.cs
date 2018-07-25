using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionCallback
{
    void SSActionEvent(Action source, int intParam = 0, GameObject objectParam = null, GameObject nextObjectParam = null);
}

public class ActionManager : MonoBehaviour, IActionCallback
{
    private Dictionary<int, Action> actions = new Dictionary<int, Action>();    //将执行的动作的字典集合
    private List<Action> waitingAdd = new List<Action>();                       //等待去执行的动作列表
    private List<int> waitingDelete = new List<int>();                          //等待删除的动作的key                

    protected void Update()
    {
        foreach (Action ac in waitingAdd)
        {
            actions[ac.GetInstanceID()] = ac;
        }
        waitingAdd.Clear();

        foreach (KeyValuePair<int, Action> kv in actions)
        {
            Action ac = kv.Value;
            if (ac.destroy)
            {
                waitingDelete.Add(ac.GetInstanceID());
            }
            else if (ac.enable)
            {
                ac.Update();
            }
        }

        foreach (int key in waitingDelete)
        {
            Action ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }

    public void RunAction(GameObject gameobject, Action action, IActionCallback manager)
    {
        action.gameobject = gameobject;
        action.transform = gameobject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }

    /*
     * 动作的回调函数
     * 传入动作,int类型参数,当前动作结束的对象
     * 无返回值
     * 根据参数执行下一个动作,调用了Role,Chess的一些方法
     */
    public void SSActionEvent(Action source, int intParam = 0, GameObject objectParam = null,GameObject nextObjectParam = null)
    {
        if(intParam == 1)
        {
            //移动动作结束后
            //设置兵马俑当前的位置
            objectParam.gameObject.GetComponent<Chess>().ReleaseCurrentPosition(); //释放当前占领
            objectParam.gameObject.GetComponent<Chess>().SetCurrentPosition(objectParam.gameObject.GetComponent<Chess>().GetNextDestination());
            objectParam.gameObject.GetComponent<Chess>().OccupyPosition(objectParam.gameObject.GetComponent<Chess>().GetCurrentPosition());  //占领新的

            objectParam.gameObject.GetComponent<Chess>().StopMoveAnimation();
            //移动到下一个位置
            objectParam.gameObject.GetComponent<Chess>().Move();
        }
        else if(intParam == 2)
        {
            //攻击动作结束后
            if(nextObjectParam.gameObject.GetComponent<Chess>().GetBlood() <= 0)
            {

                Tile tempTile = Singleton<MapController>.Instance.GetTileWithPosition(nextObjectParam.gameObject.GetComponent<Chess>().GetCurrentPosition());
                Singleton<MapController>.Instance.SetReleased(nextObjectParam.gameObject.GetComponent<Chess>().GetCurrentPosition());
                tempTile.occupyChess = null;

                nextObjectParam.gameObject.GetComponent<Chess>().ReleaseCurrentPosition(); //释放当前占领
                //血量少于0,摧毁对象
                Destroy(nextObjectParam.gameObject);
                //停止攻击状态
                objectParam.gameObject.GetComponent<Chess>().StopAttackStatus();
            }
            else if(objectParam.gameObject.GetComponent<Chess>().GetAttackStatus())
            {
                //如果正在攻击则1s后继续攻击
                StartCoroutine(PlayerAttack(objectParam, nextObjectParam));
            }
            else
            {
                //查看有没有新的目的地然后去移动
                objectParam.gameObject.GetComponent<Chess>().Move();
            }
        }
        else if(intParam == 3)
        {
            //攻击对象已经销毁后
            //停止攻击状态
            objectParam.gameObject.GetComponent<Chess>().StopAttackStatus();
        }
    }

    IEnumerator PlayerAttack(GameObject objectParam,GameObject nextObjectParam)
    {
        yield return new WaitForSeconds(1.0f);
        //移动到被攻击者旁
        objectParam.gameObject.GetComponent<Chess>().Move();
    }

    /*
     * 取消所有当前场景内所有动作
     */
    public void DestroyAll()
    {
        foreach (KeyValuePair<int, Action> kv in actions)
        {
            Action ac = kv.Value;
            ac.destroy = true;
        }
    }
}
 