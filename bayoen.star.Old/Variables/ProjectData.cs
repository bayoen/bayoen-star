﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using bayoen.library.General.Enums;
using bayoen.star.Localizations;
using bayoen.star.Functions;
using System.Windows.Controls;

namespace bayoen.star.Variables
{
    public class ProjectData : FncJson
    {
        public ProjectData()
        {
            this.Version = Config.Assembly.GetName().Version;
            this.RestartingMode = RestartingModes.None;

            this.GoalCounter = GoalCounters.Star;

            this.AutoUpdate = true;
            this.EnglishDisplay = false;
            this.EnableSlowMode = false;
        }

        //// From Configurations
        public Version Version { get; set; }        

        //// From Setting Window
        /// General
        private bool _topMost;
        public bool TopMost
        {
            get => this._topMost;
            set
            {
                if (this._topMost == value) return;

                Core.MainWindow.Topmost = value;

                this._topMost = value;
            }
        }

        private bool _autoUpdate;
        public bool AutoUpdate
        {
            get => this._autoUpdate;
            set
            {
                if (this._autoUpdate == value) return;


                this._autoUpdate = value;
            }
        }

        private string _languageCode;
        public string LanguageCode
        {
            get
            {
                if (this._languageCode != null)
                {
                    if (Config.CultureCodes.Contains(this._languageCode))
                    {
                        return this._languageCode;
                    }
                }

                string cultureCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                if (!Config.CultureCodes.Contains(cultureCode)) cultureCode = Config.CultureCodes[0];
                return cultureCode;
            }

            set
            {
                if (this._languageCode == value) return;

                Culture.Set(value);

                this._languageCode = value;
                this.Save();
            }
        }

        private bool _englishDisplay;
        public bool EnglishDisplay
        {
            get => this._englishDisplay;
            set
            {
                if (this._englishDisplay == value) return;

                this._englishDisplay = value;
                this.Save();
            }
        }

        /// Streaming
        private ChromaKeys _chromaKey;
        public ChromaKeys ChromaKey
        {
            get => this._chromaKey;
            set
            {
                if (this._chromaKey == value) return;

                int chromaKeyIndex = Config.ChromaSets.FindIndex(x => x.Item1 == value);
                if (chromaKeyIndex > -1)
                {
                    if (chromaKeyIndex == 0)
                    {
                        Core.LeagueWindow.BorderThickness = new Thickness(1);
                        Core.LeagueWindow.SetResourceReference(Control.BackgroundProperty, "WindowBackgroundBrush");
                    }
                    else
                    {
                        Core.LeagueWindow.BorderThickness = new Thickness(0);
                        Core.LeagueWindow.Background = Config.ChromaSets[chromaKeyIndex].Item2;
                    }                    
                }

                this._chromaKey = value;
            }
        }

        /// Advanced
        private bool _enableSlowMode = true;
        public bool EnableSlowMode
        {
            get => this._enableSlowMode;
            set
            {
                if (this._enableSlowMode == value) return;

                if (Core.MainWorker.IsEnabled)
                {
                    Core.MainWorker.Stop();
                    Core.MainWorker.Interval = value ? Config.DisplayIntervalSlow : Config.DisplayIntervalNormal;
                    Core.MainWorker.Start();
                }
                

                if (Core.InGameWorker.IsEnabled)
                {
                    Core.InGameWorker.Stop();
                    Core.InGameWorker.Interval = value ? Config.TrackingIntervalSlow : Config.TrackingIntervalNormal;
                    Core.InGameWorker.Start();
                }                

                this._enableSlowMode = value;
                this.Save();
            }
        }

        private bool _disableEarlyRefresh;
        public bool DisableEarlyRefresh
        {
            get => this._disableEarlyRefresh;
            set
            {
                if (this._disableEarlyRefresh == value) return;



                this._disableEarlyRefresh = value;
            }
        }



        //// For Operation
        public RestartingModes RestartingMode { get; set; }

        private TrackingModes _trackingMode;
        public TrackingModes TrackingMode
        {
            get => this._trackingMode;
            set
            {
                if (this._trackingMode == value) return;

                //Core.MainWindow.AlwaysTopModeButton.IsAccented = (value == TrackingModes.Always);
                //Core.MainWindow.LeagueTopModeButton.IsAccented = (value == TrackingModes.League);
                //Core.MainWindow.FriendlyTopModeButton.IsAccented = (value == TrackingModes.Friendly);
                //Core.MainWindow.ArcadeTopModeButton.IsAccented = (value == TrackingModes.Arcade);
                //Core.MainWindow.NoneTopModeButton.IsAccented = (value == TrackingModes.None);

                this._trackingMode = value;

                this.Save();
            }
        }

        private MatchCategories _matchType;
        public MatchCategories MatchType
        {
            get => this._matchType;
            set
            {
                if (this._matchType == value) return;



                this._matchType = value;
            }
        }

        private ScoreDisplayModes _displayMode;
        public ScoreDisplayModes DisplayMode
        {
            get => this._displayMode;
            set
            {
                if (this._displayMode == value) return;



                this._displayMode = value;
            }
        }

        private List<int> _countedStars;
        public List<int> CountedStars
        {
            get => this._countedStars ?? (this._countedStars = new List<int>(4) { 0, 0, 0, 0 });
            set
            {
                if (value != null && this._countedStars != null)
                    if (value.SequenceEqual(this._countedStars)) return;

                Core.MiniWindow.MiniScorePanel.Stars = value;
                Core.MiniOverlay.MiniScorePanel.Stars = value;

                this._countedStars = value;
                this.Save();
            }
        }

        private List<int> _countedGames;
        public List<int> CountedGames
        {
            get => this._countedGames ?? (this._countedGames = new List<int>(4) { 0, 0, 0, 0 });
            set
            {
                if (value != null && this._countedGames != null)
                    if (value.SequenceEqual(this._countedGames)) return;

                Core.MiniWindow.MiniScorePanel.Games = value;
                Core.MiniOverlay.MiniScorePanel.Games = value;

                this._countedGames = value;
                this.Save();
            }
        }

        private GoalTypes _goalType;
        public GoalTypes GoalType
        {
            get => this._goalType;
            set
            {
                if (this._goalType == value) return;

                

                this._goalType = value;
            }
        }

        private GoalCounters _goalCounter = GoalCounters.Game;
        public GoalCounters GoalCounter
        {
            get => this._goalCounter;
            set
            {
                if (this._goalCounter == value) return;

                Core.MainWindow.GoalStarButton.IsAccented = (value == GoalCounters.Star);
                Core.MainWindow.GoalGameButton.IsAccented = (value == GoalCounters.Game);
                    

                Core.MiniWindow.GoalCounter = value;
                Core.MiniOverlay.GoalCounter = value;


                this._goalCounter = value;
            }
        }

        private int _goalScore;
        public int GoalScore
        {
            get => this._goalScore;
            set
            {
                if (this._goalScore == value) return;



                this._goalScore = value;
            }
        }
       
        private RecordDisplayModes _recordDisplayMode;
        public RecordDisplayModes RecordDisplayMode
        {
            get => this._recordDisplayMode;
            set
            {
                if (this._recordDisplayMode == value) return;

                Core.EventViewer.EventModeButton.IsAccented = (value == RecordDisplayModes.Event);
                Core.EventViewer.MatchModeButton.IsAccented = (value == RecordDisplayModes.Match);                

                this._recordDisplayMode = value;
                this.Save();
            }
        }

        private int _matchPageSize = 10;
        public int MatchPageSize
        {
            get => this._matchPageSize;
            set
            {
                int size = Math.Max(5, Math.Min(20, value));

                if (this._matchPageSize == size) return;

                Core.EventViewer.PageSize = size;
                Core.EventViewer.Check();

                this._matchPageSize = size;
                this.Save();
            }
        }

        private int _leaguePageSize = 5;
        public int LeaguePageSize
        {
            get => this._leaguePageSize;
            set
            {
                int size = Math.Max(3, Math.Min(10, value));

                if (this._leaguePageSize == size) return;

                Core.LeagueWindow.PageSize = size;
                Core.LeagueWindow.Check();

                this._leaguePageSize = size;
                this.Save();
            }
        }
        

        public static ProjectData Load()
        {
            return ProjectData.Load(Config.ProjectDataFileName);
        }

        public static ProjectData Load(string path)
        {
            bool failFlag = false;
            string rawString;
            if (File.Exists(path))
            {
                rawString = File.ReadAllText(path, Config.TextEncoding);
            }
            else
            {
                rawString = "";
                failFlag = true;
            }

            ProjectData settingData = null;
            try
            {
                settingData = JsonConvert.DeserializeObject<ProjectData>(rawString, Config.JsonSerializerSetting);
            }
            catch
            {
                failFlag = true;
            }

            if (settingData == null) failFlag = true;
            if (failFlag)
            {
                settingData = new ProjectData();
                settingData.Save(path);
            }

            return settingData;
        }

        public void Save()
        {
            this.Save(Config.ProjectDataFileName);
        }

        //public void Save(string path)
        //{
        //    string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented, Config.JsonSerializerSetting);
        //    File.WriteAllText(path, jsonString, Config.TextEncoding);
        //}
    }
}
