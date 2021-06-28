// Getting and setting values from a file

using System;
using System.IO;
using UnityEngine;

namespace KSPDataExport
{
    class LoadVals
    {
        //Gets a value from the filePath, valueName, and optional create the value if it does not exist
        public static bool GetValue(string filePath, string valueName, bool createIfDoesNotExist = true)
        {
            try
            {
                foreach (string line in File.ReadLines(filePath))
                {
                    //Skip line if it starts with a comment
                    if (!line.StartsWith("//"))
                    {
                        //Split string on equals sign
                        string[] lineSides = line.Split('=');
                        if (lineSides[0] == valueName)
                        {
                            //Return right side of split line
                            return bool.Parse(lineSides[1]);
                        }
                    }
                }
                if (!createIfDoesNotExist)
                {
                    Debug.Log("[DataExport] Value not found and will not be created: " + valueName);
                    throw new Exception("Value not found in file");
                }
                else
                {
                    //Create variable if it does not exist and set it to false
                    try
                    {
                        using StreamWriter file = new StreamWriter(filePath, true);
                        file.WriteLine(valueName + "=False");
                        return false;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("[DataExport] Unable to read file in GetValue: " + e);
                        throw new ApplicationException("[DataExport] Couldn't create variable in GetValue: ", e);
                    }
                }

            }
            catch (Exception e)
            {
                Debug.Log("[DataExport] Unable to create variable in GetValue: " + e);
                return false;
            }
        }

        //Sets a value from the filePath, given the valueName and the value to set it as
        public static void SetValue(string filePath, string valueName, bool value)
        {
            string[] arrLine = File.ReadAllLines(filePath);
            bool done = false;
            for (int i = 0; i < arrLine.Length; i++)
            {
                //Skip line if it starts with a comment
                if (!arrLine[i].StartsWith("//"))
                {
                    //Split string on equals sign
                    string[] lineSides = arrLine[i].Split('=');
                    if (lineSides[0] == valueName)
                    {
                        //Set right side of line to the given value and write it to the file
                        arrLine[i] = valueName + "=" + value.ToString();
                        File.WriteAllLines(filePath, arrLine);
                        done = true;
                    }
                }
            }
            //Creates the value if none was found in the file
            if (done) return;
            try
            {
                using StreamWriter file = new StreamWriter(filePath, true);
                file.WriteLine(valueName + "=" + value.ToString());
            }
            catch (Exception e)
            {
                Debug.Log("[DataExport] Unable to set value: " + e);
                throw new ApplicationException("Error: ", e);
            }
        }
    }
}
