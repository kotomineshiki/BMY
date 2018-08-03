using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Timer : MonoBehaviour {

    public int TotalTime = 180;//总时间,单位秒

    public Text TimeText;//在UI里显示时间
    private int mumite;//分
    private int second;//秒

    void Start()
    {
        StartCoroutine(startTime());   //运行一开始就进行协程
    }

    public IEnumerator startTime()
    {
        while (TotalTime >= 0)
        {
            yield return new WaitForSeconds(1);//由于开始倒计时，需要经过一秒才开始减去1秒，
             //所以要先用yield return new WaitForSeconds(1);然后再进行TotalTime--;运算

            TotalTime--;
            TimeText.text = "Time: " + TotalTime;

            if (TotalTime <= 0)
            {
                SendTimeEndEvent();
            }

            mumite = TotalTime / 60; //输出显示分
            second = TotalTime % 60; //输出显示秒
            string length = mumite.ToString();
            if (second >= 10)
            {

                TimeText.text = "Time: " + "0" + mumite + ":" + second;
            }     //如果秒大于10的时候，就输出格式为 00：00
            else
                TimeText.text = "Time: " + "0" + mumite + ":0" + second;      //如果秒小于10的时候，就输出格式为 00：00
        }
    }
    public delegate void timeEnd();
    public static event timeEnd timeEndEvent;

    public void SendTimeEndEvent()
    {
        if (timeEndEvent != null)
        {
            timeEndEvent();
        }
    }
}
