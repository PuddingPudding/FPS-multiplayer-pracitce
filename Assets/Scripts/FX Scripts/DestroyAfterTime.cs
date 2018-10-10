using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float m_fLifeTime = 1f;
    // Use this for initialization
    void Start()
    {
        Destroy(this.gameObject, m_fLifeTime);
    }

}
