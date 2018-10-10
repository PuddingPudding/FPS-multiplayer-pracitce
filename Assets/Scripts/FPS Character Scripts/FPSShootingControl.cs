using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSShootingControl : MonoBehaviour
{
    [SerializeField] private GameObject m_concreteImpact;

    [SerializeField] private Camera m_mainCam;

    private float m_fFireRate = 15f;
    private float m_fNextTimeToFire = 0f;

    // Use this for initialization
    void Start()
    {
        //this.m_mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        this.Shoot();
    }

    void Shoot()
    {
        if(Input.GetMouseButtonDown(0) && Time.time > m_fNextTimeToFire)
        {
            m_fNextTimeToFire = Time.time + 1f / m_fFireRate;

            RaycastHit hit;
            //Instantiate(this.m_concreteImpact, m_mainCam.transform.position + (m_mainCam.transform.forward * 2), Quaternion.identity);
            //print("camera的向前角度: " + m_mainCam.transform.forward);
            //print("camera的世界角度: " + m_mainCam.transform.rotation + "camera的個人角度: " + m_mainCam.transform.localRotation);

            if (Physics.Raycast(m_mainCam.transform.position, m_mainCam.transform.forward, out hit))
            {
                print("mainCam.eulerAngles: " + m_mainCam.transform.eulerAngles + "mainCam.localEulerAngles: " + m_mainCam.transform.localEulerAngles);
                print("mainCam.forword: " + m_mainCam.transform.forward);
                //print("咱們射到 " + hit.collider.gameObject.name);
                //print("射到的物件之位子 " + hit.transform.position);
                //print("射擊到的點為 " + hit.point);

                Instantiate(this.m_concreteImpact, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }
}
