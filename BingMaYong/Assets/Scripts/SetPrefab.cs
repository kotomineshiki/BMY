using UnityEngine;
using System.Collections;

public class SetPrefab : MonoBehaviour
{
    public int selected;//选择的卡牌种类
    public GameObject tile;//选中的格子
    public GameObject newSprite;//新生成的棋子

    public ChessController chessController;
    // Use this for initialization
    void Start()
    {
        chessController = Singleton<ChessController>.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //主要方法是一直判断鼠标所指的是tile还是其他东西，以便后面的判断
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Tile")
            {
                this.tile = hit.transform.gameObject;
            }
            else
            {
                this.tile = null;
            }
        }
        else
        {
            this.tile = null;
        }
        if (Input.GetMouseButtonDown(0) && this.tile != null && selected != 0)
        {
            Tile tileTakenScript = this.tile.GetComponent<Tile>();
            if (tileTakenScript.tileState == TileState.Idle)
            {
                Debug.Log("放置");
                chessController.PlaceChessAt(tileTakenScript.tilePosition, Side.playerA,ChessType.Infantry);
                selected = 0;
            }
        }
        if(Input.GetMouseButtonDown(0) && this.tile == null)//点击地图之外的取消选择卡牌
        {
            selected = 0;
        }
    }
    public void SetPrefabInt(int number)
    {
        selected = number;//被选中的标码设置
    }
    //选择卡牌的种类
    public void SetKindOfBoardA()
    {
        selected = 1;
        Debug.Log("selected1");
    }

    public void SetKindOfBoardB()
    {
        selected = 2;
    }
    public void SetKindOfBoardC()
    {
        selected = 3;
    }
}
