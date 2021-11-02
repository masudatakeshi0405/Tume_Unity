using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ss0207_Btm : MonoBehaviour
{

    GameObject Start_Btm;

    public void OnClick()
    {
        Start_Btm.GetComponent<NextScen_Btm>().scen_num = 1;
        Debug.Log(Start_Btm.GetComponent<NextScen_Btm>().scen_num);
    }

    // Start is called before the first frame update
    void Start()
    {
        Start_Btm = GameObject.Find("strat");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
