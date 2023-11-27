using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Management;

namespace ContextMenuRestorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string regPath = "Software\\Classes\\CLSID";
        string firstkeyName = "{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}";
        string secondkeyName = "InprocServer32";
        public MainWindow()
        {
            InitializeComponent();  
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadGUI();
        }
        private void ReloadGUI()
        {
            if (CheckRegistryExists())
            {
                statusTextBlock.Text = "Old context menu is enabled";
                statusTextBlock.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                statusTextBlock.Text = "Old context menu is disabled";
                statusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            }
        }
        private void Restore_button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckRegistryExists())
            {
                RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(regPath+ "\\" + firstkeyName + "\\" + secondkeyName);
                Registry.SetValue("HKEY_CURRENT_USER\\" + regPath + "\\" + firstkeyName + "\\" + secondkeyName, "", String.Empty, RegistryValueKind.String);
                RestartExplorer();
            }
            else
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(regPath, true);
                if (registryKey != null)
                {
                    try
                    {
                        registryKey.DeleteSubKeyTree(firstkeyName);
                        RestartExplorer();
                    }catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                   
                }
            }
            ReloadGUI();
        }


        private void KillExplorer() {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Process WHERE Name='explorer.exe'");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                Process process = Process.GetProcessById(Convert.ToInt32(queryObj["ProcessId"]));
                process.Kill();
                process.WaitForExit(); 
            }

        }
        private void RestartExplorer()
        {
            try
            {
                KillExplorer();
                Process.Start("explorer.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CheckRegistryExists()
        {
            string fullregname = regPath + "\\" + firstkeyName;
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(fullregname);
            if(registryKey != null)
                return registryKey.GetSubKeyNames().Contains(secondkeyName);
            return false;
        }

        private void About_button_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }
    }
}