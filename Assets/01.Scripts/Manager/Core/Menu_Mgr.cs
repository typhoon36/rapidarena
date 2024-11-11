using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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


    public static bool IsPointerOverUIObject() //UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
    {
        PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)

			List<RaycastResult> results = new List<RaycastResult>();
			for (int i = 0; i < Input.touchCount; ++i)
			{
				a_EDCurPos.position = Input.GetTouch(i).position;  
				results.Clear();
				EventSystem.current.RaycastAll(a_EDCurPos, results);
                if (0 < results.Count)
                    return true;
			}

			return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }


}
