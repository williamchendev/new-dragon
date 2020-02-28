using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Asteroids : MonoBehaviour
{

    public Image img;
    public DrawUtil util;

    // Start is called before the first frame update
    void Start()
    {
        DrawPrim prim = new DrawPrim(1920, 1080);
        //util = new DrawUtil(img.material.text);
        util.drawFill(Color.black);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
