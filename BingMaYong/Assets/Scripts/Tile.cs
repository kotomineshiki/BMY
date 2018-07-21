using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileState{
    Obstacle,//先天不能走的
    Occupied,//因为被占领了，所以不能走的
    Idle
}

public class Tile : MonoBehaviour {
    public Vector2Int tilePosition;//这是一个坐标，标识本物体在格子地图中的逻辑位置
    public TileState tileState=TileState.Idle;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
