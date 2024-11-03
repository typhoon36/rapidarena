using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigBox : MonoBehaviour
{
    public Button m_Ok_Btn = null;
    public Button m_Close_Btn = null;

    public Toggle m_Sound_Toggle = null;
    public Slider m_Sound_Slider = null;


    void Start()
    {
        if (m_Ok_Btn != null)
            m_Ok_Btn.onClick.AddListener(OkBtnClick);

        if (m_Close_Btn != null)
            m_Close_Btn.onClick.AddListener(CloseBtnClick);

        if (m_Sound_Toggle != null)
            m_Sound_Toggle.onValueChanged.AddListener(SoundOnOff);
        //체크 상태가 변경되었을 때 호출되는 함수를 대기하는 코드

        if (m_Sound_Slider != null)
            m_Sound_Slider.onValueChanged.AddListener(SliderChanged);
        //슬라이드 상태가 변경 되었을 때 호출되는 함수 대기하는 코드



        //--- 체크상태, 슬라이드상태, 닉네임 로딩 후 UI 컨트롤에 적용
        int a_SoundOnOff = PlayerPrefs.GetInt("SoundOnOff", 1);
        if (m_Sound_Toggle != null)
        {

            m_Sound_Toggle.isOn = (a_SoundOnOff == 1) ? true : false;
        }

        if (m_Sound_Slider != null)
            m_Sound_Slider.value = PlayerPrefs.GetFloat("SoundVolume", 1.0f);

    }


    void OkBtnClick()
    {

        Time.timeScale = 1.0f;  
        Destroy(gameObject);
    }

    void CloseBtnClick()
    {
        Time.timeScale = 1.0f;  
        Destroy(gameObject);
    }

    void SoundOnOff(bool value) 
    {

        int a_IntV = (value == true) ? 1 : 0;
        PlayerPrefs.SetInt("SoundOnOff", a_IntV);

    }

    private void SliderChanged(float value)
    {
        PlayerPrefs.SetFloat("SoundVolume", value);
        Sound_Mgr.Inst.SoundVolume(value);
    }
}
