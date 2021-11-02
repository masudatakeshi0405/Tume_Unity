using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
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


    public bool rate = false;
    public bool lefBt = false;
    public bool cenBt = false;
    public bool rigBt = false;
    public bool flefBt = false;
    public bool fcenBt = false;
    public bool frigBt = false;
    public float thrust;
    public float turncou = 0; //現在の状態を示す　0.最初　1.回転選択　2.コース選択　3.サービス発射　4.相手のレシーブ　5.3打目打法選択
    public float s_style = 0; //サービスの種類　1.上回転　2.下回転
    public float s_course = 0; //サービスのコース　1.左上　2.真ん中上　3.右上　4.左　5.真ん中　6.右
    public bool OnCollision = false;
    float boundbef;
    float boundnow;
    float boundcount;

    Rigidbody rb;

    Vector3 rotat = Vector3.zero;
    Vector3 angvel = Vector3.zero;
    Vector3 angvel2 = Vector3.zero;
    Vector3 angvel3 = Vector3.zero;
    Vector3 vec3 = Vector3.zero;
    Quaternion rot;
    Quaternion q;

    GameObject coat;
    GameObject services;
    GameObject t_style; //third_style
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 100;
        coat = GameObject.Find("CoatSet");
        services = GameObject.Find("s_style");
        t_style = GameObject.Find("third_style");
        t_style.SetActive(false);
        boundcount = 0;
        turncou = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(GetAngularVelocityEstimate());

        if(OnCollision == true){
            angvel = GetAngularVelocityEstimate();
            // angvel = Quaternion.Euler(0, -90, 0) * angvel;
            vec3 = angvel.normalized;
            rb.AddTorque(-vec3*50);
            // rb.AddForce(0, 0, vec3*10); //前方に向けて発射
            OnCollision = false;
            boundcount++;
        }


        boundnow = GetVelocityEstimate().y;
        //頂点or台の外の時ボールを止める
        if(boundcount==2){
            if((boundbef>0&&boundnow<0)||transform.position.z>30){
                if(turncou==3){
                    rb.isKinematic = true;
                    EnemyShot();
                }
            }
        }else if(boundcount==3&&turncou==4){
            if((boundbef>0&&boundnow<0)||transform.position.z<-115){
                rb.isKinematic = true;
                // rb.useGravity = false;
                rate = true;
                t_style.SetActive(true);
                coat.SetActive(true);
            }
        }else{
            angvel2 = GetAngularVelocityEstimate();
        }
        boundbef = GetVelocityEstimate().y;

        if(rate == true){
            angvel3.x += angvel2.x;
            angvel3.y += angvel2.y;
            transform.rotation = Quaternion.Euler(angvel3);
        }

        if(s_style==1){
            rate = false;
            angvel.x = 1f;
            rot = Quaternion.AngleAxis(10, angvel);
            q = transform.rotation;
            transform.rotation = q * rot;
            if(turncou==1){
                turncou = 1;
            }else if(turncou==5){
                turncou = 5;
            }else{
                turncou++;
            }
        }
        if(s_style==2){
            rate = false;
            angvel.x = -1f;
            rot = Quaternion.AngleAxis(10, angvel);
            q = transform.rotation;
            transform.rotation = q * rot;
            if(turncou==1){
                turncou = 1;
            }else if(turncou==5){
                turncou = 5;
            }else{
                turncou++;
            }
        }

        if(lefBt==true){
            s_course = 1;
            turncou++;
            lefBt=false;
        }
        if(cenBt==true){
            s_course = 2;
            turncou++;
            cenBt=false;
        }
        if(rigBt==true){
            s_course = 3;
            turncou++;
            rigBt=false;
        }
        if(flefBt==true){
            s_course = 4;
            turncou++;
            flefBt = false;
        }
        if(fcenBt==true){
            s_course = 5;
            turncou++;
            fcenBt = false;
            // s_style = 0;
        }
        if(frigBt==true){
            s_course = 6;
            turncou++;
            frigBt = false;
            // s_style = 0;
        }

        if(turncou==2){
            rb.isKinematic = false;
            rb.useGravity = true;
            rate = false;
            if(s_style==1){
                rotat.x = 1.0f;
                if(s_course==1){
                    rotat.z = 0.3f;
                    rb.AddForce(-2 * thrust, -7 * thrust, 27 * thrust); //前方に向けて発射
                }else if(s_course==2){
                    rb.AddForce(0 * thrust, -7 * thrust, 27 * thrust); //前方に向けて発射
                }else if(s_course==3){
                    rotat.z = -0.3f;
                    rb.AddForce(2 * thrust, -7 * thrust, 27 * thrust); //前方に向けて発射
                }
                rotat = rotat.normalized * 100;
            }else if(s_style==2){
                rotat.x = -1.0f;
                if(s_course==1){
                    rotat.z = -1.0f;
                    rb.AddForce(-5 * thrust, 6 * thrust, 20 * thrust);
                }else if(s_course==2){
                    rb.AddForce(0 * thrust, 6 * thrust, 20 * thrust);
                }else if(s_course==3){
                    rotat.z = 1.0f;
                    rb.AddForce(5 * thrust, 6 * thrust, 20 * thrust);
                }else if(s_course==4){
                    rotat.z = -0.15f;
                    rb.AddForce(-3 * thrust, 7 * thrust, 16 * thrust);
                }else if(s_course==5){
                    rb.AddForce(0 * thrust, 7 * thrust, 16 * thrust);
                }else if(s_course==6){
                    rotat.z = 1.0f;
                    rb.AddForce(3 * thrust, 7 * thrust, 16 * thrust);
                }
                rotat = rotat.normalized * 100;
            }
            s_style=0;
            // transform.rotation = Quaternion.Euler(0, -20, 0);
            rb.AddTorque(rotat*40);
            coat.SetActive(false);
            services.SetActive(false);
            turncou=3;
        }

        if(turncou==6){
            rb.isKinematic = false;
            rb.useGravity = true;
            rate = false;
            if(s_style==1){
                rotat.x = 1.0f;
                if(s_course==1){
                    rotat.z = 0.3f;
                    rb.AddForce((transform.position.x+24) * -20, 10 * thrust, 37 * thrust); //前方に向けて発射
                }else if(s_course==2){
                    rb.AddForce(0 * thrust, 10 * thrust, 37 * thrust); //前方に向けて発射
                }else if(s_course==3){
                    rotat.z = -0.3f;
                    rb.AddForce((transform.position.x-24) * -20, 10 * thrust,37 * thrust); //前方に向けて発射
                }
                rotat = rotat.normalized * 100;
            }else if(s_style==2){
                rotat.x = -1.0f;
                if(s_course==1){
                    rotat.z = -1.0f;
                    rb.AddForce(-5 * thrust, 6 * thrust, 20 * thrust);
                }else if(s_course==2){
                    rb.AddForce(0 * thrust, 6 * thrust, 20 * thrust);
                }else if(s_course==3){
                    rotat.z = 1.0f;
                    rb.AddForce(5 * thrust, 6 * thrust, 20 * thrust);
                }else if(s_course==4){
                    rotat.z = -0.15f;
                    rb.AddForce(-3 * thrust, 7 * thrust, 16 * thrust);
                }else if(s_course==5){
                    rb.AddForce(0 * thrust, 7 * thrust, 16 * thrust);
                }else if(s_course==6){
                    rotat.z = 1.0f;
                    rb.AddForce(3 * thrust, 7 * thrust, 16 * thrust);
                }
                rotat = rotat.normalized * 100;
            }
            s_style=0;
            // transform.rotation = Quaternion.Euler(0, -20, 0);
            rb.AddTorque(rotat*40);
            coat.SetActive(false);
            services.SetActive(false);
            turncou++;
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

    //相手のうち返の関数
    void EnemyShot()
    {
        rb.isKinematic = false;
        rotat.y = 1.0f;
        rb.AddTorque(rotat*40);
        rb.AddForce(0, 10 * thrust, -37 * thrust);
        turncou = 4;
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
