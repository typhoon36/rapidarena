using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu_Mgr : MonoBehaviour
{
    public GameObject menu;
    public GameObject main;
    public GameObject option;


    public void openMainPage() //처음 화면으로 나오는 함수이다.
    {
        main.SetActive(false);
        option.SetActive(false);
        if ("GameScene" != SceneManager.GetActiveScene().name) 
        {
            Destroy(gameObject);
            Destroy(GameObject.FindWithTag("Player"));
            Destroy(GameObject.FindWithTag("MainCamera"));
            SceneManager.LoadScene("GameScene");
        }
    }

    public void openMain()
    {
        main.SetActive(true);
        option.SetActive(false);
    }

    public void openOption()
    {
        main.SetActive(false);
        option.SetActive(true);
    }

    public void closeAll()
    {
        main.SetActive(false);
        option.SetActive(false);
        Game_Mgr.Inst.m_GameObj.SetActive(false);
        SceneManager.LoadScene("Pt_LobbyScene");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            openMain();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            closeAll();
        }
    }
}
