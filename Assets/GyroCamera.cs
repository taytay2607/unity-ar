using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroCamera : MonoBehaviour {
    private Gyroscope gyro;
    private bool gyroSupport;
    private Quaternion rotfix;

    [SerializeField]
    private Transform wouldObj;
    private float startY;

    [SerializeField]
    private Transform zoomObj;

	// Use this for initialization
	void Start () {
        gyroSupport = SystemInfo.supportsGyroscope;

        GameObject canParent = new GameObject("canParent");
        canParent.transform.position = transform.position;
        transform.parent = canParent.transform;

        if (gyroSupport) {
            gyro = Input.gyro;
            gyro.enabled = true;

            canParent.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
            rotfix = new Quaternion(0, 0, 1, 0);
        }
        gyro = Input.gyro;
	}
	
	// Update is called once per frame
	void Update () {

        if (gyroSupport && startY == 0) {
            resetGyroRotation();
        }

        transform.localRotation = gyro.attitude * rotfix;
	}

    public void resetGyroRotation() {
        int x = Screen.width / 2;
        int y = Screen.height / 2;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 500)) {
            Vector3 hitPoint = hit.point;
            hitPoint.y = 0;

            float z = Vector3.Distance(Vector3.zero, hitPoint);
            zoomObj.localPosition = new Vector3(0f, zoomObj.localPosition.y, Mathf.Clamp(z, 2f, 10f));
        }

        startY = transform.eulerAngles.y;
        wouldObj.rotation = Quaternion.Euler(0f, startY, 0f);
    }
}
