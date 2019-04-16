using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    bool _change;
    bool _light;



    Actor curentActor;
    public bool mainMenu;
    public Dropdown m_Dropdown;
    public CamCustom CamCustom;
     Vector3 originalPosition;
    Quaternion originalRotation;
    Vector3 curentP;
    Vector3 lastPosition;
    Quaternion lastRotation;
    public Light lightDirectional;

    public float cameraSensitivity = 90;
    public float climbSpeed = 4;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;
    float rotationX = 0.0f;
    float rotationY = 0.0f;
    public string m_case;
    bool oneTime;
    int oldMask;
    // Start is called before the first frame update
    void Start()
    {
        oldMask = this.gameObject.GetComponent<Camera>().cullingMask;
        originalPosition = this.gameObject.transform.position;
        originalRotation = this.gameObject.transform.rotation;

        m_Dropdown = m_Dropdown.GetComponent<Dropdown>();
        CamCustom = CamCustom.GetComponent<CamCustom>();

        m_Dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged();
        });


    }
    void DropdownValueChanged()
    {

        if ("Observer" == m_Dropdown.options[m_Dropdown.value].text )
        {
            transform.position = curentActor.transform.position;
            transform.rotation = curentActor.transform.rotation;
        }
       

    }


     void Update()
    {
        //Debug.Log(oneTime);
        if (!mainMenu)
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;

            int layerMask = 1 << 8;
            layerMask = ~layerMask;
            this.gameObject.GetComponent<Camera>().cullingMask = layerMask;
        }
        else
        {
            checkCase();
        }

        if (Input.GetKey(KeyCode.LeftShift))
               {

        }
    }

    // Update is called once per frame
    void checkCase()
    {
        curentActor = CamCustom.actorView;
        if (mainMenu)
        {
            m_case = m_Dropdown.options[m_Dropdown.value].text;
            switch (m_case)
            {
                case "top":
                    transform.position = originalPosition;
                    transform.rotation = originalRotation;

                    break;
                case "First Pearson":
                    transform.position = curentActor.transform.position;
                    transform.rotation = curentActor.transform.rotation;

                    break;

                case "axonometric":
                    transform.position = new Vector3(-116, 107, -110);
                    Vector3 rot = new Vector3(30, 45, 0);
                    transform.rotation = Quaternion.Euler(rot);
                    break;

                case "Observer":
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
                    }
                    if (Cursor.lockState == CursorLockMode.None)
                        return;
                    rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
                        rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
                        rotationY = Mathf.Clamp(rotationY, -90, 90);

                        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
                        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

                        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        {
                            transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
                            transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
                        }
                        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                        {
                            transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
                            transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
                        }
                        else
                        {
                            transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
                            transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
                        }
                    
                    break;

                default:
                    break;
            }
        }
 
    }


    public void ShowLines()
    {
        _change = !_change;
        if (!_change) this.gameObject.GetComponent<Camera>().cullingMask = oldMask;

        else
        {
            int layerMask = 1 << 8;
            layerMask = ~layerMask;
            this.gameObject.GetComponent<Camera>().cullingMask = layerMask;
        }
    }

    public void turnLightOn()
    {
        _light = !_light;
        lightDirectional.gameObject.SetActive(_light);
    }


}
