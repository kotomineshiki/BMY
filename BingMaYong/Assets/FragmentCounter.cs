using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentCounter : MonoBehaviour {
    public int currentCount=10;//当前碎片的个数
    public static FragmentCounter instance;
    // Use this for initialization
    void Awake () {
        instance = this;//设置单例模式
    }
    void Start()
    {
        StartCoroutine(TimeAddCount());
    }
    IEnumerator TimeAddCount()
    {
        AddCount(1);
        yield return new WaitForSeconds(5f);
        StartCoroutine(TimeAddCount());
    }
    // Update is called once per frame
    void Update () {
		
	}
    public int GetCount()//返回当前碎片数量
    {
        return currentCount;
    }
    public void AddCount(int addsum)
    {//添加一些到碎片
        currentCount+=addsum;
    }
    public void SubCount(int subsum)
    {//减少碎片数量
        currentCount -= subsum;
    }
}
