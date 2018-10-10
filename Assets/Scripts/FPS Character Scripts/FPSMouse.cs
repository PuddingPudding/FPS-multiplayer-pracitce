using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMouse : MonoBehaviour
{
    public enum ERotationAxes
    {
        MOUSE_X,
        MOUSE_Y
    }
    [SerializeField] ERotationAxes m_axes = ERotationAxes.MOUSE_Y;

    private float m_fCurrentSensivityX = 1.5f;
    private float m_fCurrentSensivityY = 1.5f;

    private float m_fSensivityX = 1.5f;
    private float m_fSensivityY = 1.5f;

    private float m_fRotationX, m_fRotationY;

    private float m_fMinX = -360;
    private float m_fMaxX = 360;

    private float m_fMinY = -60;
    private float m_fMaxY = 60;

    private Quaternion m_originRotation;

    private float m_fMouseSensivity = 1.7f;
    // Use this for initialization
    void Start()
    {
        this.m_originRotation = this.transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.HandleRotation();
    }

    float ClampAngle(float _fAngle , float _fMin , float _fMax)
    {
        if(_fAngle < -360)
        {
            _fAngle += 360;
        }
        if(_fAngle > 360)
        {
            _fAngle -= 360;
        }

        return Mathf.Clamp(_fAngle, _fMin, _fMax);
    }

    void HandleRotation()
    {
        if (this.m_fCurrentSensivityX != this.m_fMouseSensivity || this.m_fCurrentSensivityY != this.m_fMouseSensivity)
        {
            this.m_fCurrentSensivityX = this.m_fCurrentSensivityY = this.m_fMouseSensivity;
        } //上面這一段主要是要每次調整MouseSensivity時去自動更改另外兩個

        this.m_fSensivityX = this.m_fCurrentSensivityX;
        this.m_fSensivityY = this.m_fCurrentSensivityY;

        if (m_axes == ERotationAxes.MOUSE_X)
        {
            this.m_fRotationX += Input.GetAxis("Mouse X") * this.m_fSensivityX;

            this.m_fRotationX = this.ClampAngle(this.m_fRotationX, this.m_fMinX, this.m_fMaxX);
            Quaternion xQuat = Quaternion.AngleAxis(this.m_fRotationX, Vector3.up);
            //繞著up，也就是人物頭頂的軸線去旋轉
            this.transform.localRotation = this.m_originRotation * xQuat;
        }
        if (m_axes == ERotationAxes.MOUSE_Y)
        {
            this.m_fRotationY += Input.GetAxis("Mouse Y") * this.m_fSensivityY;

            this.m_fRotationY = this.ClampAngle(this.m_fRotationY, this.m_fMinY, this.m_fMaxY);
            //先把旋轉度數設定為合理範圍之度數
            Quaternion yQuat = Quaternion.AngleAxis(-this.m_fRotationY, Vector3.right);
            //繞著right，也就是人物向右的軸線去旋轉
            this.transform.localRotation = this.m_originRotation * yQuat;
        }
    }
}
