using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�� �Ŵ���
public class Team_Mgr : MonoBehaviour
{
    void Start()
    {
        Game_Mgr.Inst.UpdateTeamCounts();
    }
}
