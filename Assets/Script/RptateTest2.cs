using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RptateTest2 : MonoBehaviour {

    GameObject selectpos;
    GameObject CoatSet;
    GameObject ButtonSet;
    GameObject leftbtm;
    GameObject centerbtm;
    GameObject rightbtm;
    GameObject frontleftbtm;
    GameObject frontcenterbtm;
    GameObject frontrightbtm;

    [Tooltip( "How many frames to average over for computing velocity" )]
    public int velocityAverageFrames = 5;
    [Tooltip( "How many frames to average over for computing angular velocity" )]
    public int angularVelocityAverageFrames = 11;

    public bool estimateOnAwake = false;

    private Coroutine routine;
    private int sampleCount;
    private Vector3[] velocitySamples;
    private Vector3[] angularVelocitySamples;

    public Vector3 axis = Vector3.up;

    public float rotSpeed = 0;
    public float power = 0.1f;
    public float friction = 0.98f;

    public float stickPower = 1000;

    public float timer = 0;

    public bool isMove  = true;
    public bool isMove2  = true;
    public bool isMove3  = true;
    public bool isMove4  = true;
    public float boundcount;
    public bool rate = false;
    public bool lefBt = false;
    public bool cenBt = false;
    public bool rigBt = false;
    public bool flefBt = false;
    public bool fcenBt = false;
    public bool frigBt = false;
    public float turn = 0;

    public Rigidbody rb;
    public float thrust;

    public bool ballshot = false;

    public bool OnCollision = false;

    Vector3 prevPos;
    
    Vector3 rotVec = Vector3.zero; // 回転ベクトル

    Vector3 rotKeyVec = Vector3.zero;

    Vector3 rotat = Vector3.zero;

    Vector3 angvel = Vector3.zero;
    Vector3 angvel2 = Vector3.zero;
    Vector3 angvel3 = Vector3.zero;

    Vector3 vec3 = Vector3.zero;

    Vector3 firstpos;
    Vector3 firstrota;

    float startTime;
    float vec2;
    float boundbef;
    float boundnow;
    float a;
    float cliccount=0;
    
    void Start () {
        //Rigidbodyを取得する
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 100;
        firstpos = transform.position;
        boundcount = 0;
        CoatSet = GameObject.Find("CoatSet");
        ButtonSet = GameObject.Find("ButtonSet");
        leftbtm = GameObject.Find("leftBtm");
        centerbtm = GameObject.Find("centerBtm");
        rightbtm = GameObject.Find("rightBtm");
        frontleftbtm = GameObject.Find("frontleftBtm");
        frontcenterbtm = GameObject.Find("frontcenterBtm");
        frontrightbtm = GameObject.Find("frontrightBtm");


        CoatSet.SetActive(false);
        // selectpos = GameObject.Find("selectposition");
    }
    
	// Update is called once per frame
	void Update () {
        
        // Debug.Log(GetVelocityEstimate());

        // マウスフリックで操作
        if (Input.GetMouseButtonDown(0)) //押された時
        {
            prevPos = Input.mousePosition; //座標取得
            startTime = Time.realtimeSinceStartup; //時間取得
            ButtonTrue();
        }

        if (Input.GetMouseButtonUp(0)) //指が離れた時
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            if(rate == true){
                cliccount++;
                rb.isKinematic = false;
                // if(cliccount==2){
                //     rate=false;
                //     cliccount=0;
                // }
            }else{
                
            }
            // boundcount = 0;
            // rb.useGravity = true;
            Vector3 pos = Input.mousePosition; //座標取得
            Vector3 diff = pos - prevPos; //指が離れた時座標 - 押された時座標
            // diff = diff.normalized;
            Vector3 diff2 = Quaternion.Euler(0, 0, -90) * diff;
            rb.AddTorque(diff2*5);
            vec2 = diff2.magnitude/10;
            rb.AddForce(diff.x, 4 * thrust, vec2*24);
        }


        if(OnCollision == true){
            angvel = GetAngularVelocityEstimate();
            // angvel = Quaternion.Euler(0, -90, 0) * angvel;
            vec3 = angvel.normalized;
            rb.AddTorque(-vec3*1000);
            // rb.AddForce(0, 0, vec3*10); //前方に向けて発射
            OnCollision = false;
            boundcount++;
        }


        boundnow = GetVelocityEstimate().y;
        //頂点or台の外の時ボールを止める
        if(boundcount>=2){
            if((boundbef>0&&boundnow<0)||transform.position.z<-105){
                rb.isKinematic = true;
                rate = true;
                CoatBtmT();
                ButtonSet.SetActive(false);
                // rb.AddTorque(angvel);
            }
        }else{
                angvel2 = GetAngularVelocityEstimate();
                CoatBtmF();
        }
        boundbef = GetVelocityEstimate().y;


        if(rate == true){
            angvel3.x += angvel2.x;
            angvel3.y += angvel2.y;
            transform.rotation = Quaternion.Euler(angvel3);
        }

        if(lefBt == true){
            boundcount = 0;
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce((transform.position.x+24) * -20, 9 * thrust, 42 * thrust);
            lefBt = false;
            turn = 1;
        }
        if(cenBt == true){
            boundcount = 0;
            rb.isKinematic = false;
            rb.useGravity = true;
            rate=false;
            rb.AddForce(transform.position.x * -20, 9 * thrust, 42 * thrust);
            cenBt = false;
            turn = 1;
        }
        if(rigBt == true){
            boundcount = 0;
            rb.isKinematic = false;
            rb.useGravity = true;
            rate=false;
            rb.AddForce((transform.position.x-24) * -20, 9 * thrust, 42 * thrust);
            rigBt = false;
            turn = 1;
        }
        if(flefBt == true){

            flefBt = false;
            turn = 1;
        }
        if(fcenBt == true){

            fcenBt = false;
            turn = 1;
        }
        if(frigBt == true){

            frigBt = false;
            turn = 1;
        }
        

        if (isMove == false) //上回転ボタン
        {
            if(rb.isKinematic == true){
                rb.isKinematic = false;
                rb.useGravity = true;
            }
            rotat.x = -1.0f;
            rotat = rotat.normalized * stickPower;
            rb.AddTorque(rotat*10000);
            rb.AddForce(Random.Range(-12, 12) * thrust, -15 * thrust, -30 * thrust); //前方に向けて発射
            // AddRotateVector(rotat*100f);
            isMove = true;
        }

        if (isMove2 == false) //下回転ボタン
        {
            if(rb.isKinematic == true){
                rb.isKinematic = false;
                rb.useGravity = true;
            }
            rotat.x = 1.0f;
            rotat = rotat.normalized * stickPower;
            rb.AddTorque(rotat*10000);
            rb.AddForce(Random.Range(-2, 2) * thrust, 7 * thrust, -15 * thrust); //前方に向けて発射
            isMove2 = true;
        }

        if (isMove3 == false) //逆横ボタン
        {
            if(rb.isKinematic == true){
                rb.isKinematic = false;
                rb.useGravity = true;
            }
            // rotat.y = 1.0f;
            // rotat = rotat.normalized * stickPower;
            // rb.AddTorque(rotat*50);
            rb.AddForce(Random.Range(-12, 12) * thrust, -10 * thrust, -25 * thrust); //前方に向けて発射
            isMove3 = true;
        }

        
        if (isMove4 == false) //順横ボタン
        {
            // rotat.y = -1.0f;
            // rotat = rotat.normalized * stickPower;
            // rb.AddTorque(rotat*1000);

            //リセットボタンにする
            transform.position = firstpos;
            rb.isKinematic = true;
            rb.useGravity = false;
            isMove4 = true;
            boundcount = 0;
            rate = false;
            turn = 0;
            // gameObject.rd.enable = true;
        }

        // 十字キーで操作
        rotKeyVec.x = Input.GetAxis("Horizontal");
        rotKeyVec.y = Input.GetAxis("Vertical");
        rotKeyVec = rotKeyVec.normalized * stickPower;

        AddRotateVector(rotKeyVec);

        // 摩擦
        rotVec *= friction;
        rotSpeed = rotVec.magnitude; //平方根の長さ(ベクトル)

        //public static Quaternion AngleAxis (float angle, Vector3 axis)
        //axisの周りをangle度回転する 毎フレーム回転させる
        // transform.rotation = Quaternion.AngleAxis(rotSpeed * Time.deltaTime, axis) * transform.rotation;
    }

    /// <summary>
    /// 回転ベクトル加算
    /// </summary>
    /// <param name="vec"></param>
    private void AddRotateVector(Vector3 vec)
    {
        Vector3 diff2 = transform.position - Camera.main.transform.position;
        diff2.Normalize();

        float p = vec.magnitude * power * 200;  // 回転速度計算

        vec.Normalize();
        
        rotVec += vec * p;  // 足す

        rotSpeed = rotVec.magnitude;    // 新しい回転速度計算
        axis = Vector3.Cross(rotVec.normalized, diff2); // 新しい回転軸計算
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + axis * 10f);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pos, pos + rotVec * 10f);

    }

    private void ButtonTrue() //ボタンを全てtrueに
    {
        isMove = true;
        isMove2 = true;
        isMove3 = true;
        isMove4 = true;
    }

    private void CoatBtmT()
    {
        CoatSet.SetActive(true);
        leftbtm.SetActive(true);
        centerbtm.SetActive(true);
        rightbtm.SetActive(true);
        frontleftbtm.SetActive(true);
        frontcenterbtm.SetActive(true);
        frontrightbtm.SetActive(true);
    }

    private void CoatBtmF()
    {
        CoatSet.SetActive(false);
        leftbtm.SetActive(false);
        centerbtm.SetActive(false);
        rightbtm.SetActive(false);
        frontleftbtm.SetActive(false);
        frontcenterbtm.SetActive(false);
        frontrightbtm.SetActive(false);
    }

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

    //当たった瞬間に呼ばれる関数
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name); // ログを表示する
        if(collision.gameObject.name == "Plane"){
            Debug.Log("Hit"); // ログを表示する
            OnCollision = true;
        }
    }

}

//操作性、ボタンの方が簡単？　わかりやすく、今は難しい