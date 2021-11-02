using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//この宣言が必要

public class Comment21 : MonoBehaviour
{

    GameObject NextBtm;
    Text myText;

    // Start is called before the first frame update
    void Start()
    {
        NextBtm = GameObject.Find("next");
        myText = GetComponentInChildren<Text>();//UIのテキストの取得の仕方
		myText.text = "レシーブを打とう！";//テキストの変更
    }

    // Update is called once per frame
    void Update()
    {
        if(NextBtm.GetComponent<NextBtm2>().count==2){
            myText.text = "まず相手が\nサービスを打ちます";//テキストの変更
        }else if(NextBtm.GetComponent<NextBtm2>().count==3){
            myText.text = "サービスが\n通り過ぎる前に";//テキストの変更
        }else if(NextBtm.GetComponent<NextBtm2>().count==4){
            myText.text = "コースを選択して\nレシーブを打とう！";//テキストの変更
        }else if(NextBtm.GetComponent<NextBtm2>().count==6){
            myText.text = "ナイスレシーブ！";//テキストの変更
        }else{
            myText.text = "レシーブを打とう！";//テキストの変更
        }
    }
}
