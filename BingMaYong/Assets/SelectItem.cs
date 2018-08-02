using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerDownHandler
{
    public bool canUse=false;
    public int cost=3;
    public SetPrefab setController;
    public ChessType chessType;//本兵牌的标号
    public Vector2Int currentTile=new Vector2Int(-1,-1);


    void Update()
    {
        if (FragmentCounter.instance.GetCount() < cost)
        {
            canUse = false;
            this.transform.GetChild(0).GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
        }
        else
        {
            canUse = true;
            this.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (canUse == false) return;//如果不可用，直接截断
                                    //此时应该选中该兵
                                    //   setController.SetChessType(chessType);
                                    //Todo被选中的特效
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canUse == false) return;//如果不可用，直接截断

    }

    public void OnDrag(PointerEventData eventData)
    {//判断当前鼠标位置，如果还在选兵板附近则不生成一个prefab，如果已经在场地里，则在对应格子上放置一个兵
        if (canUse == false) return;//如果不可用，直接截断

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//射出一个射线，看看射线是否碰撞到了格子
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Tile"&& hit.transform.gameObject.GetComponent<Tile>().tilePosition!=currentTile)//&&后面是一点点小优化，避免频繁的删除
            {
                setController.DestroyPreviewChess();
               // this.tile = hit.transform.gameObject;
         //       Debug.Log("放置物体于：" + hit.transform.gameObject.GetComponent<Tile>().tilePosition);
                setController.testPlaceAt(hit.transform.gameObject.GetComponent<Tile>().tilePosition,chessType);
                currentTile = hit.transform.gameObject.GetComponent<Tile>().tilePosition;
            }
            else
            {
            //    Debug.Log("打到了不知道什么东西");
                //setController.DestroyPreviewChess();
            }
        }
        else
        {
        //    Debug.Log("未曾打到");
           // setController.DestroyPreviewChess();
        }



    }

    public void OnEndDrag(PointerEventData eventData)
    {
        setController.DestroyPreviewChess();
        if (canUse == false) return;//如果不可用，直接截断
        if (currentTile == new Vector2Int(-1, -1)) return;//
                                                          //   Debug.Log("DragEnd");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//射出一个射线，看看射线是否碰撞到了格子
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Tile"&&hit.transform.gameObject.GetComponent<Tile>().side==Side.playerA )//&&后面是一点点小优化，避免频繁的删除
            {

                // this.tile = hit.transform.gameObject;
                //       Debug.Log("放置物体于：" + hit.transform.gameObject.GetComponent<Tile>().tilePosition);
             //   setController.testPlaceAt(hit.transform.gameObject.GetComponent<Tile>().tilePosition, chessType);
                currentTile = hit.transform.gameObject.GetComponent<Tile>().tilePosition;
                int value = setController.Cost(chessType);
                FragmentCounter.instance.SubCount(value);
                setController.PlaceAt(currentTile, chessType);
                currentTile = new Vector2Int(-1, -1);//恢复初始状态，方便下次使用
            }

        }




    }
}