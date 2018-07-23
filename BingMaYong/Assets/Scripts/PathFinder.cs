using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridType
{
    Normal,//正常
    Obstacle,//障碍物
    Start,//起点
    End//终点
}

//为了格子排序 需要继承IComparable接口实现排序
public class MapGrid : IComparable//排序接口
{
    public int x;//记录坐标
    public int y;

    public int f;//总消耗
    public int g;//当前点到起点的消耗
    public int h;//当前点到终点的消耗


    public GridType type;//格子类型
    public MapGrid fatherNode;//父节点


    //排序
    public int CompareTo(object obj)     //排序比较方法 ICloneable的方法
    {
        //升序排序
        MapGrid grid = (MapGrid)obj;
        if (this.f < grid.f)
        {
            return -1;                    //升序
        }
        if (this.f > grid.f)
        {
            return 1;                    //降序
        }
        return 0;
    }

}




public class PathFinder: MonoBehaviour
{
    //格子大小
    public int row = 10;
    public int col = 14;
    public int size = 70;                //格子大小

    public MapGrid[,] grids;            //格子数组

    public ArrayList openList;            //开启列表
    public ArrayList closeList;            //结束列表

    //开始,结束点位置
    public Vector2Int StartPoint;

    public Vector2Int EndPoint;

    public  Stack<Vector2Int> fatherNodeLocation;
    public List<Vector2Int> result;
    public List<Vector2Int> Obstacles;//传入障碍物的列表
    void Init()
    {
        find = false;
        result.Clear();
        openList.Clear();
        closeList.Clear();
        fatherNodeLocation.Clear();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {

                grids[i, j].x = i;
                grids[i, j].y = j;        //初始化格子,记录格子坐标
                grids[i, j].fatherNode = null;
                grids[i, j].type = GridType.Normal;
                grids[i, j].g = 0;
                grids[i, j].f = 0;
                grids[i, j].h = 0;
            }
        }
        grids[StartPoint.x, StartPoint.y].type = GridType.Start;
        grids[StartPoint.x, StartPoint.y].h = Manhattan(StartPoint.x, StartPoint.y);    //起点的 h 值

        grids[EndPoint.x, EndPoint.y].type = GridType.End;                    //结束点


        foreach(var i in Obstacles)//设置障碍物
        {
            grids[i.x, i.y].type = GridType.Obstacle;
        }
        openList.Add(grids[StartPoint.x, StartPoint.y]);
    }

    int Manhattan(int x, int y)                    //计算算法中的 h
    {
        return (int)(Mathf.Abs(EndPoint.x - x) + Mathf.Abs(EndPoint.y - y)) * 10;
    }

    public bool find = false;
    // Use this for initialization
    public List<Vector2Int> GeneratePath(Vector2Int start, Vector2Int end,List<Vector2Int> ob)
    {
        Init();
        StartPoint = start;
        EndPoint = end;
        Obstacles = ob;
        foreach (var i in Obstacles)//设置障碍物
        {
            grids[i.x, i.y].type = GridType.Obstacle;
        }
        grids[StartPoint.x, StartPoint.y].type = GridType.Start;
        grids[EndPoint.x, EndPoint.y].type = GridType.End;//强制让终点可以被找寻到
        while (find != true)
        {
        //    Debug.Log("Step");
            NextStep();
        }
        if(result.Count!=0)result.RemoveAt(0);
        result.Add(end);//去头加尾
        return result;
    }
    void Awake()
    {
        grids = new MapGrid[row, col];    //初始化数组
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                grids[i, j] = new MapGrid();
                grids[i, j].x = i;
                grids[i, j].y = j;        //初始化格子,记录格子坐标
                grids[i, j].fatherNode = null;
            }
        }
        fatherNodeLocation = new Stack<Vector2Int>();
        openList = new ArrayList();
        closeList = new ArrayList();
    }


    //每个格子显示的内容
    string FGH(MapGrid grid)
    {
        string str = "F" + grid.f + "\n";
        str += "G" + grid.g + "\n";
        str += "H" + grid.h + "\n";
        str += "(" + grid.x + "," + grid.y + ")";
        return str;
    }


    void NextStep()
    {
        if (openList.Count == 0)                //没有可走的点
        {
            Debug.Log("Over !");
            return;
        }
        MapGrid grid = (MapGrid)openList[0];    //取出openList数组中的第一个点
        if (grid.type == GridType.End)            //找到终点
        {
            find = true;
            ShowFatherNode(grid);        //找节点//打印路线
            return;
        }

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int rcl = Mathf.Abs(i) + Mathf.Abs(j);
                if (   (!(i == 0 && j == 0))   &&rcl!=2    )//只能横竖走
                {
                    int x = grid.x + i;
                    int y = grid.y + j;
                    //x,y不超过边界,不是障碍物,不在closList里面
                    if (x >= 0 && x < row && y >= 0 && y < col && grids[x, y].type != GridType.Obstacle && !closeList.Contains(grids[x, y]))
                    {


                        //到起点的消耗
                        int g = grid.g + (int)(Mathf.Sqrt((Mathf.Abs(i) + Mathf.Abs(j))) * 10);
                        if (grids[x, y].g == 0 || grids[x, y].g > g)
                        {
                            grids[x, y].g = g;
                            grids[x, y].fatherNode = grid;        //更新父节点
                        }
                        //到终点的消耗
                        grids[x, y].h = Manhattan(x, y);
                        grids[x, y].f = grids[x, y].g + grids[x, y].h;
                        if (!openList.Contains(grids[x, y]))
                        {
                            openList.Add(grids[x, y]);            //如果没有则加入到openlist
                        }
                        openList.Sort();                        //排序
                    }
                }
            }
        }
        //添加到关闭数组
        closeList.Add(grid);
        //从open数组删除
        openList.Remove(grid);
    }


    //回溯法 递归父节点
    void ShowFatherNode(MapGrid grid)
    {
   //     Debug.Log("调用");
        if (grid.fatherNode != null)
        {
            //print(grid.fatherNode.x + "," + grid.fatherNode.y);
            //string str = grid.fatherNode.x + "," + grid.fatherNode.y;
      //      Debug.Log("PUSH");
            fatherNodeLocation.Push(new Vector2Int(grid.fatherNode.x,grid.fatherNode.y));
            ShowFatherNode(grid.fatherNode);
        }
        if (fatherNodeLocation.Count != 0)
        {
     //       Debug.Log("添加");
            result.Add(fatherNodeLocation.Pop());
        }
    }


}