//Gettinga and setting values from a file

using System;
using System.IO;

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
                            return Boolean.Parse(lineSides[1]);
                        }
                    }
                }
                if (!createIfDoesNotExist)
                {
                    throw new Exception("Value not found in file");
                }
                else
                {
                    //Create variable if it does not exist and set it to false
                    try
                    {
                        using (StreamWriter file = new StreamWriter(filePath, true))
                        {
                            file.WriteLine(valueName + "=False");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Error: ", ex);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return false;
            }
        }

        //Sets a value from the filePath, given the valueName and the value to set it as
        public static void SetValue(string filePath, string valueName, bool value)
        {
            string valName = valueName.ToString();
            string[] arrLine = File.ReadAllLines(filePath);
            bool done = false;
            for (int i = 0; i < arrLine.Length; i++)
            {
                //Skip line if it starts with a comment
                if (!arrLine[i].StartsWith("//"))
                {
                    //Split string on equals sign
                    string[] lineSides = arrLine[i].Split('=');
                    if (lineSides[0] == valName)
                    {
                        //Set right side of line to the given value and write it to the file
                        arrLine[i] = valueName + "=" + value.ToString();
                        File.WriteAllLines(filePath, arrLine);
                        done = true;
                    }
                }
            }
            //Creates the value if none was found in the file
            if (!done)
            {
                try
                {
                    using (StreamWriter file = new StreamWriter(filePath, true))
                    {
                        file.WriteLine(valueName + "=" + value.ToString());
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Error: ", ex);
                }
            }
        }
    }
}
