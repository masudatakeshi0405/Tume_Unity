using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//この宣言が必要

public class Issue_Comment_S0506 : MonoBehaviour
{

    Text myText;
    
    GameObject Ball;

    GameObject img01;
    GameObject img02;
    GameObject img03;
    GameObject img04;
    GameObject img05;
    GameObject img06;
    GameObject img07;
    GameObject img08;
    GameObject img09;
    GameObject　Btm;

    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponentInChildren<Text>();//UIのテキストの取得の仕方
        Ball = GameObject.Find("Ball3");
        img01 = GameObject.Find("Image_issue01");
        img02 = GameObject.Find("Image_issue02");
        img03 = GameObject.Find("Image_issue03");
        img04 = GameObject.Find("Image_issue04");
        img05 = GameObject.Find("Image_issue05");
        img06 = GameObject.Find("Image_issue06");
        img07 = GameObject.Find("Image_issue07");
        img08 = GameObject.Find("Image_issue08");
        img09 = GameObject.Find("Image_issue09");
        Btm = GameObject.Find("next_rally");
    }

    // Update is called once per frame
    void Update()
    {
        switch(Ball.GetComponent<BallControl_S0506>().turncou)
        {
            case 0:
                // myText.text = "下回転とナックルサービスを学ぼう";
                ImgsFlase();
                img01.SetActive(true);
                break;
            case 1:
                // myText.text = "ナックルサービスと下回転サービスは\n相手に攻めさせないための\n基本のサービスです";//テキストの変更
                ImgsFlase();
                img02.SetActive(true);
                break;

            case 2:
                // myText.text = "下回転サービスを確認しよう";
                ImgsFlase();
                img03.SetActive(true);
                break;

            case 4:
                // myText.text = "下回転サービスはボールに\nバックスピンがかかったサービスです\nネット近くに打つことで相手が攻めづらくなります\nドライブを打つと";
                ImgsFlase();
                img04.SetActive(true);
                break;

            case 6:
                // myText.text = "台の上でラケットが振りづらく十分な回転を\nボールにかけられないためネットを超えません";
                ImgsFlase();
                img05.SetActive(true);
                break;

            case 7:
                // myText.text = "そのためツッツキでレシーブします";
                ImgsFlase();
                img06.SetActive(true);
                break;

            case 10:
                // myText.text = "ツッツキはボールに下回転をかける打法であり\nサービスの回転を利用してレシーブができます\nまたネット側に打つことで\n相手攻撃を防ぐこともできます";
                ImgsFlase();
                img07.SetActive(true);
                break;

            case 11:
                // myText.text = "次にナックルサービスについて確認します";
                ImgsFlase();
                img08.SetActive(true);
                break;

            case 14:
                // myText.text = "ナックルサービスは下回転と軌道が似ていますが\n回転が違います\nそのため相手が下回転だと思い\nツッツキを使うと";
                ImgsFlase();
                Btm.SetActive(false);
                img09.SetActive(true);
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
        img06.SetActive(false);
        img07.SetActive(false);
        img08.SetActive(false);
        img09.SetActive(false);
    }
}
