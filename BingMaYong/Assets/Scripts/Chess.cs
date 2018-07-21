using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chess : MonoBehaviour {

    public Side chessSide;

    GameObject player;
    GameObject enemy;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Chess");
	}
	
	// Update is called once per frame
	void Update () {
        Attack(player,enemy);
	}

    void Attack(GameObject player,GameObject enemy)
    {
        if (Input.GetButtonDown("Fire1"))
        {
            player.GetComponent<Animator>().SetTrigger("IsChessAttack");
        }
    }
}
