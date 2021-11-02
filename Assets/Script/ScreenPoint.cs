using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPoint : MonoBehaviour
{
    RaycastHit _hit;
    GameObject selectpos;
    GameObject Ball2;

    // Start is called before the first frame update
    void Start()
    {
        selectpos = GameObject.Find("selectposition");
        // Ball2 = GameObject.Find("Ball3");
        // selectpos.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // selectpos.SetActive(Ball2.GetComponent<RptateTest2>().rate);

        // Unity上での操作取得
        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out _hit))
                {
                    // 選択されたオブジェクトへの処理
                    HitObj();
                }
            }
        }
        // 端末上での操作取得
        else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began)
                {
                    var _ray = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(_ray, out _hit))
                    {
                        // 選択されたオブジェクトへの処理
                        HitObj();
                    }
                }
            }
        }
    }

    // 選択されたオブジェクトへの処理
    private void HitObj()
    {
        print(_hit.transform.name);
    }

}