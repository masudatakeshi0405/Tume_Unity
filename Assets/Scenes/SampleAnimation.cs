using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleAnimation : MonoBehaviour {

	// Animator コンポーネント
	private Animator animator;
	GameObject Ball;

	Vector3 pos = Vector3.zero;


	// 設定したフラグの名前
	private const string key_swing = "swing";

    void Hit()
    {
        Debug.Log("Hit");
		Ball.GetComponent<RptateTest2>().isMove = false;
        Ball.GetComponent<RptateTest2>().ballshot = true;
    }

	// void fin(){
	// 	transform.position = keeppos;
	// 	transform.rotation = keeprot;
	// }

	// 初期化メソッド
	void Start () {
		// 自分に設定されているAnimatorコンポーネントを習得する
		this.animator = GetComponent<Animator>();
		Ball = GameObject.Find("Ball3");
	}
	
	// 1フレームに1回コールされる
	void Update () {

		// 矢印下ボタンを押下している
		if (Input.GetKey(KeyCode.DownArrow)) {
			// WaitからRunに遷移する
			this.animator.SetBool(key_swing, true);
		}
		else {
			// RunからWaitに遷移する
			this.animator.SetBool(key_swing, false);
		}
		pos = transform.position;
		pos.x = Ball.transform.position.x + 30;
		transform.position = pos;
	}
}
