using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject backBlock;
    public GameObject blobBlock;

    public GameObject backText;
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
                case 0:
                    if (mx >= -1.5f && mx <= 1.2f)
                    {
                        if (my >= 1.4f && my <= 2.9f) // Play
                        {
                            Application.LoadLevel(levelName);
                        }
                        else if (my >= -0.5f && my <= 0.9f) // Ctrl
                        {
                            SwitchState(0, 1);
                        }
                        else if (my >= -2.5f && my <= -1.0f) // Info
                        {
                            SwitchState(0, 2);
                        }
                    }
                    break;
                case 1:
                    if (mx >= -1.5f && mx <= 1.2f && my >= -3.4f && my <= -1.9f)
                    {
                        SwitchState(1, 0);
                    }
                    break;
                case 2:
                    if (mx >= -1.5f && mx <= 1.2f && my >= -3.4f && my <= -1.9f)
                    {
                        SwitchState(2, 0);
                    }
                    break;
            }
        }
    }

    private void SwitchState(int oldState, int newState)
    {
        if (oldState == 0)
        {
            mainMenu.SetActive(false);
            backBlock.SetActive(true);
            blobBlock.SetActive(true);
            backText.SetActive(true);

            if (newState == 1) { ctrlText.SetActive(true); }
            else { infoText.SetActive(true); }
        }
        else if (newState == 0)
        {
            mainMenu.SetActive(true);
            backBlock.SetActive(false);
            blobBlock.SetActive(false);
            backText.SetActive(false);

            if (oldState == 1) { ctrlText.SetActive(false); }
            else { infoText.SetActive(false); }
        }
        else
        {
            if (oldState == 1)
            {
                ctrlText.SetActive(false);
                infoText.SetActive(true);
            }
            else
            {
                infoText.SetActive(false);
                ctrlText.SetActive(true);
            }
        }

        state = newState;
    }
}
