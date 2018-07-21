using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {
    public Tile[,] tiles;
    public GameObject tilePrefab;

    public int column = 10;//列数目
    public int row = 14;//行数目

    public PathFinder pathFinder;
	// Use this for initialization
	void Awake () {
        Initialize();
        pathFinder = new PathFinder();
	}
	
	// Update is called once per frame
	void Update () {
		
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
}
