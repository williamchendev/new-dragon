using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPrim
{
    // Components
    

    // Settings
    private int texture_width;

    // Variables
    private Color[] colors;

    // Create Texture Page
    public virtual GameObject createTextureObj() {
        return new GameObject();
    }

    // Draw Pixel
    public virtual void drawPixel(int x, int y, Color color) {
        colors[x + (y * texture_width)] = color;
    }

}

public class DrawPrimPartition : DrawPrim {

    // Components

    // Settings

    // Variables

    // Create Texture Page
    public override GameObject createTextureObj() {
        return new GameObject();
    }

    // Draw Pixel
    public override void drawPixel(int x, int y, Color color) {
        return;
    }

}