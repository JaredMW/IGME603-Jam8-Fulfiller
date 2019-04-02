using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject backBlock;
    public GameObject backBlock2;
    public GameObject blobBlock;
    public GameObject playBlock;
    public GameObject ctrlBlock;
    public GameObject infoBlock;

    public GameObject backTextForInfo;
    public GameObject backTextForCtrls;
    public GameObject ctrlText;
    public GameObject infoText;

    public string levelName;

    private int state = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float mx = mousePosWorld.x;
            float my = mousePosWorld.y;

            switch (state)
            {
                // If currently main menu
                case 0:
                    if (mx >= playBlock.GetComponent<BoxCollider2D>().bounds.min.x && mx <= playBlock.GetComponent<BoxCollider2D>().bounds.max.x)
                    {
                        if (my >= playBlock.GetComponent<BoxCollider2D>().bounds.min.y && my <= playBlock.GetComponent<BoxCollider2D>().bounds.max.y) // Play
                        {
                            Application.LoadLevel(levelName);
                        }
                        else if (my >= ctrlBlock.GetComponent<BoxCollider2D>().bounds.min.y && my <= ctrlBlock.GetComponent<BoxCollider2D>().bounds.max.y) // Ctrl
                        {
                            SwitchState(0, 1);
                        }
                        else if (my >= infoBlock.GetComponent<BoxCollider2D>().bounds.min.y && my <= infoBlock.GetComponent<BoxCollider2D>().bounds.max.y) // Info
                        {
                            SwitchState(0, 2);
                        }
                    }
                    break;
                // If currently ctrls
                case 1:
                    if (mx >= playBlock.GetComponent<BoxCollider2D>().bounds.min.x && mx <= playBlock.GetComponent<BoxCollider2D>().bounds.max.x
                        && my >= playBlock.GetComponent<BoxCollider2D>().bounds.min.y && my <= playBlock.GetComponent<BoxCollider2D>().bounds.max.y) // Play
                    {
                        Application.LoadLevel(levelName);
                    }
                    if (mx >= backBlock2.GetComponent<BoxCollider2D>().bounds.min.x && mx <= backBlock2.GetComponent<BoxCollider2D>().bounds.max.x
                        && my >= backBlock2.GetComponent<BoxCollider2D>().bounds.min.y && my <= backBlock2.GetComponent<BoxCollider2D>().bounds.max.y)
                    {
                        SwitchState(1, 0);
                    }
                    break;
                // If currently info
                case 2:
                    if (mx >= playBlock.GetComponent<BoxCollider2D>().bounds.min.x && mx <= playBlock.GetComponent<BoxCollider2D>().bounds.max.x
                        && my >= playBlock.GetComponent<BoxCollider2D>().bounds.min.y && my <= playBlock.GetComponent<BoxCollider2D>().bounds.max.y) // Play
                    {
                        Application.LoadLevel(levelName);
                    }
                    if (mx >= backBlock.GetComponent<BoxCollider2D>().bounds.min.x && mx <= backBlock.GetComponent<BoxCollider2D>().bounds.max.x
                        && my >= backBlock.GetComponent<BoxCollider2D>().bounds.min.y && my <= backBlock.GetComponent<BoxCollider2D>().bounds.max.y)
                    {
                        SwitchState(2, 0);
                    }
                    break;
            }
        }
    }

    private void SwitchState(int oldState, int newState)
    {
        // Leaving main menu
        if (oldState == 0)
        {
            //mainMenu.SetActive(false);

            backBlock.SetActive(true);
            backBlock2.SetActive(true);
            blobBlock.SetActive(true);
            //backTextForInfo.SetActive(true);

            // State 1 - going to information
            if (newState == 1)
            {
                ctrlBlock.SetActive(false);
                backTextForInfo.SetActive(true);
                ctrlText.SetActive(true);
            }

            // State 2 - going to controls
            else
            {
                infoBlock.SetActive(false);
                backTextForCtrls.SetActive(true);
                infoText.SetActive(true);
            }
        }

        // State 0 - main menu
        else if (newState == 0)
        {
            mainMenu.SetActive(true);
            backBlock.SetActive(false);
            backBlock2.SetActive(false);
            backTextForCtrls.SetActive(false);
            backTextForInfo.SetActive(false);
            blobBlock.SetActive(false);
            infoBlock.SetActive(true);
            ctrlBlock.SetActive(true);
            backTextForInfo.SetActive(false);

            // If coming from information
            if (oldState == 1) { ctrlText.SetActive(false); }

            // If not coming from information
            else { infoText.SetActive(false); }
        }

        // State 1/2 - Going to information or controls
        else
        {
            // Going to info
            if (newState == 1)
            {
                infoBlock.SetActive(false);
            }
            // Going to controls
            if (newState == 2)
            {
                ctrlBlock.SetActive(false);
                //backTextForCtrls.SetActive(true)
            }

            // State 1 - coming from information
            if (oldState == 1)
            {
                backTextForInfo.SetActive(false);
                ctrlText.SetActive(false);
                infoText.SetActive(true);
            }
            // State 2 - coming from controls
            else
            {
                backTextForCtrls.SetActive(false);
                infoText.SetActive(false);
                ctrlText.SetActive(true);
            }
        }

        state = newState;
    }
}
