using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    private Button StartButton;

    void Start()
    {
        StartButton.onClick.AddListener(GameStart);  
    }
    private void GameStart()
    {
        SceneManager.LoadScene(1);
        //ƒQ[ƒ€‰æ–Ê‘JˆÚ

    }
}
