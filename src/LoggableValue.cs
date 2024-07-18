using System;

namespace KSPDataExport
{
    /// <summary>
    ///     A data point that can be logged
    /// </summary>
    public class LoggableValue
    {
        public string Name { get; }
        public Category Category { get; }
        private string ConfigName { get; }
        private bool _logging;

        public bool Logging
        {
            get => _logging;
            // Save the value to the config file
            set
            {
                _logging = value;
                Config.SetValue(DataExport.CfgPath, ConfigName, value);
            }
        }

        public Func<string> Value { get; }

        public LoggableValue(string name, Category category, string configName, Func<string> value)
        {
            Name = name;
            Category = category;
            ConfigName = configName;
            Value = value;
            // Read the value from the config file on instantiation
            Logging = Config.GetValue(DataExport.CfgPath, ConfigName);
        }
    }
}
