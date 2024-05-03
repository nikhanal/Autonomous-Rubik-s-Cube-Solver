using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject arrows;
    Transform childArrow;
    public Text twoTimesText;
    
    void Start()
    {
        // Hide all arrow at the start of program
        DeactivateArrow();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeactivateMainArrow()
    {
        arrows.SetActive(false);
        twoTimesText.gameObject.SetActive(false);
    }
    public void ActivateMainArrow()
    {
        arrows.SetActive(true);
        twoTimesText.gameObject.SetActive(true);
    }

    //activate required arrow according to move
    public void DisplayArrow(string arrow)
    {
        if(arrow =="R2"|| arrow == "L2" || arrow == "D2" || arrow == "U2" || arrow == "F2" || arrow == "B2")
        {
            arrow = arrow[0].ToString();
            twoTimesText.text = "x2";
        }
        arrow += "Arrow";
        childArrow = arrows.transform.Find(arrow);
        childArrow.gameObject.SetActive(true);
    }

    public void HideArrow(string arrow)
    {
        arrow += "Arrow";
        childArrow = arrows.transform.Find(arrow);
        childArrow.gameObject.SetActive(false);
    }
    
    public void DeactivateArrow()
    {
        twoTimesText.text = "";
        foreach(Transform child in arrows.transform)
        {
            child.gameObject.SetActive(false);
        }

    }
}
