using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class TurretManager : MonoBehaviour
{
    private TileManager tileManager;
    [SerializeField]
    private float waitSpawnTime, minIntervalTime, maxIntervalTime;

    private List<Turret> turrets = new List<Turret>();

    void Start()
    {
        tileManager = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileManager>();
    }

    void Update()
    {
        if (waitSpawnTime < Time.time)
        {
            waitSpawnTime = Time.time + UnityEngine.Random.Range(minIntervalTime, maxIntervalTime);
            SpawnTurret();
        }

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Stationary)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform.tag == "Turret")
                {
                    Turret turret = hit.transform.GetComponent<Turret>();
                    KeepTurret(turret.turretType);
                }
            }
        }
    }

    void KeepTurret(TurretType type)
    {
        string t = type.ToString();
        PlayerPrefs.SetString("TURRET_KEY", t);
        SceneManager.LoadScene("Catch");
    }

    void SpawnTurret()
    {
        TurretType type = (TurretType)(int)UnityEngine.Random.Range(0, Enum.GetValues(typeof(TurretType)).Length);
        float newLat = tileManager.getLat + UnityEngine.Random.Range(-0.0001f, 0.0001f);
        float newLon = tileManager.getLon + UnityEngine.Random.Range(-0.0001f, 0.0001f);

        Turret prefab = Resources.Load("MapTurret/" + type.ToString(), typeof(Turret)) as Turret;
        Turret turret = Instantiate(prefab, Vector3.zero, Quaternion.identity) as Turret;
        turret.tileManager = tileManager;
        turret.Init(newLat, newLon);

        turrets.Add(turret);
    }

    public void UpdateTurretPosition()
    {
        if (turrets.Count == 0)
            return;

        Turret[] turret = turrets.ToArray();
        for (int i = 0; i < turret.Length; i++)
        {
            turret[i].UpdatePosition();
        }
    }
}

