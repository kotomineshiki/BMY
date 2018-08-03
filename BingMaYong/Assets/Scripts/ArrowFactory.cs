using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFactory : MonoBehaviour {

    public GameObject arrow = null;                             //弓箭预制体
    private List<GameObject> used = new List<GameObject>();     //正在被使用的弓箭
    private Queue<GameObject> free = new Queue<GameObject>();   //空闲的弓箭队列

    public GameObject GetArrow()
    {
        if (free.Count == 0)
        {
            arrow = Instantiate(Resources.Load<GameObject>("Prefabs/arrow"));
        }
        else
        {
            arrow = free.Dequeue();
            arrow.transform.localRotation = Quaternion.Euler(0, 0, 0);
            arrow.GetComponent<Rigidbody>().isKinematic = false;
            arrow.gameObject.SetActive(true);
        }
        used.Add(arrow);
        StartCoroutine(FreeAnArrow(arrow));
        return arrow;
    }
    public IEnumerator FreeAnArrow(GameObject arrow)
    {
        //两秒后回收箭
        yield return new WaitForSeconds(2);
        Singleton<ArrowFactory>.Instance.FreeArrow(arrow);
    }
    //回收箭
    public void FreeArrow(GameObject arrow)
    {
        for (int i = 0; i < used.Count; i++)
        {
            if (arrow.GetInstanceID() == used[i].gameObject.GetInstanceID())
            {
                used[i].gameObject.SetActive(false);
                free.Enqueue(used[i]);
                used.Remove(used[i]);
                break;
            }
        }
    }
}
