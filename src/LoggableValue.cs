using System;
using UnityEngine;

namespace KSPDataExport
{
    public class LoggableValue
    {
        public string Name { get; set; }
        public Category Category { get; set; }
        public string ConfigName { get; set; }
        private bool _logging;

        public bool Logging
        {
            get { return _logging; }
            set
            {
                _logging = value;
                Config.SetValue(DataExport.cfgPath, ConfigName, value);
            }
        }
        public Func<string> Value { get; set; }
        
        public LoggableValue(string name, Category category, string configName, Func<string> value)
        {
            Name = name;
            Category = category;
            ConfigName = configName;
            Value = value;
            Logging = Config.GetValue(DataExport.cfgPath, ConfigName);
        }
    }
}