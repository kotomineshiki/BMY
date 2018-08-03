using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public float thinkTime = 1.5f;
    public int thinkCount = 2;//思考名额，即一次思考能操纵的棋子数目
    void Start()
    {
        StartCoroutine(StartAI());
    }
    IEnumerator StartAI()//每隔一段时间思考一次
    {
        yield return new WaitForSeconds(thinkTime);
        Acting();
        StartCoroutine(StartAI());
    }
    private void Acting()//思考过程
    {
        for (int i = 0; i < thinkCount; ++i)
        {
            int todosth = Random.Range(0, 1000);
            if (todosth % 3 == 0) ThinkPlacing();
            if (todosth % 3 == 1) ThinkAttacking();
            if (todosth % 3 == 2) ThinkMoving();
        }
    }
    void ThinkMoving()//思考移动到哪里
    {//仿造Placing,把棋子从兵力充足的地方移动到兵力不充足的地方：Todo

    }
    void ThinkPlacing()//思考要不要放置棋子
    {
        if (WhetherPlacingOrNot())//先判断是否需要放置棋子
        {
            ChessType myType = SelectType(); //放入哪个类型棋子
            Vector2Int myPos = SelectPlacingPosition();//放在哪里
            if (myPos == new Vector2Int(-99, -99))
                return;
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

        foreach (var i in player)
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
        int shallAttack = Random.Range(0, playerB.Count);
        //foreach (Chess chess in playerB)
        //{
        Chess chess = playerB[shallAttack];
        Chess attackChess = SelectAttackChess(chess);        //选择每个棋子的攻击对象

        Chess temp = chess ?? null;
        if (temp == null)
        {
            // continue;
            return;
        }

        if (attackChess != null)
        {
            Singleton<PlayerController>.Instance.Attack(chess.gameObject, attackChess.gameObject);
            //   Debug.Log("自动！攻击");
        }
        // }
    }

    private Chess SelectAttackChess(Chess attacker)//返回被攻击的对象,给每一个潜在的攻击对象进行估价，然后攻击估价比较高的
    {//三个维度来考量:血量最小、距离最短、兵种相克
        //另外还需要一个条件
        if (attacker.GetComponent<Chess>().attackBy || attacker.GetComponent<Chess>().isMoving || attacker.GetComponent<Chess>().willAttack)
            return null;
        //int rangeRadomNum = Random.Range(0, 100);   //随机数
        List<Chess> playerA = Singleton<ChessController>.Instance.playerA;
        int[] rank = new int[playerA.Count];
        //   rank.Capacity = playerA.Count;
        int highestRank = 0;
        Chess finalChess = null;//血最少的
        for (int i = 0; i < playerA.Count; ++i)
        {
            int bloodRank = 7 - (int)(playerA[i].GetComponent<Chess>().GetBlood() / 20);//血量越低评分越高
            int distanceRank = -MapController.instance.GetPathListCount(attacker.GetCurrentPosition(), playerA[i].GetCurrentPosition()) / 3;//距离越短评分越高
            int chessRank = -2 + JudgeChess(attacker.GetComponent<Chess>().chessType, playerA[i].GetComponent<Chess>().chessType);//兵种相克时候评分高
            rank[i] = bloodRank + distanceRank + chessRank;
        }
        for (int i = 0; i < playerA.Count; ++i)
        {
            if (rank[i] > highestRank && playerA[i].attackCapacity - attacker.attackValue >= 0)//筛选条件 估值大于0且有空闲的攻击容量
            {
                finalChess = playerA[i];
            }
        }
        return finalChess;
    }
    int JudgeChess(ChessType attacker, ChessType attackee)//辅助函数，用来对攻击的棋子进行相克估值
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
    {//思路 划分区域，计算区域内兵力差值，往兵力不足的区域放兵？or在尽可能前线放兵？or 在敌人比较多的地方放兵？
        //尽量放在中间
        List<Chess> playerA = ChessController.instance.playerA;
        List<Chess> playerB = ChessController.instance.playerB;
        int[] playerCount = new int[7];//七个的数组
        List<Vector2Int> waitList = new List<Vector2Int>();//候选名单
        foreach (var i in playerB)
        {
            playerCount[ChessInArea(i.currentPosition)]++;
        }
        foreach (var i in playerA)
        {
            playerCount[ChessInArea(i.currentPosition)]--;
        }

        //经过以上处理后，playerCount里存了敌我双方兵力的差值
        //选择合适的组
        for (int i = 0; i < 7; ++i)
        {
            if (playerCount[i] >= 0 && playerCount[i] < 2)//说明此处兵力略胜于敌方，少许需要增援
            {
                Vector2Int temp=AvailableTile(i);
                if (temp != new Vector2Int(-1, -1) && MapController.instance.CanWalk(temp)) waitList.Add(temp);
            }

            if (playerCount[i] >= -2 && playerCount[i] < 0)//说明此处兵力略负于敌方，远需要增援
            {
                Vector2Int temp = AvailableTile(i);
                if (temp != new Vector2Int(-1, -1) && MapController.instance.CanWalk(temp)) waitList.Add(temp);
                temp = AvailableTile(i);
                if (temp != new Vector2Int(-1, -1) && MapController.instance.CanWalk(temp)) waitList.Add(temp);//重复次数可以调整
            }
            //剩下的要么是增援了也活不了，不增援也无所谓的
        }
        if (waitList.Count == 0)
            return new Vector2Int(-99, -99);
        int re = Random.Range(0, waitList.Count);
        return waitList[re];

    }
    Vector2Int AvailableTile(int blockNumber)//传入一个区域号，返回该区域内随机的一个位置（需要属于playerB）
    {
        var map = MapController.instance;
        List<Vector2Int> possibleList = new List<Vector2Int>();
        if (blockNumber == 0)
        {
            for (int i = 0; i <= 4; ++i)
            {
                for (int j = 1; j <= 4; ++j)
                {
                    if (map.IsSide(new Vector2Int(i, j), Side.playerB)) possibleList.Add(new Vector2Int(i, j));
                }
            }
        }
        if (blockNumber == 1)
        {
            for (int i = 5; i <= 9; ++i)
            {
                for (int j = 1; j <= 4; ++j)
                {
                    if (map.IsSide(new Vector2Int(i, j), Side.playerB)) possibleList.Add(new Vector2Int(i, j));
                }
            }
        }
        if (blockNumber == 2)
        {
            for (int i = 0; i <= 4; ++i)
            {
                for (int j = 5; j <= 8; ++j)
                {
                    if (map.IsSide(new Vector2Int(i, j), Side.playerB)) possibleList.Add(new Vector2Int(i, j));
                }
            }
        }
        if (blockNumber == 3)
        {
            for (int i = 5; i <= 9; ++i)
            {
                for (int j = 5; j <= 8; ++j)
                {
                    if (map.IsSide(new Vector2Int(i, j), Side.playerB)) possibleList.Add(new Vector2Int(i, j));
                }
            }
        }
        if (blockNumber == 4)
        {
            for (int i = 0; i <= 4; ++i)
            {
                for (int j = 9; j <= 12; ++j)
                {
                    if (map.IsSide(new Vector2Int(i, j), Side.playerB)) possibleList.Add(new Vector2Int(i, j));
                }
            }
        }
        if (blockNumber == 5)
        {
            for (int i = 5; i <= 9; ++i)
            {
                for (int j = 9; j <= 12; ++j)
                {
                    if (map.IsSide(new Vector2Int(i, j), Side.playerB)) possibleList.Add(new Vector2Int(i, j));
                }
            }
        }
        if (blockNumber == 6)
        {
            for(int i = 0; i < 9; ++i)
            {
                if (map.IsSide(new Vector2Int(i, 13), Side.playerB)) possibleList.Add(new Vector2Int(i, 13));
                if (map.IsSide(new Vector2Int(i, 0), Side.playerB)) possibleList.Add(new Vector2Int(i, 0));
            }
        }
        if (possibleList.Count == 0) return new Vector2Int(-1, -1);//错误返回
        int re = Random.Range(0, possibleList.Count);//随机返回
     //   Debug.Log("re"+re+blockNumber);

        return possibleList[re];
    }

    int ChessInArea(Vector2Int chessPos)//输入一个棋子的位置，返回该位置所在的区号
    {
        if (chessPos.y >= 1 && chessPos.y <= 4 && chessPos.x >= 0 && chessPos.x <= 4) return 0;//areaA Row 1-4,Col 0-4
        if (chessPos.y >= 1 && chessPos.y <= 4 && chessPos.x >= 5 && chessPos.x <= 9) return 1; //areaB Row 1-4,Col 5-9
        if (chessPos.y >= 5 && chessPos.y <= 8 && chessPos.x >= 0 && chessPos.x <= 4) return 2;//areaC Row 5-8,Col 0-4
        if (chessPos.y >= 5 && chessPos.y <= 8 && chessPos.x >= 5 && chessPos.x <= 9) return 3;//areaD Row 5-8,Col 5-9
        if (chessPos.y >= 9 && chessPos.y <= 12 && chessPos.x >= 0 && chessPos.x <= 4) return 4;//areaE Row 9-12,Col 0-4
        if (chessPos.y >= 9 && chessPos.y <= 12 && chessPos.x >= 5 && chessPos.x <= 9) return 5;//areaF Row 9-12,Col 5-9
        return 6;//剩余两行不考虑了

    }
    ChessType SelectType()
    {//我想了下决定随机
        int re = Random.Range(0, 100);
        if (re < 60) return ChessType.Infantry;
        if (re < 95) return ChessType.Shoot;
        return ChessType.Car;
    }
}
