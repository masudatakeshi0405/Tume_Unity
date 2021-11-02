using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//この宣言が必要

public class Issue_Comment : MonoBehaviour
{

    Text myText;
    
    GameObject Ball;

    GameObject img01;
    GameObject img02;
    GameObject img03;
    GameObject img04;
    GameObject img05;
    GameObject imgL01;
    GameObject imgL02;
    GameObject imgL03;
    GameObject imgL04;
    GameObject imgL05;
    GameObject NextTextBtm;
    GameObject NextRallyBtm;


    // Start is called before the first frame update
    void Start()
    {
        
        // myText = GetComponentInChildren<Text>();//UIのテキストの取得の仕方
        Ball = GameObject.Find("Ball3");
        img01 = GameObject.Find("Image_issue01");
        img02 = GameObject.Find("Image_issue02");
        img03 = GameObject.Find("Image_issue03");
        img04 = GameObject.Find("Image_issue04");
        img05 = GameObject.Find("Image_issue05");
        imgL01 = GameObject.Find("Image_learning01");
        imgL02 = GameObject.Find("Image_learning02");
        imgL03 = GameObject.Find("Image_learning03");
        imgL04 = GameObject.Find("Image_learning04");
        imgL05 = GameObject.Find("Image_learning05");
        NextTextBtm = GameObject.Find("next_text");
        NextRallyBtm = GameObject.Find("next_rally");
        // ImgsFlase();
    }

    // Update is called once per frame
    void Update()
    {  
        // Debug.Log(Ball.GetComponent<BallControl_BMC>().issue_num);
        switch(Ball.GetComponent<BallControl_BMC>().issue_num){
            case 1:
                if(Ball.GetComponent<BallControl_BMC>().turncou==-3){
                    ImgsFlase();
                    imgL01.SetActive(true);
                    NextTextBtm.SetActive(true);
                    NextRallyBtm.SetActive(false);
                }else if(Ball.GetComponent<BallControl_BMC>().turncou==-2){
                    ImgsFlase();
                    img01.SetActive(true);
                    NextTextBtm.SetActive(false);
                    NextRallyBtm.SetActive(true);
                }
                // myText.text = "問題\n相手はドライブでレシーブしてくる\nネットを越えさせないために\n正しいサービスを打て";//テキストの変更
                break;

            case 2:
                if(Ball.GetComponent<BallControl_BMC>().turncou==-3){
                    ImgsFlase();
                    imgL02.SetActive(true);
                    NextTextBtm.SetActive(true);
                    NextRallyBtm.SetActive(false);
                }else if(Ball.GetComponent<BallControl_BMC>().turncou==-2){
                    ImgsFlase();
                    img02.SetActive(true);
                    NextTextBtm.SetActive(false);
                    NextRallyBtm.SetActive(true);
                }
                // myText.text = "問題\n3球目にフォアドライブを打つために\n正しいサービスを打て\n相手はクロスに長いツッツキを打つ傾向があります";//テキストの変更
                break;

            case 3:
                if(Ball.GetComponent<BallControl_BMC>().turncou==-3){
                    ImgsFlase();
                    imgL03.SetActive(true);
                    NextTextBtm.SetActive(true);
                    NextRallyBtm.SetActive(false);
                }else if(Ball.GetComponent<BallControl_BMC>().turncou==-2){
                    ImgsFlase();
                    img03.SetActive(true);
                    NextTextBtm.SetActive(false);
                    NextRallyBtm.SetActive(true);
                }
                // myText.text = "問題\n相手はツッツキでレシーブしてくる\n3打目に強打するために正しいサービスを打て";//テキストの変更
                break;

            case 4:
                if(Ball.GetComponent<BallControl_BMC>().turncou==-3){
                    ImgsFlase();
                    imgL04.SetActive(true);
                    NextTextBtm.SetActive(true);
                    NextRallyBtm.SetActive(false);
                }else if(Ball.GetComponent<BallControl_BMC>().turncou==-2){
                    ImgsFlase();
                    img04.SetActive(true);
                    NextTextBtm.SetActive(false);
                    NextRallyBtm.SetActive(true);
                }
                // myText.text = "問題\n3球目にフリックを打つために\n正しいサービスを打て\n相手はネット側のサービスは\n正面のコースに返す傾向があります";//テキストの変更
                break;

            case 5:
                if(Ball.GetComponent<BallControl_BMC>().turncou==-3){
                    ImgsFlase();
                    imgL05.SetActive(true);
                    NextTextBtm.SetActive(true);
                    NextRallyBtm.SetActive(false);
                }else if(Ball.GetComponent<BallControl_BMC>().turncou==-2){
                    ImgsFlase();
                    img05.SetActive(true);
                    NextTextBtm.SetActive(false);
                    NextRallyBtm.SetActive(true);
                }
                // myText.text = "問題\n3球目にチキータを打つために\n正しいサービスを打て\n相手はネット側のサービスは\n正面のコースに返す傾向があります";//テキストの変更
                break;
        }
    }

    void ImgsFlase()
    {
        img01.SetActive(false);
        img02.SetActive(false);
        img03.SetActive(false);
        img04.SetActive(false);
        img05.SetActive(false);
        imgL01.SetActive(false);
        imgL02.SetActive(false);
        imgL03.SetActive(false);
        imgL04.SetActive(false);
        imgL05.SetActive(false);
    }
}
