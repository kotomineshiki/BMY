using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour {
    public GameObject clickedObjectA;
    public GameObject clickedObjectB;
    private PlayerController playerController;    //玩家控制器
   // public GameObject role;                       //目前没用生成角色所以先在外部创建然后赋值
    // Use this for initialization
    void Start ()
    {
        playerController = Singleton<PlayerController>.Instance;

    }

    // Update is called once per frame
    void Update() {
        //按下空格
		if(Input.GetKeyDown(KeyCode.Space))
        {
            //调用玩家控制器的Move方法
          //  playerController.Move(role,new Vector2(0,0));
        }
        
        if (Input.GetButtonDown("Fire1"))//获得鼠标点击的东西
        {
         //   Debug.Log("Fire");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayhit;
            if (Physics.Raycast(ray, out rayhit))//情况1两个都是空 情况2已经点了物体再点
            {
                Debug.Log("HIt");
                UIAdd(rayhit.transform.gameObject);//向列表中添加物体
            }
            else
            {
                clickedObjectA = null;
                clickedObjectB = null;
            }
            //Debug.Log(rayhit.transform.name);

            UIAct();//执行调用动作
        }
    }
    void UIAct()
    {
        if (clickedObjectA==null)
        {
            Debug.Log("当前无操作");
            return;
        }
        if (clickedObjectA!=null && clickedObjectB ==null)
        {
            Debug.Log("待定");
            //看阵营决定是执行显示细节功能还是没有功能
        }else
        if (clickedObjectA.tag == "Chess" && clickedObjectB.tag == "Chess") {
            Debug.Log("执行A攻击B");
            clickedObjectA = null;
            clickedObjectB = null;
        }else
        if (clickedObjectA.tag == "Chess" && clickedObjectB.tag == "Tile")
        {
            Debug.Log("执行A移动到B");
            //调用玩家控制器的Move方法
            playerController.Move(clickedObjectA, new Vector2(0, 0));

            clickedObjectA = null;
            clickedObjectB = null;
        }

    }
    void UIAdd(GameObject input) {
        if (clickedObjectA == null)
        {//不做筛选，可以自由添加
            if (input.tag == "Chess")
            {
                clickedObjectA = input;
            }else
            if (input.tag == "Tile")
            {
                clickedObjectA = input;
            }
        }else

        if (clickedObjectB == null)
        {
            if (clickedObjectA.tag == "Chess" && input.tag == "Chess")
            {
                if (clickedObjectA.GetComponent<Chess>().chessSide == input.GetComponent<Chess>().chessSide)
                {
                    clickedObjectA = input;//此时后者应该覆盖前者
                }
                else
                {//前后不同的情况
                    if (clickedObjectA.GetComponent<Chess>().chessSide == Side.playerA)
                    {
                        clickedObjectB = input;//正常添加的情况
                    }
                    else
                    {
                        clickedObjectA = input;//A被覆盖掉
                    }
                }
            }
            if (clickedObjectA.tag == "Tile" && input.tag == "Tile")
            {
                clickedObjectA = input;//后者覆盖前者
            }
            if (clickedObjectA.tag == "Tile" && input.tag == "Chess")
            {
                clickedObjectA = input;//后者覆盖前者
            }
            if (clickedObjectA.tag == "Chess" && input.tag == "Tile")
            {
                if (clickedObjectA.GetComponent<Chess>().chessSide == Side.playerB)
                {
                    clickedObjectA = input;//此时A是敌人，所以要让格子顶替掉A
                }
                else
                {
                    clickedObjectB = input;//正常加入
                }

            }
        }else
        if (clickedObjectA != null && clickedObjectB != null)
        {
            Debug.Log("非法溢出");
        }

    }

}
