using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Asteroids : MonoBehaviour
{

    // Singleton
    public static Asteroids instance {get; private set; }

    // Components
    private RawImage img;
    private DrawPrim prim;
    private DrawUtil util;

    // Settings
    [HeaderAttribute("Screen")]
    [SerializeField] private int screen_width = 480;
    [SerializeField] private int screen_height = 270;

    [Range(1, 4)]
    [SerializeField] private int screen_scale = 4;

    [HeaderAttribute("Ship")]
    [SerializeField] private float screen_shipoffset = 800;

    // Variables
    public Vector2 camera_position;
    private Texture2D screen_tex;

    private ShipBehaviour player_ship;

    // Singleton Awake Event
    void Awake()
    {
        // Singleton
        if (GameObject.FindGameObjectWithTag("GameController") != null) {
            Destroy(gameObject);
            return;
        }

        gameObject.tag = "GameController";
        instance = this;

        // Game Settings
        if (Application.platform != RuntimePlatform.WindowsEditor) {
            Screen.fullScreen = true;
            screen_width = Screen.width;
            screen_height = Screen.height;
            screen_scale = 1;
        }

        Application.targetFrameRate = 140;
        QualitySettings.vSyncCount = 1;
    }

    // Initialization Event
    void Start()
    {
        // Components
        prim = new DrawPrim(screen_width, screen_height);
        util = new DrawUtil(prim);
        util.drawFill(Color.clear);
        screen_tex = prim.texture;
        GameObject img_obj = createImage(screen_tex);
        img_obj.name = "Screen";
        img = img_obj.GetComponent<RawImage>();
        img.GetComponent<RectTransform>().localScale = new Vector3(screen_scale, screen_scale, 1f);

        // Variables
        camera_position = Vector2.zero;

        player_ship = gameObject.AddComponent<ShipBehaviour>();
    }

    // Update Event
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            Application.LoadLevel(Application.loadedLevel);
        }

        // Calculate Camera Position
        //Vector2 camera_position = new Vector2(camera_x, camera_y);
        camera_position = Vector2.Lerp(camera_position, new Vector2(player_ship.position.x + (Mathf.Cos(player_ship.rotation * Mathf.Deg2Rad) * screen_shipoffset), player_ship.position.y + (Mathf.Sin(player_ship.rotation * Mathf.Deg2Rad) * screen_shipoffset)), 3.5f * Time.deltaTime);
        //camera_x = camera_position.x;
        //camera_y = camera_position.y;

        // Draw Screen
        util.drawFill(Color.clear);

        // Draw Ship

        // Update Screen
        screen_tex.SetPixels(prim.colors);
        screen_tex.Apply();
        img.texture = screen_tex;
    }

    // Create Image Object
    public GameObject createImage(Texture2D texture) {
        GameObject img_obj = new GameObject("Generated_Image_Object");
        img_obj.transform.parent = transform;
        RectTransform new_rt = img_obj.AddComponent<RectTransform>();
        new_rt.sizeDelta = new Vector2(texture.width, texture.height);
        new_rt.transform.localPosition = Vector3.zero;
        RawImage new_img = img_obj.AddComponent<RawImage>();
        new_img.texture = texture;
        return img_obj;
    }

    // Draw Ship
    private void drawShip() {
        // Ship Settings
        int ship_size = 3;

        // Ship Variables
        int temp_x = 0;
        int temp_y = 0;
        float temp_angle = 0;

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

        // Draw Ship
        for (int i = 0; i < ship_lines.Length; i++) {
            float distance_1 = (Mathf.Sqrt(Mathf.Pow(ship_lines[i].x, 2) + Mathf.Pow(ship_lines[i].y, 2))) * ship_size;
            float angle_1 = Mathf.Atan2(ship_lines[i].y, ship_lines[i].x);
            float distance_2 = (Mathf.Sqrt(Mathf.Pow(ship_lines[i].z, 2) + Mathf.Pow(ship_lines[i].w, 2))) * ship_size;
            float angle_2 = Mathf.Atan2(ship_lines[i].w, ship_lines[i].z);

            util.drawLine(temp_x + Mathf.RoundToInt(Mathf.Cos(angle_1 + temp_angle) * distance_1), temp_y + Mathf.RoundToInt(Mathf.Sin(angle_1 + temp_angle) * distance_1), temp_x + Mathf.RoundToInt(Mathf.Cos(angle_2 + temp_angle) * distance_2), temp_y + Mathf.RoundToInt(Mathf.Sin(angle_2 + temp_angle) * distance_2), Color.white);
        }

        for (int i = 0; i < ship_lines.Length; i++) {
            float distance_1 = (Mathf.Sqrt(Mathf.Pow(-ship_lines[i].x, 2) + Mathf.Pow(ship_lines[i].y, 2))) * ship_size;
            float angle_1 = Mathf.Atan2(ship_lines[i].y, -ship_lines[i].x);
            float distance_2 = (Mathf.Sqrt(Mathf.Pow(-ship_lines[i].z, 2) + Mathf.Pow(ship_lines[i].w, 2))) * ship_size;
            float angle_2 = Mathf.Atan2(ship_lines[i].w, -ship_lines[i].z);

            util.drawLine(temp_x + Mathf.RoundToInt(Mathf.Cos(angle_1 + temp_angle) * distance_1), temp_y + Mathf.RoundToInt(Mathf.Sin(angle_1 + temp_angle) * distance_1), temp_x + Mathf.RoundToInt(Mathf.Cos(angle_2 + temp_angle) * distance_2), temp_y + Mathf.RoundToInt(Mathf.Sin(angle_2 + temp_angle) * distance_2), Color.white);
        }

    }
    
}
