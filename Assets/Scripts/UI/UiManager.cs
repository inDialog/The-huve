using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject ui1, ui2;
    public int count;
    public static UiManager instance;

    void Awake()
    {
        instance = this;
    }

    public void Ui1(){
        ui1.gameObject.SetActive(true);
        ui2.gameObject.SetActive(false);
    }

    public void Ui2()
    {
        ui1.gameObject.SetActive(false);
        ui2.gameObject.SetActive(true);
    }
    public void CloseAll()
    {
        ui1.gameObject.SetActive(false);
        ui2.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("im presing");
            count ++;
            if (count > 2) count = 0;
            if (count == 1) CloseAll();
            if (count == 2) Ui2();

        }

    }



}
