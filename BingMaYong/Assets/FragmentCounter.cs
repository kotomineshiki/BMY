using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FragmentCounter : MonoBehaviour {
    public int currentCount=5;//当前碎片的个数
    public float addTime = 3f;
    public static FragmentCounter instance;
    public Text ShowLayer;
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
        yield return new WaitForSeconds(addTime);
        StartCoroutine(TimeAddCount());
    }
    // Update is called once per frame
    void Update () {
        ShowLayer.text = currentCount.ToString();
	}
    public int GetCount()//返回当前碎片数量
    {
        return currentCount;
    }
    public void AddCount(int addsum)
    {//添加一些到碎片
        currentCount+=addsum;
        if (currentCount >= 12) currentCount = 12;//最大是12
    }
    public void SubCount(int subsum)
    {//减少碎片数量
        currentCount -= subsum;
    }
}
