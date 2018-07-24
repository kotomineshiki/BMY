using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IUserGUI : MonoBehaviour {
	IUserAction action;
	GUIStyle labelStyle;
	// Use this for initialization
	void Start () {
		action = Director.getDirector().currentSceneController as IUserAction;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	 void OnGUI() {
        labelStyle = new GUIStyle("label");
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.fontSize = Screen.height/15;
        GUI.color = Color.black;
        if(action.getResult() == 1) {
        	
            GUI.Label(new Rect(Screen.width/2 - Screen.width/8,Screen.height/2 - Screen.height/8,Screen.width/4,Screen.height/4), "A WIN!",labelStyle);
        }
        else if(action.getResult() == 2) {
            GUI.Label(new Rect(Screen.width/2 - Screen.width/8,Screen.height/2 - Screen.height/8,Screen.width/4,Screen.height/4), "B WIN!",labelStyle);
        }
    }
}
