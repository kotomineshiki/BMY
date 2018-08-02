using UnityEngine;
using System.Collections;

public class SetPrefab : MonoBehaviour
{//这个类是放置兵种的中央控制器
    public ChessType chessType;
    public GameObject tile;//选中的格子
    public GameObject newSprite;//新生成的棋子
    public GameObject previewChess;//预览的棋子
    public ChessController chessController;
    // Use this for initialization
    void Awake()
    {
        chessController = Singleton<ChessController>.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetChessType(ChessType input)
    {
        chessType = input;
    }
    //选择卡牌的种类

    public GameObject testPlaceAt(Vector2Int position,ChessType input)
    {
        if (MapController.instance.CanWalk(position)&&MapController.instance.IsSide(position,Side.playerA))//既可以走，也属于该阵营
        {
            previewChess = chessController.testPlaceAt(position,input);
        }
        else
        {
            //Debug.Log("该位置无法放置");
            return null;
        }
        return previewChess;
    }
    public GameObject PlaceAt(Vector2Int position,ChessType input)
    {
        DestroyPreviewChess();
        GameObject temp;
        if (MapController.instance.CanWalk(position) && MapController.instance.IsSide(position, Side.playerA))
        {
            temp=chessController.PlaceChessAt(position,Side.playerA, input);
        }
        else
        {
            Debug.Log("该位置无法放置");
            return null;
        }
        return temp;
    }
    public void DestroyPreviewChess()
    {
        if (previewChess != null)
        {
            chessController.playerA.Remove(previewChess.GetComponent<Chess>());
            DestroyImmediate(previewChess);
            previewChess = null;
        }
       
    }
}
