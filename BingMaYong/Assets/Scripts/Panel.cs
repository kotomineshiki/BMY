using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    public Button button;   
     // Use this for initialization
    void Awake () {
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Scenes/New Test Scene YQ");
    }
}
