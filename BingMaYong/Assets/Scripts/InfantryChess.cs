using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//步兵佣
public class NewBehaviourScript : Chess
{
    // Use this for initialization
    void Start ()
    {
        chessType = ChessType.Infantry;

        forCarChessHurt = 10;
        forShootChessHurt = 10;
        forInfantryChessHurt = 10;

        attackRange = new List<Vector2Int>(){
        new Vector2Int(0,1),
        new Vector2Int(-1,0),  new Vector2Int(1,0),
        new Vector2Int(0,-1)};
    }
    /*
     * 传入被攻击者
     * 返回被攻击者的伤害
     * 根据被攻击者类型判断伤害值
     */
    public float GetChessHurt(GameObject victim)
    {
        if (victim.gameObject.GetComponent<Chess>().chessType == ChessType.Shoot)
        {
            return forShootChessHurt;
        }
        else if (victim.gameObject.GetComponent<Chess>().chessType == ChessType.Infantry)
        {
            return forInfantryChessHurt;
        }
        else
        {
            return forCarChessHurt;
        }
    }
}
