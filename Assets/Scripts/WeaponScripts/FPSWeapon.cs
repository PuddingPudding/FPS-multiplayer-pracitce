using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSWeapon : MonoBehaviour
{
    private GameObject m_muzzleFlash;
    // Use this for initialization
    void Awake()
    {
        this.m_muzzleFlash = this.transform.Find("Muzzle Flash").gameObject;
        //transform.Find會沿著自己在Hierarchy底下的子物件找
        this.m_muzzleFlash.SetActive(false);
    }

    public void Shoot()
    {
        StartCoroutine(this.TurnOnMuzzleFlash());
    }

    IEnumerator TurnOnMuzzleFlash()
    {
        this.m_muzzleFlash.transform.localEulerAngles = new Vector3(Random.Range(0f, 360f)
        , this.m_muzzleFlash.transform.localEulerAngles.y
        , this.m_muzzleFlash.transform.localEulerAngles.z); //讓子彈火花每次出現時都隨機轉方向

        this.m_muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        this.m_muzzleFlash.SetActive(false);
    }
}
