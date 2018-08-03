using UnityEngine;
using System.Collections;

public class Global : MonoBehaviour
{
    public static Global instance;

    static Global()
    {

    }

    public void DoSomeThings()
    {
        Debug.Log("DoSomeThings");
    }

    void Start()
    {
        Debug.Log("Start");
    }
    private void Awake()
    {
        if(instance != null)
        {
            GameObject go = (GameObject)Resources.Load("Prefabs\bow");
            DontDestroyOnLoad(go);
            instance = go.AddComponent<Global>();
        }
    }

}