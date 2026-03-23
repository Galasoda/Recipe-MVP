using IniParser.Model;
using System.Windows.Forms;

namespace SBC_2D.Infrastructures.Ini
{
    public class IniStore
    {
        public string SetupFilePath { get; } = Application.StartupPath + @"\Setup\Setup.ini";
        public Setup Setup { get; set; }
    }
}