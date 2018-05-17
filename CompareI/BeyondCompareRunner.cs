using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareI
{
    class BeyondCompareRunner
    {
        const string BCOMPARE_PATH = @"SOFTWARE\Scooter Software\Beyond Compare";
        const string EXE_PATH_VALUE_NAME = "ExePath";
        public void LaunchNamedComparison(string comparisonName)
        {
            var key = Registry.LocalMachine.OpenSubKey(BCOMPARE_PATH, false);
            string path = key.GetValue(EXE_PATH_VALUE_NAME) as string;

            System.Diagnostics.Process.Start(path, comparisonName);
        }
    }
}
