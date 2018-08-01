using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    float thinkTime = 4f;
    void Start()
    {
       // StartCoroutine(StartAI());
    }
    IEnumerator StartAI()//每隔一段时间思考一次
    {
        yield return new WaitForSeconds(thinkTime);
        Acting();
        StartCoroutine(StartAI());
    }
    private void Acting()//思考过程
    {
        ThinkPlacing();
        ThinkAttacking();
    }
    void ThinkPlacing()//思考要不要放置棋子
    {
        if (WhetherPlacingOrNot())//先判断是否需要放置棋子
        {
            ChessType myType = SelectType(); //放入哪个类型棋子
            Vector2Int myPos = SelectPlacingPosition();//放在哪里
            Singleton<ChessController>.Instance.PlaceChessAt(myPos, Side.playerB, myType);
            AIFragmentCounter.instance.SubCount(3);
        }
    }

    bool WhetherPlacingOrNot()
    {
        //判断思路1：如果当前场上我方的估值占劣势，则积极放入棋子
        //判断思路2：看看场上是否已经满足容量了
    //    int Capacity = 0;
        List<Chess> playerA = Singleton<ChessController>.Instance.playerA;
        List<Chess> playerB = Singleton<ChessController>.Instance.playerB;
        if (AIFragmentCounter.instance.currentCount < 3) return false;
        if (CountValue(playerA) > CountValue(playerB))//占劣势，应该放棋子？？但是怎么放要联动考虑
        {
            return true;
        }
        

        return true;
    }
    int CountValue(List<Chess> player)
    {
        int count = 0;
        
        foreach(var i in player)
        {
            if (i.chessType == ChessType.Car) count += 5;
            if (i.chessType == ChessType.Infantry) count += 3;
            if (i.chessType == ChessType.Shoot) count += 3;
        }
        return count;
    }
    void ThinkAttacking()//思考要不要让棋子攻击
    {
        List<Chess> playerB = Singleton<ChessController>.Instance.playerB;
        foreach (Chess chess in playerB)
        {
            Chess attackChess = SelectAttackChess(chess);        //选择每个棋子的攻击对象

            Chess temp = chess ?? null;
            if (temp == null)
            {
                continue;
            }

            if (attackChess != null)
            {
                Singleton<PlayerController>.Instance.Attack(chess.gameObject, attackChess.gameObject);
             //   Debug.Log("自动！攻击");
            }
        }
    }

    private Chess SelectAttackChess(Chess attacker)//返回被攻击的对象,给每一个潜在的攻击对象进行估价，然后攻击估价比较高的
    {//三个维度来考量:血量最小、距离最短、兵种相克
        //另外还需要一个条件
        
        int rangeRadomNum = Random.Range(0, 100);   //随机数
        List<Chess> playerA = Singleton<ChessController>.Instance.playerA;
        int[] rank = new int[playerA.Count];
     //   rank.Capacity = playerA.Count;
        int highestRank = 0;
        Chess finalChess = null;//血最少的
        for(int i = 0; i < playerA.Count; ++i)
        {
            int bloodRank = 7-(int)(playerA[i].GetComponent<Chess>().GetBlood() / 20);//血量越低评分越高
            int distanceRank =- MapController.instance.GetPathListCount(attacker.GetCurrentPosition(),playerA[i].GetCurrentPosition())/3;//距离越短评分越高
            int chessRank = -2+JudgeChess(attacker.GetComponent<Chess>().chessType, playerA[i].GetComponent<Chess>().chessType);//兵种相克时候评分高
            rank[i] = bloodRank + distanceRank + chessRank;
        }
        for(int i = 0; i < playerA.Count; ++i)
        {
            if (rank[i] > highestRank && playerA[i].attackCapacity - attacker.attackValue >= 0)//筛选条件 估值大于0且有空闲的攻击容量
            {
                finalChess = playerA[i];
            }
        }
        return finalChess;
    }
    int JudgeChess(ChessType attacker,ChessType attackee)//辅助函数，用来对攻击的棋子进行相克估值
    {
        if (attacker == ChessType.Infantry && attackee == ChessType.Shoot)
            return 4;
        if (attacker == ChessType.Infantry && attackee == ChessType.Infantry)
            return 3;
        if (attacker == ChessType.Infantry && attackee == ChessType.Car)
            return 2;
        if (attacker == ChessType.Infantry && attackee == ChessType.Castle)
            return 3;
        if (attacker == ChessType.Shoot && attackee == ChessType.Shoot)
            return 3;
        if (attacker == ChessType.Shoot && attackee == ChessType.Infantry)
            return 3;
        if (attacker == ChessType.Shoot && attackee == ChessType.Car)
            return 4;
        if (attacker == ChessType.Shoot && attackee == ChessType.Castle)
            return 5;
        if (attacker == ChessType.Car && attackee == ChessType.Shoot)
            return 4;
        if (attacker == ChessType.Car && attackee == ChessType.Infantry)
            return 5;
        if (attacker == ChessType.Car && attackee == ChessType.Car)
            return 2;
        if (attacker == ChessType.Car && attackee == ChessType.Castle)
            return 2;
        return 0;
    }
    Vector2Int SelectPlacingPosition()//返回放置棋子的位置
    {//思路 划分区域，计算区域内兵力值，往兵力不足的区域放兵？or在尽可能前线放兵？or 在敌人比较多的地方放兵？
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
    ChessType SelectType()
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
