using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompareI
{
    class SqlClipboard
    {
        public void CopyContents(IEnumerable<string> sqlFiles)
        {
            List<string> lines = new List<string>();

            foreach (var file in sqlFiles.OrderBy(s=>s))
            {
                lines.AddRange(File.ReadAllLines(file));
            }

            var line = lines.Aggregate((s1, s2) => s1 + "\n" + s2);

            Clipboard.SetText(line);
        }
    }
}
