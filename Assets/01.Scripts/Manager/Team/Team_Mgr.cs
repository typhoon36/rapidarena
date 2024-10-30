using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ÆÀ ¸Å´ÏÀú
public class Team_Mgr : MonoBehaviour
{
    void Start()
    {
        Game_Mgr.Inst.UpdateTeamCounts();
    }
}
