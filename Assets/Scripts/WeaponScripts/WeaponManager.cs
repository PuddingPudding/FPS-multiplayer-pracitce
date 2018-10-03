using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_listWeapons;
    
    public List<GameObject> Weapons
    {
        get { return this.m_listWeapons; }
    }

}
