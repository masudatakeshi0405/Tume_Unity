using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainScript : MonoBehaviour
{

    GameObject TutScene;
    GameObject S_Scene;

    public bool TutSceneflag = false;
    public bool S_Sceneflag = false;

    // Start is called before the first frame update
    void Start()
    {
        TutScene = GameObject.Find("TokkunSelect");
        //S_Scene = GameObject.Find("ServceSelectScene");
        TutScene.SetActive(false);
        //S_Scene.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(TutSceneflag==true){
            TutScene.SetActive(true);
        }else{
            TutScene.SetActive(false);
        }

        // if(S_Sceneflag==true){
        //     S_Scene.SetActive(true);
        // }else{
        //     S_Scene.SetActive(false);
        // }
    }
}
