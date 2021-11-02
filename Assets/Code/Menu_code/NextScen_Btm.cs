using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScen_Btm : MonoBehaviour
{

    public int scen_num = 0;

    GameObject study01;
    GameObject study02;
    GameObject study03;
    GameObject issue;
    GameObject study01_c;
    GameObject study02_c;
    GameObject study03_c;
    GameObject issue_c;

    public void OnClick()
    {
        switch(scen_num)
        {
            case 1:
                SceneManager.LoadScene("S_Style02-07");
                break;

            case 2:
                SceneManager.LoadScene("S_Style05-06");
                break;

            case 3:
                SceneManager.LoadScene("S_Style03-04");
                break;

            case 4:
                SceneManager.LoadScene("S_Style02-07");
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        study01 = GameObject.Find("study01");
        study02 = GameObject.Find("study02");
        study03 = GameObject.Find("study03");
        issue = GameObject.Find("issue");
        study01_c = GameObject.Find("study01_c");
        study02_c = GameObject.Find("study02_c");
        study03_c = GameObject.Find("study03_c");
        issue_c = GameObject.Find("issue_c");
    }

    // Update is called once per frame
    void Update()
    {
        switch(scen_num)
        {
            case 1:
                BtmFalse();
                study01.SetActive(false);
                study01_c.SetActive(true);
                break;

            case 2:
                BtmFalse();
                study03.SetActive(false);
                study03_c.SetActive(true);
                break;

            case 3:
                BtmFalse();
                study03.SetActive(false);
                study03_c.SetActive(true);
                break;

            case 4:
                BtmFalse();
                issue.SetActive(false);
                issue_c.SetActive(true);
                break;

            default:
                BtmFalse();
                break;
        }
    }

    void BtmFalse()
    {
        study01.SetActive(true);
        study02.SetActive(true);
        study03.SetActive(true);
        issue.SetActive(true);
        study01_c.SetActive(false);
        study02_c.SetActive(false);
        study03_c.SetActive(false);
        issue_c.SetActive(false);
    }
}
