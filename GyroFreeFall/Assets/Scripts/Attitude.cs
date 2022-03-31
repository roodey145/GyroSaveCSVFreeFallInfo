using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Attitude : MonoBehaviour
{
    public float movementSpeed = 20;
    Gyroscope gyro;

    float MAX_AGNLE = 60;
    private bool _collectData = false;
    private volatile bool _savingData = false;
    private int collection = 1;
    private List<string> _gravity = new List<string>();

    private void Start()
    {
        Debug.Log(Application.dataPath);
        gyro = Input.gyro;

        if (SystemInfo.supportsGyroscope)
        {
            gyro.enabled = true;
            Debug.LogWarning("Gyro Enabled");
        }
    }

    private void Update()
    {

        /*Vector3 rotation = new Vector3(); ;
        rotation.x = gyro.gravity.y * MAX_AGNLE;
        rotation.z = -gyro.gravity.x * MAX_AGNLE;

        Debug.Log("X: " + rotation.x);
        Debug.Log("Z: " + rotation.z);

        transform.eulerAngles = rotation;*/
        if (_collectData && !_savingData)
            _gravity.Add(gyro.gravity.x + ","
                           + gyro.gravity.y + ","
                           + gyro.gravity.z);
            // Debug.Log(gyro.gravity);
    }


    public void toggle()
    {
        // Make sure the button is not clicked while 
        // The system is saving the data on a file
        if (_savingData)
            return;
        _collectData = !_collectData;

        if(_collectData == false && _gravity.Count > 0)
        { // There is data to be saved
            SaveData(_gravity);
        }
    }

    public void SaveData(List<string> dataToBeSaved)
    {
        _savingData = true;

        Debug.Log("Started: " + dataToBeSaved.Count);
        StreamWriter sw = new StreamWriter(Application.dataPath + "DataSet" + (collection++) + ".csv", false);
        // Write the headers
        sw.WriteLine("X,Y,Z");

        for(int i = 0; i < dataToBeSaved.Count; i++)
        {
            sw.WriteLine(dataToBeSaved[i]);
        }

        // Close the file to make sure the data is written on it
        sw.Close();

        // Clear the old data
        dataToBeSaved.Clear();
        _savingData = false;
    }
}
