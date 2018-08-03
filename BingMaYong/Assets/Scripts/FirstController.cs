using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FirstController : MonoBehaviour, IUserAction {
	private int result = 0; // 0->游戏中，1->A胜利，2->B胜利
    public RectTransform endPanel;
    public RectTransform losePanel;

    // Use this for initialization
    void Start () {
		Director director = Director.getDirector();
		director.currentSceneController = this;
		Castle.destroyEvent += setResult; // 监听宫殿是否被毁灭，即游戏是否结束
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int getResult() {
		return result;
	}

	public void setResult(int result) {
		this.result = result;
        if(result == 1)
        {
            Time.timeScale = 0;
            losePanel.gameObject.SetActive(true);
        }
        else if(result == 2)
        {
            Time.timeScale = 0;
            endPanel.gameObject.SetActive(true);
        }
	}
}
