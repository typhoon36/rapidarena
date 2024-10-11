using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class Item_Mgr : MonoBehaviour
{
    #region SingleTon
    public static Item_Mgr Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion
}
