using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum ChessType
{//这个枚举设置棋子的类型,立射俑,车俑,步兵佣
    Shoot,
    Car,
    Infantry
}
public class Chess : MonoBehaviour {

    public Side chessSide;
    public ChessType chessType;           //棋子类型
    Vector2Int currentPosition;           //当前位于格子坐标
    public float hurt = 10;               //兵马俑的伤害
    public float blood = 100;             //兵马俑的血量
    public Slider chessSlider;


    public List<Vector2Int> attackRange;  //攻击范围
    public float forCarChessHurt;         //给车的伤害
    public float forShootChessHurt;       //给立射俑伤害
    public float forInfantryChessHurt;    //给步兵俑伤害


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
    public void OccupyCurrentPosition()
    {
        MapController.instance.SetOccupied(currentPosition,chessSide);

    }
    public void ReleaseCurrentPosition() {
        MapController.instance.SetReleased(currentPosition);
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
        if (blo <= 0)
            blo = 0;
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
    /*
     * 返回兵马俑可攻击到的格子Vector2Int值,使用List容器
     */
    public List<Vector2Int> GetAttackRange()
    {
        List<Vector2Int> range = new List<Vector2Int>();
        foreach (Vector2Int vec in attackRange)
        {
            range.Add(currentPosition + vec);
        }
        return range;
    }
}
