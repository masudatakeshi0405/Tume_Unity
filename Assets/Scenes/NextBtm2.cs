using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextBtm2 : MonoBehaviour
{

    Button button;　// ボタンのコンポーネント


    GameObject text;
    GameObject tesxt2;

    public float count;


    //-------------点滅変数----------------
    //public
    public float speed = 1.0f;

    //private
    private Text text3;
    private Image image;
    private float time;

    private enum ObjType{
        TEXT,
        IMAGE
    };
    private ObjType thisObjType = ObjType.TEXT;
    //-------------点滅変数----------------


    public void OnClick()
    {
        Debug.Log("押された！"); //
        if(count<5){
            count++;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        text = GameObject.Find("comment");
        tesxt2 = GameObject.Find("comment2");

        //-------------点滅初期----------------
        //アタッチしてるオブジェクトを判別
        if (this.gameObject.GetComponent<Image>()) {
            thisObjType = ObjType.IMAGE;
            image = this.gameObject.GetComponent<Image>();
        }else if (this.gameObject.GetComponent<Text>()) {
            thisObjType = ObjType.TEXT;
            text3 = this.gameObject.GetComponent<Text>();
        }
        //-------------点滅変数----------------
    }

    // Update is called once per frame
    void Update()
    {
        
        //-------------点滅----------------
        //オブジェクトのAlpha値を更新
        // if (thisObjType == ObjType.IMAGE) {
        //     image.color = GetAlphaColor(image.color);
        // }
        // else if (thisObjType == ObjType.TEXT) {
        //     text3.color = GetAlphaColor(text3.color);
        // }
        if(count<5){
            image.color = GetAlphaColor(image.color);
        }else{
            image.color = GetAlphaColorZero(image.color);
        }
        //-------------点滅----------------
        
        if(count==1){
            text.SetActive(true);
            tesxt2.SetActive(false);
        }else if(count==2){
            text.SetActive(true);
            tesxt2.SetActive(false);
        }else if(count==3){
            text.SetActive(true);
            tesxt2.SetActive(false);
        }else if(count==4){
            text.SetActive(true);
            tesxt2.SetActive(false);
        }else if(count==6){
            text.SetActive(true);
        }else{
            text.SetActive(false);
            tesxt2.SetActive(false);
        }
    }

    //-------------点滅関数----------------
    //Alpha値を更新してColorを返す
    Color GetAlphaColor(Color color) {
        time += Time.deltaTime * 5.0f * speed;
        color.a = Mathf.Sin(time) * 0.5f + 0.5f;

        return color;
    }

    Color GetAlphaColorZero(Color color) {
        //time += Time.deltaTime * 5.0f * speed;
        color.a = 0f;

        return color;
    }
}
