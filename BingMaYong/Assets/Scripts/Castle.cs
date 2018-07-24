using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : Chess {
	public delegate void destroy(int result);
	public static event destroy destroyEvent;
	private int winState; // 当某一方宫殿的血条将为0时，对场控传递的参数
	// Use this for initialization
	void Start () {
		SetBlood(100f);
		if(chessSide == Side.playerA)
			winState = 2;
		else
			winState = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (GetBlood() <= 0) {
			this.gameObject.SetActive(false);
			if(destroyEvent != null) {
				destroyEvent(winState);
			}
		}		
	}
}
