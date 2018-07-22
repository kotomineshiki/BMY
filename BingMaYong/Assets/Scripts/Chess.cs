using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chess : MonoBehaviour {

    public Side chessSide;
    Vector2Int currentPosition;           //当前位于格子坐标
    public float hurt = 10;               //兵马俑的伤害
    public float blood = 100;             //兵马俑的血量
    public Slider chessSlider; 
    /*
     * 得到兵马俑的当前位置
     * 返回Vector2Int类型的当前位置
     */
    public Vector2Int GetCurrentPosition()
    {
        return currentPosition;
    }

    /*
     * 设置兵马俑的当前位置
     * 传入需要设置的位置pos
     */
    public void SetCurrentPosition(Vector2Int pos)
    {
       currentPosition = pos;
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
        Debug.Log("set"+ blo);
        blood = blo;
        chessSlider.value = blo;
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
