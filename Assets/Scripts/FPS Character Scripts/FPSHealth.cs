using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FPSHealth : NetworkBehaviour
{
    [SyncVar, SerializeField]
    private float m_fHP = 20;
    
    public void TakeDmg(float _fDmg)
    {
        if(this.isServer)
        {
            m_fHP -= _fDmg;
            print("DAMAGE RECEIVED " + m_fHP);

            if(m_fHP <= 0)
            {

            }
        }
    }
}
