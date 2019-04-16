using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ui_interactive : MonoBehaviour
{
    public RectTransform[] rt;
    public Button[] _menu;
    float[] lerp = new float[2];

    float[] maximum = new float[2], minimum = new float[2];
    float[] maximum1 = new float[2], minimum1 = new float[2];
   public CameraManager cameraManager;

    RectTransform[] buttonRect = new RectTransform[2];


    int[] _counter = new int[2];

    Vector2[] origPosition = new Vector2[2];
    // Start is called before the first frame update
    void Start()
    {
        cameraManager = cameraManager.GetComponent<CameraManager>();

        for (int i = 0; i <= 1; i++)
        {
            //minimum[i] = rt[i].sizeDelta.y;
         

            minimum1[i] = 0;
            maximum1[i] = 360;

            buttonRect[i] = _menu[i].gameObject.GetComponent<RectTransform>();
            origPosition [i] = rt[i].localPosition;

            minimum[i] = origPosition[i].x;

            if(i == 1 ) maximum[i] = origPosition[i].x - rt[i].rect.height / 1.4f;
            else maximum[i] = origPosition[i].x + rt[i].rect.height / 1.4f;
                
           


        }

        _menu[0].onClick.AddListener(() => ButtonClicked(0));
        _menu[1].onClick.AddListener(() => ButtonClicked(1));

        //minimum[1] = 0;
        //maximum[1] = 0;

    }
 

    private void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            lerp[i] += Time.deltaTime / 0.5f;
            float temp = 0;
            temp = Mathf.Lerp(minimum[i], maximum[i], lerp[i]);
            Rect tmp = rt[i].rect;
            //tmp.position = new Vector2(origPosition[i].x + temp, origPosition[i].y);
            rt[i].localPosition = new Vector2(temp, origPosition[i].y); 
            //rt[i].sizeDelta = new Vector2(rt[i].sizeDelta.x, temp);

            float temp2 = Mathf.Lerp(minimum1[i], maximum1[i], lerp[i]);
            if (_counter[i] != 0)
                buttonRect[i].localRotation = Quaternion.Euler(0, temp2 ,0);
        }

        //if () customCamera.mainMenu = true;
    }
   

    void ButtonClicked(int buttonNo)
    {
        _counter[buttonNo]++;
        if (_counter[buttonNo] > 1) _counter[buttonNo] = 0;

        ChangeDirection(buttonNo);
        //ChangeDirection1(buttonNo);

    }

    void ChangeDirection(int index){
            float temps = maximum[index];
            maximum[index] = minimum[index];
            minimum[index] = temps;
            lerp[index] = 0.0f;
    }

    private Vector2 GetMidPoint(Rect recta)
    {
        Vector2 borderPint = new Vector2(0, recta.yMax);
        return borderPint;

    }
    void ChangeDirection1(int index)
    {
        float temps = maximum1[index];
        maximum1[index] = minimum1[index];
        minimum1[index] = temps;
        lerp[index] = 0.0f;
    }

    void OnDisable()
    {
        cameraManager.mainMenu = false;
    }

    private void OnEnable()
    {
        cameraManager.mainMenu = true;

    }

}
