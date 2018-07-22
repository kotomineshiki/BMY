using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionCallback
{
    void SSActionEvent(Action source, int intParam = 0, GameObject objectParam = null);
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
    public void SSActionEvent(Action source, int intParam = 0, GameObject objectParam = null)
    {
        if(intParam == 1)
        {
            //设置兵马俑当前的位置
            objectParam.gameObject.GetComponent<Chess>().SetCurrentPosition(objectParam.gameObject.GetComponent<Role>().GetNextDestination());
            objectParam.gameObject.GetComponent<Role>().StopMoveAnimation();
            //移动到下一个位置
            objectParam.gameObject.GetComponent<Role>().Move();
        }
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
 