using System.ComponentModel;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Reflection;
using System;
using Microsoft.Win32;

namespace WpfSampleApplication
{
    [RunInstaller(true)]
    public partial class WpfSampleInstaller : System.Configuration.Install.Installer
    {
        public WpfSampleInstaller()
        {
            InitializeComponent();
        }

        private string GetIISInstallationPath()
        {
            // Custom logic to retrieve the IIS installation path dynamically
            string iisPath = "";

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\InetStp"))
            {
                if (key != null)
                {
                    iisPath = key.GetValue("PathWWWRoot").ToString();
                }
            }

            return iisPath;
        }
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            var iisPath = GetIISInstallationPath();
            // 獲取 Web.config 文件的路徑
            string configFilePath = $"{iisPath}\\CRIEWebAPI\\Web.config";

            // 加載 Web.config 文件
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFilePath;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            // 獲取連接字符串的配置節
            ConnectionStringSettings connectionStringSettings = config.ConnectionStrings.ConnectionStrings["Expert_cancerEntities"];
            string flag = "";
            if (connectionStringSettings != null)
            {
                flag = connectionStringSettings.ToString();
            }
            else
            {
                flag = "0";
            }


            if (connectionStringSettings != null)
            {
                // 獲取用戶輸入的值
                string dataSource = Context.Parameters["DataSource"];
                string initialCatalog = Context.Parameters["InitialCatalog"];
                string userId = Context.Parameters["UserID"];
                string password = Context.Parameters["Password"];
                string persistsecurityinfo = Context.Parameters["PersistSecurityInfo"];
                string multipleactiveresultsets = Context.Parameters["MultipleActiveResultSets"];



                // 更新連接字符串的值
                string connectionString = connectionStringSettings.ConnectionString;
                var text = connectionStringSettings.ToString().Split(';');
                string ds = text[0].Substring(12, text[0].Length - 12);
                string ic = text[1].Substring(16, text[1].Length - 16);
                string uid = text[2].Substring(8, text[2].Length - 8);
                string pw = text[3].Substring(9, text[3].Length - 9);
                string psi = text[4].Substring(22, text[4].Length - 22);
                string mars = text[5].Substring(27, text[5].Length - 25);

                connectionString = connectionString.Replace(ds, dataSource);
                connectionString = connectionString.Replace(ic, initialCatalog);
                connectionString = connectionString.Replace(uid, userId);
                connectionString = connectionString.Replace(pw, password);
                connectionString = connectionString.Replace(psi, persistsecurityinfo);
                connectionString = connectionString.Replace(mars, multipleactiveresultsets);
                connectionStringSettings.ConnectionString = connectionString;
                File.WriteAllText("C://text.txt", connectionString);
                // 保存 Web.config 文件的修改
                config.Save(ConfigurationSaveMode.Modified);
            }
        }
    }
}
