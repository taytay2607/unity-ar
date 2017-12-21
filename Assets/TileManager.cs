using UnityEngine;
using System;
using System.Collections;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    private Settings _settings;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Texture2D texture;
    private GameObject tile;

    [SerializeField]
    private GameObject service;

    private float oldLat, oldLon;
    private float lat, lon;

    IEnumerator Start()
    {
        while (!Input.location.isEnabledByUser)
        {
            print("Activate gps");
            yield return new WaitForSeconds(1f);
        }

        Input.location.Start(10f, 5f);

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        lat = Input.location.lastData.latitude;
        lon = Input.location.lastData.longitude;

        StartCoroutine(loadTiles(_settings.zoom));

        while (Input.location.isEnabledByUser)
        {

            yield return new WaitForSeconds(1f);
        }

        yield return StartCoroutine(Start());
    }

    public IEnumerator loadTiles(int zoom)
    {
        int size = _settings.size;
        string key = _settings.key;
        string style = _settings.style;

        lat = Input.location.lastData.latitude;
        lon = Input.location.lastData.longitude;

        // string url = String.Format("https://api.mapbox.com/v4/mapbox.{5}/{0},{1},{2}/{3}x{3}@2x.png?access_token={4}", lon, lat, zoom, size, key, style);
        string url = String.Format("https://api.mapbox.com/styles/v1/mapbox/light-v9/static/{0},{1},{2},0/{3}x{3}@2x?access_token=pk.eyJ1IjoidGF5dGF5MjYwNyIsImEiOiJjamJnODBvZ3QyemZrMnlsbHB1aDFyemppIn0.prfpdLy7t5PDM3WfFsULHA", lon, lat, zoom,size);
        WWW www = new WWW(url);
        yield return www;

        texture = www.texture;

        if (tile == null)
        {
            tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
            tile.name = "Tile " + lat + "" + lon;
            tile.transform.localScale = Vector3.one * _settings.scale;
            tile.GetComponent<Renderer>().material = _settings.material;
            tile.transform.parent = transform;
        }

        tile.GetComponent<Renderer>().material.mainTexture = texture;

        yield return new WaitForSeconds(1f);

        oldLat = lat;
        oldLon = lon;

        yield return StartCoroutine(loadTiles(_settings.zoom));
    }

    void Update()
    {
        service.SetActive(!Input.location.isEnabledByUser);
    }

    [Serializable]
    public class Settings
    {
        [SerializeField]
        public Vector2 centerPosition;
        [SerializeField]
        public Material material;
        [SerializeField]
        public int zoom = 18;
        [SerializeField]
        public int size = 640;
        [SerializeField]
        public float scale = 1f;
        [SerializeField]
        public int range = 1;
        [SerializeField]
        public string key = "KEY_HERE";
        [SerializeField]
        public string style = "emerald";
    }
}