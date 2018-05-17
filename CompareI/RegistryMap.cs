using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareI
{
    class RegistryMap
    {
        const RegistryHive MAP_HIVE = RegistryHive.LocalMachine;
        const string MAP_LOCATION = @"SOFTWARE\CompareI";

        /// <summary>
        /// Key is solution path. Value is named comparison from beyond compare
        /// </summary>
        public IReadOnlyDictionary<string, string> SolutionMap { get; private set; }

        public RegistryMap()
        {
            var baseKey = RegistryKey.OpenBaseKey(MAP_HIVE, RegistryView.Registry64);
            var mapKey = baseKey.OpenSubKey(MAP_LOCATION, false);
            if (mapKey != null)
            {
                var valueNames = mapKey.GetValueNames();

                Dictionary<string, string> map = new Dictionary<string, string>();

                foreach (var valueName in valueNames)
                {
                    if (mapKey.GetValueKind(valueName) == RegistryValueKind.String)
                    {
                        map.Add(valueName, mapKey.GetValue(valueName) as string);
                    }
                }

                SolutionMap = map;
            }
            else
            {
                SolutionMap = new Dictionary<string, string>();
            }
        }
    }
}
