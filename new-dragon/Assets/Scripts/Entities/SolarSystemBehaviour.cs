using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemBehaviour : MonoBehaviour
{
    // Components
    [SerializeField] private Material planetoid_material = null;

    // Settings

    // Variables
    private List<PlanetoidBehaviour> planetoids;

    // Instantiate
    void Start()
    {
        // Variables
        planetoids = new List<PlanetoidBehaviour>();



        // Debug
        PlanetoidBehaviour center_star = createPlanetoid();
        center_star.star = true;
        center_star.planet_size = 1f;

        float ring_size = 3f;
        float orbit_spd_av = 1f;

        int ring_num = Random.Range(6, 8);
        for (int i = 0; i < ring_num; i++) {
            float planet_size = Random.Range(0.3f, 0.6f) * orbit_spd_av;
            float orbit_spd = Random.Range(0.01f, 0.01f) * orbit_spd_av;
            if (Random.Range(0f, 1f) < 0.5f) {
                orbit_spd = -orbit_spd;
            }

            PlanetoidBehaviour new_planet = createPlanetoid();
            new_planet.transform.parent = center_star.transform;
            new_planet.orbit = true;
            new_planet.orbit_spd = orbit_spd;
            new_planet.orbit_size = ring_size;
            new_planet.planet_size = planet_size;
            if (Random.Range(0f, 1f) < 0.1f) {
                new_planet.star = true;
            }

            if (planet_size > 0.2f) {
                if (Random.Range(0f, 1f) < 0.5f) {
                    PlanetoidBehaviour new_orbit_planet = createPlanetoid();
                    new_orbit_planet.transform.parent = new_planet.transform;
                    new_orbit_planet.orbit = true;
                    new_orbit_planet.orbit_spd = orbit_spd * Random.Range(1.2f, 2.5f);
                    new_orbit_planet.orbit_size = planet_size * 3;
                    new_planet.orbit_size = ring_size + (planet_size * 1.5f);
                    float new_planet_size = Random.Range(0.2f, 0.4f) * planet_size;
                    new_orbit_planet.planet_size = new_planet_size;
                    if (Random.Range(0f, 1f) < 0.4f) {
                        new_orbit_planet.star = true;
                    }
                    ring_size += (planet_size * 3);
                }
            }

            ring_size += (planet_size * 2) + Random.Range(0.8f, 1.5f);
            orbit_spd_av -= 0.12f;
        }
    }

    // Update
    void Update()
    {
        
    }

    // Instantiate Planetoid
    public PlanetoidBehaviour createPlanetoid() {
        GameObject new_planetoid_obj = new GameObject("planetoid_" + planetoids.Count);
        new_planetoid_obj.transform.parent = gameObject.transform;
        new_planetoid_obj.transform.localPosition = Vector3.zero;

        PlanetoidBehaviour new_planetoid = new_planetoid_obj.AddComponent<PlanetoidBehaviour>();
        new_planetoid.solar_system = this;
        planetoids.Add(new_planetoid);

        return new_planetoid;
    }

    // Getter Methods
    public Material planetoid_mat {
        get {
            return planetoid_material;
        }
    }

}

public class PlanetoidBehaviour : MonoBehaviour {

    // Settings
    public SolarSystemBehaviour solar_system {private get; set; }

    public bool star {private get; set; }
    public bool orbit {private get; set; }
    public float orbit_spd {private get; set; }
    public float orbit_size {private get; set; }

    public float planet_size {private get; set; }

    private float rotate_spd;
    private float rotate_spd_a;
    private float rotate_spd_b;

    private float orbit_offset;

    // Variables
    private GameObject layer_obj;
    private List<GameObject> layers;
    private LineRenderer lr;

    // Planetoid Initialization
    void Start()
    {
        // Debug
        //star = false;
        //planet_size = 1f;

        // Settings
        rotate_spd = Random.Range(3f, 18f);
        if (Random.Range(0f, 1f) < 0.5f) {
            rotate_spd *= -1;
        }
        rotate_spd_a = Random.Range(2f, 8f) + rotate_spd;
        rotate_spd_b = Random.Range(2f, 4f) + rotate_spd_a;

        orbit_offset = Random.Range(0f, Mathf.PI * 2f);

        // Variables
        layer_obj = new GameObject("planet_layers");
        layer_obj.transform.parent = gameObject.transform;
        layer_obj.transform.localPosition = Vector3.zero;
        layer_obj.transform.eulerAngles = new Vector3(Random.Range(-30f, 30f), 0f, Random.Range(-15f, 15f));
        layers = new List<GameObject>();

        // Debug 
        if (star) {
            createLayer();
            textureLayer(0);
            createLayer();
            textureLayer(1);
        }
        else {
            if (Random.Range(0f, 1f) > 0.65f) {
                createLayer();
                createLayer();
                textureLayer(0);
                textureLayer(1);
            }
            else {
                createLayer();
                createLayer();
                createLayer();
                textureLayer(0);
                textureLayer(1);
                textureLayer(2);
            }
        }

        if (orbit) {
            // LineRenderer
            GameObject line = new GameObject("Orbit_Line");
            line.transform.parent = transform;
            lr = line.AddComponent<LineRenderer>();
            lr.startColor = Color.white;
            lr.endColor = Color.white;
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;
            lr.loop = true;
            lr.material = solar_system.planetoid_mat;
            lr.useWorldSpace = false;

            int segments = Mathf.RoundToInt(30 * orbit_size);
            lr.positionCount = segments;
            for (int i = 0; i < segments; i++) {
                Vector3 ring_pos = new Vector3(Mathf.Cos((i / (segments * 1f)) * 2 * Mathf.PI), Mathf.Sin((i / (segments * 1f)) * 2 * Mathf.PI), 0f);
                lr.SetPosition(i, (ring_pos * orbit_size));
            }
        }
    }

    // Planetoid Update Behaviour
    void Update()
    {
        // Rotate Planet
        layers[0].transform.localEulerAngles = new Vector3(0f, Time.time * rotate_spd, 0f);
        if (layers.Count > 1) {
            layers[1].transform.localEulerAngles = new Vector3(0f, Time.time * Mathf.Sign(rotate_spd) * rotate_spd_a, 0f);
        }
        if (layers.Count > 2) {
            layers[2].transform.localEulerAngles = new Vector3(0f, Time.time * Mathf.Sign(rotate_spd) * rotate_spd_b, 0f);
        }

        if (orbit) {
            transform.position = transform.parent.position + (orbit_size * new Vector3(Mathf.Cos((Time.time * orbit_spd) + orbit_offset), Mathf.Sin((Time.time * orbit_spd) + orbit_offset), 0f));
            lr.transform.position = transform.parent.position;
        }
    }

    // Layer Init
    public void createLayer() {
        // Create Layer
        float layer_size = planet_size + (layers.Count * 0.05f);
        GameObject new_layer = new GameObject();
        MeshFilter layer_mf = new_layer.AddComponent<MeshFilter>();
        layer_mf.mesh = OctahedronSphereCreator.Create(3, layer_size);
        new_layer.AddComponent<MeshRenderer>();
        new_layer.name = "layer_" + layers.Count;
        new_layer.transform.parent = layer_obj.transform;
        new_layer.transform.localPosition = Vector3.zero;
        layers.Add(new_layer);
    }

    // Layer Texture Init
    public void textureLayer(int layer) {
        // Create Material
        Material new_mat = new Material(solar_system.planetoid_mat.shader);

        // Set Material Texture
        int texture_width = Mathf.RoundToInt(314 * (planet_size + (layer * 0.1f)));
        Texture2D new_layer_tex = new Texture2D(texture_width, texture_width, TextureFormat.ARGB32, false);

        Color[] tex_colors;
        if (layer == 1) {
            if (Random.Range(0f, 1f) > 0.5f) {
                tex_colors = createTextureAtmosphere(texture_width);
            }
            else {
                tex_colors = createTextureAtmosphere2(texture_width);
            }
        }
        else if (layer == 2) {
            tex_colors = createTextureClouds(texture_width);
        }
        else {
            if (star) {
                tex_colors = createTextureStar(texture_width);
            }
            else {
                tex_colors = createTextureTerrestrial(texture_width);
            }
        }

        new_layer_tex.SetPixels(tex_colors);
        new_layer_tex.Apply();
        new_layer_tex.filterMode = FilterMode.Point;
        new_mat.SetTexture("_MainTex", new_layer_tex);

        // Set Material Renderer
        MeshRenderer temp_mr = layers[layer].GetComponent<MeshRenderer>();
        temp_mr.material = new_mat;
    }

    // Texture Generation
    public Color[] createTextureTerrestrial(int width) {
        Color[] colors = new Color[width * width];

        for (int i = 0; i < colors.Length; i++) {
            colors[i] = Color.black;
        }

        /*
        Color[] simplex_colors = new Color[width * width];
        for (int h = 0; h < width; h++) {
            for (int w = 0; w < width; w++) {
                Color temp_color = Color.black;

                float simplex_scale = 2;
                float simplex_seed = Random.Range(0f, 8f);
                float simplex_random = SimplexNoise.SeamlessNoise(w / (width * 1f), h / (width * 1f), simplex_scale, simplex_scale, simplex_seed);

                if (simplex_random > 0.6f) {
                    //temp_color = Color.white;
                }

                simplex_colors[w + (h * width)] = temp_color;
            }
        }

        simplex_colors = cellularAutomata(cellularAutomata2(cellularAutomata2(cellularAutomata2(cellularAutomata(cellularAutomata(simplex_colors, Color.black), Color.black), Color.black), Color.black), Color.black), Color.black);
        */

        return colors;
    }

    public Color[] createTextureStar(int width) {
        Color[] colors = new Color[width * width];

        for (int i = 0; i < colors.Length; i++) {
            colors[i] = Color.white;
        }

        return colors;
    }

    public Color[] createTextureAtmosphere(int width) {
        Color[] colors = new Color[width * width];

        Color[] simplex_colors = new Color[width * width];
        for (int h = 0; h < width; h++) {
            for (int w = 0; w < width; w++) {
                Color temp_color = Color.clear;

                float simplex_scale = Random.Range(1, 3) * 5f;
                float simplex_seed = Random.Range(0f, 1f);
                float simplex_random = SimplexNoise.SeamlessNoise(w / (width * 1f), h / (width * 1f), simplex_scale, simplex_scale, simplex_seed);

                //temp_color = Color.Lerp(Color.black, Color.white, perlin_random);

                if (simplex_random > 0.3f) {
                    temp_color = Color.white;
                }

                simplex_colors[w + (h * width)] = temp_color;
            }
        }

        simplex_colors = cellularAutomata(cellularAutomata(cellularAutomata(simplex_colors, Color.clear), Color.clear), Color.clear);

        return simplex_colors;
    }

    public Color[] createTextureAtmosphere2(int width) {
        Color[] colors = new Color[width * width];

        Color[] simplex_colors = new Color[width * width];
        for (int h = 0; h < width; h++) {
            for (int w = 0; w < width; w++) {
                Color temp_color = Color.clear;

                float simplex_scale = 5;
                float simplex_seed = Random.Range(0f, 1f);
                float simplex_random = SimplexNoise.SeamlessNoise(w / (width * 1f), h / (width * 1f), simplex_scale, simplex_scale, simplex_seed);

                if (simplex_random > 0.4f) {
                    temp_color = Color.white;
                }

                simplex_colors[w + (h * width)] = temp_color;
            }
        }

        simplex_colors = cellularAutomata2(cellularAutomata2(cellularAutomata(cellularAutomata(simplex_colors, Color.clear), Color.clear), Color.clear), Color.clear);

        return simplex_colors;
    }

    public Color[] createTextureClouds(int width) {
        Color[] colors = new Color[width * width];

        Color[] simplex_colors = new Color[width * width];
        for (int h = 0; h < width; h++) {
            for (int w = 0; w < width; w++) {
                Color temp_color = Color.clear;

                float simplex_scale = Random.Range(1, 4) * 2f;
                float simplex_seed = Random.Range(0f, 1f);
                float simplex_random = SimplexNoise.SeamlessNoise(w / (width * 1f), h / (width * 1f), simplex_scale, simplex_scale, simplex_seed);

                if (simplex_random > 0.5f) {
                    temp_color = Color.white;
                }

                simplex_colors[w + (h * width)] = temp_color;
            }
        }

        simplex_colors = cellularAutomata2(cellularAutomata(simplex_colors, Color.clear), Color.clear);

        return simplex_colors;
    }

    // Cellular Automata
    public Color[] cellularAutomata(Color[] array, Color dead_color) {
        Color[] new_array = new Color[array.Length];
        int width = Mathf.RoundToInt(Mathf.Sqrt(array.Length));

        for (int h = 0; h < width; h++) {
            for (int w = 0; w < width; w++) {
                // Count Neighbors
                int neighbors = 0;
                if (w == 0) {
                    if (array[(width - 1) + (h * width)] == Color.white) {
                        neighbors++;
                    }
                    if (array[w + (h * width) + 1] == Color.white) {
                        neighbors++;
                    }
                }
                else if (w == width - 1) {
                    if (array[(h * width)] == Color.white) {
                        neighbors++;
                    }
                    if (array[w + (h * width) - 1] == Color.white) {
                        neighbors++;
                    }
                }
                else {
                    if (array[w + (h * width) + 1] == Color.white) {
                        neighbors++;
                    }
                    if (array[w + (h * width) - 1] == Color.white) {
                        neighbors++;
                    }
                }

                if (h == 0) {
                    if (array[w + ((width - 1) * width)] == Color.white) {
                        neighbors++;
                    }
                    if (array[w + ((h + 1) * width)] == Color.white) {
                        neighbors++;
                    }
                }
                else if (h == width - 1) {
                    if (array[w] == Color.white) {
                        neighbors++;
                    }
                    if (array[w + ((h - 1) * width)] == Color.white) {
                        neighbors++;
                    }
                }
                else {
                    if (array[w + ((h + 1) * width)] == Color.white) {
                        neighbors++;
                    }
                    if (array[w + ((h - 1) * width)] == Color.white) {
                        neighbors++;
                    }
                }

                // Consequences
                new_array[w + (h * width)] = array[w + (h * width)];
                if (neighbors > 1) {
                    new_array[w + (h * width)] = Color.white;
                }
                else if (neighbors == 0) {
                    new_array[w + (h * width)] = dead_color;
                }
            }
        }

        return new_array;
    }

    public Color[] cellularAutomata2(Color[] array, Color dead_color) {
        Color[] new_array = new Color[array.Length];
        int width = Mathf.RoundToInt(Mathf.Sqrt(array.Length));

        for (int h = 0; h < width; h++) {
            for (int w = 0; w < width; w++) {
                // Count Neighbors
                int neighbors = 0;
                if (w == 0) {
                    if (array[(width - 1) + (h * width)] == Color.white) {
                        neighbors++;
                    }
                    if (array[w + (h * width) + 1] == Color.white) {
                        neighbors++;
                    }
                }
                else if (w == width - 1) {
                    if (array[(h * width)] == Color.white) {
                        neighbors++;
                    }
                    if (array[w + (h * width) - 1] == Color.white) {
                        neighbors++;
                    }
                }
                else {
                    if (array[w + (h * width) + 1] == Color.white) {
                        neighbors++;
                    }
                    if (array[w + (h * width) - 1] == Color.white) {
                        neighbors++;
                    }
                }

                if (h == 0) {
                    if (array[w + ((width - 1) * width)] == Color.white) {
                        neighbors++;
                    }
                    if (array[w + ((h + 1) * width)] == Color.white) {
                        neighbors++;
                    }
                }
                else if (h == width - 1) {
                    if (array[w] == Color.white) {
                        neighbors++;
                    }
                    if (array[w + ((h - 1) * width)] == Color.white) {
                        neighbors++;
                    }
                }
                else {
                    if (array[w + ((h + 1) * width)] == Color.white) {
                        neighbors++;
                    }
                    if (array[w + ((h - 1) * width)] == Color.white) {
                        neighbors++;
                    }
                }

                // Consequences
                new_array[w + (h * width)] = array[w + (h * width)];
                if (neighbors > 0) {
                    new_array[w + (h * width)] = Color.white;
                }
                else if (neighbors == 0) {
                    new_array[w + (h * width)] = dead_color;
                }
            }
        }

        return new_array;
    }
}
