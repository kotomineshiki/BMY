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
    public GameObject infantryChessPrefab;
    public GameObject carChessPrefab;
    public GameObject shootChessPrefab;

    /*todo 在某一格子上放置一个棋子*/
    public void PlaceChessAt(Vector2Int placeAt,Side side,ChessType chessType)
    {
        GameObject temp;
        Vector3 pos = this.transform.parent.GetChild(0).GetComponent<MapController>().GetWorldPosition(placeAt);
        //temp = Instantiate(chessPrefab, pos, Quaternion.identity);
        if(ChessType.Car == chessType)
            temp = Instantiate(carChessPrefab);
        else if (ChessType.Infantry == chessType)
            temp = Instantiate(infantryChessPrefab);
        else
            temp = Instantiate(shootChessPrefab);

        temp.transform.position = pos;
        if(side==Side.playerA)playerA.Add(temp.GetComponent<Chess>());
        if (side == Side.playerB) playerB.Add(temp.GetComponent<Chess>());
        temp.GetComponent<Chess>().chessSide = side;
        temp.GetComponent<Chess>().SetCurrentPosition(placeAt);
<<<<<<< HEAD
        temp.GetComponent<Chess>().OccupyCurrentPosition();//占领当前位置
=======
        temp.GetComponent<Chess>().OccupyPosition(placeAt);//占领当前位置

        Tile tempTile = Singleton<MapController>.Instance.GetTileWithPosition(placeAt);
        tempTile.occupyChess = temp;
>>>>>>> 90b174afedd190de36852bea4948bd8a652859ce
    }
    
	// Use this for initialization
	void Start () {
        PlaceChessAt(new Vector2Int(3, 3),Side.playerA,ChessType.Car);
        PlaceChessAt(new Vector2Int(5, 8), Side.playerA, ChessType.Infantry);
        PlaceChessAt(new Vector2Int(6, 8), Side.playerA, ChessType.Shoot);
        PlaceChessAt(new Vector2Int(7, 3), Side.playerB, ChessType.Infantry);
        PlaceChessAt(new Vector2Int(1, 1), Side.playerB, ChessType.Shoot);
        PlaceChessAt(new Vector2Int(2, 4), Side.playerB, ChessType.Car);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
