﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

using bayoen.library.General.Enums;

namespace bayoen.star
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            List<Process> overlappedProcesses = Process.GetProcessesByName(Config.Assembly.GetName().Name).ToList();
            overlappedProcesses.RemoveAll(x => x.Id == Process.GetCurrentProcess().Id);

            if (overlappedProcesses.Count == 0) { Core.Initialize(); }
            else if (Core.Project.RestartingMode > RestartingModes.None)
            {
                bool settingFlag = (Core.Project.RestartingMode == RestartingModes.RestartWithSetting);
                Core.Project.RestartingMode = RestartingModes.None;
                Core.Project.Save();

                Core.Initialize();
                if (settingFlag) Core.SettingWindow.Show();
            }
            else
            {
                System.Media.SystemSounds.Hand.Play();
                MessageBox.Show("Already running!", Config.Assembly.GetName().Name, MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);
            }
        }
    }
}
