using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButton : MonoBehaviour
{
    public Animator popUpClip;


    void Start()
    {
        popUpClip = GetComponent<Animator>();
    }
    public void StartPopUp()
    {
        popUpClip.SetBool("IsOpen", true);
    }

    public void ClosePopUp()
    {
        popUpClip.SetBool("IsOpen", false);
    }
    

}
