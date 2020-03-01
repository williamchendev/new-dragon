using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonBehaviour : MonoBehaviour
{
    // Settings
    public int size = 80;
    [SerializeField] private int min_tiles = 100;
    [SerializeField] private int max_tiles = 120;

    // Variables
    public float x_position;
    public float y_position;
    public List<Vector2> hexagons;

    // Init Hexagons
    public void Awake() {
        // Grid
        hexagons = new List<Vector2>();
        hexagons.Add(new Vector2(0f, 0f));

        // Geometric Variables
        float length = Mathf.Sqrt(3) * (size / 2f);

        // Spawn Variables
        int random_tile_amount = Random.Range(min_tiles, max_tiles);

        List<HexagonSpawner> spawners = new List<HexagonSpawner>();
        spawners.Add(new HexagonSpawner(0f, 0f, length, -1));

        float spawn_chance = 0.5f;

        // Spawn Hexagons
        while (hexagons.Count < random_tile_amount) {
            // Iterate through spawners
            for (int i = 0; i < spawners.Count; i++) {
                hexagons.Add(spawners[i].spawn_point(hexagons));
            }

            // Spawn New Spawners
            if (Random.Range(0f, 1f) < spawn_chance) {
                int new_spawners = Random.Range(1, 5);
                for (int t = 0; t < new_spawners; t++) {
                    spawners.Add(new HexagonSpawner(spawners[0].position.x, spawners[0].position.y, length, Random.Range(6, 12)));
                }
            }

            // Delete Dead Spawners
            for (int q = spawners.Count - 1; q >= 0; q--) {
                if (spawners[q].destroy_this){
                    spawners.RemoveAt(q);
                }
            }
        }
    }
}

public class HexagonSpawner {

    // Variables
    private float x_pos;
    private float y_pos;
    private float direction;
    private float direction_timer;
    private float destroy_timer;

    private float length;

    // Instantiate Spawner
    public HexagonSpawner(float x, float y, float l, float destroy_delay) {
        // Variables
        x_pos = x;
        y_pos = y;
        length = l;
        destroy_timer = destroy_delay;

        direction = 30;
        direction_timer = 0;
    }

    // Find Spawn Point
    public Vector2 spawn_point(List<Vector2> points) {
        bool found_point = false;
        while (!found_point) { 
            // Find Direction
            direction_timer--;
            if (direction_timer <= 0) {
                float old_direction = direction;
                while (old_direction == direction) {
                    direction = (Random.Range(0, 5) * 60f);
                }
                direction_timer = Random.Range(2, 8);
            }

            y_pos += Mathf.Cos(direction * Mathf.Deg2Rad) * (length * 2);
            x_pos += Mathf.Sin(direction * Mathf.Deg2Rad) * (length * 2);

            bool contains_point = false;
            for (int q = 0; q < points.Count; q++) {
                if (new Vector2(x_pos, y_pos) == points[q]) {
                    contains_point = true;
                    break;
                }
            }

            if (!contains_point) {
                found_point = true;
            }
        }
        destroy_timer--;
        return new Vector2(x_pos, y_pos);
    }

    public bool destroy_this {
        get {
            return destroy_timer == 0;
        }
    }

    public Vector2 position {
        get {
            return new Vector2(x_pos, y_pos);
        }
    }

}
