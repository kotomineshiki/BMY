using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {
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
        Initialize();
        SetObstacle(new Vector2Int(5, 5));
        SetObstacle(new Vector2Int(4, 5));
        SetObstacle(new Vector2Int(3, 5));
        SetObstacle(new Vector2Int(2, 5));
    }
	
	// Update is called once per frame
	void Update () {

	}
    /*todo 寻路函数返回位置*/
    public Vector2Int GetNextStep(Vector2Int currentPosition, Vector2Int destination)
    {
        //     Debug.Log("!!!!!!!!!!!" + pathFinder.GetNextStep(currentPosition, destination)+currentPosition+" "+destination);
        //return pathFinder.GetNextStep(currentPosition, destination);
        List<Vector2Int> currentObstacles=new List<Vector2Int>();//目前的障碍格子
        for(int i = 0; i < column;i++)
        {
            for(int j = 0; j < row;j++)
            {
                if(IsObstacle(new Vector2Int(i, j)))
                {
                    currentObstacles.Add(new Vector2Int(i, j));
                }
            }
        }
        Debug.Log(pathFinder.GeneratePath(currentPosition, destination, currentObstacles)[0]);
        return pathFinder.GeneratePath(currentPosition, destination, currentObstacles)[0];
    }
    private void Initialize()//这个函数用来逐行逐列创建地图，创建后格子的父类为当前类
    {
        tiles = new Tile[column, row];
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                GameObject obj = MonoBehaviour.Instantiate(tilePrefab, new Vector3((i - (column / 2)) * 1.1f, (j - (row / 2))*1.1f,0), Quaternion.identity) as GameObject;

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

    public bool IsObstacle(Vector2Int input)//传入一个格子坐标，返回该格子是否是不可逾越的
    {
        if (tiles[input.x, input.y].tileState == TileState.Occupied || tiles[input.x, input.y].tileState == TileState.Obstacle)
            return true;
        else return false;
    }
    public void SetObstacle(Vector2Int pos)//传入格子坐标，把一个格子设置成不可逾越的
    {
        tiles[pos.x, pos.y].tileState = TileState.Obstacle;
        tiles[pos.x, pos.y].GetComponent<MeshRenderer>().material = obstacleMateiral;
    }
    public void SetOccupied(Vector2Int pos)//传入格子坐标，把一个格子设置成被一方占领的
    {
        tiles[pos.x, pos.y].tileState = TileState.Occupied;

    }
}
