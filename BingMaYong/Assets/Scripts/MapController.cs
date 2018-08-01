using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {
    public static MapController instance;
    public List<Vector2Int> orderList=new List<Vector2Int>();//表示被预定的地点列表
    public bool OrderPosition(Vector2Int orderpos)//预定一个位置，返回是否成功//可能会遇到敌我预定冲突的问题
    {
        foreach(var i in orderList)
        {
            if (orderpos == i) return false;
        }
        orderList.Add(orderpos);
        return true;

    }
    public void RedoOrder(Vector2Int orderpos)//取消该位置的预定
    {
        if(orderList.Contains(orderpos))
            orderList.Remove(orderpos);
    }
    public bool IsInOrder(Vector2Int orderpos)
    {
        foreach( var i in orderList)
        {
            if (i == orderpos)
            {
                return true;
            }
        }
        return false;
    }


    public Tile[,] tiles;
    public GameObject tilePrefab;

    public int column = 10;//列数目
    public int row = 14;//行数目

    public PathFinder pathFinder;
    public List<Vector2Int> testRoute;
    public Material playerAMaterial;
    public Material playerBMaterial;
    public Material obstacleMateiral;
	// Use this for initialization
	void Awake () {
        instance = this;//设置单例模式

        Initialize();
        SetObstacle(new Vector2Int(5, 5));
        SetObstacle(new Vector2Int(4, 5));
        SetObstacle(new Vector2Int(3, 5));
        SetObstacle(new Vector2Int(2, 5));
        InitialOccupied();
    }
	void InitialOccupied()
    {
        for(int i = 0; i < 10;++i)
        {
            for(int j = 0; j < 3; ++j)
            {
                SetOwner(new Vector2Int(i, j), Side.playerA);
            }
        }
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 11; j < 14; ++j)
            {
                SetOwner(new Vector2Int(i, j), Side.playerB);
            }
        }
    }
	// Update is called once per frame
	void Update () {

	}
    /*todo 寻路函数返回位置*/
    public Vector2Int GetNextStep(Vector2Int currentPosition, Vector2Int destination)
    {

        List<Vector2Int> currentObstacles=new List<Vector2Int>();//目前的障碍格子列表
        for(int i = 0; i < column;i++)
        {
            for(int j = 0; j < row;j++)
            {
                if(!CanWalk(new Vector2Int(i, j)))//如果不能走
                {
                    currentObstacles.Add(new Vector2Int(i, j));
                }
            }
        }
        List<Vector2Int> answer = pathFinder.GeneratePath(currentPosition, destination, currentObstacles);
        //Debug.Log(pathFinder.GeneratePath(currentPosition, destination, currentObstacles)[0]);
        if (answer.Count == 0)
        {
            Debug.Log("未能找到路径");
            return new Vector2Int(-1, -1);//其他函数也要配合检验！！！！！！！！！！！！
        }
    /*    if (!CanWalk(answer[0]))
        {
            Debug.Log("asdf");
            return currentPosition;

        }else*/
        return pathFinder.GeneratePath(currentPosition, destination, currentObstacles)[0];
    }
    private void Initialize()//这个函数用来逐行逐列创建地图，创建后格子的父类为当前类
    {
        tiles = new Tile[column, row];
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                GameObject obj = MonoBehaviour.Instantiate(tilePrefab, new Vector3((i - (column / 2)) * 1.6f, (j - (row / 2))*1.6f,0), Quaternion.identity) as GameObject;

                tiles[i, j] = obj.GetComponent<Tile>();
                tiles[i, j].tilePosition = new Vector2Int(i, j);
                obj.transform.SetParent(transform);
            }
        }
    }
    public Vector3 GetWorldPosition(Vector2Int input)//输入一个格子的格子坐标，返回该格子对应的正方体的世界坐标
    {//未测试
        return tiles[input.x, input.y].gameObject.transform.position;
    }

    public bool CanWalk(Vector2Int input)//传入一个格子坐标，返回该格子是否是不可走的
    {
        if (tiles[input.x, input.y].tileState == TileState.Occupied || tiles[input.x, input.y].tileState == TileState.Obstacle)
            return false;
        else return true;
    }
    public void SetObstacle(Vector2Int pos)//传入格子坐标，把一个格子设置成不可逾越的
    {
        tiles[pos.x, pos.y].tileState = TileState.Obstacle;
        tiles[pos.x, pos.y].GetComponent<MeshRenderer>().material = obstacleMateiral;
    }
    public void SetOwner(Vector2Int pos,Side side)//传入格子坐标，把一个格子设置成被一方占领的
    {
        if (side == Side.playerA) tiles[pos.x, pos.y].GetComponent<MeshRenderer>().material = playerAMaterial;
        if (side == Side.playerB) tiles[pos.x, pos.y].GetComponent<MeshRenderer>().material = playerBMaterial;
        tiles[pos.x, pos.y].side = side;
    }
    public void SetOccupied(Vector2Int pos,Side side)
    {
        tiles[pos.x, pos.y].tileState = TileState.Occupied;//只适用于当前走在的格子上
        tiles[pos.x, pos.y].side = side;//设定当前格子的归属权
        tiles[pos.x, pos.y].GetComponent<MeshRenderer>().material.color =new  Color(0.5f, 0.5f, 0.5f);
        SetOwner(pos, side);
    }
    public void SetReleased(Vector2Int pos)//释放一个格子的控制权，让它可以被走
    {
        tiles[pos.x, pos.y].tileState = TileState.Idle;
        tiles[pos.x, pos.y].GetComponent<MeshRenderer>().material =( tiles[pos.x, pos.y].side == Side.playerA) ? playerAMaterial : playerBMaterial;
    }


    /*
     * 传入位置得到该位置的Tile
     */ 
    public Tile GetTileWithPosition(Vector2Int pos)
    {
        return tiles[pos.x,pos.y];
    }


    public int GetPathListCount(Vector2Int currentPosition, Vector2Int destination)
    {

        List<Vector2Int> currentObstacles = new List<Vector2Int>();//目前的障碍格子列表
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (!CanWalk(new Vector2Int(i, j)))//如果不能走
                {
                    currentObstacles.Add(new Vector2Int(i, j));
                }
            }
        }
        var temptest = pathFinder.GeneratePath(currentPosition, destination, currentObstacles);

        return pathFinder.GeneratePath(currentPosition, destination, currentObstacles).Count;
    }
    public bool IsSide(Vector2Int pos,Side testSide)
    {//传入一个位置和测试的边，返回该格子是否属于该阵营
        if (tiles[pos.x, pos.y].side == testSide) return true;
        else return false;
    }
}
