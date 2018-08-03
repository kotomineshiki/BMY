using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FirstController : MonoBehaviour, IUserAction {
	private int result = 0; // 0->游戏中，1->A胜利，2->B胜利
    public GameObject endPanel;
    public GameObject losePanel;

    // Use this for initialization
    void Start () {
		Director director = Director.getDirector();
		director.currentSceneController = this;
		Castle.destroyEvent += setResult; // 监听宫殿是否被毁灭，即游戏是否结束
        Timer.timeEndEvent += timeEndResult; //时间是否停止，结算游戏占有格子

    }
	
	// Update is called once per frame
	void Update () {
		
	}

	public int getResult() {
		return result;
	}

	public void setResult(int result) {
		this.result = result;
        //根据结果显示哪一个界面
        if(result == 1)
        {
            Time.timeScale = 0;
            GameObject tempGo = losePanel ?? null;
            if (tempGo == null)
            {
                GameObject root = GameObject.Find("Canvas");
                losePanel = root.transform.Find("LosePanel").gameObject;
            }

            losePanel.SetActive(true);
        }
        else if(result == 2)
        {
            Time.timeScale = 0;
            GameObject tempGo = endPanel ?? null;
            if (tempGo == null)
            {
                GameObject root = GameObject.Find("Canvas");
                endPanel = root.transform.Find("EndPanel").gameObject;
            }
            endPanel.SetActive(true);
        }
	}
    public void timeEndResult()
    {
        int column = 10;//列数目
        int row = 14;//行数目
        int playerAcount = 0;
        int playerBcount = 0;
        Tile[,] tiles = Singleton<MapController>.Instance.tiles;
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (tiles[i, j].side == Side.playerA)
                    playerAcount++;
                else if (tiles[i, j].side == Side.playerB)
                    playerBcount++;
            }
        }
        if(playerBcount >= playerAcount)
        {
            Time.timeScale = 0;
            GameObject tempGo = losePanel ?? null;
            if (tempGo == null)
            {
                GameObject root = GameObject.Find("Canvas");
                losePanel = root.transform.Find("LosePanel").gameObject;
            }

            losePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 0;
            GameObject tempGo = endPanel ?? null;
            if (tempGo == null)
            {
                GameObject root = GameObject.Find("Canvas");
                endPanel = root.transform.Find("EndPanel").gameObject;
            }
            endPanel.SetActive(true);
        }
    }
}
