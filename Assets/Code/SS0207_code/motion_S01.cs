using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class motion_S01 : MonoBehaviour
{

    private Animator anim;  //Animatorをanimという変数で定義する
    private Vector3 offset;      //相対距離取得用

    float x;

    GameObject Ball;

    Transform myTransform;
    Vector3 pos; 

    // Start is called before the first frame update
    void Start()
    {
        //変数animに、Animatorコンポーネントを設定する
        anim = gameObject.GetComponent<Animator>();

        Ball = GameObject.Find("Ball3");

        // pos = myTransform.position;
        pos = new Vector3(20f, -56f, 47.5f);
    }

    // Update is called once per frame
    void Update()
    {
        //
        switch(Ball.GetComponent<BallControl_BMC_S01>().s_course)
        {
            case 1:
                if (Ball.GetComponent<BallControl_BMC_S01>().turncou==1)
                {
                    //Bool型のパラメーターであるblRotをTrueにする
                    anim.SetBool("forehand", true);
                }else if (Ball.GetComponent<BallControl_BMC_S01>().turncou==2){
                    //Bool型のパラメーターであるblRotをTrueにする
                    anim.SetBool("forehandswing", true);
                    anim.SetBool("forehand", false);
                }else{
                    anim.SetBool("forehandswing", false);
                }
                break;

            case 2:
                if (Ball.GetComponent<BallControl_BMC_S01>().turncou==1)
                {
                    //Bool型のパラメーターであるblRotをTrueにする
                    anim.SetBool("backhand", true);
                }else if (Ball.GetComponent<BallControl_BMC_S01>().turncou==2){
                    //Bool型のパラメーターであるblRotをTrueにする
                    anim.SetBool("backhandswing", true);
                    anim.SetBool("backhand", false);
                }else{
                    anim.SetBool("backhandswing", false);
                }
                break;

            case 3:
                if (Ball.GetComponent<BallControl_BMC_S01>().turncou==1)
                {
                    //Bool型のパラメーターであるblRotをTrueにする
                    anim.SetBool("backhand", true);
                }else if (Ball.GetComponent<BallControl_BMC_S01>().turncou==2){
                    //Bool型のパラメーターであるblRotをTrueにする
                    anim.SetBool("backhandswing", true);
                    anim.SetBool("backhand", false);
                }else{
                    anim.SetBool("backhandswing", false);
                }
                break;

            case 4:
                if (Ball.GetComponent<BallControl_BMC_S01>().boundcount==2)
                {
                    //Bool型のパラメーターであるblRotをTrueにする
                    anim.SetBool("frontfore", true);
                }else if (Ball.GetComponent<BallControl_BMC_S01>().boundcount==3||Ball.GetComponent<BallControl_BMC_S01>().boundcount==0){
                    //Bool型のパラメーターであるblRotをTrueにする
                    anim.SetBool("frontfore", false);
                }
                break;

            case 5:
                if (Ball.GetComponent<BallControl_BMC_S01>().boundcount==2)
                {
                    //Bool型のパラメーターであるblRotをTrueにする
                    anim.SetBool("frontback", true);
                }else if (Ball.GetComponent<BallControl_BMC_S01>().boundcount==3||Ball.GetComponent<BallControl_BMC_S01>().boundcount==0){
                    //Bool型のパラメーターであるblRotをTrueにする
                    anim.SetBool("frontback", false);
                }
                break;

            case 6:
                if (Ball.GetComponent<BallControl_BMC_S01>().boundcount==2)
                {
                    //Bool型のパラメーターであるblRotをTrueにする
                    anim.SetBool("frontback", true);
                }else if (Ball.GetComponent<BallControl_BMC_S01>().boundcount==3||Ball.GetComponent<BallControl_BMC_S01>().boundcount==0){
                    //Bool型のパラメーターであるblRotをTrueにする
                    anim.SetBool("frontback", false);
                }
                break;
        }
        pos = transform.position;
        pos.x = Ball.transform.position.x + 10;
        transform.position = pos;
        this.transform.rotation = Quaternion.Euler(10.0f, 180.0f, 0f);
    }
}
