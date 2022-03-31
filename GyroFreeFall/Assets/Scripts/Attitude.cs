using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Attitude : MonoBehaviour
{
    // Get access to the gyroscope in the global scope
    private Gyroscope _gyro;

    // Flags
    //-----------------------------------------
    private bool _collectData = false;
    private volatile bool _savingData = false;
    //-----------------------------------------

    // Current save number
    private int _collection = 1;

    // A list to save the current record info (X, Y, Z)
    // A float was used instead of string to save memory space
    /* Each float is 4 byte, however, each string is 2byte * character.length
     * This may differ depending on the character encoding */
    private List<float[]> _gravity = new List<float[]>();

    private void Start()
    {
        // Write the path in which the information will be saved in
        // Debug.Log(Application.dataPath);

        // Get access to the gyroscope
        _gyro = Input.gyro;

        // Check if the gyroscope supported
        if (SystemInfo.supportsGyroscope)
        {
            // Enable the gyroscope
            _gyro.enabled = true;

            // Write that the gyro is enabled
            // Debug.LogWarning("Gyro Enabled");
        }
    }

    private void Update()
    {


        /* Check if the user started recording the gyro info 
         * and the last record is not being saved atm */
        if (_collectData && !_savingData)
            // Record the gyro gravity-vector info
            _gravity.Add(new float[] {_gyro.gravity.x,
                           _gyro.gravity.y,
                           _gyro.gravity.z } );
    }


    public void toggle()
    {
        /* Make sure the button is not clicked while 
         * The system is saving the data on a file */
        if (_savingData)
            return;

        // Reverese the flag value
        _collectData = !_collectData;

        /* The _collectData starts being false, after the first click
         * it will become true, therefor the if-statement below will not
         * be executed unless the user clicks twice on the button, to ensure
         * there is info recorded*/
        if(_collectData == false && _gravity.Count > 0)
        { // There is data to be saved
            SaveData(_gravity);
        }
    }

    public void SaveData(List<float[]> dataToBeSaved)
    {
        // Indicate that the data are being saved
        // The _saveingData is volatile, to make sure the new value will be
        // saved to the main memory directly
        _savingData = true;

        // To know the current record number 
        // Debug.Log("Started: " + dataToBeSaved.Count);
        
        // Create a stream writer to start writing to a file
        /* A post-increment is used on the _collection variable to make sure
         * the name of the saved files are different */
        /* Note: There is no slash before the file name (DataSet...csv) which means the file
         * will not be in the last directoy of the dataPath field but in the folder one level
         * before it. In other word the files will not be saved in the Assets folder but
         * in the GyroFreeFall (the parent of the Assets folder) */
        StreamWriter sw = new StreamWriter(Application.dataPath + "DataSet" + (_collection++) + ".csv", false);
        // Write the headers on the first line of the file
        sw.WriteLine("X,Y,Z");

        // Loop through all the list float arrays to save their values
        for(int i = 0; i < dataToBeSaved.Count; i++)
        {
            // Write this element x, y, z and seperate the values with commas
            sw.WriteLine(dataToBeSaved[i][0] + ", " + dataToBeSaved[i][1] + ", " + dataToBeSaved[i][2]);
        }

        // Close the file to make sure the data is written on it
        // In other word to make sure the buffer is written directly 
        // On the file in the secondary storage (internal storage, Universal Serial Bus) etc.
        sw.Close();

        // Clear the old data, to prepare the list to save new record
        dataToBeSaved.Clear();

        // Indicate the end of the record saving process to external file
        _savingData = false;
    }
}
