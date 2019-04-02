// Jared White
// April 1st, 2019
// DayCycle.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// A manager class that changes tile colors to simulate lighting, and controls the length of the game
/// </summary>
public class DayCycle : MonoBehaviour
{
    // =========================================================================
    // ||                             READ-ME:                                ||
    // ||                            How to use                               ||
    // =========================================================================
    // ||                                                                     ||
    // || Attach this class onto a manager object.                            ||
    // || Activate the day cycle by calling the StartDayCycle() function.     ||
    // || Set how long a single game lasts by setting "Length Of Day" in the  ||
    // ||   Inspector.                                                        ||
    // ||                                                                     ||
    // || You can stop the day cycle by calling the StopDayCycle() function,  ||
    // ||   but this class also self-regulates itself and executes EndDay()   ||
    // ||   functions, which you can add onto.                                ||
    // ||                                                                     ||
    // || The class will simulate light based on time of day by changing the  ||
    // ||   color of Tilemap objects. Separate your tilemaps into different   ||
    // ||   layers: outdoor layers, indoor layers, light sources, and light   ||
    // ||   halos. Place them into the appropriate Lists/fields in the        ||
    // ||   Inspector on this object.                                         ||
    // ||                                                                     ||
    // || Attach the parent (empty GameObject) of the Day Over UI to the      ||
    // ||   appropriate field. All of its childrens' opacities, if they are   ||
    // ||   either a Text object or an Image object, will be set to 0 at      ||
    // ||   start, and on Day Over their opacities will be restored to their  ||
    // ||   original values. Ensure they have EITHER Image or Text attached,  ||
    // ||   and NOT both components.                                          ||
    // ||                                                                     ||
    // =========================================================================


    // ======================
    //         FIELDS
    // ======================
    #region Fields and Members
    public bool startActivated = false;     // Whether the day cycle should start activated when the scene loads
    public float lengthOfDay;               // The amount of seconds in a day (game length)

    public Gradient lightColorOverTime;     // The set of colors that will appear outdoors over the course of a day
    public Gradient insideLightOverTime;    // The set of colors that will appear indoors over the course of a day
    public Gradient sourceColorOverTime;    // The set of colors that will appear on light sources over the course of a day
    public Gradient lightOpacityOverTime;   // The opacity of light source sprights over the course of a day
    public List<Tilemap> outdoorTilemaps;   // Tiles that are represented to be outdoors in the world
    public List<Tilemap> indoorTilemaps;    // Tiles that are represented to be indoors in the world
    public List<Tilemap> lightSources;      // Tiles that represent objects that are sources of light
    public List<Tilemap> lightHalos;        // Tiles that represent halos of light

    public GameObject dayOverOverlayParent; // An empty GameObject attached to the Canvas UI that displays at the end of the day.
    public GameObject restartButton;        // The restart button
    public float overlayAppearanceTime;     // The amount of time it takes the overlay to appear
    public float buttonAppearanceTime;      // The amount of time it takes the Next Day button to appear

    private Color currentOutdoorColor;  // The current light color outside the house
    private Color currentIndoorColor;   // The current light color inside the house
    private Color currentSourceColor;   // The current light color for light sources
    private Color currentHaloAlpha;     // The current opacity of light source halos

    private float dayTimer = 0f;        // A timer that keeps track of what the current time is
    private float percentThruDay = 0f;  // The current percentage of how far through the day we currently are
    private bool dayCycling = false;    // Whether time is currently progressing in the game
    private bool dayOver = false;       // Whether the day has ended (for external use and tracking purposes, if needed)

    private float uiTimer = 0f;         // A timer for tracking how long it should be taking the UI to appear
    private float uiTimerStart;         // A helper variable
    private float buttonTimer = 0f;     // A timer for tracking how long it should be taking the restart button to appear
    private List<Color> ogUIopacities;  // The starting UI opacity values
    #endregion


    // ======================
    //       PROPERTIES
    // ======================
    #region Properties
    /// <summary>
    /// Get the current outdoor lighting color of the world
    /// </summary>
    public Color CurrentOutdoorColor
    {
        get { return currentOutdoorColor; }
        set
        {
            currentOutdoorColor = value;
            SetColors();
        }
    }

    /// <summary>
    /// Get the current indoor lighting color of the world
    /// </summary>
    public Color CurrentIndoorColor
    {
        get { return currentIndoorColor; }
        set
        {
            currentIndoorColor = value;
            SetColors();
        }
    }

    /// <summary>
    /// Get or set the color for objects that represent sources of light
    /// </summary>
    public Color CurrentLightSourceColor
    {
        get { return currentSourceColor; }
        set
        {
            currentSourceColor = value;
            SetColors();
        }
    }

    /// <summary>
    /// Get or set the color and opacity for light source halos
    /// </summary>
    public Color CurrentLightOpacity
    {
        get { return currentHaloAlpha; }
        set
        {
            currentHaloAlpha = value;
            SetColors();
        }
    }

    /// <summary>
    /// Get or set whether time is currently progressing
    /// </summary>
    public bool DayCycling
    {
        get { return dayCycling; }
        set
        {
            if (value == true && dayCycling == false)
            {
                StartDayCycle();
            }
            else if (value == false && dayCycling == true)
            {
                StopDayCycle();
            }
            else
            {
                dayCycling = value;
            }
        }
    }

    /// <summary>
    /// Get whether the day has ended
    /// </summary>
    public bool DayOver
    {
        get { return dayOver; }
    }
    #endregion


    // ======================
    //        METHODS
    // ======================
    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        // Validate members
        if (lengthOfDay <= 0f)
        {
            Debug.LogError("Cannot have a negative or zero length of day; setting lengthOfDay to 300");
            lengthOfDay = 300f;
        }

        
        // Turn off the opacity of the Day Over UI.
        ogUIopacities = new List<Color>();
        Color thisColor;

        for (int i = 0; i < dayOverOverlayParent.transform.childCount; i++)
        {
            // Turn off image color opacity
            if (dayOverOverlayParent.transform.GetChild(i).GetComponent<Image>())
            {
                thisColor = dayOverOverlayParent.transform.GetChild(i).GetComponent<Image>().color;
                dayOverOverlayParent.transform.GetChild(i).GetComponent<Image>().color
                    = new Color(thisColor.r, thisColor.g, thisColor.b, 0);
                
                // Store the original opacity of this
                ogUIopacities.Add(thisColor);
            }
            // Turn of text color opacity
            else if (dayOverOverlayParent.transform.GetChild(i).GetComponent<Text>())
            {
                thisColor = dayOverOverlayParent.transform.GetChild(i).GetComponent<Text>().color;
                dayOverOverlayParent.transform.GetChild(i).GetComponent<Text>().color
                    = new Color(thisColor.r, thisColor.g, thisColor.b, 0);
                
                // Store the original opacity of this
                ogUIopacities.Add(thisColor);
            }
        }

        thisColor = restartButton.GetComponent<Image>().color;
        restartButton.GetComponent<Image>().color
            = new Color(thisColor.r, thisColor.g, thisColor.b, 0);

        thisColor = restartButton.transform.GetChild(0).GetComponent<Text>().color;
        restartButton.transform.GetChild(0).GetComponent<Text>().color
            = new Color(thisColor.r, thisColor.g, thisColor.b, 0);


        // Execute startup actions
        if (startActivated)
        {
            StartDayCycle();
        }
        else
        {
            // Set the colors of the world to match the starting time light
            currentOutdoorColor = lightColorOverTime.Evaluate(dayTimer);
            currentIndoorColor = insideLightOverTime.Evaluate(dayTimer);
            currentSourceColor = sourceColorOverTime.Evaluate(dayTimer);
            currentHaloAlpha  = lightOpacityOverTime.Evaluate(dayTimer);
            SetColors();
        }
    }


    // Update is called once per frame
    void Update()
    {
        // Update the day cycle when it is turned on
        if (dayCycling)
        {
            UpdateDayCycle();
        }

        // Update DayOver UI
        UpdateUI();

        if (dayOver && Input.GetMouseButtonDown(0))
        {
            Application.LoadLevel("LevelFunctional");
        }
    }


    /// <summary>
    /// Increment day cycle timers, and then set the world colors
    /// </summary>
    private void UpdateDayCycle()
    {
        // Update the time data relating to time of day
        dayTimer += Time.deltaTime;
        percentThruDay = dayTimer / lengthOfDay;

        // If the day is not yet over...
        if (percentThruDay < 1f)
        {
            // Update the colors
            SetColors();
        }
        else
        {
            // Validate percentThruDay and run all end of day functions
            percentThruDay = 1f;
            EndDay();
        }
    }


    /// <summary>
    /// Update the Day Over UI
    /// </summary>
    private void UpdateUI()
    {
        // If the timer is activated, update
        if (uiTimer > 0f)
        {
            // If the full time has passed, turn the opacity up to their original values
            if (Time.time > uiTimer)
            {
                // Turn off the opacity of the Day Over UI
                Color thisColor;
                for (int i = 0; i < dayOverOverlayParent.transform.childCount; i++)
                {
                    // Turn off image color opacity
                    if (dayOverOverlayParent.transform.GetChild(i).GetComponent<Image>())
                    {
                        thisColor = dayOverOverlayParent.transform.GetChild(i).GetComponent<Image>().color;
                        dayOverOverlayParent.transform.GetChild(i).GetComponent<Image>().color
                            = new Color(thisColor.r, thisColor.g, thisColor.b, ogUIopacities[i].a);
                    }
                    // Turn of text color opacity
                    if (dayOverOverlayParent.transform.GetChild(i).GetComponent<Text>())
                    {
                        thisColor = dayOverOverlayParent.transform.GetChild(i).GetComponent<Text>().color;
                        dayOverOverlayParent.transform.GetChild(i).GetComponent<Text>().color
                            = new Color(thisColor.r, thisColor.g, thisColor.b, ogUIopacities[i].a);
                    }
                }
                uiTimer = 0f;

                // Turn on the timer for the appearing button
                buttonTimer = Time.time + buttonAppearanceTime;
                uiTimerStart = Time.time;
            }

            // Otherwise, increment the opacities
            else
            {
                float timePercentage = 1f - (uiTimer - Time.time) / (uiTimer - uiTimerStart);

                // Turn off the opacity of the Day Over UI
                Color thisColor;
                for (int i = 0; i < dayOverOverlayParent.transform.childCount; i++)
                {
                    // Turn off image color opacity
                    if (dayOverOverlayParent.transform.GetChild(i).GetComponent<Image>())
                    {
                        thisColor = dayOverOverlayParent.transform.GetChild(i).GetComponent<Image>().color;
                        dayOverOverlayParent.transform.GetChild(i).GetComponent<Image>().color
                            = new Color(thisColor.r,
                                thisColor.g,
                                thisColor.b,
                                ogUIopacities[i].a * timePercentage);
                    }
                    // Turn of text color opacity
                    if (dayOverOverlayParent.transform.GetChild(i).GetComponent<Text>())
                    {
                        thisColor = dayOverOverlayParent.transform.GetChild(i).GetComponent<Text>().color;
                        dayOverOverlayParent.transform.GetChild(i).GetComponent<Text>().color
                            = new Color(thisColor.r,
                                thisColor.g,
                                thisColor.b,
                                ogUIopacities[i].a * timePercentage);
                    }
                }
            }
        }

        // If the restart button is currently appearing
        if (buttonTimer > 0f)
        {
            // If the current time has surpassed the timer, end the timer
            if (Time.time > buttonTimer)
            {
                Color thisColor = restartButton.GetComponent<Image>().color;
                restartButton.GetComponent<Image>().color
                    = new Color(thisColor.r, thisColor.g, thisColor.b, 1);

                thisColor = restartButton.transform.GetChild(0).GetComponent<Text>().color;
                restartButton.transform.GetChild(0).GetComponent<Text>().color
                    = new Color(thisColor.r, thisColor.g, thisColor.b, 1);

                buttonTimer = 0f;
            }

            else
            {
                float timePercentage = 1f - (buttonTimer - Time.time) / (buttonTimer - uiTimerStart);

                Color thisColor = restartButton.GetComponent<Image>().color;
                restartButton.GetComponent<Image>().color
                    = new Color(thisColor.r, thisColor.g, thisColor.b, timePercentage);

                thisColor = restartButton.transform.GetChild(0).GetComponent<Text>().color;
                restartButton.transform.GetChild(0).GetComponent<Text>().color
                    = new Color(thisColor.r, thisColor.g, thisColor.b, timePercentage);
            }
        }
    }


    /// <summary>
    /// Start the daytime progression cycle
    /// </summary>
    public void StartDayCycle()
    {
        dayCycling = true;
    }


    /// <summary>
    /// Stop the daytime progression cycle
    /// </summary>
    public void StopDayCycle()
    {
        dayCycling = false;
        SetColors();
    }


    /// <summary>
    /// Set the color of objects in the scene to be equivalent to the lightColorOverTime,
    /// insideLightOverTime, sourceColorOverTime, and lightOpacityOverTime gradients,
    /// relative to the current time that has passed in the game.
    /// </summary>
    private void SetColors()
    {
        currentOutdoorColor = lightColorOverTime.Evaluate(percentThruDay);
        currentIndoorColor = insideLightOverTime.Evaluate(percentThruDay);
        currentSourceColor = sourceColorOverTime.Evaluate(percentThruDay);
        currentHaloAlpha  = lightOpacityOverTime.Evaluate(percentThruDay);

        // (NOTE: I know this isn't the most efficient way to do this, it kind of just happened.
        // I'm sorry, to whoever is reading this.)

        // Set the colors for the tiles in outdoor tilemaps
        for (int i = 0; i < outdoorTilemaps.Count; i++)
        {
            outdoorTilemaps[i].color = currentOutdoorColor;
        }

        // Set the colors for the tiles in indoor tilemaps
        for (int i = 0; i < indoorTilemaps.Count; i++)
        {
            indoorTilemaps[i].color = currentIndoorColor;
        }

        // Set the colors for tiles that are sources of light
        for (int i = 0; i < lightSources.Count; i++)
        {
            lightSources[i].color = currentSourceColor;
        }

        // Set the opacity for light sources
        for (int i = 0; i < lightHalos.Count; i++)
        {
            lightHalos[i].color = currentHaloAlpha;
        }
    }


    /// <summary>
    /// Set the timer for the appearance of the overlay
    /// </summary>
    private void AppearUI()
    {
        uiTimer = Time.time + overlayAppearanceTime;
        uiTimerStart = Time.time;
    }


    /// <summary>
    /// Actions to execute once the end of the day has approached and passed
    /// </summary>
    private void EndDay()
    {
        StopDayCycle();
        dayOver = true;

        // Add extra "game over" functions HERE
        // ...
        // ...

        // Appear the Day Over UI
        AppearUI();
    }
    #endregion
}
