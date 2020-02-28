using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawPrim
{

    // Settings
    private int texture_width;
    private int texture_height;

    // Variables
    private Color[] colors;

    // Initialize
    public DrawPrim(int width, int height) {
        colors = new Color[width * height];
        texture_width = width;
        texture_height = height;
    }

    // Create Texture Page
    public Texture2D createTexture() {
        Texture2D temp_tex = new Texture2D(texture_width, texture_height);
        temp_tex.SetPixels(colors);
        temp_tex.Apply();
        return temp_tex;
    }

    // Draw Pixel
    public virtual void drawPixel(int x, int y, Color color) {
        colors[x + (y * texture_width)] = color;
    }

    // Draw Fill
    public virtual void drawFill(Color color) {
        for (int i = 0; i < colors.Length; i++) {
            colors[i] = color;
        }
    }
}

public class DrawPrimPartition : DrawPrim {

    // Settings
    private int texture_width;
    private int texture_height;
    private int texture_size;

    // Variables
    private List<Color[]> color_pages;

    // Partitioned Prim
    public DrawPrimPartition(int width, int height, int page_size) : base(width, height) {

	    texture_size = page_size;
	    texture_width = width;
	    texture_height = height;

	    color_pages = new List<Color[]>();
	    for (int h = 0; h < height; h++) {
		    for (int w = 0; w < width; w++) {
			
			    Color[] temp_color_page = new Color[page_size * page_size];
			    for (int i = 0; i < temp_color_page.Length; i++) {
				    temp_color_page[i] = Color.clear;
			    } 
			    color_pages.Add(temp_color_page);
			
		    }
	    }

    }

    // Create Texture Page
    public GameObject createTextureObj() {
        GameObject temp_texture_obj = new GameObject("Texture Page");
        temp_texture_obj.transform.position = Vector3.zero;
        for (int h = 0; h < texture_height; h++) {
		    for (int w = 0; w < texture_width; w++) {
                GameObject temp_texpage = new GameObject("Texture: (" + w + ", " + h + ")");
                Image temp_img = temp_texpage.AddComponent<Image>();

                temp_texpage.transform.parent = temp_texture_obj.transform;

            }
        }
        return temp_texture_obj;
    }

    // Draw Pixel
    public override void drawPixel(int x, int y, Color color) {
	    int temp_page_x = x / texture_size;
	    int temp_page_y = y / texture_size;
	    int temp_x = x / texture_size;
	    int temp_y = y / texture_size;
	    Color[] color_page = color_pages[temp_page_x * (temp_page_y * texture_width)];
	    color_page[temp_x + (temp_y * texture_size)] = color;
    }

    // Draw Fill
    public override void drawFill(Color color) {
        for (int i = 0; i < color_pages.Count; i++) {
            Color[] temp_page = color_pages[i];
            for (int q = 0; q < temp_page.Length; q++) {
                temp_page[q] = color;
            }
        }
    }
}