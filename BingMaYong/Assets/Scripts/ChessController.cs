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
    public GameObject castleChessPrefab;

    public GameObject testPlaceAt(Vector2Int placeAt,ChessType chessType)//这个类是用来预览放置效果的，不会影响场上局面
    {
        GameObject temp;
        if (chessType == ChessType.Infantry)
        {
            temp= Instantiate(infantryChessPrefab);
        }
        else if(chessType==ChessType.Shoot)
        {
            temp = Instantiate(shootChessPrefab);
        }
        else
        {
            temp = Instantiate(carChessPrefab);
        }
        temp.transform.position = MapController.instance.GetWorldPosition(placeAt);
        return temp;
    }
    /*todo 在某一格子上放置一个棋子*/
    public GameObject PlaceChessAt(Vector2Int placeAt,Side side,ChessType chessType)
    {
        if (chessType == ChessType.Castle)//堡垒是独立的一个逻辑
        {
            GameObject cas = Instantiate(castleChessPrefab);

            if (side == Side.playerA)
            {
                Vector3 pos1 = this.transform.parent.GetChild(0).GetComponent<MapController>().GetWorldPosition(new Vector2Int(4, 0));
                Vector3 pos2= this.transform.parent.GetChild(0).GetComponent<MapController>().GetWorldPosition(new Vector2Int(5, 0));
                cas.transform.position = (pos1 + pos2) / 2;
                MapController.instance.SetObstacle(new Vector2Int(4, 0));
                MapController.instance.SetObstacle(new Vector2Int(5, 0));
                cas.transform.GetChild(2).GetComponent<Chess>().SetCurrentPosition(new Vector2Int(4, 0));
                cas.transform.GetChild(3).GetComponent<Chess>().SetCurrentPosition(new Vector2Int(5, 0));
                cas.transform.GetChild(2).GetComponent<Chess>().chessSide = side;
                cas.transform.GetChild(3).GetComponent<Chess>().chessSide = side;
            }
            else if (side == Side.playerB)
            {
                Vector3 pos1 = this.transform.parent.GetChild(0).GetComponent<MapController>().GetWorldPosition(new Vector2Int(4, 13));
                Vector3 pos2 = this.transform.parent.GetChild(0).GetComponent<MapController>().GetWorldPosition(new Vector2Int(5, 13));
                cas.transform.position = (pos1 + pos2) / 2;
                MapController.instance.SetObstacle(new Vector2Int(4, 13));
                MapController.instance.SetObstacle(new Vector2Int(5, 13));
                cas.transform.GetChild(2).GetComponent<Chess>().SetCurrentPosition(new Vector2Int(4, 13));
                cas.transform.GetChild(3).GetComponent<Chess>().SetCurrentPosition(new Vector2Int(5, 13));
                cas.transform.GetChild(2).GetComponent<Chess>().chessSide = side;
                cas.transform.GetChild(3).GetComponent<Chess>().chessSide = side;
            }

            return cas;
        }
        GameObject temp;
        Vector3 pos = this.transform.parent.GetChild(0).GetComponent<MapController>().GetWorldPosition(placeAt);
        //temp = Instantiate(chessPrefab, pos, Quaternion.identity);
        if(ChessType.Car == chessType)
            temp = Instantiate(carChessPrefab);
        else if (ChessType.Infantry == chessType)
            temp = Instantiate(infantryChessPrefab);
        else //if(ChessType.Shoot==chessType)
            temp = Instantiate(shootChessPrefab);
// if (ChessType.Castle == chessType)


        temp.transform.position = pos;
        if(side==Side.playerA)playerA.Add(temp.GetComponent<Chess>());
        if (side == Side.playerB) playerB.Add(temp.GetComponent<Chess>());
        temp.GetComponent<Chess>().SetChessSide(side);
        temp.GetComponent<Chess>().SetCurrentPosition(placeAt);


        temp.GetComponent<Chess>().OccupyPosition(placeAt);//占领当前位置

        Tile tempTile = Singleton<MapController>.Instance.GetTileWithPosition(placeAt);
        tempTile.occupyChess = temp;

        temp.GetComponent<Chess>().OnWalk += HandleOnWalk;
        return temp;
    }
    void HandleOnWalk(Vector2Int pos)
    {
        Debug.Log("Yes");

    }
	// Use this for initialization
	void Start () {
     PlaceChessAt(new Vector2Int(3, 3),Side.playerA,ChessType.Car);
    //   PlaceChessAt(new Vector2Int(5, 8), Side.playerA, ChessType.Infantry);
    //   PlaceChessAt(new Vector2Int(6, 8), Side.playerA, ChessType.Shoot);
       PlaceChessAt(new Vector2Int(7, 3), Side.playerB, ChessType.Infantry);
       PlaceChessAt(new Vector2Int(1, 1), Side.playerB, ChessType.Shoot);
       PlaceChessAt(new Vector2Int(2, 4), Side.playerB, ChessType.Car);
        PlaceChessAt(new Vector2Int(0, 3), Side.playerA, ChessType.Castle);
        PlaceChessAt(new Vector2Int(0, 3), Side.playerB, ChessType.Castle);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
