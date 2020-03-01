using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour
{

    // Settings
    [Header("Movement")]
    [SerializeField] private float spd = 12;
    [SerializeField] private float wep_spd = 30;
    [SerializeField] private float clamp_spd = 16;
    [SerializeField] private float friction = 0.15f;

    [SerializeField] private float rotation_lerp_spd = 5;

    [SerializeField] private float wep_gain_lerp = 0.4f;
    [SerializeField] private float wep_drop_lerp = 0.25f;
    [SerializeField] private float wep_burnout_drop_lerp = 0.5f;

    [SerializeField] private float trail_delay = 0.02f;

    [Header("GUI")]
    [SerializeField] private int gui_wep_width = 20;
    [SerializeField] private int gui_wep_height = 100;
    [SerializeField] private int gui_wep_xoffset = 48;
    [SerializeField] private int gui_wep_yoffset = 48;

    // Variables
    private float x_position;
    private float y_position;

    private float z_rotation;

    private Vector2 velocity;

    private float war_emergency_power;
    private bool wep_active;
    private bool wep_burnout;

    public Vector3[] trail;
    private float trail_timer;

    // Initialization Event
    void Start()
    {
        // Variables
        velocity = Vector2.zero;

        war_emergency_power = 0;
        wep_active = false;
        wep_burnout = false;
    }

    // Update Event
    void Update()
    {
        // Movement Variables
        float ship_spd = spd;
        Vector2 direction_facing = Vector2.zero;
        
        // War Emergency Power
        wep_active = false;
        if (Input.GetKey(KeyCode.LeftShift) && !wep_burnout) {
            wep_active = true;
            ship_spd = wep_spd;
            war_emergency_power = Mathf.Lerp(war_emergency_power, 1.5f, Time.deltaTime * wep_gain_lerp);
            if (war_emergency_power > 1f) {
                wep_burnout = true;
            }

            if (trail == null) {
                trail = new Vector3[10];
                for (int i = 0; i < trail.Length; i++) {
                    trail[i] = new Vector3(x_position, y_position, z_rotation);
                }
                trail_timer = 0;
            }

            int burnout_offset = Mathf.FloorToInt(5f * war_emergency_power);
            x_position += Random.Range(-burnout_offset, burnout_offset);
            y_position += Random.Range(-burnout_offset, burnout_offset);
        }
        else {
            trail = null;
            if (wep_burnout) {
                war_emergency_power = Mathf.Lerp(war_emergency_power, -0.5f, Time.deltaTime * wep_burnout_drop_lerp);

                if (war_emergency_power < 0f) {
                    wep_burnout = false;
                }
            }
            else {
                war_emergency_power = Mathf.Lerp(war_emergency_power, -0.5f, Time.deltaTime * wep_drop_lerp);
            }
        }
        war_emergency_power = Mathf.Clamp01(war_emergency_power);

        // Movement
        if (!wep_burnout) {
            // Vertical Acceleration
            if (Input.GetKey(KeyCode.W)) {
                if (!wep_active) {
                    velocity.y += Mathf.Clamp(ship_spd, 0f, Mathf.Clamp(clamp_spd - velocity.y, 0f, Mathf.Infinity)) * Time.deltaTime;
                }
                else {
                    velocity.y += ship_spd * Time.deltaTime;
                }
                direction_facing.y = 1;
            }
            else if (Input.GetKey(KeyCode.S)) {
                if (!wep_active) {
                    velocity.y += Mathf.Clamp(-ship_spd, Mathf.Clamp(-clamp_spd - velocity.y, -Mathf.Infinity, 0f), 0f) * Time.deltaTime;
                }
                else {
                    velocity.y -= ship_spd * Time.deltaTime;
                }
                direction_facing.y = -1;
            }
            else {
                velocity.y = velocity.y * Mathf.Pow(friction, Time.deltaTime);
            }

            // Horizontal Acceleration
            if (Input.GetKey(KeyCode.A)) {
                if (!wep_active) {
                    velocity.x += Mathf.Clamp(-ship_spd, Mathf.Clamp(-clamp_spd - velocity.x, -Mathf.Infinity, 0f), 0f) * Time.deltaTime;
                }
                else {
                    velocity.x -= ship_spd * Time.deltaTime;
                }
                direction_facing.x = -1;
            }
            else if (Input.GetKey(KeyCode.D)) {
                if (!wep_active) {
                    velocity.x += Mathf.Clamp(ship_spd, 0f, Mathf.Clamp(clamp_spd - velocity.x, 0f, Mathf.Infinity)) * Time.deltaTime;
                }
                else {
                    velocity.x += ship_spd * Time.deltaTime;
                }
                direction_facing.x = 1;
            }
            else {
                velocity.x = velocity.x * Mathf.Pow(friction, Time.deltaTime);
            }
        }
        else {
            // Engine Burnout
            velocity.x = velocity.x * Mathf.Pow(friction, Time.deltaTime);
            velocity.y = velocity.y * Mathf.Pow(friction, Time.deltaTime);
        }

        // Rotation
        if (direction_facing != Vector2.zero) {
            z_rotation = Mathf.LerpAngle(z_rotation, Mathf.Rad2Deg * (Mathf.Atan2(direction_facing.y, direction_facing.x)), Time.deltaTime * rotation_lerp_spd);
        }

        // Collision Checking
        x_position += velocity.x;
        y_position += velocity.y;

        // Trail Render
        if (trail != null) {
            trail_timer -= Time.deltaTime;
                if (trail_timer <= 0) {
                for (int t = 1; t < trail.Length; t++) {
                    trail[t - 1] = trail[t];
                }
                trail_timer = trail_delay;
            }
            trail[0] = Vector3.Lerp(trail[0], trail[1], Time.deltaTime);
            trail[trail.Length - 1] = new Vector3(x_position, y_position, z_rotation);
        }
    }

    // Getter Methods
    public Vector2Int position {
        get {
            return new Vector2Int(Mathf.RoundToInt(x_position), Mathf.RoundToInt(y_position));
        }
    }

    public float rotation {
        get {
            return z_rotation;
        }
    }

    public float wep {
        get {
            return war_emergency_power;
        }
    }

    public Vector2Int gui_wep_dimensions {
        get {
            return new Vector2Int(gui_wep_width, gui_wep_height);
        }
    }

    public Vector2Int gui_wep_offset {
        get {
            return new Vector2Int(gui_wep_xoffset, gui_wep_yoffset);
        }
    }
}
