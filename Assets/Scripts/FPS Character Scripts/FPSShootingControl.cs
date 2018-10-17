using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FPSShootingControl : NetworkBehaviour
{
    [SerializeField] private GameObject m_concreteImpact, m_bloodImpact;
    [SerializeField] private float m_fDmgAmount = 5f;
    [SerializeField] private Camera m_mainCam;

    private float m_fFireRate = 10f;
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
                //print("mainCam.eulerAngles: " + m_mainCam.transform.eulerAngles + "mainCam.localEulerAngles: " + m_mainCam.transform.localEulerAngles);
                //print("mainCam.forword: " + m_mainCam.transform.forward);
                print("tag: " + hit.transform.tag);
                //print("咱們射到 " + hit.collider.gameObject.name);
                //print("射到的物件之位子 " + hit.transform.position);
                //print("射擊到的點為 " + hit.point);
                if(hit.transform.tag == "Enemy")
                {
                    CmdDealDmg(hit.transform.gameObject, hit.point, hit.normal);
                }
                else
                {
                    Instantiate(this.m_concreteImpact, hit.point, Quaternion.LookRotation(hit.normal));
                }                
            }
        }
    }

    [Command]
    void CmdDealDmg(GameObject _obj , Vector3 _pos , Vector3 _rotation)
    {
        _obj.GetComponent<FPSHealth>().TakeDmg(m_fDmgAmount);
        Instantiate(m_bloodImpact, _pos, Quaternion.LookRotation(_rotation));
        print("有打到人了");
    }
}
