using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Asteroids : MonoBehaviour
{
    // Components
    public RawImage raw;
    private RectTransform rect;

    // Settings
    [SerializeField] private bool update_scale;
    [SerializeField] private int screen_width = 480;
    [SerializeField] private int screen_height = 270;

    [Range(1, 4)]
    [SerializeField] private int screen_scale = 4;

    [SerializeField] private float screen_shipoffset = 800;

    // Variables
    private Color[] colors;
    private Texture2D texture;

    private Color[] background;

    private Vector2 camera_position;

    // Entities
    private ShipBehaviour player_ship;

    // Debug
    public Text fps_counter;
    private HexagonBehaviour dungeon;

    // Screen Initialization
    void Awake()
    {
        if (Application.platform != RuntimePlatform.WindowsEditor) {
            Screen.fullScreen = true;
            screen_width = Screen.width;
            screen_height = Screen.height;
            screen_scale = 1;
        }
    }

    // Initialization Event
    void Start()
    {
        // Components
        raw = gameObject.GetComponent<RawImage>();
        rect = gameObject.GetComponent<RectTransform>();

        // Settings
        update_scale = true;

        // Variables
        background = new Color[screen_width * screen_height];
        colors = new Color[screen_width * screen_height];
        for (int i = 0; i < colors.Length; i++) {
            bool star = Random.Range(0f, 1f) < 0.001f;
            background[i] = Color.black;
            if (star) {
                background[i] = Color.white;
            }
        }
        background.CopyTo(colors, 0);

        camera_position = Vector2.zero;

        // Entities
        player_ship = createShip();

        // Texture
        texture = new Texture2D(screen_width, screen_height, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        updateScreen();

        // Debug
        fps_counter.GetComponent<RectTransform>().localPosition = new Vector3((-screen_width / 2) + 54, (-screen_height / 2) + 10, 0);
        dungeon = createHexagonDungeon();
    }

    // Update Event
    void Update()
    {
        // Calculate Camera Position
        camera_position = Vector2.Lerp(camera_position, new Vector2(player_ship.position.x + (Mathf.Cos(player_ship.rotation * Mathf.Deg2Rad) * screen_shipoffset), player_ship.position.y + (Mathf.Sin(player_ship.rotation * Mathf.Deg2Rad) * screen_shipoffset)), 3.5f * Time.deltaTime);

        // Refresh Screen
        background.CopyTo(colors, 0);


        //drawPixel(Random.Range(0, screen_width), Random.Range(0, screen_height), Color.white);
        //drawCircle(100, 100, 50, 120, Color.white);
        //drawCircle(0, 0, 50, 60, 0, Color.white, false);

        // Draw Dungeon
        drawHexagonDungeon(dungeon);

        // Draw Player
        drawShip(player_ship);
        
        // Update Screen
        updateScreen();

        // FPS Counter
        int fps = (int)(1.0f / Time.deltaTime);
        fps_counter.text = "FPS: " + fps.ToString();

        // Quit Game
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    // Initialize Ship
    private ShipBehaviour createShip() {
        GameObject ship_obj = new GameObject("Ship");
        ShipBehaviour new_ship = ship_obj.AddComponent<ShipBehaviour>();
        return new_ship;
    }

    // Initialize Hexagon Dungeon
    private HexagonBehaviour createHexagonDungeon() {
        GameObject hex_obj = new GameObject("HexagonDungeon");
        HexagonBehaviour new_dungeon = hex_obj.AddComponent<HexagonBehaviour>();
        return new_dungeon;
    }

    // Draw Ship
    private void drawShip(ShipBehaviour ship) {
        // Ship Settings
        int ship_size = 3;

        // Ship Variables
        int temp_x = ship.position.x - Mathf.RoundToInt(camera_position.x);
        int temp_y = ship.position.y - Mathf.RoundToInt(camera_position.y);
        float temp_angle = (ship.rotation - 90) * Mathf.Deg2Rad;

        // Ship Shape Array
        Vector4[] ship_lines = new Vector4[15];

        // Ship Side
        ship_lines[0] = new Vector4(-3, 7, -3, -5);
        ship_lines[1] = new Vector4(-3, -5, -4, -5);
        ship_lines[2] = new Vector4(-4, -5, -5, -6);
        ship_lines[3] = new Vector4(-5, -6, -7, -4);
        ship_lines[4] = new Vector4(-7, -4, -5, -1);
        ship_lines[5] = new Vector4(-5, -1, -5, 5);
        ship_lines[6] = new Vector4(-5, 5, -3, 7);

        // Ship Bracket
        ship_lines[7] = new Vector4(-3, 3, -1, 5);
        ship_lines[8] = new Vector4(-1, 5, -1, 3);
        ship_lines[9] = new Vector4(-1, 3, -3, 1);
        ship_lines[10] = new Vector4(-3, 1, -3, 3);

        // Ship Core
        ship_lines[11] = new Vector4(0, 2, -3, -1);
        ship_lines[12] = new Vector4(-3, -4, -2, -4);
        ship_lines[13] = new Vector4(-2, -4, -2, -6);
        ship_lines[14] = new Vector4(-2, -6, 0, -4);

        // Ship Window
        //ship_lines[15] = new Vector4(0, -1, -2, -2);
        //ship_lines[16] = new Vector4(-2, -2, 0, -2);

        // Draw Ship
        for (int i = 0; i < ship_lines.Length; i++) {
            float distance_1 = (Mathf.Sqrt(Mathf.Pow(ship_lines[i].x, 2) + Mathf.Pow(ship_lines[i].y, 2))) * ship_size;
            float angle_1 = Mathf.Atan2(ship_lines[i].y, ship_lines[i].x);
            float distance_2 = (Mathf.Sqrt(Mathf.Pow(ship_lines[i].z, 2) + Mathf.Pow(ship_lines[i].w, 2))) * ship_size;
            float angle_2 = Mathf.Atan2(ship_lines[i].w, ship_lines[i].z);

            drawLine(temp_x + Mathf.RoundToInt(Mathf.Cos(angle_1 + temp_angle) * distance_1), temp_y + Mathf.RoundToInt(Mathf.Sin(angle_1 + temp_angle) * distance_1), temp_x + Mathf.RoundToInt(Mathf.Cos(angle_2 + temp_angle) * distance_2), temp_y + Mathf.RoundToInt(Mathf.Sin(angle_2 + temp_angle) * distance_2), Color.white);
        }

        for (int i = 0; i < ship_lines.Length; i++) {
            float distance_1 = (Mathf.Sqrt(Mathf.Pow(-ship_lines[i].x, 2) + Mathf.Pow(ship_lines[i].y, 2))) * ship_size;
            float angle_1 = Mathf.Atan2(ship_lines[i].y, -ship_lines[i].x);
            float distance_2 = (Mathf.Sqrt(Mathf.Pow(-ship_lines[i].z, 2) + Mathf.Pow(ship_lines[i].w, 2))) * ship_size;
            float angle_2 = Mathf.Atan2(ship_lines[i].w, -ship_lines[i].z);

            drawLine(temp_x + Mathf.RoundToInt(Mathf.Cos(angle_1 + temp_angle) * distance_1), temp_y + Mathf.RoundToInt(Mathf.Sin(angle_1 + temp_angle) * distance_1), temp_x + Mathf.RoundToInt(Mathf.Cos(angle_2 + temp_angle) * distance_2), temp_y + Mathf.RoundToInt(Mathf.Sin(angle_2 + temp_angle) * distance_2), Color.white);
        }

        // Draw Ship Trail
        if (ship.trail != null) {
            Vector2 ship_trail_point = Vector2.zero;
            float ship_trail_direction = 0;
            float ship_trail_angle = 0;

            for (int i = -3; i <= 3; i += 3) {
                ship_trail_point = new Vector2(-3, -7);
                ship_trail_direction = (Mathf.Sqrt(Mathf.Pow((ship_trail_point.x * ship_size) - i, 2) + Mathf.Pow((ship_trail_point.y * ship_size) + i, 2)));
                ship_trail_angle = Mathf.Atan2(ship_trail_point.y, ship_trail_point.x);

                for (int t = 1; t < ship.trail.Length; t++) {
                    float angle_1 = (ship.trail[t - 1].z - 90) * Mathf.Deg2Rad;
                    float angle_2 = (ship.trail[t].z - 90) * Mathf.Deg2Rad;

                    float point_1x = ship.trail[t - 1].x + (Mathf.Cos(angle_1 + ship_trail_angle) * ship_trail_direction);
                    float point_1y = ship.trail[t - 1].y + (Mathf.Sin(angle_1 + ship_trail_angle) * ship_trail_direction);
                
                    float point_2x = ship.trail[t].x + (Mathf.Cos(angle_2 + ship_trail_angle) * ship_trail_direction);
                    float point_2y = ship.trail[t].y + (Mathf.Sin(angle_2 + ship_trail_angle) * ship_trail_direction);
                
                    drawLine(Mathf.RoundToInt(point_1x) - Mathf.RoundToInt(camera_position.x), Mathf.RoundToInt(point_1y) - Mathf.RoundToInt(camera_position.y), Mathf.RoundToInt(point_2x) - Mathf.RoundToInt(camera_position.x), Mathf.RoundToInt(point_2y) - Mathf.RoundToInt(camera_position.y), Color.white);
                }
            }

            drawCircle(Mathf.RoundToInt(temp_x + (Mathf.Cos(temp_angle + ship_trail_angle) * ship_trail_direction)), Mathf.RoundToInt(temp_y + (Mathf.Sin(temp_angle + ship_trail_angle) * ship_trail_direction)), 3, Color.white);

            for (int i = -3; i <= 3; i += 3) {
                ship_trail_point = new Vector2(3, -7);
                ship_trail_direction = (Mathf.Sqrt(Mathf.Pow((ship_trail_point.x * ship_size) + i, 2) + Mathf.Pow((ship_trail_point.y * ship_size) + i, 2)));
                ship_trail_angle = Mathf.Atan2(ship_trail_point.y, ship_trail_point.x);

                for (int t = 1; t < ship.trail.Length; t++) {
                    float angle_1 = (ship.trail[t - 1].z - 90) * Mathf.Deg2Rad;
                    float angle_2 = (ship.trail[t].z - 90) * Mathf.Deg2Rad;

                    float point_1x = ship.trail[t - 1].x + (Mathf.Cos(angle_1 + ship_trail_angle) * ship_trail_direction);
                    float point_1y = ship.trail[t - 1].y + (Mathf.Sin(angle_1 + ship_trail_angle) * ship_trail_direction);
                
                    float point_2x = ship.trail[t].x + (Mathf.Cos(angle_2 + ship_trail_angle) * ship_trail_direction);
                    float point_2y = ship.trail[t].y + (Mathf.Sin(angle_2 + ship_trail_angle) * ship_trail_direction);
                
                    drawLine(Mathf.RoundToInt(point_1x) - Mathf.RoundToInt(camera_position.x), Mathf.RoundToInt(point_1y) - Mathf.RoundToInt(camera_position.y), Mathf.RoundToInt(point_2x) - Mathf.RoundToInt(camera_position.x), Mathf.RoundToInt(point_2y) - Mathf.RoundToInt(camera_position.y), Color.white);
                }
            }

            drawCircle(Mathf.RoundToInt(temp_x + (Mathf.Cos(temp_angle + ship_trail_angle) * ship_trail_direction)), Mathf.RoundToInt(temp_y + (Mathf.Sin(temp_angle + ship_trail_angle) * ship_trail_direction)), 3, Color.white);
        }

        // Draw Ship GUI
        int wep_gui_width = ship.gui_wep_dimensions.x;
        int wep_gui_height = ship.gui_wep_dimensions.y;
        int wep_gui_xoffset = ship.gui_wep_offset.x;
        int wep_gui_yoffset = ship.gui_wep_offset.y;
        drawRectangle((-screen_width / 2) + wep_gui_xoffset, (-screen_height / 2) + wep_gui_yoffset, (-screen_width / 2) + wep_gui_xoffset + wep_gui_width, (-screen_height / 2) + wep_gui_yoffset + wep_gui_height, Color.white, true);
        drawRectangle((-screen_width / 2) + wep_gui_xoffset + 2, (-screen_height / 2) + wep_gui_yoffset + 2, (-screen_width / 2) + wep_gui_xoffset + wep_gui_width - 2, (-screen_height / 2) + wep_gui_yoffset + Mathf.RoundToInt(ship.wep * (wep_gui_height - 4)) + 2, Color.white);
    }

    // Draw Hexagon Dungeon
    private void drawHexagonDungeon(HexagonBehaviour hex_behave) {
        int temp_x = Mathf.RoundToInt(hex_behave.x_position - camera_position.x);
        int temp_y = Mathf.RoundToInt(hex_behave.y_position - camera_position.y);
        for (int i = 0; i < hex_behave.hexagons.Count; i++) {
            Vector2 temp_pos = hex_behave.hexagons[i];
            if (Vector2.Distance(camera_position, temp_pos) < 300) {
                drawCircle(Mathf.RoundToInt(temp_pos.x) + temp_x, Mathf.RoundToInt(temp_pos.y) + temp_y, hex_behave.size, 60, 0, Color.white);
                drawCircle(Mathf.RoundToInt(temp_pos.x) + temp_x, Mathf.RoundToInt(temp_pos.y) + temp_y, hex_behave.size - 20, 60, 0, Color.white);
            }
        }
    }

    // Draw Pixel
    private void drawPixel(int x, int y, Color color) {
        //if (Mathf.Abs(x) < screen_width / 2) {
            //if (Mathf.Abs(y) < screen_height / 2) {
                colors[(x + (screen_width / 2)) + ((y + (screen_height / 2)) * screen_width)] = color;
            //}
        //}
    }

    // Draw Line
    private void drawLine(int x1, int y1, int x2, int y2, Color color) {
        int distance = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(x2 - x1, 2) + Mathf.Pow(y2 - y1, 2)));
        float angle = Mathf.Atan2(y2 - y1, x2 - x1);

        for (int i = 0; i <= distance; i++) {
            int temp_x = Mathf.RoundToInt(x1 + (Mathf.Cos(angle) * i));
            int temp_y = Mathf.RoundToInt(y1 + (Mathf.Sin(angle) * i));
            drawPixel(temp_x, temp_y, color);
        }
    }

    // Draw Rectangle
    private void drawRectangle(int x1, int y1, int x2, int y2, Color color) {
        drawRectangle(x1, y1, x2, y2, color, false);
    }

    private void drawRectangle(int x1, int y1, int x2, int y2, Color color, bool outline) {
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
    private void drawCircle(int x, int y, float radius, Color color) {
        float circumfrence = radius * 2f * Mathf.PI;
        float radial_interpolation = 360f / circumfrence;
        drawCircle(x, y, radius, radial_interpolation, color);
    }

    private void drawCircle(int x, int y, float radius, float interpolation, Color color) {
        drawCircle(x, y, radius, interpolation, 90, color);
    }

    private void drawCircle(int x, int y, float radius, float interpolation, float angle, Color color) {
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
                int temp_distance_a = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(x - temp_x1, 2) + Mathf.Pow(y - temp_y1, 2)));
                float temp_angle_a = Mathf.Atan2(y - temp_y1, x - temp_x1);

                int temp_distance_b = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(x - temp_x2, 2) + Mathf.Pow(y - temp_y2, 2)));
                float temp_angle_b = Mathf.Atan2(y - temp_y2, x - temp_x2);

                for (float r = 0; r <= radius; r++) {
                    int temp_x_a = Mathf.RoundToInt(temp_x1 + (Mathf.Cos(temp_angle_a) * r));
                    int temp_y_a = Mathf.RoundToInt(temp_y1 + (Mathf.Sin(temp_angle_a) * r));
                    int temp_x_b = Mathf.RoundToInt(temp_x2 + (Mathf.Cos(temp_angle_b) * r));
                    int temp_y_b = Mathf.RoundToInt(temp_y2 + (Mathf.Sin(temp_angle_b) * r));
                    drawLine(temp_x_a, temp_y_a, temp_x_b, temp_y_b, color);
                }
            }

            temp_x1 = temp_x2;
            temp_y1 = temp_y2;
        }
    }

    // Update Colors
    private void updateScreen() {
        if (update_scale) {
            rect.sizeDelta = new Vector2(screen_width, screen_height);
            rect.localScale = new Vector2(screen_scale, screen_scale);
        }

        texture.SetPixels(colors);
        texture.Apply();

        raw.texture = texture;
    }
}