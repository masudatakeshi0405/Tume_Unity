using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl_First : MonoBehaviour
{
    [Tooltip( "How many frames to average over for computing velocity" )]
    public int velocityAverageFrames = 5;
    [Tooltip( "How many frames to average over for computing angular velocity" )]
    public int angularVelocityAverageFrames = 11;

    public bool estimateOnAwake = false;

    private Coroutine routine;
    private int sampleCount;
    private Vector3[] velocitySamples;
    private Vector3[] angularVelocitySamples;


    //sound変数
    public AudioClip sound1;
    public AudioClip sound2;
    AudioSource audioSource;


    Rigidbody rb;

    public bool OnCollision = false;
    public bool OnCollision2 = false;
    public bool s_check = false; //サービスチェック
    public bool reset = false; //リセット
    public bool ShotCheck = false;
    public float turncou = -1; 
    //0.最初の表示
    //1.下回転とナックルの簡単な説明
    //2.サービスを確認しよう
    //3.下回転サービス打つ
    //4.下回転説明１
    //5.ドライブで返せない
    //6.そのため〜
    //7.ツッツキレシーブ
    //8.ツッツキは下回転に有効〜
    //9.次にナックルサービスについて〜　打ってみよう
    //10.ナックルを打つ
    //11.軌道は似ているけど、回転違う．ツッツキすると
    //12.ツッツキでレシーブ
    //13.ボールが浮きチャンスとなる
    //14.しかし、ナックルはバレると簡単に返させてしまうため注意して使う必要あり
    public float thrust;
    public int s_style = 0; //サービス打法 1.ナックル 2.下回転 3.順横下 4.逆横下 5.順横 6.逆横
    public int s_course = 0; //サービスコース 1.フォア 2.ミドル 3.バック 4.前フォア 5.前ミドル 6.前バック
    public int r_style = 0; //レシーブ打法 1.ドライブ 2.ツッツキ 3.ストップ 4.フリック 5.チキータ
    public int r_course = 0; //レシーブコース
    public int issue_num = 0; //テキストの順番

    public float boundcount; //バウンド回数
    float boundbef; //前フレームのボールのy軸速度
    float boundnow; //現在フレームのボールのy軸速度
    float AngularVelocity_y;
    
    bool down = false; //ボールの上昇下降判定

    string[] ss_name = new string[] {"","ナックル回転","下回転","順横下回転","逆横下回転","順横回転","逆横回転"};
    string[] sc_name = new string[] {"","フォア","ミドル","バック","前フォア","前ミドル","前バック"};
    string[] rs_name = new string[] {"","ドライブ","ツッツキ","ストップ","フリック","チキータ"};
    string[] rc_name = new string[] {"","フォア","ミドル","バック","前フォア","前ミドル","前バック"};

    //Vector3 angvel2 = Vector3.zero;
    Vector3 rotat = Vector3.zero;

    GameObject style_select; //サービススタイル選択
    GameObject coatset; //コース選択
    GameObject S_Check; //サービス確定画面
    GameObject result_title; //ラリー結果のタイトル
    GameObject firstshot_result; //1打目結果のボタン
    GameObject first_arrow; //1打目と2打目の矢印ボタン
    GameObject secondshot_result; //2打目結果のボタン
    GameObject issue; //問題文のパネル
    GameObject issue2; //問題文のパネル
    GameObject next_rally;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 100;
        // s_style = Random.Range(1, 7);
        // s_course = Random.Range(1, 7);
        // r_style = Random.Range(1, 6);
        r_style = 1;
        r_course = Random.Range(1, 6);
        s_style = 0;
        s_course = 0;
        // r_style = 0;
        // r_course = 0;
        turncou=0;
        ShotCheck = false;
        // issue_num = Random.Range(1,6); //問題決定
        // issue_num = 2;
        rb.isKinematic = true; //ボールを止める
        style_select = GameObject.Find("StyleSelect");
        coatset = GameObject.Find("CoatSet");
        S_Check = GameObject.Find("s_check");
        result_title = GameObject.Find("title");
        firstshot_result = GameObject.Find("firstshot");
        first_arrow = GameObject.Find("1-2");
        secondshot_result = GameObject.Find("secondshot");
        issue = GameObject.Find("Issue");
        issue2 = GameObject.Find("Issue2");
        next_rally = GameObject.Find("next_btm");

        //非表示
        S_Check.SetActive(false);
        coatset.SetActive(false);
        issue2.SetActive(false);
        // result_title.SetActive(false);
        // firstshot_result.SetActive(false);
        // first_arrow.SetActive(false);
        // secondshot_result.SetActive(false);

        //soundのComponentを取得
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        //テキスト表示非表示
        // if(turncou!=-2){　
        //     issue.SetActive(false);
        // }

        //リセットボタン
        if(reset==true){
            init(); //初期化
            reset = false;
        }

        // if(turncou<0){
        //     //サービスを選択したかどうかの確認(打法とコースの両方を選択したかどうか)
        //     if(s_style>0 && s_course>0){
        //         s_check = true;
        //     }else{
        //         S_Check.SetActive(false);
        //     }

        //     //選択したサービスとコース確認
        //     if(s_check==true){
        //         S_Check.SetActive(true);
        //         s_check = false;
        //     }
        // }


        // Debug.Log(boundnow); // ログを表示する
        //バウンド判定
        if(OnCollision == true){
            Vector3 angvel = Vector3.zero;
            angvel = GetAngularVelocityEstimate();
            // angvel = Quaternion.Euler(0, -90, 0) * angvel;
            Vector3 vec3 = Vector3.zero;
            vec3 = angvel.normalized;
            //rb.AddTorque(-vec3*50);
            // rb.AddForce(0, 0, vec3*10); //前方に向けて発射

            //跳ねる音をならす
            audioSource.PlayOneShot(sound1);

            OnCollision = false;
            boundcount++;
        }

        //ネットに当たったら止める
        if(OnCollision2 == true){
            rb.isKinematic = true;
            // t_style = 6;
            // t_course = 0;
            // secondo_arrow.SetActive(true);
            // thirdshot_result.SetActive(true);
            // rally_result.SetActive(true);
            if(turncou==5){
                turncou++;
                ShotCheck = false;
            }
            OnCollision2 = false;
        }

        boundnow = GetVelocityEstimate().y; //ボールのy軸の速度取得

        //ボールの落ぎわを判定
        if(boundbef>0&&boundnow<0){
            down=true;
        }
        if(boundbef<0&&boundnow>0){
            down=false;
        }

        //頂点or台の外の時ボールを止める
        if(boundcount==2){
            if((boundbef>0&&boundnow<0)||transform.position.z>30){
                if(turncou==6){
                    turncou++;
                    AngularVelocity_y = GetAngularVelocityEstimate().x;
                    rb.isKinematic = true;
                    ShotCheck = false;
                    r_style = 2;
                    r_course = Random.Range(4,7);
                    // EnemyShot();
                    //StopBall = true;
                    //NextBtm.GetComponent<NextButtom>().count++;
                    // first_arrow.SetActive(true);
                    // secondshot_result.SetActive(true);
                    // EnemyShot();
                // }else if(turncou==1){
                //     AngularVelocity_y = GetAngularVelocityEstimate().x;
                //     rb.isKinematic = true;
                //     //StopBall = true;
                //     //NextBtm.GetComponent<NextButtom>().count++;
                //     // turncou++;
                //     // first_arrow.SetActive(true);
                //     // secondshot_result.SetActive(true);
                //     EnemyShot();
                //     turncou++;
                //     ShotCheck = false;
                }else if(turncou==12){
                    AngularVelocity_y = GetAngularVelocityEstimate().x;
                    ShotCheck = false;
                    rb.isKinematic = true;
                    //StopBall = true;
                    //NextBtm.GetComponent<NextButtom>().count++;
                    turncou++;
                    // first_arrow.SetActive(true);
                    // secondshot_result.SetActive(true);
                    r_style = 2;
                    r_course = 5;
                    EnemyShot();
                }
            }
        }else if(boundcount==3){
            if((boundbef>0&&boundnow<0)||transform.position.z<-110){
                if(turncou==8){
                    ShotCheck = false;
                    rb.isKinematic = true;
                    turncou++;
                }else if(turncou==12){
                    // ShotCheck==false;
                    ShotCheck = false;
                    rb.isKinematic = true;
                    turncou++;
                }else if(turncou==13){
                    rb.isKinematic = true;
                    turncou++;
                }
            }
        }
        boundbef = GetVelocityEstimate().y; //ボールのy軸の速度取得

        // if(turncou==0){
        //     // Debug.Log(ss_name[s_style] + "," + sc_name[s_course]);
        //     // S_Check.SetActive(false); //確認画面を消す
        //     // style_select.SetActive(false); //打法選択画面を消す
        //     // coatset.SetActive(false); //コース選択画面を消す
        //     // result_title.SetActive(true); //ラリー結果タイトル
        //     // firstshot_result.SetActive(true); //1打目の結果を文字表示する
        //     // first_arrow.SetActive(false); //矢印
        //     Time.timeScale = 2f;
        //     //toss();
        //     turncou++;
        // }

        //0.最初の表示
        //1.横回転と逆横回転の簡単な説明
        //2.サービスを確認しよう
        //3.サービスの選択をするタイミング
        if(turncou==5){
            coatset.SetActive(true);
            //サービスを選択したかどうかの確認(打法とコースの両方を選択したかどうか)
            if(s_style>0 && s_course>0){
                s_check = true;
            }else{
                S_Check.SetActive(false);
                issue.SetActive(false);
                issue2.SetActive(true);
            }

            //選択したサービスとコース確認
            if(s_check==true){
                S_Check.SetActive(true);
                issue.SetActive(true);
                next_rally.SetActive(false);
                issue2.SetActive(false);
                s_check = false;
            }

            // s_style = 5;
            // s_course = 5;
            // if(ShotCheck==false){
            //     ServShot();
            //     issue.SetActive(false);
            //     // turncou++;
            //     ShotCheck = true;
            // }
        }
        //4.横回転説明→ストップで返そうとすると
        if(turncou==6){
            if(ShotCheck==false){
                // Debug.Log("a");
                coatset.SetActive(false);
                issue.SetActive(false);
                S_Check.SetActive(false);
                ServShot();
                ShotCheck = true;
            }
        }
        //5.軌道見る
        // if(turncou==5){
        //     issue.SetActive(true);
        // }
        //6.
        if(turncou==7){
            issue.SetActive(true);
            next_rally.SetActive(true);
        }

        if(turncou==8){
            if(ShotCheck==false){
                // Debug.Log("a");
                coatset.SetActive(false);
                issue.SetActive(false);
                EnemyShot();
                ShotCheck = true;
            }
        }

        if(turncou==9){
            issue.SetActive(true);
        }
        //7.ドライブで返せない
        //8.そのためツッツキでレシーブ〜
        //9.ツッツキは下回転に有効〜
        // if(turncou==7){
        //     s_style = 0;
        //     s_course = 0;
        //     // transformを取得
        //     Transform myTransform = this.transform;
        //     //座標取得
        //     Vector3 pos = myTransform.position;
        //     pos.x = -10.0f;    // x座標
        //     pos.y = -10.0f;    // y座標
        //     pos.z = -110.0f;    // z座標
        //     myTransform.position = pos;  // 座標を設定
        // }

        // if(turncou==8){
        //     if(ShotCheck==false){
        //         s_style = 6;
        //         s_course = 5;
        //         r_style = 3;
        //         r_course = 5;
        //         boundcount=0;
        //         ServShot();
        //         issue.SetActive(false);
        //         ShotCheck = true;
        //     }
        // }
        // //10.次にナックルサービスについて〜　打ってみよう
        // if(turncou==10){
        //     issue.SetActive(true);
        // }
        // if(turncou==11){
        //     s_style = 0;
        //     s_course = 0;
        //     boundcount=0;
        //     // transformを取得
        //     Transform myTransform = this.transform;
        //     //座標取得
        //     Vector3 pos = myTransform.position;
        //     pos.x = -10.0f;    // x座標
        //     pos.y = -10.0f;    // y座標
        //     pos.z = -110.0f;    // z座標
        //     myTransform.position = pos;  // 座標を設定
        // }
        // // 11.ナックルを打つ
        // if(turncou==12){
        //     // s_style = 6;
        //     // s_course = 5;
        //     // boundcount = 0;
        //     // ServShot();
        //     // issue.SetActive(false);
        //     if(ShotCheck==false){
        //         s_style = 6;
        //         s_course = 5;
        //         ServShot();
        //         issue.SetActive(false);
        //         ShotCheck = true;
        //     }
        // }
        // // 11.軌道は似ているけど、回転違う．ツッツキすると
        // if(turncou==14){
        //     issue.SetActive(true);
        // }
        //12.ツッツキでレシーブ
        // if(turncou==14){
        //     r_style = 2;
        //     r_course = 5;
        //     if(ShotCheck==false){
        //         EnemyShot();
        //         issue.SetActive(false);
        //         ShotCheck = true;
        //     }
        // }
        //13.ボールが浮きチャンスとなる
        // if(turncou==15){
        //     issue.SetActive(true);
        // }
        //14.しかし、ナックルはバレると簡単に返させてしまうため注意して使う必要あり
    }

    //トス
    void toss()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(0, 10*thrust, 0);
    }

    void ServShot()
    {
        //ボールを打つ音
        audioSource.PlayOneShot(sound2);

        Time.timeScale = 2f;
        rb.isKinematic = false;
        switch(s_style)
        {
            case 1:
            // Debug.Log("b");
            //----------------------------上回転サービス----------------------------
                //ナックル
                // rotat.x = 10.0f;
                // rb.AddTorque(rotat*100);
                // rb.AddForce(-4 * thrust, -7 * thrust, 30 * thrust); //フォア
                // rb.AddForce(3 * thrust, -7 * thrust, 30 * thrust); //ミドル
                // rb.AddForce(10 * thrust, -7 * thrust, 30 * thrust); //バック
                // rb.AddForce(-4 * thrust, -10 * thrust, 24 * thrust); //前フォア
                // rb.AddForce(3 * thrust, -10 * thrust, 24 * thrust); //前ミドル
                // rb.AddForce(10 * thrust, -10 * thrust, 24 * thrust); //前バック
                rotat.x = 3.0f;
                rb.AddTorque(rotat*100);
                switch(s_course)
                {
                    case 1:
                        rb.AddForce(-4 * thrust, -7 * thrust, 30 * thrust); //フォア
                        break;
                    case 2:
                        rb.AddForce(3 * thrust, -7 * thrust, 30 * thrust); //ミドル
                        break;
                    case 3:
                        rb.AddForce(10 * thrust, -7 * thrust, 30 * thrust); //バック
                        break;
                    case 4:
                        rb.AddForce(-4 * thrust, -10 * thrust, 24 * thrust); //前フォア
                        break;
                    case 5:
                        rb.AddForce(3 * thrust, -10 * thrust, 24 * thrust); //前ミドル
                        break;
                    case 6:
                        rb.AddForce(10 * thrust, -10 * thrust, 24 * thrust); //前バック
                        break;
                }
                break;

            case 2:
            //----------------------------下回転サービス----------------------------
                //下回転
                // rotat.x = -10.0f;
                // rb.AddTorque(rotat*100);
                // rb.AddForce(-3 * thrust, -11 * thrust, 30 * thrust); //フォア
                // rb.AddForce(2 * thrust, -11 * thrust, 30 * thrust); //ミドル
                // rb.AddForce(7 * thrust, -11 * thrust, 30 * thrust); //バック
                // rb.AddForce(-3 * thrust, -10 * thrust, 23 * thrust); //前フォア
                // rb.AddForce(2 * thrust, -10 * thrust, 23 * thrust); //前ミドル
                // rb.AddForce(7 * thrust, -11 * thrust, 23 * thrust); //前バック
                rotat.x = -10.0f;
                rb.AddTorque(rotat*100);
                switch(s_course)
                {
                    case 1:
                        rb.AddForce(-3 * thrust, -11 * thrust, 30 * thrust); //フォア
                        break;
                    case 2:
                        rb.AddForce(2 * thrust, -11 * thrust, 30 * thrust); //ミドル
                        break;
                    case 3:
                        rb.AddForce(7 * thrust, -11 * thrust, 30 * thrust); //バック
                        break;
                    case 4:
                        rb.AddForce(-3 * thrust, -10 * thrust, 23 * thrust); //前フォア
                        break;
                    case 5:
                        rb.AddForce(2 * thrust, -10 * thrust, 23 * thrust); //前ミドル
                        break;
                    case 6:
                        rb.AddForce(7 * thrust, -11 * thrust, 23 * thrust); //前バック
                        break;
                }
                break;

            case 3:
            //----------------------------順横下サービス----------------------------
                //順横下
                // rotat.x = -10.0f;
                // rotat.y = 10.0f;
                // rb.AddTorque(rotat*100);
                // rb.AddForce(-3 * thrust, -7 * thrust, 32 * thrust); //フォア
                // rb.AddForce(2 * thrust, -7 * thrust, 32 * thrust); //ミドル
                // rb.AddForce(7 * thrust, -7 * thrust, 33 * thrust); //バック
                // rb.AddForce(-3 * thrust, -10 * thrust, 23 * thrust); //前フォア
                // rb.AddForce(2 * thrust, -10 * thrust, 23 * thrust); //前ミドル
                // rb.AddForce(7 * thrust, -11 * thrust, 23 * thrust); //前バック
                rotat.x = -10.0f;
                rotat.y = -10.0f;
                rb.AddTorque(rotat*100);
                switch(s_course)
                {
                    case 1:
                        rb.AddForce(-3 * thrust, -7 * thrust, 32 * thrust); //フォア
                        break;
                    case 2:
                        rb.AddForce(2 * thrust, -7 * thrust, 32 * thrust); //ミドル
                        break;
                    case 3:
                        rb.AddForce(7 * thrust, -7 * thrust, 33 * thrust); //バック
                        break;
                    case 4:
                        rb.AddForce(-3 * thrust, -10 * thrust, 23 * thrust); //前フォア
                        break;
                    case 5:
                        rb.AddForce(2 * thrust, -10 * thrust, 23 * thrust); //前ミドル
                        break;
                    case 6:
                        rb.AddForce(7 * thrust, -11 * thrust, 23 * thrust); //前バック
                        break;
                }
                break;

            case 4:
            //----------------------------逆横下サービス----------------------------
                //逆横下
                // rotat.x = -10.0f;
                // rotat.y = -10.0f;
                // rb.AddTorque(rotat*100);
                // rb.AddForce(-3 * thrust, -7 * thrust, 32 * thrust); //フォア
                // rb.AddForce(2 * thrust, -7 * thrust, 32 * thrust); //ミドル
                // rb.AddForce(7 * thrust, -7 * thrust, 33 * thrust); //バック
                // rb.AddForce(-3 * thrust, -10 * thrust, 23 * thrust); //前フォア
                // rb.AddForce(2 * thrust, -10 * thrust, 23 * thrust); //前ミドル
                // rb.AddForce(7 * thrust, -11 * thrust, 23 * thrust); //前バック
                rotat.x = -10.0f;
                rotat.y = -10.0f;
                rb.AddTorque(rotat*100);
                switch(s_course)
                {
                    case 1:
                        rb.AddForce(-3 * thrust, -7 * thrust, 32 * thrust); //フォア
                        break;
                    case 2:
                        rb.AddForce(2 * thrust, -7 * thrust, 32 * thrust); //ミドル
                        break;
                    case 3:
                        rb.AddForce(7 * thrust, -7 * thrust, 33 * thrust); //バック
                        break;
                    case 4:
                        rb.AddForce(-3 * thrust, -10 * thrust, 23 * thrust); //前フォア
                        break;
                    case 5:
                        rb.AddForce(2 * thrust, -10 * thrust, 23 * thrust); //前ミドル
                        break;
                    case 6:
                        rb.AddForce(7 * thrust, -11 * thrust, 23 * thrust); //前バック
                        break;
                }
                break;

            case 5:
            //----------------------------順横サービス----------------------------
                //順横
                // rotat.y = -10.0f;
                // rb.AddTorque(rotat*100);
                // rb.AddForce(-3 * thrust, -7 * thrust, 32 * thrust); //フォア
                // rb.AddForce(2 * thrust, -7 * thrust, 32 * thrust); //ミドル
                // rb.AddForce(9 * thrust, -7 * thrust, 33 * thrust); //バック
                // rb.AddForce(-3 * thrust, -10 * thrust, 23 * thrust); //前フォア
                // rb.AddForce(2 * thrust, -10 * thrust, 23 * thrust); //前ミドル
                // rb.AddForce(8 * thrust, -11 * thrust, 23 * thrust); //前バック
                rotat.y = -10.0f;
                rb.AddTorque(rotat*100);
                switch(s_course)
                {
                    case 1:
                        rb.AddForce(-3 * thrust, -7 * thrust, 32 * thrust); //フォア
                        break;
                    case 2:
                        rb.AddForce(2 * thrust, -7 * thrust, 32 * thrust); //ミドル
                        break;
                    case 3:
                        rb.AddForce(9 * thrust, -7 * thrust, 33 * thrust); //バック
                        break;
                    case 4:
                        rb.AddForce(-3 * thrust, -10 * thrust, 23 * thrust); //前フォア
                        break;
                    case 5:
                        rb.AddForce(2 * thrust, -10 * thrust, 23 * thrust); //前ミドル
                        break;
                    case 6:
                        rb.AddForce(8 * thrust, -11 * thrust, 23 * thrust); //前バック
                        break;
                }
                break;

            case 6:
            //----------------------------逆横サービス----------------------------
                //逆横
                // rotat.y = 10.0f;
                // rb.AddTorque(rotat*100);
                // rb.AddForce(-3 * thrust, -7 * thrust, 32 * thrust); //フォア
                // rb.AddForce(2 * thrust, -7 * thrust, 32 * thrust); //ミドル
                // rb.AddForce(9 * thrust, -7 * thrust, 33 * thrust); //バック
                // rb.AddForce(-3 * thrust, -10 * thrust, 23 * thrust); //前フォア
                // rb.AddForce(2 * thrust, -10 * thrust, 23 * thrust); //前ミドル
                // rb.AddForce(8 * thrust, -11 * thrust, 23 * thrust); //前バック
                rotat.y = 10.0f;
                rb.AddTorque(rotat*100);
                switch(s_course)
                {
                    case 1:
                        rb.AddForce(-3 * thrust, -7 * thrust, 32 * thrust); //フォア
                        break;
                    case 2:
                        rb.AddForce(2 * thrust, -7 * thrust, 32 * thrust); //ミドル
                        break;
                    case 3:
                        rb.AddForce(9 * thrust, -7 * thrust, 33 * thrust); //バック
                        break;
                    case 4:
                        rb.AddForce(-3 * thrust, -10 * thrust, 23 * thrust); //前フォア
                        break;
                    case 5:
                        rb.AddForce(2 * thrust, -10 * thrust, 23 * thrust); //前ミドル
                        break;
                    case 6:
                        rb.AddForce(8 * thrust, -11 * thrust, 23 * thrust); //前バック
                        break;
                }
                break;

            case 7:
            //----------------------------ナックルサービス----------------------------
                rotat.x = -3.0f;
                rb.AddTorque(rotat*100);
                switch(s_course)
                {
                    case 1:
                        rb.AddForce(-3 * thrust, -11 * thrust, 30 * thrust); //フォア
                        break;
                    case 2:
                        rb.AddForce(2 * thrust, -11 * thrust, 30 * thrust); //ミドル
                        break;
                    case 3:
                        rb.AddForce(7 * thrust, -11 * thrust, 30 * thrust); //バック
                        break;
                    case 4:
                        rb.AddForce(-3 * thrust, -10 * thrust, 23 * thrust); //前フォア
                        break;
                    case 5:
                        rb.AddForce(2 * thrust, -10 * thrust, 23 * thrust); //前ミドル
                        break;
                    case 6:
                        rb.AddForce(7 * thrust, -11 * thrust, 23 * thrust); //前バック
                        break;
                }
                break;
        }
    }

    void EnemyShot()
    {
        //ボールを打つ音
        audioSource.PlayOneShot(sound2);
        
        rb.isKinematic = false;
        switch(r_style)
        {
            case 2:
            //--------------------------ツッツキ--------------------------------
                rotat.x = 10.0f;
                rb.AddTorque(rotat*100);
                switch(s_style)
                {
                    case 1:
                    // //上回転→ツッツキ・横回転→ツッツキ
                    // // rb.AddForce((transform.position.x-22) * -10, 13 * thrust, -20 * thrust); //フォア
                    // // rb.AddForce((transform.position.x) * -10, 13 * thrust, -20 * thrust); //ミドル
                    // // rb.AddForce((transform.position.x+22) * -10, 13 * thrust, -20 * thrust); //バック
                    // switch(r_course)
                    // {
                    //     case 1:
                    //         rb.AddForce((transform.position.x-22) * -10, 13 * thrust, -20 * thrust); //フォア
                    //         break;
                    //     case 2:
                    //         rb.AddForce((transform.position.x) * -10, 13 * thrust, -20 * thrust); //ミドル
                    //         break;
                    //     case 3:
                    //         rb.AddForce((transform.position.x+22) * -10, 13 * thrust, -20 * thrust); //バック
                    //         break;
                    // }
                    // break;
                        switch(r_course){
                            case 1:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -32 * thrust);
                                }else{
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -28 * thrust); //フォア
                                }
                                break;
                            case 2:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -32 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x) * -20, 12 * thrust, -28 * thrust); //ミドル
                                }
                                break;
                            case 3:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, -32 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x+22) * -20, 12 * thrust, -28 * thrust); //バック
                                }
                                break;
                            case 4:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -28 * thrust);
                                }else{
                                    rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -25 * thrust); //フォア
                                }
                                break;
                            case 5:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -28 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x) * -20, 10 * thrust, -25 * thrust); //ミドル
                                }
                                break;
                            case 6:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, -28 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -25 * thrust); //バック
                                }
                                break;
                        }
                        break;

                    case 2:
                    //下回転→ツッツキ
                    // rb.AddForce((transform.position.x-22) * -15, 13 * thrust, -30 * thrust); //フォア
                    // rb.AddForce((transform.position.x) * -15, 13 * thrust, -30 * thrust); //ミドル
                    // rb.AddForce((transform.position.x+22) * -15, 13 * thrust, -30 * thrust); //バック
                    // rb.AddForce((transform.position.x-22) * -15, 10 * thrust, -18 * thrust); //前フォア
                    // rb.AddForce((transform.position.x) * -15, 10 * thrust, -18 * thrust); //前ミドル
                    // rb.AddForce((transform.position.x+22) * -15, 10 * thrust, -18 * thrust); //前バック
                        switch(r_course)
                        {
                            case 1:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -32 * thrust);
                                }else{
                                    rb.AddForce((transform.position.x-22) * -15, 11 * thrust, -25 * thrust); //フォア
                                }
                                break;
                            case 2:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -32 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x) * -15, 11 * thrust, -25 * thrust); //ミドル
                                }
                                break;
                            case 3:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, -32 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x+22) * -15, 11 * thrust, -25 * thrust);
                                }
                                break;
                            case 4:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x-22) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                            case 5:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                            case 6:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x+22) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                        }
                        break;  

                    case 3:
                    //(逆)横下→ツッツキ
                    // rb.AddForce((transform.position.x-22) * -15, 13 * thrust, -30 * thrust); //フォア
                    // rb.AddForce((transform.position.x) * -15, 13 * thrust, -30 * thrust); //ミドル
                    // rb.AddForce((transform.position.x+22) * -15, 13 * thrust, -30 * thrust); //バック
                    // rb.AddForce((transform.position.x-22) * -15, 10 * thrust, -18 * thrust); //前フォア
                    // rb.AddForce((transform.position.x) * -15, 10 * thrust, -18 * thrust); //前ミドル
                    // rb.AddForce((transform.position.x+22) * -15, 10 * thrust, -18 * thrust); //前バック
                        switch(r_course)
                        {
                            case 1:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -32 * thrust);
                                }else{
                                    rb.AddForce((transform.position.x-22) * -15, 11 * thrust, -25 * thrust); //フォア
                                }
                                break;
                            case 2:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -32 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x) * -15, 11 * thrust, -25 * thrust); //ミドル
                                }
                                break;
                            case 3:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, -32 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x+22) * -15, 11 * thrust, -25 * thrust);
                                }
                                break;
                            case 4:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x-22) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                            case 5:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                            case 6:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x+22) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                        }
                        break;

                    case 4:
                        switch(r_course)
                        {
                            case 1:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -32 * thrust);
                                }else{
                                    rb.AddForce((transform.position.x-22) * -15, 11 * thrust, -25 * thrust); //フォア
                                }
                                break;
                            case 2:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -32 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x) * -15, 11 * thrust, -25 * thrust); //ミドル
                                }
                                break;
                            case 3:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, -32 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x+22) * -15, 11 * thrust, -25 * thrust);
                                }
                                break;
                            case 4:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x-22) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                            case 5:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                            case 6:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x+22) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                        }
                        break;

                    case 5:
                        switch(r_course)
                        {
                            case 1:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -32 * thrust);
                                }else{
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -28 * thrust); //フォア
                                }
                                break;
                            case 2:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -32 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x) * -20, 12 * thrust, -28 * thrust); //ミドル
                                }
                                break;
                            case 3:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, -32 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x+22) * -20, 12 * thrust, -28 * thrust); //バック
                                }
                                break;
                            case 4:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -28 * thrust);
                                }else{
                                    rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -25 * thrust); //フォア
                                }
                                break;
                            case 5:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -28 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x) * -20, 10 * thrust, -25 * thrust); //ミドル
                                }
                                break;
                            case 6:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, -28 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -25 * thrust); //バック
                                }
                                break;
                        }
                        break;

                    case 6:
                        switch(r_course)
                        {
                            case 1:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -32 * thrust);
                                }else{
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -28 * thrust); //フォア
                                }
                                break;
                            case 2:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -32 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x) * -20, 12 * thrust, -28 * thrust); //ミドル
                                }
                                break;
                            case 3:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, -32 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x+22) * -20, 12 * thrust, -28 * thrust); //バック
                                }
                                break;
                            case 4:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -28 * thrust);
                                }else{
                                    rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -25 * thrust); //フォア
                                }
                                break;
                            case 5:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -28 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x) * -20, 10 * thrust, -25 * thrust); //ミドル
                                }
                                break;
                            case 6:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, -28 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -25 * thrust); //バック
                                }
                                break;
                        }
                        break;

                    case 7:
                    switch(r_course)
                    {
                        case 1:
                            rb.AddForce((transform.position.x-22) * -10, 13 * thrust, -20 * thrust); //フォア
                            break;
                        case 2:
                            rb.AddForce((transform.position.x) * -10, 13 * thrust, -20 * thrust); //ミドル
                            break;
                        case 3:
                            rb.AddForce((transform.position.x+22) * -10, 13 * thrust, -20 * thrust); //バック
                            break;
                        case 4:
                            rb.AddForce((transform.position.x-22) * -15, 13 * thrust, -18 * thrust); //前フォア
                            break;
                        case 5:
                            rb.AddForce((transform.position.x) * -15, 13 * thrust, -18 * thrust); //前ミドル
                            break;
                        case 6:
                            rb.AddForce((transform.position.x+22) * -15, 13 * thrust, -18 * thrust); //前バック
                            break;
                    }
                    break;
                }
                break;

            case 3:
                rotat.y = 3.0f;
                rb.AddTorque(rotat*100);
                switch(s_style)
                {
                    case 5:
                    //順横→ストップ
                    // rotat.x = -1.0f;
                    // rb.AddTorque(rotat*100);
                    // rb.AddForce((transform.position.x) * -12, 13 * thrust, -21 * thrust); //フォア
                    // rb.AddForce((transform.position.x+22) * -12, 13 * thrust, -21 * thrust); //ミドル
                    // rb.AddForce((transform.position.x+44) * -12, 13 * thrust, -21 * thrust); //バック
                    // rb.AddForce((transform.position.x) * -13, 10 * thrust, -18 * thrust); //前フォア
                    // rb.AddForce((transform.position.x+22) * -13, 10 * thrust, -18 * thrust); //前ミドル
                    // rb.AddForce((transform.position.x+44) * -13, 10 * thrust, -18 * thrust); //前バック
                        rotat.y = -2.0f;
                        rb.AddTorque(rotat*100);
                        switch(r_course)
                        {
                            case 1:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -32 * thrust);
                                }else{
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -28 * thrust); //フォア
                                }
                                break;
                            case 2:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, -32 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x+22) * -20, 12 * thrust, -28 * thrust); //ミドル
                                }
                                break;
                            case 3:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+44) * -15, 12 * thrust, -32 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x+44) * -20, 12 * thrust, -28 * thrust); //バック
                                }
                                break;
                            case 4:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -28 * thrust);
                                }else{
                                    rb.AddForce((transform.position.x) * -20, 10 * thrust, -25 * thrust); //フォア
                                }
                                break;
                            case 5:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, -28 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -25 * thrust); //ミドル
                                }
                                break;
                            case 6:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+44) * -15, 12 * thrust, -28 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x+44) * -20, 10 * thrust, -25 * thrust); //バック
                                }
                                break;
                        }
                        break;

                    case 6:
                    //逆横→ストップ
                    // rotat.x = -1.0f;
                    // rb.AddTorque(rotat*100);
                    // rb.AddForce((transform.position.x-44) * -12, 13 * thrust, -21 * thrust); //フォア
                    // rb.AddForce((transform.position.x-22) * -12, 13 * thrust, -21 * thrust); //ミドル
                    // rb.AddForce((transform.position.x) * -12, 13 * thrust, -21 * thrust); //バック
                    // rb.AddForce((transform.position.x-44) * -13, 10 * thrust, -18 * thrust); //前フォア
                    // rb.AddForce((transform.position.x-22) * -13, 10 * thrust, -18 * thrust); //前ミドル
                    // rb.AddForce((transform.position.x) * -13, 10 * thrust, -18 * thrust); //前バック
                        rotat.y = 2.0f;
                        rb.AddTorque(rotat*100);
                        switch(r_course)
                        {
                            case 1:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-44) * -15, 12 * thrust, -32 * thrust);
                                }else{
                                    rb.AddForce((transform.position.x-44) * -15, 12 * thrust, -28 * thrust); //フォア
                                }
                                break;
                            case 2:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -32 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x-22) * -20, 12 * thrust, -28 * thrust); //ミドル
                                }
                                break;
                            case 3:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -32 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x) * -20, 12 * thrust, -28 * thrust); //バック
                                }
                                break;
                            case 4:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-44) * -15, 12 * thrust, -28 * thrust);
                                }else{
                                    rb.AddForce((transform.position.x-44) * -20, 10 * thrust, -25 * thrust); //フォア
                                }
                                break;
                            case 5:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -28 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -25 * thrust); //ミドル
                                }
                                break;
                            case 6:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -28 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x) * -20, 10 * thrust, -25 * thrust); //バック
                                }
                                break;
                        }
                        break;
                }
                break;
        }
    }

    //当たった瞬間に呼ばれる関数
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name); // ログを表示する
        if(collision.gameObject.name == "Plane"){
            Debug.Log("Hit"); // ログを表示する
            OnCollision = true;
        }
        if(collision.gameObject.name == "Net"){
            OnCollision2 = true;
        }
    }

    void init()
    {
        turncou = -1;
        r_style = Random.Range(1, 6);
        r_course = Random.Range(1, 7);
        s_style = 0;
        s_course = 0;
        boundcount = 0;
        S_Check.SetActive(false);
        firstshot_result.SetActive(false);
        first_arrow.SetActive(false);
        secondshot_result.SetActive(false);
        style_select.SetActive(true);
        coatset.SetActive(true);
        rb.isKinematic = true;

        // transformを取得
        Transform myTransform = this.transform;

        // 座標を取得
        Vector3 pos = myTransform.position;
        pos.x = -10.0f;    // x座標
        pos.y = -10.0f;    // y座標
        pos.z = -110.0f;    // z座標

        myTransform.position = pos;  // 座標を設定
    }


    //ここからボールのベクトルと各速度の取得の関数
    //-------------------------------------------------
    public void BeginEstimatingVelocity()
    {
        FinishEstimatingVelocity();

        routine = StartCoroutine( EstimateVelocityCoroutine() );
    }


    //-------------------------------------------------
    public void FinishEstimatingVelocity()
    {
        if ( routine != null )
        {
            StopCoroutine( routine );
            routine = null;
        }
    }


    //-------------------------------------------------
    public Vector3 GetVelocityEstimate()
    {
        // Compute average velocity
        Vector3 velocity = Vector3.zero;
        int velocitySampleCount = Mathf.Min( sampleCount, velocitySamples.Length );
        if ( velocitySampleCount != 0 )
        {
            for ( int i = 0; i < velocitySampleCount; i++ )
            {
                velocity += velocitySamples[i];
            }
            velocity *= ( 1.0f / velocitySampleCount );
        }

        return velocity;
    }


    //-------------------------------------------------
    public Vector3 GetAngularVelocityEstimate()
    {
        // Compute average angular velocity
        Vector3 angularVelocity = Vector3.zero;
        int angularVelocitySampleCount = Mathf.Min( sampleCount, angularVelocitySamples.Length );
        if ( angularVelocitySampleCount != 0 )
        {
            for ( int i = 0; i < angularVelocitySampleCount; i++ )
            {
                angularVelocity += angularVelocitySamples[i];
            }
            angularVelocity *= ( 1.0f / angularVelocitySampleCount );
        }

        return angularVelocity;
    }


    //-------------------------------------------------
    public Vector3 GetAccelerationEstimate()
    {
        Vector3 average = Vector3.zero;
        for ( int i = 2 + sampleCount - velocitySamples.Length; i < sampleCount; i++ )
        {
            if ( i < 2 )
                continue;

            int first = i - 2;
            int second = i - 1;

            Vector3 v1 = velocitySamples[first % velocitySamples.Length];
            Vector3 v2 = velocitySamples[second % velocitySamples.Length];
            average += v2 - v1;
        }
        average *= ( 1.0f / Time.deltaTime );
        return average;
    }


    //-------------------------------------------------
    void Awake()
    {
        velocitySamples = new Vector3[velocityAverageFrames];
        angularVelocitySamples = new Vector3[angularVelocityAverageFrames];

        if ( estimateOnAwake )
        {
            BeginEstimatingVelocity();
        }
    }


    //-------------------------------------------------
    private IEnumerator EstimateVelocityCoroutine()
    {
        sampleCount = 0;

        Vector3 previousPosition = transform.position;
        Quaternion previousRotation = transform.rotation;
        while ( true )
        {
            yield return new WaitForEndOfFrame();

            float velocityFactor = 1.0f / Time.deltaTime;

            int v = sampleCount % velocitySamples.Length;
            int w = sampleCount % angularVelocitySamples.Length;
            sampleCount++;

            // Estimate linear velocity
            velocitySamples[v] = velocityFactor * ( transform.position - previousPosition );

            // Estimate angular velocity
            Quaternion deltaRotation = transform.rotation * Quaternion.Inverse( previousRotation );

            float theta = 2.0f * Mathf.Acos( Mathf.Clamp( deltaRotation.w, -1.0f, 1.0f ) );
            if ( theta > Mathf.PI )
            {
                theta -= 2.0f * Mathf.PI;
            }

            Vector3 angularVelocity = new Vector3( deltaRotation.x, deltaRotation.y, deltaRotation.z );
            if ( angularVelocity.sqrMagnitude > 0.0f )
            {
                angularVelocity = theta * velocityFactor * angularVelocity.normalized;
            }

            angularVelocitySamples[w] = angularVelocity;

            previousPosition = transform.position;
            previousRotation = transform.rotation;
        }
    }
}
