using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawUtil
{
    // Components
    private Texture2D tex;

    // Variables
    private int screen_width;
    private int screen_height;

    private Color[] colors;

    // Class Instantiation
    public DrawUtil(int width, int height) {
        screen_width = width;
        screen_height = height;

        colors = new Color[width * height];
    }

    // Draw Pixel
    public void drawPixel(int x, int y, Color color) {
        if (Mathf.Abs(x) < screen_width / 2) {
            if (Mathf.Abs(y) < screen_height / 2) {
                colors[(x + (screen_width / 2)) + ((y + (screen_height / 2)) * screen_width)] = color;
            }
        }
    }

    // Draw Line
    public void drawLine(int x1, int y1, int x2, int y2, Color color) {
        int distance = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(x2 - x1, 2) + Mathf.Pow(y2 - y1, 2)));
        float angle = Mathf.Atan2(y2 - y1, x2 - x1);

        for (int i = 0; i <= distance; i++) {
            int temp_x = Mathf.RoundToInt(x1 + (Mathf.Cos(angle) * i));
            int temp_y = Mathf.RoundToInt(y1 + (Mathf.Sin(angle) * i));
            drawPixel(temp_x, temp_y, color);
        }
    }

    // Draw Rectangle
    public void drawRectangle(int x1, int y1, int x2, int y2, Color color) {
        drawRectangle(x1, y1, x2, y2, color, false);
    }

    public void drawRectangle(int x1, int y1, int x2, int y2, Color color, bool outline) {
        if (outline) {
            drawLine(x1, y1, x2, y1, color);
            drawLine(x1, y1, x1, y2, color);
            drawLine(x2, y2, x2, y1, color);
            drawLine(x2, y2, x1, y2, color);
        }
        else {
            int temp_min_x = Mathf.Min(x1, x2);
            int temp_max_x = Mathf.Max(x1, x2);
            int temp_min_y = Mathf.Min(y1, y2);
            int temp_max_y = Mathf.Max(y1, y2);
            for (int w = 0; w <= temp_max_x - temp_min_x; w++) {
                for (int h = 0; h <= temp_max_y - temp_min_y; h++) {
                    drawPixel(temp_min_x + w, temp_min_y + h, color);
                }
            }
        }
    }

    // Draw Circle
    public void drawCircle(int x, int y, float radius, Color color) {
        float circumfrence = radius * 2f * Mathf.PI;
        float radial_interpolation = 360f / circumfrence;
        drawCircle(x, y, radius, radial_interpolation, color);
    }

    public void drawCircle(int x, int y, float radius, float interpolation, Color color) {
        drawCircle(x, y, radius, interpolation, 90, color);
    }

    public void drawCircle(int x, int y, float radius, float interpolation, float angle, Color color) {
        int temp_x1 = Mathf.RoundToInt(x + (Mathf.Cos(Mathf.Deg2Rad * angle) * radius));
        int temp_y1 = Mathf.RoundToInt(y + (Mathf.Sin(Mathf.Deg2Rad * angle) * radius));

        float radial_interpolation = interpolation;
        for (float i = radial_interpolation; i <= 360; i += radial_interpolation) {
            float temp_angle = Mathf.Deg2Rad * (i + angle);
            int temp_x2 = Mathf.RoundToInt(x + (Mathf.Cos(temp_angle) * radius));
            int temp_y2 = Mathf.RoundToInt(y + (Mathf.Sin(temp_angle) * radius));

            drawLine(temp_x1, temp_y1, temp_x2, temp_y2, color);

            temp_x1 = temp_x2;
            temp_y1 = temp_y2;
        }
    }

    public void drawCircle(int x, int y, float radius, float interpolation, float angle, Color color, bool outline) {
        int temp_x1 = Mathf.RoundToInt(x + (Mathf.Cos(Mathf.Deg2Rad * angle) * radius));
        int temp_y1 = Mathf.RoundToInt(y + (Mathf.Sin(Mathf.Deg2Rad * angle) * radius));

        float radial_interpolation = interpolation;
        for (float i = radial_interpolation; i <= 360; i += radial_interpolation) {
            float temp_angle = Mathf.Deg2Rad * (i + angle);
            int temp_x2 = Mathf.RoundToInt(x + (Mathf.Cos(temp_angle) * radius));
            int temp_y2 = Mathf.RoundToInt(y + (Mathf.Sin(temp_angle) * radius));

            if (outline) {
                drawLine(temp_x1, temp_y1, temp_x2, temp_y2, color);
            }
            else {
                int temp_distance_a = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(temp_x2 - temp_x1, 2) + Mathf.Pow(temp_y2 - temp_y1, 2)));
                float temp_angle_a = Mathf.Atan2(temp_y2 - temp_y1, temp_x2 - temp_x1);
                for (float q = 0; q <= temp_distance_a; q++) {
                    int temp_x = Mathf.RoundToInt(temp_x1 + (Mathf.Cos(temp_angle_a) * i));
                    int temp_y = Mathf.RoundToInt(temp_y1 + (Mathf.Sin(temp_angle_a) * i));
                    drawLine(x, y, temp_x, temp_y, color);
                }
            }

            temp_x1 = temp_x2;
            temp_y1 = temp_y2;
        }
    }

    // Refresh Screen
    public void drawFillAll(Color color) {
        for (int i = 0; i < colors.Length; i++) {
            colors[i] = color;
        }
    }

    // Getter Method Texture
    public Texture2D texture {
        get {
            tex.SetPixels(colors);
            tex.Apply();

            return tex;
        }
    }
}
