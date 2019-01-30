using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationCubeColorChanger : MonoBehaviour {

    public GameObject Back;
    public GameObject Bottom;
    public GameObject Front;
    public GameObject Left;
    public GameObject Right;
    public GameObject Top;

    private Color backColor = new Color32(127, 127, 255, 255);
    private Color bottomColor = new Color32(210, 255, 210, 255);
    private Color frontColor = new Color32(210, 210, 255, 255);
    private Color leftColor = new Color32(255, 210, 210, 255);
    private Color rightColor = new Color32(255, 127, 127, 255);
    private Color topColor = new Color32(127, 255, 127, 255);

    private Color blueColor = new Color32(120, 160, 255, 255);

    void Start() {
        Back.GetComponent<Renderer>().material.SetColor("_Color", blueColor);
        Bottom.GetComponent<Renderer>().material.SetColor("_Color", blueColor);
        Front.GetComponent<Renderer>().material.SetColor("_Color", blueColor);
        Left.GetComponent<Renderer>().material.SetColor("_Color", blueColor);
        Right.GetComponent<Renderer>().material.SetColor("_Color", blueColor);
        Top.GetComponent<Renderer>().material.SetColor("_Color", blueColor);
    }

    public void ColorizeCube() {
        Back.GetComponent<Renderer>().material.SetColor("_Color", backColor);
        Bottom.GetComponent<Renderer>().material.SetColor("_Color", bottomColor);
        Front.GetComponent<Renderer>().material.SetColor("_Color", frontColor);
        Left.GetComponent<Renderer>().material.SetColor("_Color", leftColor);
        Right.GetComponent<Renderer>().material.SetColor("_Color", rightColor);
        Top.GetComponent<Renderer>().material.SetColor("_Color", topColor);
    }

    public void UncolorizeCube() {
        Back.GetComponent<Renderer>().material.SetColor("_Color", blueColor);
        Bottom.GetComponent<Renderer>().material.SetColor("_Color", blueColor);
        Front.GetComponent<Renderer>().material.SetColor("_Color", blueColor);
        Left.GetComponent<Renderer>().material.SetColor("_Color", blueColor);
        Right.GetComponent<Renderer>().material.SetColor("_Color", blueColor);
        Top.GetComponent<Renderer>().material.SetColor("_Color", blueColor);
    }
}
