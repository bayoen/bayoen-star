﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using LiteDB;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using bayoen.library.General.Enums;
using bayoen.star.Localizations;
using bayoen.star.Windows;
using bayoen.star.Variables;

namespace bayoen.star
{
    public static partial class Core
    {
        public static void Initialize()
        {
            Culture.Set(Core.Project.LanguageCode);
            Core.Project.Version = Config.Version;
            Core._settingWindow = new SettingWindow();

            Core.InitialWorker = new BackgroundWorker();
            Core.InitialWorker.DoWork += InitialWorker_DoWork;
            Core.InitialWorker.RunWorkerCompleted += InitialWorker_RunWorkerCompleted;
            Core.InitialWorker.RunWorkerAsync();
        }

        private static void InitialWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                Core.MainWindow.Show();
                Core.MainWindow.InitialStatusResource = "InitialGrid-Message-Begin-String";
            });
            Thread.Sleep(Config.ThreadSleepMilliseconds);

            
            

            // Check online
            bool disconnected = false;
            for (int onlineTrial = 1; onlineTrial <= Config.OnlineTrialMax; onlineTrial++)
            {
                if (Core.IsGoogleOn) break;
                else
                {
                    if (onlineTrial == Config.OnlineTrialMax) disconnected = true;
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        Core.MainWindow.SetFormatInitialStatusKey("InitialGrid-Message-Internet-Check-String", onlineTrial);
                    });
                    Thread.Sleep(2000); // 2 seconds
                }
            }

            if (disconnected)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    Core.MainWindow.InitialStatusResource = "InitialGrid-Message-Internet-Disconnected-String";
                });
                Thread.Sleep(Config.ThreadSleepMilliseconds);

                Application.Current.Dispatcher.Invoke(delegate
                {
                    Core.TrayIcon.Visibility = Visibility.Visible;
                    Core.MainWindow.InitialStatusResource = "InitialGrid-Message-End-String";
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    Core.MainWindow.InitialStatusResource = "InitialGrid-Message-Update-String";
                });
                Thread.Sleep(Config.ThreadSleepMilliseconds);

                List<string> updateList = Core.BuildUpdateList();


            }



            //if (!Core.MainWorker.IsEnabled) Core.MainWorker.Initiate();
            //if (!Core.InGameWorker.IsEnabled) Core.InGameWorker.Initiate();

            Application.Current.Dispatcher.Invoke(delegate
            {
                Core.TrayIcon.Visibility = Visibility.Visible;
                Core.MainWindow.InitialStatusResource = "InitialGrid-Message-End-String";
            });
            Thread.Sleep(1000);
        }

        private static void InitialWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                Core.MainWindow.IsInitial = false;
#if DEBUG
                //Core.LeagueWindow.Show();
                //Core.CapturableWindow.Show();
                //Core.DebugWindow.Show();
                //Core.DashboardWindow.Show();
                //Core.LeagueWindow.Show();
                //Core.MiniWindow.Show();
                //Core.MiniOverlay.Show();
                //Core.SettingWindow.Show();
#endif
            });

            Core.InitialWorker.Dispose();
        }
    }
}
