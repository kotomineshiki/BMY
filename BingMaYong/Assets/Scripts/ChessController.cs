using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Side{//这个枚举类用来表明棋子的阵营，也用来标识格子所属的阵营
    playerA,
    playerB,
    neutral
}
public class ChessController : MonoBehaviour {
    public List<Chess> playerA;
    public List<Chess> playerB;
    public GameObject chessPrefab;
    /*todo 在某一格子上放置一个棋子*/
    public void PlaceChessAt(Vector2Int placeAt,Side side)
    {
        GameObject temp;
        Vector3 pos = this.transform.parent.GetChild(0).GetComponent<MapController>().GetWorldPosition(placeAt);
        //temp = Instantiate(chessPrefab, pos, Quaternion.identity);
        temp = Instantiate(chessPrefab);
        temp.transform.position = pos;
        if(side==Side.playerA)playerA.Add(temp.GetComponent<Chess>());
        if (side == Side.playerB) playerB.Add(temp.GetComponent<Chess>());
        temp.GetComponent<Chess>().chessSide = side;
        temp.GetComponent<Chess>().SetCurrentPosition(placeAt);
        temp.GetComponent<Chess>().OccupyCurrentPosition();//占领当前位置
    }
    
	// Use this for initialization
	void Start () {
        PlaceChessAt(new Vector2Int(3, 3),Side.playerA);
        PlaceChessAt(new Vector2Int(5, 8), Side.playerB);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
