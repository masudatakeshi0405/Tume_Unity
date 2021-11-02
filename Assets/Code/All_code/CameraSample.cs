using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSample : MonoBehaviour {
 
    private GameObject Ball;   //プレイヤー情報格納用
    private Vector3 offset;      //相対距離取得用
 
	// Use this for initialization
	void Start () {
        
        //オブジェクトのBallの情報を取得
        Ball = GameObject.Find("Ball3");
 
        // MainCamera(自分自身)とplayerとの相対距離を求める
        offset = transform.position - Ball.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
 
        //カメラの新しい座標の値を代入する
        transform.position = Ball.transform.position + offset; //Ball座標に+offset分の位置に移動
 
	}
}
