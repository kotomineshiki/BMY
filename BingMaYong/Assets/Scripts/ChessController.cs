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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
