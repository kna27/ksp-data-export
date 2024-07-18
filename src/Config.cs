using System;
using System.IO;
using UnityEngine;

namespace KSPDataExport
{
    /// <summary>
    ///     Getting and setting values from a file
    /// </summary>
    internal abstract class Config
    {
        /// <summary>
        ///     Gets a value from a file and optionally create it if it doesn't exist
        /// </summary>
        /// <param name="filePath">Path of the file</param>
        /// <param name="valueName">The name of the value you want to search for</param>
        /// <param name="createIfDoesNotExist">Create the value in the file as false if not found</param>
        /// <returns>The value from the file</returns>
        public static bool GetValue(string filePath, string valueName, bool createIfDoesNotExist = true)
        {
            try
            {
                foreach (string line in File.ReadLines(filePath))
                {
                    // Skip line if it starts with a comment
                    if (line.StartsWith("//")) continue;
                    //Split string on equals sign
                    string[] lineSides = line.Split('=');
                    if (lineSides[0] == valueName)
                        //Return right side of split line
                        return bool.Parse(lineSides[1]);
                }

                if (!createIfDoesNotExist)
                {
                    Debug.Log("[DataExport] Value not found and will not be created: " + valueName);
                    throw new Exception("Value not found in file");
                }

                // Create variable if it does not exist and set it to false
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
            catch (Exception e)
            {
                Debug.Log("[DataExport] Unable to create variable in GetValue: " + e);
                return false;
            }
        }

        /// <summary>
        ///     Sets a value from the filePath, given the valueName and the value to set it as
        /// </summary>
        /// <param name="filePath">Path of the file</param>
        /// <param name="valueName">The name of the value you want to set</param>
        /// <param name="value">The value you want to set it to</param>
        public static void SetValue(string filePath, string valueName, bool value)
        {
            string[] arrLine = File.ReadAllLines(filePath);
            bool done = false;
            for (int i = 0; i < arrLine.Length; i++)
                // Skip line if it starts with a comment
                if (!arrLine[i].StartsWith("//"))
                {
                    // Split string on equals sign
                    string[] lineSides = arrLine[i].Split('=');
                    if (lineSides[0] == valueName)
                    {
                        // Set right side of line to the given value and write it to the file
                        arrLine[i] = valueName + "=" + value;
                        File.WriteAllLines(filePath, arrLine);
                        done = true;
                    }
                }

            // Creates the value if none was found in the file
            if (done) return;
            try
            {
                using StreamWriter file = new StreamWriter(filePath, true);
                file.WriteLine(valueName + "=" + value);
            }
            catch (Exception e)
            {
                Debug.Log("[DataExport] Unable to set value: " + e);
                throw new ApplicationException("Error: ", e);
            }
        }
    }
}
