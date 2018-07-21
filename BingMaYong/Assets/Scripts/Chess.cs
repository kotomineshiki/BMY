using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chess : MonoBehaviour {

    public Side chessSide;
    Vector2Int currentPosition;//当前位于格子坐标
   // GameObject player;
    //GameObject enemy;

    public float hurt = 10;               //兵马俑的伤害
    public float blood = 100;             //兵马俑的血量

	void Start ()
    {
     //   player = GameObject.FindGameObjectWithTag("Chess");
	}
    public Vector2Int GetCurrentPosition()
    {
        return currentPosition;
    }


    /*
     * 得到兵马俑的伤害
     * 返回float类型的hurt
     */
    public float GetHurt()
    {
        return hurt;
    }
    /*
     * 设置此兵马俑的血量
     * 传入float类型的血量blo
     */
    public void SetBoold(float blo)
    {
        Debug.Log(blo);
        blood = blo;
    }
    /*
     * 返回当前兵马俑的血量
     * 返回float类型的blood
     */
    public float GetBoold()
    {
        return blood;
    }
}
