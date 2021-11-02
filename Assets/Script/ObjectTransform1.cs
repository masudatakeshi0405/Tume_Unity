using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTransform1 : MonoBehaviour
{
    private GameObject Ball;   //プレイヤー情報格納用
    private Vector3 offset;      //相対距離取得用
    GameObject RedLine;

    // Start is called before the first frame update
    void Start()
    {
        //オブジェクトのBallの情報を取得
        Ball = GameObject.Find("Ball3");
        RedLine = GameObject.Find("RedLine");

        // とplayerとの相対距離を求める
        offset = transform.position - Ball.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //の新しい座標の値を代入する
        transform.position = Ball.transform.position;
        if(Ball.GetComponent<RptateTest2>().turn == 0){
            RedLine.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
