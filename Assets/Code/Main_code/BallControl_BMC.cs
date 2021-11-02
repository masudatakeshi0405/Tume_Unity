using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl_BMC : MonoBehaviour
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
    public bool judge = false; //問題正誤判定
    public bool repeat = false; //同じ問題をやるかどうか
    public float turncou = -2; //現在の状態を示す　0.最初　1.回転選択　2.コース選択　3.サービス発射　4.相手のレシーブ　5.3打目打法選択
    public float thrust;
    public float boundcount; //バウンド回数
    public int s_style = 0; //サービス打法 1.ナックル 2.下回転 3.順横下 4.逆横下 5.順横 6.逆横
    public int s_course = 0; //サービスコース 1.フォア 2.ミドル 3.バック 4.前フォア 5.前ミドル 6.前バック
    public int r_style = 0; //レシーブ打法 1.ドライブ 2.ツッツキ 3.ストップ 4.フリック 5.チキータ
    public int r_course = 0; //レシーブコース
    public int t_style = 0; //3球目 1.ドライブ 2.ツッツキ 3.ストップ 4.フリック 5.チキータ 6.ネット
    public int t_course = 0; //3球目コース
    public int issue_num = 0; //問題番号
    List<int> issue_array = new List<int>();
    public int issue_cou = 0; //解いた問題数
    public int issue_ok_cou = 0; //正解数
    
    float boundbef; //前フレームのボールのy軸速度
    float boundnow; //現在フレームのボールのy軸速度
    float AngularVelocity_y;
    
    bool down = false; //ボールの上昇下降判定

    string[] ss_name = new string[] {"","上回転","下回転","順横下回転","逆横下回転","順横回転","逆横回転","ナックル"};
    string[] sc_name = new string[] {"","フォア","ミドル","バック","前フォア","前ミドル","前バック"};
    string[] rs_name = new string[] {"","ドライブ","ツッツキ","ストップ","フリック","チキータ"};
    string[] rc_name = new string[] {"","フォア","ミドル","バック","前フォア","前ミドル","前バック"};
    string[] ts_name = new string[] {"","ドライブ","ツッツキ","ストップ","フリック","チキータ","ネット"};
    string[] tc_name = new string[] {"","フォア","ミドル","バック","前フォア","前ミドル","前バック"};

    //Vector3 angvel2 = Vector3.zero;
    Vector3 rotat = Vector3.zero;

    GameObject style_select; //サービススタイル選択
    GameObject coatset; //コース選択
    GameObject S_Check; //サービス確定画面
    GameObject result_title; //ラリー結果のタイトル
    GameObject firstshot_result; //1打目結果のボタン
    GameObject first_arrow; //1打目と2打目の矢印ボタン
    GameObject secondshot_result; //2打目結果のボタン
    GameObject secondo_arrow; //2打目と3打目の矢印ボタン
    GameObject thirdshot_result; //3打目結果のボタン
    GameObject rally_result; //ラリー結果(成功・失敗)
    GameObject issue; //問題文のパネル
    GameObject result_panel; //
    GameObject finish_panel; //

    
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
        turncou=-3;
        issue_array.Add(1);
        issue_array.Add(2);
        issue_array.Add(3);
        issue_array.Add(4);
        issue_array.Add(5);
        issue_num = Random.Range(1, 6);
        // issue_num = issue_array[Random.Range(1, 6)]; //問題決定
        issue_array.Remove(issue_num);
        // issue_num = 1;
        rb.isKinematic = true; //ボールを止める
        style_select = GameObject.Find("StyleSelect");
        coatset = GameObject.Find("CoatSet");
        S_Check = GameObject.Find("s_check");
        result_title = GameObject.Find("title");
        firstshot_result = GameObject.Find("firstshot");
        first_arrow = GameObject.Find("1-2");
        secondshot_result = GameObject.Find("secondshot");
        secondo_arrow = GameObject.Find("2-3");
        thirdshot_result = GameObject.Find("thirdshot");
        rally_result = GameObject.Find("rally_result");
        issue = GameObject.Find("Issue");
        result_panel = GameObject.Find("result_paneru");
        finish_panel = GameObject.Find("Finish_paneru");

        //非表示
        S_Check.SetActive(false);
        result_title.SetActive(false);
        firstshot_result.SetActive(false);
        first_arrow.SetActive(false);
        secondshot_result.SetActive(false);
        secondo_arrow.SetActive(false);
        thirdshot_result.SetActive(false);
        rally_result.SetActive(false);
        result_panel.SetActive(false);
        finish_panel.SetActive(false);

        //soundのComponentを取得
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //
        if(issue_cou==4){
            finish_panel.SetActive(true);
            result_panel.SetActive(false);
            issue.SetActive(false);
        }

        //問題表示非表示
        if(turncou>-2){　
            issue.SetActive(false);
        }

        //問題決定によるレシーブ決定
        // switch(issue_num){
        //     case 1:
        //         t_style = 1;
        //         t_course = Random.Range(1, 4);
        //         break;
        // }

        //リセットボタン
        if(reset==true){
            init(); //初期化
            reset = false;
            repeat　=　false;
        }

        if(turncou<0){
            //サービスを選択したかどうかの確認(打法とコースの両方を選択したかどうか)
            if(s_style>0 && s_course>0){
                s_check = true;
            }else{
                S_Check.SetActive(false);
            }

            //選択したサービスとコース確認
            if(s_check==true){
                S_Check.SetActive(true);
                s_check = false;
            }
        }
         

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
            t_style = 6;
            t_course = 0;
            secondo_arrow.SetActive(true);
            thirdshot_result.SetActive(true);
            rally_result.SetActive(true);
            result_panel.SetActive(true);
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
                if(turncou==1){
                    AngularVelocity_y = GetAngularVelocityEstimate().x;
                    rb.isKinematic = true;
                    //StopBall = true;
                    //NextBtm.GetComponent<NextButtom>().count++;
                    turncou++;
                    first_arrow.SetActive(true);
                    secondshot_result.SetActive(true);
                    EnemyModel();
                    EnemyShot();
                }
            }
        }else if(boundcount==3){
            if((boundbef>0&&boundnow<0)||transform.position.z<-110){
                if(turncou==2){
                    rb.isKinematic = true;
                    turncou++;
                    if(transform.position.z>-40){

                    }else{
                        thirdshot_result.SetActive(true);
                        secondo_arrow.SetActive(true);
                        ThirdShot();
                    }
                }
            }
            //angvel2 = GetAngularVelocityEstimate();
        }else if(boundcount==4){
            if((boundbef>0&&boundnow<0)||transform.position.z>30){
                rally_result.SetActive(true);
                result_panel.SetActive(true);
                rb.isKinematic = true;
            }
        }
        boundbef = GetVelocityEstimate().y; //ボールのy軸の速度取得

        //サービス
        if(turncou==0){
                Debug.Log(ss_name[s_style] + "," + sc_name[s_course]);
                S_Check.SetActive(false); //確認画面を消す
                style_select.SetActive(false); //打法選択画面を消す
                coatset.SetActive(false); //コース選択画面を消す
                result_title.SetActive(true); //ラリー結果タイトル
                firstshot_result.SetActive(true); //1打目の結果を文字表示する
                first_arrow.SetActive(false); //矢印

                //ボールを打つ音
                audioSource.PlayOneShot(sound2);

                Time.timeScale = 2f;
                //Comment.SetActive(false);
                //toss();
                //NextBtm.GetComponent<NextBtm_RS01>().count=8;
                turncou++;
                //start=false;
                rb.isKinematic = false;
                Debug.Log("a");
                switch(s_style)
                {
                    case 1:
                    Debug.Log("b");
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
                //上回転サービス
                //rotat.x = 10.0f;
                //rb.AddTorque(rotat*100);
                // rb.AddForce(-4 * thrust, -7 * thrust, 30 * thrust); //左
                // rb.AddForce(3 * thrust, -7 * thrust, 30 * thrust); //中
                // rb.AddForce(9 * thrust, -7 * thrust, 30 * thrust); //右
                // rb.AddForce(-4 * thrust, -10 * thrust, 22 * thrust); //左下
                //rb.AddForce(3 * thrust, -10 * thrust, 22 * thrust); //中下
                // rb.AddForce(11 * thrust, -10 * thrust, 22 * thrust); //右下

            }
    }

    //トス
    void toss()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(0, 10*thrust, 0);
    }

    void EnemyShot()
    {
        rb.isKinematic = false;

        //ボールを打つ音
        audioSource.PlayOneShot(sound2);

        switch(r_style)
        {
            case 1:
            //--------------------------ドライブ--------------------------------
                rotat.x = -10.0f;
                rb.AddTorque(rotat*100);
                switch(s_style)
                {
                    case 1:
                    //上回転→ドライブ
                    // rb.AddTorque(rotat*100);
                    // rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                    // rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //ミドル
                    // rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //バック
                    switch(r_course)
                    {
                        case 1:
                            rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                            break;
                        case 2:
                            rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //ミドル
                            break;
                        case 3:
                            rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //バック
                            break;
                        case 4:
                            rb.AddForce((transform.position.x-10) * -20, 6 * thrust, -25 * thrust); //フォア
                            break;
                        case 5:
                            rb.AddForce((transform.position.x) * -20, 6 * thrust, -25 * thrust); //ミドル
                            break;
                        case 6:
                            rb.AddForce((transform.position.x+10) * -20, 6 * thrust, -25 * thrust); //バック
                            break;
                    }
                    break;

                    case 2:
                    //下回転→ドライブ
                    // rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                    // rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //ミドル
                    // rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //バック
                    // rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //前フォア
                    // rb.AddForce(transform.position.x * -20, 2 * thrust, -36 * thrust); //前ミドル
                    // rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バック
                    switch(r_course)
                    {
                        case 1:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust);
                            }else{
                                rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //フォア
                            }
                            break;
                        case 2:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //ミドル
                            }else{
                                rb.AddForce((transform.position.x) * -20, 2 * thrust, -36 * thrust); //ミドル
                            }
                            break;
                        case 3:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //バック
                            }else{
                                rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //バック
                            }
                            break;
                        case 4:
                            rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //前フォア
                            break;
                        case 5:
                            rb.AddForce(transform.position.x * -20, 2 * thrust, -36 * thrust); //前ミドル
                            break;
                        case 6:
                            rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バック
                            break;
                    }
                    break;

                    case 3:
                    //(逆)横下→ドライブ
                    // rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                    // rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //ミドル
                    // rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //バック
                    // rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //前フォア
                    // rb.AddForce(transform.position.x * -20, 2 * thrust, -36 * thrust); //前ミドル
                    // rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バック
                    switch(r_course)
                    {
                        case 1:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust);
                            }else{
                                rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //フォア
                            }
                            break;
                        case 2:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //ミドル
                            }else{
                                rb.AddForce((transform.position.x) * -20, 2 * thrust, -36 * thrust); //ミドル
                            }
                            break;
                        case 3:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //バック
                            }else{
                                rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //バック
                            }
                            break;
                        case 4:
                            rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //前フォア
                            break;
                        case 5:
                            rb.AddForce(transform.position.x * -20, 2 * thrust, -36 * thrust); //前ミドル
                            break;
                        case 6:
                            rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バック
                            break;
                    }
                    break;

                    case 4:
                    switch(r_course)
                    {
                        case 1:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust);
                            }else{
                                rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //フォア
                            }
                            break;
                        case 2:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //ミドル
                            }else{
                                rb.AddForce((transform.position.x) * -20, 2 * thrust, -36 * thrust); //ミドル
                            }
                            break;
                        case 3:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //バック
                            }else{
                                rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //バック
                            }
                            break;
                        case 4:
                            rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //前フォア
                            break;
                        case 5:
                            rb.AddForce(transform.position.x * -20, 2 * thrust, -36 * thrust); //前ミドル
                            break;
                        case 6:
                            rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バック
                            break;
                    }
                    break;

                    case 5:
                    //横→ドライブ
                    // rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                    // rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //ミドル
                    // rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //バック
                    switch(r_course)
                    {
                        case 1:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust);
                            }else{
                                rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //フォア
                            }
                            break;
                        case 2:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //ミドル
                            }else{
                                rb.AddForce((transform.position.x) * -20, 2 * thrust, -36 * thrust); //ミドル
                            }
                            break;
                        case 3:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //バック
                            }else{
                                rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //バック
                            }
                            break;
                        case 4:
                            rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //前フォア
                            break;
                        case 5:
                            rb.AddForce(transform.position.x * -20, 2 * thrust, -36 * thrust); //前ミドル
                            break;
                        case 6:
                            rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バック
                            break;
                    }
                    break;

                    case 6:
                    switch(r_course)
                    {
                        case 1:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust);
                            }else{
                                rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //フォア
                            }
                            break;
                        case 2:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //ミドル
                            }else{
                                rb.AddForce((transform.position.x) * -20, 2 * thrust, -36 * thrust); //ミドル
                            }
                            break;
                        case 3:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //バック
                            }else{
                                rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //バック
                            }
                            break;
                        case 4:
                            rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //前フォア
                            break;
                        case 5:
                            rb.AddForce(transform.position.x * -20, 2 * thrust, -36 * thrust); //前ミドル
                            break;
                        case 6:
                            rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バック
                            break;
                    }
                    break;

                    case 7:
                    switch(r_course)
                    {
                        case 1:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust);
                            }else{
                                rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //フォア
                            }
                            break;
                        case 2:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //ミドル
                            }else{
                                rb.AddForce((transform.position.x) * -20, 2 * thrust, -36 * thrust); //ミドル
                            }
                            break;
                        case 3:
                            if(s_course<=3){
                                rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //バック
                            }else{
                                rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //バック
                            }
                            break;
                        case 4:
                            rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //前フォア
                            break;
                        case 5:
                            rb.AddForce(transform.position.x * -20, 2 * thrust, -36 * thrust); //前ミドル
                            break;
                        case 6:
                            rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バック
                            break;
                    }
                    break;
                }
                break;

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
            //--------------------------ストップ--------------------------------
                switch(s_style)
                {
                    case 1:
                    //上回転→ストップ
                    // rotat.x = 1.0f;
                    // rb.AddTorque(rotat*100);
                    // rb.AddForce((transform.position.x-22) * -12, 13 * thrust, -20 * thrust); //フォア
                    // rb.AddForce((transform.position.x) * -12, 13 * thrust, -20 * thrust); //ミドル
                    // rb.AddForce((transform.position.x+22) * -12, 13 * thrust, -20 * thrust); //バック
                    // rb.AddForce((transform.position.x-22) * -13, 11 * thrust, -19 * thrust); //前フォア
                    // rb.AddForce((transform.position.x) * -13, 11 * thrust, -18 * thrust); //前ミドル
                    // rb.AddForce((transform.position.x+22) * -13, 11 * thrust, -19 * thrust); //前バック
                    rotat.x = 5.0f;
                    rb.AddTorque(rotat*100);
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

                    case 2:
                    //下回転→ストップ
                    // rotat.x = -1.0f;
                    // rb.AddTorque(rotat*100);
                    // rb.AddForce((transform.position.x-22) * -12, 13 * thrust, -20 * thrust); //フォア
                    // rb.AddForce((transform.position.x) * -12, 13 * thrust, -20 * thrust); //ミドル
                    // rb.AddForce((transform.position.x+22) * -12, 13 * thrust, -20 * thrust); //バック
                    // rb.AddForce((transform.position.x-22) * -13, 10 * thrust, -18 * thrust); //前フォア
                    // rb.AddForce((transform.position.x) * -13, 10 * thrust, -18 * thrust); //前ミドル
                    // rb.AddForce((transform.position.x+22) * -13, 10 * thrust, -18 * thrust); //前バック
                    rotat.x = -3.0f;
                    rb.AddTorque(rotat*100);
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
                    //順横下→ストップ レシーブは左にそれる
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
                                    rb.AddForce((transform.position.x) * -15, 11 * thrust, -25 * thrust); //フォア
                                }
                                break;
                            case 2:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, -32 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x)+22 * -15, 11 * thrust, -25 * thrust); //ミドル
                                }
                                break;
                            case 3:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+44) * -15, 12 * thrust, -32 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x+44) * -15, 11 * thrust, -25 * thrust);
                                }
                                break;
                            case 4:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                            case 5:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+22) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x+22) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                            case 6:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x+44) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x+44) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                        }
                        break;

                    case 4:
                    //逆横下→ストップ　レシーブは右に逸れる
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
                                    rb.AddForce((transform.position.x-44) * -15, 11 * thrust, -25 * thrust); //フォア
                                }
                                break;
                            case 2:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -32 * thrust); //ミドル
                                }else{
                                    rb.AddForce((transform.position.x-22) * -15, 11 * thrust, -25 * thrust); //ミドル
                                }
                                break;
                            case 3:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, -32 * thrust); //バック
                                }else{
                                    rb.AddForce((transform.position.x) * -15, 11 * thrust, -25 * thrust);
                                }
                                break;
                            case 4:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-44) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x-44) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                            case 5:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x-22) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x-22) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                            case 6:
                                if(s_course<=3){
                                    rb.AddForce((transform.position.x) * -15, 11 * thrust, -25 * thrust); //フォア
                                }else{
                                    rb.AddForce((transform.position.x) * -13, 10 * thrust, -19 * thrust); //前フォア
                                }
                                break;
                        }
                        break;

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

                    case 7:
                        rotat.x = -1.0f;
                        rb.AddTorque(rotat*100);
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
                }
                break;

            case 4:
            //--------------------------フリック--------------------------------
            // バック側の場合は無理、台外の場合もよろしくない
                rotat.x = -10.0f;
                rb.AddTorque(rotat*100);

                switch(s_course)
                {
                    case 3:
                        rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //フォア
                        break;
                    case 6:
                        rb.AddForce((transform.position.x-22) * -20, 2 * thrust, -36 * thrust); //フォア
                        break;
                default:
                    switch(s_style)
                    {
                        case 1:
                        //上回転→フリック
                        // rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                        // rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //ミドル
                        // rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //バックには打てないはず
                        // rb.AddForce((transform.position.x-22) * -25, 6 * thrust, -38 * thrust); //フォア
                        // rb.AddForce((transform.position.x) * -25, 6 * thrust, -38 * thrust); //ミドル
                        // rb.AddForce((transform.position.x+22) * -25, 6 * thrust, -38 * thrust); //バックには打てません
                            switch(r_course)
                            {
                                case 1:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }
                                    break;
                                case 2:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }
                                    break;
                                case 3:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }
                                    break;
                                case 4:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -32 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -34 * thrust); //前フォア
                                    }
                                    
                                    break;
                                case 5:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 12 * thrust, -32 * thrust); //ミドル
                                    }else{
                                        rb.AddForce(transform.position.x * -25, 8 * thrust, -34 * thrust); //前ミドル
                                    }
                                    break;
                                case 6:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 12 * thrust, -32 * thrust); //バックは無理
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -34 * thrust); //前バックは無理
                                    }
                                    break;
                            }
                            break;

                        case 2:
                        //下回転→フリック
                        // rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                        // rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //ミドル
                        // rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //バックは無理
                        // rb.AddForce((transform.position.x-22) * -25, 6 * thrust, -38 * thrust); //前フォア
                        // rb.AddForce(transform.position.x * -25, 6 * thrust, -38 * thrust); //前ミドル
                        // rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バックは無理
                            switch(r_course)
                            {
                                case 1:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }
                                    break;
                                case 2:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }
                                    break;
                                case 3:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }
                                    break;
                                case 4:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -25, 6 * thrust, -38 * thrust); //前フォア
                                    }
                                    
                                    break;
                                case 5:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }else{
                                        rb.AddForce(transform.position.x * -25, 6 * thrust, -38 * thrust); //前ミドル
                                    }
                                    break;
                                case 6:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バックは無理
                                    }
                                    break;
                            }
                            break;

                        case 3:
                        //(逆)横下→フリック
                        // rb.AddForce((transform.position.x-22) * -25, 6 * thrust, -38 * thrust); //フォア
                        // rb.AddForce(transform.position.x * -25, 6 * thrust, -38 * thrust); //ミドル
                        // rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //バックは無理
                            switch(r_course)
                            {
                                case 1:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }
                                    break;
                                case 2:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }
                                    break;
                                case 3:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }
                                    break;
                                case 4:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -25, 6 * thrust, -38 * thrust); //前フォア
                                    }
                                    
                                    break;
                                case 5:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }else{
                                        rb.AddForce(transform.position.x * -25, 6 * thrust, -38 * thrust); //前ミドル
                                    }
                                    break;
                                case 6:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バックは無理
                                    }
                                    break;
                            }
                            break;

                        case 4:
                            switch(r_course)
                            {
                                case 1:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }
                                    break;
                                case 2:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }
                                    break;
                                case 3:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }
                                    break;
                                case 4:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -25, 6 * thrust, -38 * thrust); //前フォア
                                    }
                                    
                                    break;
                                case 5:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }else{
                                        rb.AddForce(transform.position.x * -25, 6 * thrust, -38 * thrust); //前ミドル
                                    }
                                    break;
                                case 6:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バックは無理
                                    }
                                    break;
                            }
                            break;

                        case 5:
                        //(逆)横→フリック
                            switch(r_course)
                            {
                                case 1:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }
                                    break;
                                case 2:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }
                                    break;
                                case 3:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }
                                    break;
                                case 4:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -32 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -34 * thrust); //前フォア
                                    }
                                    
                                    break;
                                case 5:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 12 * thrust, -32 * thrust); //ミドル
                                    }else{
                                        rb.AddForce(transform.position.x * -25, 8 * thrust, -34 * thrust); //前ミドル
                                    }
                                    break;
                                case 6:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 12 * thrust, -32 * thrust); //バックは無理
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -34 * thrust); //前バックは無理
                                    }
                                    break;
                            }
                            break;

                        case 6:
                            switch(r_course)
                            {
                                case 1:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }
                                    break;
                                case 2:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }
                                    break;
                                case 3:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }
                                    break;
                                case 4:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -32 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -34 * thrust); //前フォア
                                    }
                                    
                                    break;
                                case 5:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 12 * thrust, -32 * thrust); //ミドル
                                    }else{
                                        rb.AddForce(transform.position.x * -25, 8 * thrust, -34 * thrust); //前ミドル
                                    }
                                    break;
                                case 6:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 12 * thrust, -32 * thrust); //バックは無理
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -34 * thrust); //前バックは無理
                                    }
                                    break;
                            }
                            break;

                        case 7:
                            switch(r_course)
                            {
                                case 1:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }
                                    break;
                                case 2:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }
                                    break;
                                case 3:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }
                                    break;
                                case 4:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -25, 6 * thrust, -38 * thrust); //前フォア
                                    }
                                    
                                    break;
                                case 5:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }else{
                                        rb.AddForce(transform.position.x * -25, 6 * thrust, -38 * thrust); //前ミドル
                                    }
                                    break;
                                case 6:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バックは無理
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                }
                break;
                

            case 5:
            // //--------------------------チキータ--------------------------------
                rotat.x = -10.0f;
                rb.AddTorque(rotat*100);

                switch(s_course)
                {
                    case 1:
                        rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //フォア
                        break;
                    case 4:
                        rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //フォア
                        break;
                default:
                    switch(s_style){
                        case 1:
                        //上回転→チキータ
                            switch(r_course)
                            {
                                case 1:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }
                                    break;
                                case 2:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }
                                    break;
                                case 3:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }
                                    break;
                                case 4:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -32 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -34 * thrust); //前フォア
                                    }
                                    break;
                                case 5:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 12 * thrust, -32 * thrust); //ミドル
                                    }else{
                                        rb.AddForce(transform.position.x * -25, 8 * thrust, -34 * thrust); //前ミドル
                                    }
                                    break;
                                case 6:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 12 * thrust, -32 * thrust); //バックは無理
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -34 * thrust); //前バックは無理
                                    }
                                    break;
                            }
                            break;

                        case 2:
                        //下回転→チキータ
                            switch(r_course){
                                case 1:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }
                                    break;
                                case 2:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }
                                    break;
                                case 3:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }
                                    break;
                                case 4:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -25, 6 * thrust, -38 * thrust); //前フォア
                                    }
                                    
                                    break;
                                case 5:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }else{
                                        rb.AddForce(transform.position.x * -25, 6 * thrust, -38 * thrust); //前ミドル
                                    }
                                    break;
                                case 6:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バックは無理
                                    }
                                    break;
                            }
                            break;
                        
                        case 3:
                        //(逆)横下→チキータ
                            switch(r_course){
                                case 1:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }
                                    break;
                                case 2:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }
                                    break;
                                case 3:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }
                                    break;
                                case 4:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -25, 6 * thrust, -38 * thrust); //前フォア
                                    }
                                    
                                    break;
                                case 5:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }else{
                                        rb.AddForce(transform.position.x * -25, 6 * thrust, -38 * thrust); //前ミドル
                                    }
                                    break;
                                case 6:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バックは無理
                                    }
                                    break;
                            }
                            break;

                        case 4:
                            switch(r_course){
                                case 1:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }
                                    break;
                                case 2:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }
                                    break;
                                case 3:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }
                                    break;
                                case 4:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -25, 6 * thrust, -38 * thrust); //前フォア
                                    }
                                    
                                    break;
                                case 5:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }else{
                                        rb.AddForce(transform.position.x * -25, 6 * thrust, -38 * thrust); //前ミドル
                                    }
                                    break;
                                case 6:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バックは無理
                                    }
                                    break;
                            }
                            break;

                        case 5:
                        //(逆)横→チキータ
                            switch(r_course){
                                case 1:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }
                                    break;
                                case 2:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }
                                    break;
                                case 3:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }
                                    break;
                                case 4:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -32 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -34 * thrust); //前フォア
                                    }
                                    
                                    break;
                                case 5:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 12 * thrust, -32 * thrust); //ミドル
                                    }else{
                                        rb.AddForce(transform.position.x * -25, 8 * thrust, -34 * thrust); //前ミドル
                                    }
                                    break;
                                case 6:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 12 * thrust, -32 * thrust); //バックは無理
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -34 * thrust); //前バックは無理
                                    }
                                    break;
                            }
                            break;

                        case 6:
                            switch(r_course){
                                case 1:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }
                                    break;
                                case 2:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }
                                    break;
                                case 3:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }
                                    break;
                                case 4:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -15, 12 * thrust, -32 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -34 * thrust); //前フォア
                                    }
                                    
                                    break;
                                case 5:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 12 * thrust, -32 * thrust); //ミドル
                                    }else{
                                        rb.AddForce(transform.position.x * -25, 8 * thrust, -34 * thrust); //前ミドル
                                    }
                                    break;
                                case 6:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 12 * thrust, -32 * thrust); //バックは無理
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -34 * thrust); //前バックは無理
                                    }
                                    break;
                            }
                            break;

                        case 7:
                        //ナックルサービス
                            switch(r_course){
                                case 1:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }
                                    break;
                                case 2:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }
                                    break;
                                case 3:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }
                                    break;
                                case 4:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, -36 * thrust); //フォア
                                    }else{
                                        rb.AddForce((transform.position.x-22) * -25, 6 * thrust, -38 * thrust); //前フォア
                                    }
                                    
                                    break;
                                case 5:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, -36 * thrust); //ミドル
                                    }else{
                                        rb.AddForce(transform.position.x * -25, 6 * thrust, -38 * thrust); //前ミドル
                                    }
                                    break;
                                case 6:
                                    if(s_course<=3){
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, -36 * thrust); //バックは無理
                                    }else{
                                        rb.AddForce((transform.position.x+22) * -20, 2 * thrust, -36 * thrust); //前バックは無理
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                }
                break;
        }
    }


    //3球目
    void ThirdShot()
    {
        rb.isKinematic = false;

        //ボールを打つ音
        audioSource.PlayOneShot(sound2);

        switch(t_style){
            case 1:
            //--------------------------ドライブ--------------------------------
                rotat.x = 10.0f;
                rb.AddTorque(rotat*100);
                switch(r_style)
                {
                    case 2:
                        if(r_course<=3){
                            switch(t_course)
                            {
                                case 1:
                                    rb.AddForce((transform.position.x+22) * -18, 10 * thrust, 35 * thrust); //フォア
                                    break;
                                case 2:
                                    rb.AddForce((transform.position.x) * -18, 10 * thrust, 35 * thrust); //フォア
                                    break;
                                case 3:
                                    rb.AddForce((transform.position.x-22) * -18, 10 * thrust, 35 * thrust); //フォア
                                    break;
                                case 4:
                                    rb.AddForce((transform.position.x+22) * -18, 6 * thrust, 35 * thrust); //フォア
                                    break;
                                case 5:
                                    rb.AddForce((transform.position.x) * -18, 6 * thrust, 35 * thrust); //フォア
                                    break;
                                case 6:
                                    rb.AddForce((transform.position.x-22) * -18, 6 * thrust, 35 * thrust); //フォア
                                    break;
                            }
                        }else{
                            if(transform.position.z>-100){
                                if(s_style==7){
                                    switch(t_course)
                                    {
                                        case 1:
                                            rb.AddForce((transform.position.x+22) * -18, 5 * thrust, 35 * thrust); //フォア
                                            break;
                                        case 2:
                                            rb.AddForce((transform.position.x) * -18, 5 * thrust, 35 * thrust); //フォア
                                            break;
                                        case 3:
                                            rb.AddForce((transform.position.x-22) * -18, 5 * thrust, 35 * thrust); //フォア
                                            break;
                                        case 4:
                                            rb.AddForce((transform.position.x+22) * -18, 5 * thrust, 35 * thrust); //フォア
                                            break;
                                        case 5:
                                            rb.AddForce((transform.position.x) * -18, 5 * thrust, 35 * thrust); //フォア
                                            break;
                                        case 6:
                                            rb.AddForce((transform.position.x-22) * -18, 5 * thrust, 35 * thrust); //フォア
                                            break;
                                    }
                                }else{
                                    switch(t_course)
                                    {
                                        case 1:
                                            rb.AddForce((transform.position.x+22) * -18, 2 * thrust, 35 * thrust); //フォア
                                            break;
                                        case 2:
                                            rb.AddForce((transform.position.x) * -18, 2 * thrust, 35 * thrust); //フォア
                                            break;
                                        case 3:
                                            rb.AddForce((transform.position.x-22) * -18, 2 * thrust, 35 * thrust); //フォア
                                            break;
                                        case 4:
                                            rb.AddForce((transform.position.x+22) * -18, 2 * thrust, 35 * thrust); //フォア
                                            break;
                                        case 5:
                                            rb.AddForce((transform.position.x) * -18, 2 * thrust, 35 * thrust); //フォア
                                            break;
                                        case 6:
                                            rb.AddForce((transform.position.x-22) * -18, 2 * thrust, 35 * thrust); //フォア
                                            break;
                                    }
                                }
                            }else{
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -18, 10 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -18, 10 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -18, 10 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -18, 6 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -18, 6 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -18, 6 * thrust, 35 * thrust); //フォア
                                        break;
                                }
                            }
                        }
                        break;

                    case 3:
                        if(r_course<=3){
                            switch(t_course)
                            {
                                case 1:
                                    rb.AddForce((transform.position.x+22) * -18, 6 * thrust, 35 * thrust); //フォア
                                    break;
                                case 2:
                                    rb.AddForce((transform.position.x) * -18, 6 * thrust, 35 * thrust); //フォア
                                    break;
                                case 3:
                                    rb.AddForce((transform.position.x-22) * -18, 6 * thrust, 35 * thrust); //フォア
                                    break;
                                case 4:
                                    rb.AddForce((transform.position.x+22) * -18, 6 * thrust, 35 * thrust); //フォア
                                    break;
                                case 5:
                                    rb.AddForce((transform.position.x) * -18, 6 * thrust, 35 * thrust); //フォア
                                    break;
                                case 6:
                                    rb.AddForce((transform.position.x-22) * -18, 6 * thrust, 35 * thrust); //フォア
                                    break;
                            }
                        }else{
                            if(transform.position.z<-100){
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -18, 6 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -18, 6 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -18, 6 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -18, 6 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -18, 6 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -18, 6 * thrust, 35 * thrust); //フォア
                                        break;
                                }
                            }else{
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -18, 10 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -18, 10 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -18, 10 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -18, 6 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -18, 6 * thrust, 35 * thrust); //フォア
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -18, 6 * thrust, 35 * thrust); //フォア
                                        break;
                                }
                            }
                        }
                        break;

                    default:
                        switch(t_course)
                        {
                            case 1:
                                rb.AddForce((transform.position.x+22) * -18, 10 * thrust, 35 * thrust); //フォア
                                break;
                            case 2:
                                rb.AddForce((transform.position.x) * -18, 10 * thrust, 35 * thrust); //フォア
                                break;
                            case 3:
                                rb.AddForce((transform.position.x-22) * -18, 10 * thrust, 35 * thrust); //フォア
                                break;
                            case 4:
                                rb.AddForce((transform.position.x+22) * -18, 6 * thrust, 35 * thrust); //フォア
                                break;
                            case 5:
                                rb.AddForce((transform.position.x) * -18, 6 * thrust, 35 * thrust); //フォア
                                break;
                            case 6:
                                rb.AddForce((transform.position.x-22) * -18, 6 * thrust, 35 * thrust); //フォア
                                break;
                        }
                        break;
                }
                break;

            case 2:
            //--------------------------ツッツキ--------------------------------
                rotat.x = -10.0f;
                rb.AddTorque(rotat*100);
                switch(r_style){
                    case 2:
                        if(r_course<=3){
                            switch(t_course)
                            {
                                case 1:
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 32 * thrust);
                                    break;
                                case 2:
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, 32 * thrust); //ミドル
                                    break;
                                case 3:
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 32 * thrust); //バック
                                    break;
                                case 4:
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 28 * thrust);
                                    break;
                                case 5:
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, 28 * thrust); //ミドル
                                    break;
                                case 6:
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 28 * thrust); //バック
                                    break;
                            }
                        }else{
                            if(transform.position.z>-100){
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -15, 11 * thrust, 25 * thrust); //フォア
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -15, 11 * thrust, 25 * thrust); //ミドル
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -15, 11 * thrust, 25 * thrust);
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -13, 8 * thrust, 20 * thrust); //前フォア
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -13, 8 * thrust, 20 * thrust); //前フォア
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -13, 8 * thrust, 20 * thrust); //前フォア
                                        break;
                                }
                            }else{
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 32 * thrust);
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -15, 12 * thrust, 32 * thrust); //ミドル
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 32 * thrust); //バック
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 28 * thrust);
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -15, 12 * thrust, 28 * thrust); //ミドル
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 28 * thrust); //バック
                                        break;
                                }
                            }
                        }
                        break;

                    case 3:
                        if(r_course<=3){
                            switch(t_course)
                            {
                                case 1:
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 32 * thrust);
                                    break;
                                case 2:
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, 32 * thrust); //ミドル
                                    break;
                                case 3:
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 32 * thrust); //バック
                                    break;
                                case 4:
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 28 * thrust);
                                    break;
                                case 5:
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, 28 * thrust); //ミドル
                                    break;
                                case 6:
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 28 * thrust); //バック
                                    break;
                            }
                        }else{
                            if(transform.position.z>-100){
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -15, 11 * thrust, 25 * thrust); //フォア
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -15, 11 * thrust, 25 * thrust); //ミドル
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -15, 11 * thrust, 25 * thrust);
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -13, 8 * thrust, 20 * thrust); //前フォア
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -13, 8 * thrust, 20 * thrust); //前フォア
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -13, 8 * thrust, 20 * thrust); //前フォア
                                        break;
                                }
                            }else{
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 32 * thrust);
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -15, 12 * thrust, 32 * thrust); //ミドル
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 32 * thrust); //バック
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 28 * thrust);
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -15, 12 * thrust, 28 * thrust); //ミドル
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 28 * thrust); //バック
                                        break;
                                }
                            }
                        }
                        break;

                    default:
                        switch(t_course)
                        {
                            case 1:
                                rb.AddForce((transform.position.x+22) * -18, 10 * thrust, 35 * thrust); //フォア
                                break;
                            case 2:
                                rb.AddForce((transform.position.x) * -18, 10 * thrust, 35 * thrust); //フォア
                                break;
                            case 3:
                                rb.AddForce((transform.position.x-22) * -18, 10 * thrust, 35 * thrust); //フォア
                                break;
                            case 4:
                                rb.AddForce((transform.position.x+22) * -18, 6 * thrust, 35 * thrust); //フォア
                                break;
                            case 5:
                                rb.AddForce((transform.position.x) * -18, 6 * thrust, 35 * thrust); //フォア
                                break;
                            case 6:
                                rb.AddForce((transform.position.x-22) * -18, 6 * thrust, 35 * thrust); //フォア
                                break;
                        }
                        break;
                }
                break;

            case 3:
            //--------------------------ストップ--------------------------------
                switch(r_style){
                    case 2:
                        rotat.x = -5.0f;
                        rb.AddTorque(rotat*100);
                        if(r_course<=3){
                            switch(t_course)
                            {
                                case 1:
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 32 * thrust);
                                    break;
                                case 2:
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, 32 * thrust); //ミドル
                                    break;
                                case 3:
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 32 * thrust); //バック
                                    break;
                                case 4:
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 28 * thrust);
                                    break;
                                case 5:
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, 28 * thrust); //ミドル
                                    break;
                                case 6:
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 28 * thrust); //バック
                                    break;
                            }
                        }else{
                            if(transform.position.z>-100){
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -15, 11 * thrust, 25 * thrust); //フォア
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -15, 11 * thrust, 25 * thrust); //ミドル
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -15, 11 * thrust, 25 * thrust);
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -13, 8 * thrust, 20 * thrust); //前フォア
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -13, 8 * thrust, 20 * thrust); //前フォア
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -13, 8 * thrust, 20 * thrust); //前フォア
                                        break;
                                }
                            }else{
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 32 * thrust);
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -15, 12 * thrust, 32 * thrust); //ミドル
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 32 * thrust); //バック
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 28 * thrust);
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -15, 12 * thrust, 28 * thrust); //ミドル
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 28 * thrust); //バック
                                        break;
                                }
                            }
                        }
                        break;

                    case 3:
                        rotat.x = -5.0f;
                        rb.AddTorque(rotat*100);
                        if(r_course<=3){
                            switch(t_course)
                            {
                                case 1:
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 32 * thrust);
                                    break;
                                case 2:
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, 32 * thrust); //ミドル
                                    break;
                                case 3:
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 32 * thrust); //バック
                                    break;
                                case 4:
                                    rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 28 * thrust);
                                    break;
                                case 5:
                                    rb.AddForce((transform.position.x) * -15, 12 * thrust, 28 * thrust); //ミドル
                                    break;
                                case 6:
                                    rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 28 * thrust); //バック
                                    break;
                            }
                        }else{
                            if(transform.position.z>-100){
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -15, 11 * thrust, 25 * thrust); //フォア
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -15, 11 * thrust, 25 * thrust); //ミドル
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -15, 11 * thrust, 25 * thrust);
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -13, 8 * thrust, 20 * thrust); //前フォア
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -13, 8 * thrust, 20 * thrust); //前フォア
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -13, 8 * thrust, 20 * thrust); //前フォア
                                        break;
                                }
                            }else{
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 32 * thrust);
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -15, 12 * thrust, 32 * thrust); //ミドル
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 32 * thrust); //バック
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -15, 12 * thrust, 28 * thrust);
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -15, 12 * thrust, 28 * thrust); //ミドル
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -15, 12 * thrust, 28 * thrust); //バック
                                        break;
                                }
                            }
                        }
                        break;

                    default:
                        rotat.x = 5.0f;
                        rb.AddTorque(rotat*100);
                        switch(t_course)
                        {
                            case 1:
                                rb.AddForce((transform.position.x+22) * -18, 10 * thrust, 35 * thrust); //フォア
                                break;
                            case 2:
                                rb.AddForce((transform.position.x) * -18, 10 * thrust, 35 * thrust); //フォア
                                break;
                            case 3:
                                rb.AddForce((transform.position.x-22) * -18, 10 * thrust, 35 * thrust); //フォア
                                break;
                            case 4:
                                rb.AddForce((transform.position.x+22) * -18, 6 * thrust, 35 * thrust); //フォア
                                break;
                            case 5:
                                rb.AddForce((transform.position.x) * -18, 6 * thrust, 35 * thrust); //フォア
                                break;
                            case 6:
                                rb.AddForce((transform.position.x-22) * -18, 6 * thrust, 35 * thrust); //フォア
                                break;
                        }
                        break;
                }
                break;

            case 4:
            //--------------------------フリック--------------------------------
                rotat.x = -10.0f;
                rb.AddTorque(rotat*100);
                switch(r_course)
                {
                    case 3:
                        rb.AddForce((transform.position.x-22) * -20, 2 * thrust, 36 * thrust); //フォア
                        break;
                    case 6:
                        rb.AddForce((transform.position.x-22) * -20, 2 * thrust, 36 * thrust); //フォア
                        break;
                default:
                    switch(r_style){
                        case 2:
                            if(r_course<3){
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, 36 * thrust); //ミドル
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, 36 * thrust); //バックは無理
                                        break;
                                }
                            }else{
                                if(transform.position.z>-100){
                                    switch(t_course)
                                    {
                                        case 1:
                                            rb.AddForce((transform.position.x+22) * -20, 6 * thrust, 38 * thrust); //フォア
                                            break;
                                        case 2:
                                            rb.AddForce((transform.position.x) * -20, 6 * thrust, 38 * thrust); //ミドル
                                            break;
                                        case 3:
                                            rb.AddForce((transform.position.x-22) * -20, 6 * thrust, 38 * thrust); //バックは無理
                                            break;
                                        case 4:
                                            rb.AddForce((transform.position.x+22) * -25, 4 * thrust, 38 * thrust); //前フォア
                                            break;
                                        case 5:
                                            rb.AddForce(transform.position.x * -25, 4 * thrust, 38 * thrust); //前ミドル
                                            break;
                                        case 6:
                                            rb.AddForce((transform.position.x-22) * -20, 4 * thrust, 36 * thrust); //前バックは無理
                                            break;
                                    }
                                }else{
                                    switch(t_course)
                                    {
                                        case 1:
                                            rb.AddForce((transform.position.x+22) * -20, 10 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 2:
                                            rb.AddForce((transform.position.x) * -20, 10 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 3:
                                            rb.AddForce((transform.position.x-22) * -20, 10 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 4:
                                            rb.AddForce((transform.position.x+22) * -20, 8 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 5:
                                            rb.AddForce((transform.position.x) * -20, 8 * thrust, 36 * thrust); //ミドル
                                            break;
                                        case 6:
                                            rb.AddForce((transform.position.x-22) * -20, 8 * thrust, 36 * thrust); //バックは無理
                                            break;
                                    }
                                }
                            }
                            break;

                        case 3:
                            if(r_course<3){
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, 36 * thrust); //ミドル
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, 36 * thrust); //バックは無理
                                        break;
                                }
                            }else{
                                if(transform.position.z>-100){
                                    switch(t_course)
                                    {
                                        case 1:
                                            rb.AddForce((transform.position.x+22) * -20, 6 * thrust, 38 * thrust); //フォア
                                            break;
                                        case 2:
                                            rb.AddForce((transform.position.x) * -20, 6 * thrust, 38 * thrust); //ミドル
                                            break;
                                        case 3:
                                            rb.AddForce((transform.position.x-22) * -20, 6 * thrust, 38 * thrust); //バックは無理
                                            break;
                                        case 4:
                                            rb.AddForce((transform.position.x+22) * -25, 4 * thrust, 38 * thrust); //前フォア
                                            break;
                                        case 5:
                                            rb.AddForce(transform.position.x * -25, 4 * thrust, 38 * thrust); //前ミドル
                                            break;
                                        case 6:
                                            rb.AddForce((transform.position.x-22) * -20, 4 * thrust, 36 * thrust); //前バックは無理
                                            break;
                                    }
                                }else{
                                    switch(t_course)
                                    {
                                        case 1:
                                            rb.AddForce((transform.position.x+22) * -20, 10 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 2:
                                            rb.AddForce((transform.position.x) * -20, 10 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 3:
                                            rb.AddForce((transform.position.x-22) * -20, 10 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 4:
                                            rb.AddForce((transform.position.x+22) * -20, 8 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 5:
                                            rb.AddForce((transform.position.x) * -20, 8 * thrust, 36 * thrust); //ミドル
                                            break;
                                        case 6:
                                            rb.AddForce((transform.position.x-22) * -20, 8 * thrust, 36 * thrust); //バックは無理
                                            break;
                                    }
                                }
                            }
                            break;

                        default:
                            switch(t_course){
                                case 1:
                                    rb.AddForce((transform.position.x+22) * -20, 10 * thrust, 36 * thrust); //フォア
                                    break;
                                case 2:
                                    rb.AddForce((transform.position.x) * -20, 10 * thrust, 36 * thrust); //フォア
                                    break;
                                case 3:
                                    rb.AddForce((transform.position.x-22) * -20, 10 * thrust, 36 * thrust); //フォア
                                    break;
                                case 4:
                                    rb.AddForce((transform.position.x+22) * -20, 8 * thrust, 36 * thrust); //フォア
                                    break;
                                case 5:
                                    rb.AddForce((transform.position.x) * -20, 8 * thrust, 36 * thrust); //ミドル
                                    break;
                                case 6:
                                    rb.AddForce((transform.position.x-22) * -20, 8 * thrust, 36 * thrust); //バックは無理
                                    break;
                            }
                            break;
                    }
                    break;
                }
                break;

            case 5:
            //--------------------------チキータ--------------------------------
                rotat.x = -10.0f;
                rb.AddTorque(rotat*100);
                switch(r_course)
                {
                    case 1:
                        rb.AddForce((transform.position.x-22) * -20, 2 * thrust, 36 * thrust); //フォア
                        break;
                    case 4:
                        rb.AddForce((transform.position.x-22) * -20, 2 * thrust, 36 * thrust); //フォア
                        break;
                default:
                    switch(r_style){
                        case 2:
                            if(r_course<3){
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, 36 * thrust); //ミドル
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, 36 * thrust); //バックは無理
                                        break;
                                }
                            }else{
                                if(transform.position.z>-100){
                                    switch(t_course)
                                    {
                                        case 1:
                                            rb.AddForce((transform.position.x+22) * -20, 6 * thrust, 38 * thrust); //フォア
                                            break;
                                        case 2:
                                            rb.AddForce((transform.position.x) * -20, 6 * thrust, 38 * thrust); //ミドル
                                            break;
                                        case 3:
                                            rb.AddForce((transform.position.x-22) * -20, 6 * thrust, 38 * thrust); //バックは無理
                                            break;
                                        case 4:
                                            rb.AddForce((transform.position.x+22) * -25, 4 * thrust, 38 * thrust); //前フォア
                                            break;
                                        case 5:
                                            rb.AddForce(transform.position.x * -25, 4 * thrust, 38 * thrust); //前ミドル
                                            break;
                                        case 6:
                                            rb.AddForce((transform.position.x-22) * -20, 4 * thrust, 36 * thrust); //前バックは無理
                                            break;
                                    }
                                }else{
                                    switch(t_course)
                                    {
                                        case 1:
                                            rb.AddForce((transform.position.x+22) * -20, 10 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 2:
                                            rb.AddForce((transform.position.x) * -20, 10 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 3:
                                            rb.AddForce((transform.position.x-22) * -20, 10 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 4:
                                            rb.AddForce((transform.position.x+22) * -20, 8 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 5:
                                            rb.AddForce((transform.position.x) * -20, 8 * thrust, 36 * thrust); //ミドル
                                            break;
                                        case 6:
                                            rb.AddForce((transform.position.x-22) * -20, 8 * thrust, 36 * thrust); //バックは無理
                                            break;
                                    }
                                }
                            }
                            break;

                        case 3:
                            if(r_course<3){
                                switch(t_course)
                                {
                                    case 1:
                                        rb.AddForce((transform.position.x+22) * -20, 10 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 2:
                                        rb.AddForce((transform.position.x) * -20, 10 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 3:
                                        rb.AddForce((transform.position.x-22) * -20, 10 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 4:
                                        rb.AddForce((transform.position.x+22) * -20, 8 * thrust, 36 * thrust); //フォア
                                        break;
                                    case 5:
                                        rb.AddForce((transform.position.x) * -20, 8 * thrust, 36 * thrust); //ミドル
                                        break;
                                    case 6:
                                        rb.AddForce((transform.position.x-22) * -20, 8 * thrust, 36 * thrust); //バックは無理
                                        break;
                                }
                            }else{
                                if(transform.position.z>-100){
                                    switch(t_course)
                                    {
                                        case 1:
                                            rb.AddForce((transform.position.x+22) * -20, 6 * thrust, 38 * thrust); //フォア
                                            break;
                                        case 2:
                                            rb.AddForce((transform.position.x) * -20, 6 * thrust, 38 * thrust); //ミドル
                                            break;
                                        case 3:
                                            rb.AddForce((transform.position.x-22) * -20, 6 * thrust, 38 * thrust); //バックは無理
                                            break;
                                        case 4:
                                            rb.AddForce((transform.position.x+22) * -25, 4 * thrust, 38 * thrust); //前フォア
                                            break;
                                        case 5:
                                            rb.AddForce(transform.position.x * -25, 4 * thrust, 38 * thrust); //前ミドル
                                            break;
                                        case 6:
                                            rb.AddForce((transform.position.x-22) * -20, 4 * thrust, 36 * thrust); //前バックは無理
                                            break;
                                    }
                                }else{
                                    switch(t_course)
                                    {
                                        case 1:
                                            rb.AddForce((transform.position.x+22) * -20, 10 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 2:
                                            rb.AddForce((transform.position.x) * -20, 10 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 3:
                                            rb.AddForce((transform.position.x-22) * -20, 10 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 4:
                                            rb.AddForce((transform.position.x+22) * -20, 8 * thrust, 36 * thrust); //フォア
                                            break;
                                        case 5:
                                            rb.AddForce((transform.position.x) * -20, 8 * thrust, 36 * thrust); //ミドル
                                            break;
                                        case 6:
                                            rb.AddForce((transform.position.x-22) * -20, 8 * thrust, 36 * thrust); //バックは無理
                                            break;
                                    }
                                }
                            }
                            break;

                        default:
                            switch(t_course){
                                case 1:
                                    rb.AddForce((transform.position.x+22) * -20, 10 * thrust, 36 * thrust); //フォア
                                    break;
                                case 2:
                                    rb.AddForce((transform.position.x) * -20, 10 * thrust, 36 * thrust); //フォア
                                    break;
                                case 3:
                                    rb.AddForce((transform.position.x-22) * -20, 10 * thrust, 36 * thrust); //フォア
                                    break;
                                case 4:
                                    rb.AddForce((transform.position.x+22) * -20, 8 * thrust, 36 * thrust); //フォア
                                    break;
                                case 5:
                                    rb.AddForce((transform.position.x) * -20, 8 * thrust, 36 * thrust); //ミドル
                                    break;
                                case 6:
                                    rb.AddForce((transform.position.x-22) * -20, 8 * thrust, 36 * thrust); //バックは無理
                                    break;
                            }
                            break;
                    }
                    break;
                }
                break;
        }
    }

    void EnemyModel()
    {
        switch(issue_num){
            case 1:
                r_style = 1;
                r_course = Random.Range(1,7);
                t_style = 0;
                t_course = 0;
                if(s_style==2&&s_course>3){
                    issue_ok_cou++;
                    judge = true;
                }else if(s_style==7&&s_course>3){
                    issue_ok_cou++;
                    judge = true;
                }
                break;

            case 2:
                r_style = 2;
                t_style = 1;
                t_course = 1;
                switch(s_course){
                    case 1:
                        r_course = 1;
                        issue_ok_cou++;
                        judge = true;
                        break;

                    case 2:
                        r_course = 1;
                        issue_ok_cou++;
                        judge = true;
                        break;

                    case 3:
                        r_course = 3;
                        break;
                    
                    case 4:
                        r_course = 1;
                        issue_ok_cou++;
                        judge = true;
                        break;

                    case 5:
                        r_course = 1;
                        issue_ok_cou++;
                        judge = true;
                        break;

                    case 6:
                        r_course = 3;
                        break;
                }
                break;

            case 3:
                r_style = 2;
                r_course = Random.Range(1,7);
                t_style = 1;
                t_course = Random.Range(1,4);
                if(s_style==2||s_style==3||s_style==4){
                    if(s_course>3){

                    }else{
                        issue_ok_cou++;
                        judge = true;
                    }
                }else{
                    issue_ok_cou++;
                    judge = true;
                }
                break;

            case 4:
                t_style = 4;
                t_course = Random.Range(1,2);
                switch(s_course){
                    case 4:
                        r_style = Random.Range(2,4);
                        r_course = 6;
                        break;

                    case 5:
                        r_style = Random.Range(2,4);
                        r_course = 5;
                        break;

                    case 6:
                        r_style = Random.Range(2,4);
                        r_course = 4;
                        break;

                    default:
                        r_style = Random.Range(1,7);
                        r_course = Random.Range(1,7);
                        break;
                }
                if(r_course!=3&&r_course!=6){
                    issue_ok_cou++;
                    judge = true;
                }
                break;

            case 5:
                t_style = 5;
                t_course = Random.Range(2,4);
                switch(s_course){
                    case 4:
                        r_style = Random.Range(2,4);
                        r_course = 6;
                        break;

                    case 5:
                        r_style = Random.Range(2,4);
                        r_course = 5;
                        break;

                    case 6:
                        r_style = Random.Range(2,4);
                        r_course = 4;
                        break;

                    default:
                        r_style = Random.Range(1,7);
                        r_course = Random.Range(1,7);
                        break;
                }
                if(r_course!=1&&r_course!=4){
                    issue_ok_cou++;
                    judge = true;
                }
                break;                
        }
    }


    void Judgement()
    {
        switch(issue_num)
        {
            case 1:
                if(t_style==6){
                    judge = true;
                }
                break;

            case 2:
                if(r_course<=2){
                    judge = true;
                }
                break;

            case 3:
                if(r_course<=2){
                    judge = true;
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
        turncou = -3;
        r_style = Random.Range(1, 6);
        r_course = Random.Range(1, 7);
        s_style = 0;
        s_course = 0;
        boundcount = 0;
        S_Check.SetActive(false);
        result_title.SetActive(false);
        firstshot_result.SetActive(false);
        first_arrow.SetActive(false);
        secondshot_result.SetActive(false);
        secondo_arrow.SetActive(false);
        thirdshot_result.SetActive(false);
        rally_result.SetActive(false);
        style_select.SetActive(true);
        coatset.SetActive(true);
        issue.SetActive(true);
        result_panel.SetActive(false);
        rb.isKinematic = true;
        if(repeat==true){
            issue_num = issue_num; //同じ問題
        }else{
            issue_cou++;
            issue_num = issue_array[Random.Range(1,6-issue_cou)]; //問題決定
            issue_array.Remove(issue_num);
        }

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
