using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutBtm : MonoBehaviour
{

    GameObject G_System;

    public void OnClick()
    {
        G_System.GetComponent<mainScript>().TutSceneflag = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        G_System = GameObject.Find("GameSystem");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
