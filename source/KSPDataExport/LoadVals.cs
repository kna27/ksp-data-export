using System;
using System.IO;

namespace KSPDataExport
{
    class LoadVals
    {
        public static bool GetValue(string filePath, string valueName, bool createIfDoesNotExist = true)
        {
            try
            {
                foreach (string line in File.ReadLines(filePath))
                {
                    if (!line.StartsWith("//"))
                    {
                        string[] lineSides = line.Split('=');
                        if (lineSides[0] == valueName)
                        {
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

        public static void SetValue(string filePath, string valueName, bool value)
        {
            string valName = valueName.ToString();
            string[] arrLine = File.ReadAllLines(filePath);
            bool done = false;
            for (int i = 0; i < arrLine.Length; i++)
            {
                if (!arrLine[i].StartsWith("//"))
                {
                    string[] lineSides = arrLine[i].Split('=');
                    if (lineSides[0] == valName)
                    {
                        arrLine[i] = valueName + "=" + value.ToString();
                        File.WriteAllLines(filePath, arrLine);
                        done = true;
                    }
                }
            }
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
