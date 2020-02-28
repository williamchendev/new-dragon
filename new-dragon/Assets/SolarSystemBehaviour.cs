using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemBehaviour : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

public class PlanetoidBehaviour : MonoBehaviour {

    // Settings

    // Variables
    public List<GameObject> layers;

    // Planetoid Initialization
    void Start()
    {
        // Variables
        layers = new List<GameObject>();
    }

    // Planetoid Update Behaviour
    void Update()
    {
        
    }

    // Layer Init
    public void createLayer() {
        GameObject new_layer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        layers.Add(new_layer);
    }

    // Layer Texture Init
    public void textureLayer(int layer) {
        // Create Color Data
        int temp_width = 100;
        Color[] temp_tex_array = new Color[temp_width * temp_width];
        for (int h = 0; h < temp_width; h++) {
            for (int w = 0; w < temp_width; w++) {
                Color temp_color = Color.black;
                if (Mathf.PerlinNoise(w / (temp_width * 1f), h / (temp_width * 1f)) < 0.5f) {
                    temp_color = Color.white;
                }
                temp_tex_array[w + (h * temp_width)] = temp_color;
            }
        }

        // Create Texture
        Texture2D temp_tex = new Texture2D(temp_width, temp_width);
        temp_tex.filterMode = FilterMode.Point;
        temp_tex.SetPixels(temp_tex_array);
        temp_tex.Apply();
    }

}
