using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder:MonoBehaviour{//这个类从MapController中分离出来，专门负责寻路
    public float[,] valueMatrix;
    public List<Vector2Int> obstacles;//不可逾越之地的一个列表
    public Vector2Int destiny;//目的地
    void Awake()
    {
        valueMatrix = new float[10, 14];//此处写的不好
        for(int i=0;i<10; ++i)
        {
            for(int j = 0; j < 14; ++j)
            {
                valueMatrix[i, j] = 0;
            }
        }

    }
    void Evaluating(Vector2Int destination)
    {
        /*用来赋予权值的函数,步骤是
         * 1.按照到目的地的哈密顿距离赋予权值
         * 2.按照尽可能贴近友军的方向走，并且尽可能远离敌方？？
         * 3.所有不能走的地块都强制设置权值为非常负的数；
         * 
         */
        this.GetComponent<MapController>();

        for (int i = 0; i < 10; ++i)//step1
        {
            for (int j = 0; j < 14; ++j)
            {
                valueMatrix[i, j] += -HamiltonDistance(destination,new Vector2Int(i,j));//越靠近目的地，其权值应该越大
            }
        }
        //step2
        for (int i = 0; i < 10; ++i)//step3
        {
            for (int j = 0; j < 14; ++j)
            {
                if (this.GetComponent<MapController>().IsObstacle(new Vector2Int(i, j)))
                {
                    valueMatrix[i, j] = -10000f;
                }   //所有不能走的地方，其权值为-10000
            }
        }

    }
    void Clear()//清空当前估值矩阵内的所有数据
    {
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 14; ++j)
            {
                valueMatrix[i, j] =0;//清空
            }
        }
    }
    public Vector2Int GetNextStep(Vector2Int currentPosition,Vector2Int destination)
    {
        /*传入当前位置和目的地
         *返回下一个位置的格子坐标
         */
        Clear();
        Evaluating(destination);
        var goodPositions = FindGoodPositions(currentPosition);//获取邻接区域中估值比当前区域大的地方

        return SelectPosition(goodPositions);
    }
    Vector2Int SelectPosition(List<Vector2Int> positions)//辅助函数，这个函数从待选择的列表中选择一个位置作为返回值
    {
        if (positions.Count == 0) Debug.Log("竟然没有值得走的。");

        return positions[(int)Random.Range(0,positions.Count)];//暂时只选择最上面的
    }
    List<Vector2Int> FindGoodPositions(Vector2Int currentPosition)
    {
        //辅助函数，输入一个位置，返回和该位置邻接的位置的值比它大的位置的列表
        List<Vector2Int> temp = new List<Vector2Int>();
        if (currentPosition.x + 1 < 10  )//边界条件判定
        {
            if (valueMatrix[currentPosition.x + 1, currentPosition.y]>valueMatrix[currentPosition.x,currentPosition.y]) {
                temp.Add(new Vector2Int(currentPosition.x + 1, currentPosition.y ));
            }

        }
        if ( currentPosition.y - 1 >=0)//边界条件判定
        {
            if (valueMatrix[currentPosition.x , currentPosition.y - 1] > valueMatrix[currentPosition.x, currentPosition.y])
            {
                temp.Add(new Vector2Int(currentPosition.x , currentPosition.y - 1));
            }

        }
        if (currentPosition.x - 1 >=0 )//边界条件判定
        {
            if (valueMatrix[currentPosition.x - 1, currentPosition.y ] > valueMatrix[currentPosition.x, currentPosition.y])
            {
                temp.Add(new Vector2Int(currentPosition.x - 1, currentPosition.y ));
            }

        }
        if ( currentPosition.y + 1 <14)//边界条件判定
        {
            if (valueMatrix[currentPosition.x , currentPosition.y + 1] > valueMatrix[currentPosition.x, currentPosition.y])
            {
                temp.Add(new Vector2Int(currentPosition.x , currentPosition.y + 1));
            }

        }
        return temp;
    }
    public List<Vector2Int> GetTheRoute(Vector2Int startPosition,Vector2Int destination)
    {//传入开始位置和目的地，返回一条路径
        List<Vector2Int> route = new List<Vector2Int>();
        var currentPosition = startPosition;
        while (GetNextStep(currentPosition,destination) !=destination)
        {
            var temp = GetNextStep(currentPosition, destination);
            route.Add(temp);
            currentPosition = temp;
        }
        route.Add(currentPosition);
        return route;
    }
    int HamiltonDistance(Vector2Int pointA, Vector2Int pointB)//哈密顿距离，传入两个格子坐标计算其间的哈密顿距离
    {
        Vector2Int delta = pointA - pointB;
        int value = Mathf.Abs(delta.x) + Mathf.Abs(delta.y);
        return value; 
    }
}
