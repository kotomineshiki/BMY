using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFragment : MonoBehaviour {
	// Use this for initialization
    //爆炸后碎片+1在两秒后消失
	void Start () {
        StartCoroutine(Disappeare());
    }
    IEnumerator Disappeare()//每隔一段时间思考一次
    {
        yield return new WaitForSeconds(2f);
        this.gameObject.SetActive(false);
        Destroy(this.gameObject.transform.parent.gameObject);
    }
}
