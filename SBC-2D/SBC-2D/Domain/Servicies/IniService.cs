using SBC_2D.Infrastructures.Ini;
using System;
using System.Linq;
using SBC_2D.Shared;
using System.Windows.Forms;

namespace SBC_2D.Domain.Servicies
{
    public class IniService
    {
        private readonly IniStore _store;
        public IniService(IniStore store)
        {
            _store = store;
        }

        public Setup GetSetup()
        {
            var pathConfig = GetPathConfig();
            var deviceConfig = new DeviceConfig();
            var productionConfig = new ProductionConfig();
            _store.Setup = new Setup(deviceConfig, productionConfig, pathConfig);
            return _store.Setup;
        }


        public string GetName()
        {
            string section = "Production";
            string key = "ModelName";
            string defaultValue = "";
            if (!IniFile.TryGetValue(section, key, defaultValue, _store.SetupFilePath, out string modelName))
                return defaultValue;
            return modelName;
        }

        public void SaveName(string name)
        {
            string section = "Production";
            string key = "ModelName";
        }

        public PathConfig GetPathConfig()
        {
            PathConfig path = new PathConfig();
            string basePath = Application.StartupPath;
            string bugInfo = string.Empty;
            string section = "Path";

            if (!IniFile.TryGetValue(section, "SqLiteFile", "", _store.SetupFilePath, out string sqLiteFile))
                bugInfo += "Missing SqLiteFile.\r\n";
            else
                path.SqLiteFile = basePath + sqLiteFile;

            if (!string.IsNullOrEmpty(bugInfo))
            {
            }

            return path;
        }
    }
}
