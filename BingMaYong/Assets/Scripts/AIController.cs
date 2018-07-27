using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    float thinkTime = 4f;
    void Start()
    {
        StartCoroutine(StartAI());
    }
    IEnumerator StartAI()
    {
        yield return new WaitForSeconds(thinkTime);
        OnController();
        StartCoroutine(StartAI());
    }
    private void OnController()
    {
        if(AIFragmentCounter.instance.currentCount >= 3)
        {
            ChessType myType = SelectType(); //放入哪个类型棋子
            Vector2Int myPos = SelectPosition();//放在哪里
            Singleton<ChessController>.Instance.PlaceChessAt(myPos, Side.playerB, myType);
            AIFragmentCounter.instance.SubCount(3);
        }
        List<Chess> playerB = Singleton<ChessController>.Instance.playerB;
        foreach(Chess chess in playerB)
        {
            Chess attackChess = SelectAttackChess();        //棋子攻击谁
           
            Chess temp = chess ?? null;
            if (temp == null)
            {
                continue;
            }

            if (attackChess != null)
            {
                Singleton<PlayerController>.Instance.Attack(chess.gameObject, attackChess.gameObject);
            }
        }
    }

    private Chess SelectAttackChess()
    {
        int rangeRadomNum = Random.Range(0, 100);   //随机数
        List<Chess> playerA = Singleton<ChessController>.Instance.playerA;
        if (rangeRadomNum <= 40)//40%的几率随机攻击
        {
            if (playerA.Count == 0)
                return null;
            else
                return playerA[rangeRadomNum % playerA.Count];
        }
        float minBlood = 9999;
        Chess finalChess = null;
        foreach (Chess chess in playerA)
        {
            if (chess == null)
                continue;
            //选择血最少的攻击
            if(chess.GetBlood() < minBlood)
            {
                minBlood = chess.GetBlood();
                finalChess = chess;
            }
        }
        return finalChess;
    }

    private Vector2Int SelectPosition()
    {
        //尽量放在中间
        List<Vector2Int> playerBVectors = new List<Vector2Int>();
        for(int j = 4; j < 14; j++)
        {
            for(int i = 5;i >= 0;i--)
            {
                if(MapController.instance.CanWalk(new Vector2Int(i, j)) && Singleton<MapController>.Instance.IsSide(new Vector2Int(i,j),Side.playerB))
                {
                    playerBVectors.Add(new Vector2Int(i, j));
                }
            }
            for (int i = 9; i >= 6; i--)
            {
                if (MapController.instance.CanWalk(new Vector2Int(i, j)) && Singleton<MapController>.Instance.IsSide(new Vector2Int(i, j), Side.playerB))
                {
                    playerBVectors.Add(new Vector2Int(i, j));
                }
            }
        }
        return playerBVectors[0];

    }
    private ChessType SelectType()
    {
        //出步兵：对面车数量>=3 跪射最多 
        //出车：  对面兵最多 跪射>=3
        //出跪射：对面兵<=3  不知道出什么的时候
        List<Chess> playerA = Singleton<ChessController>.Instance.playerA;
        int infantryCount = 0;
        int ShootCount = 0;
        int CarCount = 0;
        foreach(Chess chess in playerA)
        {
            if (chess == null)
                continue;
            if(chess.chessType == ChessType.Infantry)
            {
                infantryCount++;
            }
            else if(chess.chessType == ChessType.Car)
            {
                ShootCount++;
            }
            else if (chess.chessType == ChessType.Shoot)
            {
                CarCount++;
            }
        }
        if(infantryCount <= 3)
        {
            return ChessType.Shoot;
        }
        if(CarCount >= 3 || (ShootCount >= infantryCount && ShootCount >= CarCount))
        {
            return ChessType.Infantry;
        }
        if(ShootCount >= 3 || (infantryCount >= ShootCount && infantryCount >= CarCount))
        {
            return ChessType.Car;
        }
        else
        {
            return ChessType.Shoot;
        }
    }
}
