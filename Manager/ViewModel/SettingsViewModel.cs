using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ControlzEx.Theming;
using Core;
using Core.Models;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using Manager.Messages;
using Manager.Models;
using Manager.Properties;
using Manager.Services;
using ServicesSettings = Services.Properties.Settings;
using Manager.ViewModel.Shared;
using Services.Concrete.Movie;
using Services.Exceptions;
using Services.Interfaces;
using Application = System.Windows.Application;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Newtonsoft.Json;
using Services.Models.GitHub;
using System.Net;
using System.Net.Http;

namespace Manager.ViewModel
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Properties

        private int _resolutionWidth = Settings.Default.ResolutionWidth;

        private int _resolutionHeight = Settings.Default.ResolutionHeight;

        private bool _resolutionFullscreen = Settings.Default.IsFullscreen;

        private bool _isShowAllAccounts = Settings.Default.IsShowAllAccounts;

        private bool _isShowOnlyAccountDemos = Settings.Default.ShowOnlyAccountDemos;

        private bool _isShowAllPlayers = Settings.Default.IsShowAllPlayers;

        private string _launchParameters = Settings.Default.LaunchParameters;

        private string _steamApiKey = ServicesSettings.Default.SteamApiKey;

        private int _demosListSize = Settings.Default.DemosListSize;

        private bool _useSimpleRadar = Settings.Default.UseSimpleRadar;

        private bool _isCheckForUpdatesEnabled = Settings.Default.EnableCheckUpdate;

        private string _hlaeExecutableLocation = ServicesSettings.Default.HlaeExecutableLocation;
        public string HlaeExecutableLocation
        {
            get { return _hlaeExecutableLocation; }
            set { Set(() => HlaeExecutableLocation, ref _hlaeExecutableLocation, value); }
        }

        private bool _isHlaeCustomLocationEnabled = ServicesSettings.Default.IsHlaeCustomLocationEnabled;
        public bool IsHlaeCustomLocationEnabled
        {
            get { return _isHlaeCustomLocationEnabled; }
            set { Set(() => IsHlaeCustomLocationEnabled, ref _isHlaeCustomLocationEnabled, value); }
        }

        private string _hlaeParameters = ServicesSettings.Default.HlaeParameters;
        public string HlaeParameters
        {
            get { return _hlaeParameters; }
            set
            {
                ServicesSettings.Default.HlaeParameters = value;
                Set(() => HlaeParameters, ref _hlaeParameters, value);
            }
        }

        private bool _isFfmpegCustomLocationEnabled = ServicesSettings.Default.IsFfmpegCustomLocationEnabled;
        public bool IsFfmpegCustomLocationEnabled
        {
            get { return _isFfmpegCustomLocationEnabled; }
            set { Set(() => IsFfmpegCustomLocationEnabled, ref _isFfmpegCustomLocationEnabled, value); }
        }

        private string _ffmpegExecutableLocation = ServicesSettings.Default.FfmpegExecutableLocation;
        public string FfmpegExecutableLocation
        {
            get { return _ffmpegExecutableLocation; }
            set { Set(() => FfmpegExecutableLocation, ref _ffmpegExecutableLocation, value); }
        }

        private RelayCommand _clearDemosDataCacheCommand;

        private RelayCommand _importCustomDataCacheCommand;

        private RelayCommand _exportCustomDataCacheCommand;

        private RelayCommand _navigateToLogFile;

        private RelayCommand _selectCsgoExePathCommand;

        private RelayCommand _enableHlaeCommand;

        private RelayCommand _addAccountCommand;

        private RelayCommand<Account> _removeAccountCommand;

        private RelayCommand _syncAccountNickname;

        private RelayCommand _setAsWatchAccountCommand;

        private RelayCommand _selectDownloadFolderPath;

        private RelayCommand _deleteVdmFilesCommand;

        private readonly ICacheService _cacheService;

        private readonly IDemosService _demosService;

        private readonly IDialogService _dialogService;

        private readonly ISteamService _steamService;

        private readonly IAccountStatsService _accountStatsService;

        private List<ComboboxSelector> _maxConcurrentAnalyzes;

        private string _selectedMaxConcurrentAnalyzes;

        private List<ComboboxSelector> _themes;

        private ComboboxSelector _selectedTheme;

        private List<ComboboxSelector> _languages;

        private ComboboxSelector _selectedLanguage;

        private bool _isUseCustomActionGeneration = Settings.Default.UseCustomActionsGeneration;

        private bool _showDateColumn = Settings.Default.ShowDateColumn;

        private bool _showBombPlantedColumn = Settings.Default.ShowBombPlantedColumn;

        private bool _showBombDefusedColumn = Settings.Default.ShowBombDefusedColumn;

        private bool _showBombExplodedColumn = Settings.Default.ShowBombExplodedColumn;

        private bool _showDemoNameColumn = Settings.Default.ShowDemoNameColumn;

        private bool _showMapNameColumn = Settings.Default.ShowMapNameColumn;

        private bool _showTeam1NameColumn = Settings.Default.ShowTeam1NameColumn;

        private bool _showTeam2NameColumn = Settings.Default.ShowTeam2NameColumn;

        private bool _showScoreTeam1Column = Settings.Default.ShowScoreTeam1Column;

        private bool _showScoreTeam2Column = Settings.Default.ShowScoreTeam2Column;

        private bool _showHostnameColumn = Settings.Default.ShowHostnameColumn;

        private bool _showClientnameColumn = Settings.Default.ShowClientnameColumn;

        private bool _showDemoTypeColumn = Settings.Default.ShowDemoTypeColumn;

        private bool _showFramerateColumn = Settings.Default.ShowFramerateColumn;

        private bool _showTicksColumn = Settings.Default.ShowTicksColumn;

        private bool _showTickrateColumn = Settings.Default.ShowTickrateColumn;

        private bool _showTradeKillColumn = Settings.Default.ShowTradeKillColumn;

        private bool _showTradeDeathColumn = Settings.Default.ShowTradeDeathColumn;

        private bool _showOneKillColumn = Settings.Default.ShowOneKillColumn;

        private bool _showTwoKillsColumn = Settings.Default.ShowTwoKillsColumn;

        private bool _showThreeKillsColumn = Settings.Default.ShowThreeKillsColumn;

        private bool _showFourKillsColumn = Settings.Default.ShowFourKillsColumn;

        private bool _showFiveKillsColumn = Settings.Default.ShowFiveKillsColumn;

        private bool _showTotalKillsColumn = Settings.Default.ShowTotalKillsColumn;

        private bool _showDeathsColumn = Settings.Default.ShowDeathsColumn;

        private bool _showAssistsColumn = Settings.Default.ShowAssistsColumn;

        private bool _showKdColumn = Settings.Default.ShowKdColumn;

        private bool _showHsColumn = Settings.Default.ShowHsColumn;

        private bool _showKastColumn = Settings.Default.ShowKastColumn;

        private bool _showJumpKillColumn = Settings.Default.ShowJumpKillCoulmn;

        private bool _showCrouchKillColumn = Settings.Default.ShowCrouchKillsColumn;

        private bool _showTkColumn = Settings.Default.ShowTkColumn;

        private bool _showEkColumn = Settings.Default.ShowEkColumn;

        private bool _showMvpColumn = Settings.Default.ShowMvpColumn;

        private bool _showPlayerScoreColumn = Settings.Default.ShowPlayerScoreColumn;

        private bool _showEseaRwsColumn = Settings.Default.ShowEseaRwsColumn;

        private bool _showClutch1v1Column = Settings.Default.ShowClutch1v1Column;

        private bool _showClutch1v2Column = Settings.Default.ShowClutch1v2Column;

        private bool _showClutch1v3Column = Settings.Default.ShowClutch1v3Column;

        private bool _showClutch1v4Column = Settings.Default.ShowClutch1v4Column;

        private bool _showClutch1v5Column = Settings.Default.ShowClutch1v5Column;

        private bool _showStartMoneyTeam1Column = Settings.Default.ShowStartMoneyTeam1Column;

        private bool _showStartMoneyTeam2Column = Settings.Default.ShowStartMoneyTeam2Column;

        private bool _showEquipementValueTeam1Column = Settings.Default.ShowEquipementValueTeam1Column;

        private bool _showEquipementValueTeam2Column = Settings.Default.ShowEquipementValueTeam2Column;

        private bool _showWinnerClanNameColumn = Settings.Default.ShowWinnerClanNameColumn;

        private bool _showCtTeamName = Settings.Default.ShowCtTeamName;

        private bool _showTteamName = Settings.Default.ShowTteamName;

        private bool _showWinnerSideColumn = Settings.Default.ShowWinnerSideColumn;

        private bool _showDurationColumn = Settings.Default.ShowDurationColumn;

        private bool _dateFormatEuropean = Settings.Default.DateFormatEuropean;

        private bool _showCommentColumn = Settings.Default.ShowCommentColumn;

        private bool _showClutchCountColumn = Settings.Default.ShowClutchCountColumn;

        private bool _showTotalDamageHealthColumn = Settings.Default.ShowTotalDamageHealthColumn;

        private bool _showTotalDamageArmorColumn = Settings.Default.ShowTotalDamageArmorColumn;

        private bool _showAverageDamageColumn = Settings.Default.ShowAverageDamageColumn;

        private bool _showBanColumns = Settings.Default.ShowBanColumns;

        private bool _showKillPerRoundColumn = Settings.Default.ShowKillPerRoundColumn;

        private bool _showAssistPerRoundColumn = Settings.Default.ShowAssistPerRoundColumn;

        private bool _showDeathPerRoundColumn = Settings.Default.ShowDeathPerRoundColumn;

        private bool _showRoundTypeColumn = Settings.Default.ShowRoundTypeColumn;

        private bool _showSideTroubleColumn = Settings.Default.ShowSideTroubleColumn;

        private bool _showTeamTroubleColumn = Settings.Default.ShowTeamTroubleColumn;

        private bool _showtimeDeathColumn = Settings.Default.ShowTimeDeathColumn;

        private bool _showRoundPlayedColumn = Settings.Default.ShowRoundPlayedColumn;

        private string _csgoExePath = Settings.Default.CsgoExePath;

        private string _hlaeConfigParentFolderPath = Settings.Default.HlaeConfigParentFolderPath;

        private string _downloadFolderPath = Settings.Default.DownloadFolder;

        private bool _enableHlae = Settings.Default.EnableHlae;

        private bool _enableHlaeConfigParent = Settings.Default.EnableHlaeConfigParent;

        private bool _limitStatsSelectedFolder = Settings.Default.LimitStatsFolder;

        private bool _ignoreLaterBan = Settings.Default.IgnoreLaterBan;

        private bool _startBotOnLaunch = Settings.Default.StartBotOnLaunch;

        private bool _closeBotOnExit = Settings.Default.CloseBotOnExit;

        private DateTime _dateStatsFrom = Settings.Default.DateStatsFrom;

        private DateTime _dateStatsTo = IsInDesignModeStatic ? DateTime.Today : Settings.Default.DateStatsTo;

        private List<DemoStatus> _statusList;

        /// <summary>
        /// List of user's accounts saved
        /// </summary>
        private ObservableCollection<Account> _accounts = new ObservableCollection<Account>();

        /// <summary>
        /// Account currently selected on the settings view
        /// </summary>
        private Account _selectedAccount;

        /// <summary>
        /// Account selected on the Home view
        /// </summary>
        private Account _selectedStatsAccount;

        /// <summary>
        /// Account selected for watch features
        /// </summary>
        private Account _selectedWatchAccount;

        /// <summary>
        /// SteamID of the account used for "watch" (lowlights / highlights) buttons
        /// </summary>
        private long _watchAccountSteamId = Settings.Default.WatchAccountSteamId;

        /// <summary>
        /// If the SteamID is 0 it means that the user want to see all stats
        /// </summary>
        public bool ShowOnlyAccountStats => Settings.Default.SelectedStatsAccountSteamID != 0;

        /// <summary>
        /// Demos cache size
        /// </summary>
        private long _cacheSize;

        #endregion

        #region Accessors

        public RelayCommand<bool> EnableHlaeCustomLocationCommand { get; }
        public RelayCommand ResetCustomHlaeLocationCommand { get; }
        public RelayCommand UpdateCustomHlaeLocationCommand { get; }
        public RelayCommand RevealHlaeExecutableCommand { get; }
        public RelayCommand EnableHlaeConfigFolderCommand { get; }
        public RelayCommand UpdateHlaeConfigFolderCommand { get; }
        public RelayCommand<bool> EnableFfmpegCustomLocationCommand { get; }
        public RelayCommand RevealFfmpegExecutableCommand { get; }
        public RelayCommand UpdateCustomFfmpegLocationCommand { get; }
        public RelayCommand ResetCustomFfmpegLocationCommand { get; }

        public RelayCommand ApplySteamApiKeyCommand { get; }

        public RelayCommand ResetSteamApiKeyCommand { get; }

        public List<ComboboxSelector> Languages
        {
            get { return _languages; }
            set { Set(() => Languages, ref _languages, value); }
        }

        public ComboboxSelector SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                Set(() => SelectedLanguage, ref _selectedLanguage, value);
                if (!string.IsNullOrEmpty(value.Id) && value.Id != Settings.Default.Language)
                {
                    Settings.Default.Language = value.Id;
                    Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
            }
        }

        public List<ComboboxSelector> MaxConcurrentAnalyzes
        {
            get { return _maxConcurrentAnalyzes; }
            set { Set(() => MaxConcurrentAnalyzes, ref _maxConcurrentAnalyzes, value); }
        }


        public string SelectedMaxConcurrentAnalyzes
        {
            get { return _selectedMaxConcurrentAnalyzes; }
            set
            {
                Set(() => SelectedMaxConcurrentAnalyzes, ref _selectedMaxConcurrentAnalyzes, value);
                Settings.Default.MaxConcurrentAnalyzes = int.Parse(value);
            }
        }

        public List<ComboboxSelector> Themes
        {
            get { return _themes; }
            set { Set(() => Themes, ref _themes, value); }
        }

        public ComboboxSelector SelectedTheme
        {
            get { return _selectedTheme; }
            set
            {
                Set(() => SelectedTheme, ref _selectedTheme, value);
                if (value.Id != null)
                {
                    Settings.Default.Theme = value.Id;
                    ThemeManager.Current.ChangeThemeBaseColor(Application.Current, value.Id);
                }
            }
        }

        public Account SelectedAccount
        {
            get { return _selectedAccount; }
            set { Set(() => SelectedAccount, ref _selectedAccount, value); }
        }

        public Account SelectedStatsAccount
        {
            get { return _selectedStatsAccount; }
            set
            {
                Set(() => SelectedStatsAccount, ref _selectedStatsAccount, value);
                if (value == null)
                {
                    Settings.Default.SelectedStatsAccountSteamID = 0;
                    _demosService.SelectedStatsAccountSteamId = 0;
                    _accountStatsService.SelectedStatsAccountSteamId = 0;
                    _cacheService.Filter.SteamId = 0;
                }
                else
                {
                    long steamId;
                    long.TryParse(value.SteamId, out steamId);
                    Settings.Default.SelectedStatsAccountSteamID = steamId;
                    _demosService.SelectedStatsAccountSteamId = steamId;
                    _accountStatsService.SelectedStatsAccountSteamId = steamId;
                    _cacheService.Filter.SteamId = steamId;
                    IsShowAllAccounts = false;
                }

                RaisePropertyChanged(() => ShowOnlyAccountStats);
                Messenger.Default.Send(new SelectedAccountChangedMessage());
            }
        }

        public ObservableCollection<Account> Accounts
        {
            get { return _accounts; }
            set { Set(() => Accounts, ref _accounts, value); }
        }

        public bool IsShowAllAccounts
        {
            get { return _isShowAllAccounts; }
            set
            {
                Settings.Default.IsShowAllAccounts = value;
                if (value && SelectedStatsAccount != null)
                {
                    SelectedStatsAccount = null;
                }

                Set(() => IsShowAllAccounts, ref _isShowAllAccounts, value);
            }
        }

        public bool IsShowOnlyAccountDemos
        {
            get { return _isShowOnlyAccountDemos; }
            set
            {
                Settings.Default.ShowOnlyAccountDemos = value;
                Set(() => IsShowOnlyAccountDemos, ref _isShowOnlyAccountDemos, value);
            }
        }

        public Account SelectedWatchAccount
        {
            get { return _selectedWatchAccount; }
            set
            {
                Set(() => SelectedWatchAccount, ref _selectedWatchAccount, value);
                if (value != null)
                {
                    long steamId;
                    long.TryParse(value.SteamId, out steamId);
                    WatchAccountSteamId = steamId;
                }
                else
                {
                    WatchAccountSteamId = 0;
                }
            }
        }

        public bool UseSimpleRadar
        {
            get => _useSimpleRadar;
            set
            {
                Set(() => UseSimpleRadar, ref _useSimpleRadar, value);
                Settings.Default.UseSimpleRadar = value;
            }
        }

        public bool IsShowAllPlayers
        {
            get { return _isShowAllPlayers; }
            set
            {
                Set(() => IsShowAllPlayers, ref _isShowAllPlayers, value);
                Settings.Default.IsShowAllPlayers = value;
            }
        }

        public bool IsUseCustomActionGeneration
        {
            get { return _isUseCustomActionGeneration; }
            set
            {
                Settings.Default.UseCustomActionsGeneration = value;
                Set(() => IsUseCustomActionGeneration, ref _isUseCustomActionGeneration, value);
            }
        }

        public bool IsCheckForUpdatesEnabled
        {
            get { return _isCheckForUpdatesEnabled; }
            set
            {
                Settings.Default.EnableCheckUpdate = value;
                Set(() => IsCheckForUpdatesEnabled, ref _isCheckForUpdatesEnabled, value);
            }
        }

        public int ResolutionWidth
        {
            get { return _resolutionWidth; }
            set
            {
                Settings.Default.ResolutionWidth = value;
                Set(() => ResolutionWidth, ref _resolutionWidth, value);
            }
        }

        public int ResolutionHeight
        {
            get { return _resolutionHeight; }
            set
            {
                Settings.Default.ResolutionHeight = value;
                Set(() => ResolutionHeight, ref _resolutionHeight, value);
            }
        }

        public bool ResolutionFullscreen
        {
            get { return _resolutionFullscreen; }
            set
            {
                Settings.Default.IsFullscreen = value;
                Set(() => ResolutionFullscreen, ref _resolutionFullscreen, value);
            }
        }

        public string SteamApiKey
        {
            get { return _steamApiKey; }
            set { Set(() => SteamApiKey, ref _steamApiKey, value); }
        }

        public string LaunchParameters
        {
            get { return _launchParameters; }
            set
            {
                Settings.Default.LaunchParameters = value;
                Set(() => LaunchParameters, ref _launchParameters, value);
            }
        }

        public bool DateFormatUsa
        {
            get { return _dateFormatEuropean; }
            set
            {
                Settings.Default.DateFormatEuropean = value;
                Set(() => DateFormatUsa, ref _dateFormatEuropean, value);
            }
        }

        public int DemosListSize
        {
            get { return _demosListSize; }
            set
            {
                Settings.Default.DemosListSize = value;
                Set(() => DemosListSize, ref _demosListSize, value);
            }
        }

        public bool ShowWinnerClanNameColumn
        {
            get { return _showWinnerClanNameColumn; }
            set
            {
                Settings.Default.ShowWinnerClanNameColumn = value;
                Set(() => ShowWinnerClanNameColumn, ref _showWinnerClanNameColumn, value);
            }
        }

        public bool ShowCtTeamNameColumn
        {
            get { return _showCtTeamName; }
            set
            {
                Settings.Default.ShowCtTeamName = value;
                Set(() => ShowCtTeamNameColumn, ref _showCtTeamName, value);
            }
        }

        public bool ShowTteamNameColumn
        {
            get { return _showTteamName; }
            set
            {
                Settings.Default.ShowTteamName = value;
                Set(() => ShowTteamNameColumn, ref _showTteamName, value);
            }
        }

        public bool ShowWinnerSideColumn
        {
            get { return _showWinnerSideColumn; }
            set
            {
                Settings.Default.ShowWinnerSideColumn = value;
                Set(() => ShowWinnerSideColumn, ref _showWinnerSideColumn, value);
            }
        }

        public bool ShowStartMoneyTeam1Column
        {
            get { return _showStartMoneyTeam1Column; }
            set
            {
                Settings.Default.ShowStartMoneyTeam1Column = value;
                Set(() => ShowStartMoneyTeam1Column, ref _showStartMoneyTeam1Column, value);
            }
        }

        public bool ShowStartMoneyTeam2Column
        {
            get { return _showStartMoneyTeam2Column; }
            set
            {
                Settings.Default.ShowStartMoneyTeam2Column = value;
                Set(() => ShowStartMoneyTeam2Column, ref _showStartMoneyTeam2Column, value);
            }
        }

        public bool ShowEquipementValueTeam1Column
        {
            get { return _showEquipementValueTeam1Column; }
            set
            {
                Settings.Default.ShowEquipementValueTeam1Column = value;
                Set(() => ShowEquipementValueTeam1Column, ref _showEquipementValueTeam1Column, value);
            }
        }

        public bool ShowEquipementValueTeam2Column
        {
            get { return _showEquipementValueTeam2Column; }
            set
            {
                Settings.Default.ShowEquipementValueTeam2Column = value;
                Set(() => ShowEquipementValueTeam2Column, ref _showEquipementValueTeam2Column, value);
            }
        }

        public bool ShowDateColumn
        {
            get { return _showDateColumn; }
            set
            {
                Settings.Default.ShowDateColumn = value;
                Set(() => ShowDateColumn, ref _showDateColumn, value);
            }
        }

        public bool ShowDemoNameColumn
        {
            get { return _showDemoNameColumn; }
            set
            {
                Settings.Default.ShowDemoNameColumn = value;
                Set(() => ShowDemoNameColumn, ref _showDemoNameColumn, value);
            }
        }

        public bool ShowMapNameColumn
        {
            get { return _showMapNameColumn; }
            set
            {
                Settings.Default.ShowMapNameColumn = value;
                Set(() => ShowMapNameColumn, ref _showMapNameColumn, value);
            }
        }

        public bool ShowHostnameColumn
        {
            get { return _showHostnameColumn; }
            set
            {
                Settings.Default.ShowHostnameColumn = value;
                Set(() => ShowHostnameColumn, ref _showHostnameColumn, value);
            }
        }

        public bool ShowClientnameColumn
        {
            get { return _showClientnameColumn; }
            set
            {
                Settings.Default.ShowClientnameColumn = value;
                Set(() => ShowClientnameColumn, ref _showClientnameColumn, value);
            }
        }

        public bool ShowDemoTypeColumn
        {
            get { return _showDemoTypeColumn; }
            set
            {
                Settings.Default.ShowDemoTypeColumn = value;
                Set(() => ShowDemoTypeColumn, ref _showDemoTypeColumn, value);
            }
        }

        public bool ShowFramerateColumn
        {
            get { return _showFramerateColumn; }
            set
            {
                Settings.Default.ShowFramerateColumn = value;
                Set(() => ShowFramerateColumn, ref _showFramerateColumn, value);
            }
        }

        public bool ShowTicksColumn
        {
            get { return _showTicksColumn; }
            set
            {
                Settings.Default.ShowTicksColumn = value;
                Set(() => ShowTicksColumn, ref _showTicksColumn, value);
            }
        }

        public bool ShowTickrateColumn
        {
            get { return _showTickrateColumn; }
            set
            {
                Settings.Default.ShowTickrateColumn = value;
                Set(() => ShowTickrateColumn, ref _showTickrateColumn, value);
            }
        }

        public bool ShowTeam1NameColumn
        {
            get { return _showTeam1NameColumn; }
            set
            {
                Settings.Default.ShowTeam1NameColumn = value;
                Set(() => ShowTeam1NameColumn, ref _showTeam1NameColumn, value);
            }
        }

        public bool ShowTeam2NameColumn
        {
            get { return _showTeam2NameColumn; }
            set
            {
                Settings.Default.ShowTeam2NameColumn = value;
                Set(() => ShowTeam2NameColumn, ref _showTeam2NameColumn, value);
            }
        }

        public bool ShowScoreTeam1Column
        {
            get { return _showScoreTeam1Column; }
            set
            {
                Settings.Default.ShowScoreTeam1Column = value;
                Set(() => ShowScoreTeam1Column, ref _showScoreTeam1Column, value);
            }
        }

        public bool ShowScoreTeam2Column
        {
            get { return _showScoreTeam2Column; }
            set
            {
                Settings.Default.ShowScoreTeam2Column = value;
                Set(() => ShowScoreTeam2Column, ref _showScoreTeam2Column, value);
            }
        }

        public bool ShowBombPlantedColumn
        {
            get { return _showBombPlantedColumn; }
            set
            {
                Settings.Default.ShowBombPlantedColumn = value;
                Set(() => ShowBombPlantedColumn, ref _showBombPlantedColumn, value);
            }
        }

        public bool ShowBombExplodedColumn
        {
            get { return _showBombExplodedColumn; }
            set
            {
                Settings.Default.ShowBombExplodedColumn = value;
                Set(() => ShowBombExplodedColumn, ref _showBombExplodedColumn, value);
            }
        }

        public bool ShowBombDefusedColumn
        {
            get { return _showBombDefusedColumn; }
            set
            {
                Settings.Default.ShowBombDefusedColumn = value;
                Set(() => ShowBombDefusedColumn, ref _showBombDefusedColumn, value);
            }
        }

        public bool ShowJumpKillColumn
        {
            get { return _showJumpKillColumn; }
            set
            {
                Settings.Default.ShowJumpKillCoulmn = value;
                Set(() => ShowJumpKillColumn, ref _showJumpKillColumn, value);
            }
        }

        public bool ShowCrouchKillColumn
        {
            get { return _showCrouchKillColumn; }
            set
            {
                Settings.Default.ShowCrouchKillsColumn = value;
                Set(() => ShowCrouchKillColumn, ref _showCrouchKillColumn, value);
            }
        }

        public bool ShowTradeDeathColumn
        {
            get { return _showTradeDeathColumn; }
            set
            {
                Settings.Default.ShowTradeDeathColumn = value;
                Set(() => ShowTradeDeathColumn, ref _showTradeDeathColumn, value);
            }
        }

        public bool ShowTradeKillColumn
        {
            get { return _showTradeKillColumn; }
            set
            {
                Settings.Default.ShowTradeKillColumn = value;
                Set(() => ShowTradeKillColumn, ref _showTradeKillColumn, value);
            }
        }

        public bool ShowOneKillColumn
        {
            get { return _showOneKillColumn; }
            set
            {
                Settings.Default.ShowOneKillColumn = value;
                Set(() => ShowOneKillColumn, ref _showOneKillColumn, value);
            }
        }

        public bool ShowTwoKillsColumn
        {
            get { return _showTwoKillsColumn; }
            set
            {
                Settings.Default.ShowTwoKillsColumn = value;
                Set(() => ShowTwoKillsColumn, ref _showTwoKillsColumn, value);
            }
        }

        public bool ShowThreeKillsColumn
        {
            get { return _showThreeKillsColumn; }
            set
            {
                Settings.Default.ShowThreeKillsColumn = value;
                Set(() => ShowThreeKillsColumn, ref _showThreeKillsColumn, value);
            }
        }

        public bool ShowFourKillsColumn
        {
            get { return _showFourKillsColumn; }
            set
            {
                Settings.Default.ShowFourKillsColumn = value;
                Set(() => ShowFourKillsColumn, ref _showFourKillsColumn, value);
            }
        }

        public bool ShowFiveKillsColumn
        {
            get { return _showFiveKillsColumn; }
            set
            {
                Settings.Default.ShowFiveKillsColumn = value;
                Set(() => ShowFiveKillsColumn, ref _showFiveKillsColumn, value);
            }
        }

        public bool ShowTotalKillsColumn
        {
            get { return _showTotalKillsColumn; }
            set
            {
                Settings.Default.ShowTotalKillsColumn = value;
                Set(() => ShowTotalKillsColumn, ref _showTotalKillsColumn, value);
            }
        }

        public bool ShowDeathsColumn
        {
            get { return _showDeathsColumn; }
            set
            {
                Settings.Default.ShowDeathsColumn = value;
                Set(() => ShowDeathsColumn, ref _showDeathsColumn, value);
            }
        }

        public bool ShowAssistsColumn
        {
            get { return _showAssistsColumn; }
            set
            {
                Settings.Default.ShowAssistsColumn = value;
                Set(() => ShowAssistsColumn, ref _showAssistsColumn, value);
            }
        }

        public bool ShowHsColumn
        {
            get { return _showHsColumn; }
            set
            {
                Settings.Default.ShowHsColumn = value;
                Set(() => ShowHsColumn, ref _showHsColumn, value);
            }
        }

        public bool ShowKastColumn
        {
            get { return _showKastColumn; }
            set
            {
                Settings.Default.ShowKastColumn = value;
                Set(() => ShowKastColumn, ref _showKastColumn, value);
            }
        }

        public bool ShowKdColumn
        {
            get { return _showKdColumn; }
            set
            {
                Settings.Default.ShowKdColumn = value;
                Set(() => ShowKdColumn, ref _showKdColumn, value);
            }
        }

        public bool ShowMvpColumn
        {
            get { return _showMvpColumn; }
            set
            {
                Settings.Default.ShowMvpColumn = value;
                Set(() => ShowMvpColumn, ref _showMvpColumn, value);
            }
        }

        public bool ShowTkColumn
        {
            get { return _showTkColumn; }
            set
            {
                Settings.Default.ShowTkColumn = value;
                Set(() => ShowTkColumn, ref _showTkColumn, value);
            }
        }

        public bool ShowEkColumn
        {
            get { return _showEkColumn; }
            set
            {
                Settings.Default.ShowEkColumn = value;
                Set(() => ShowEkColumn, ref _showEkColumn, value);
            }
        }

        public bool ShowPlayerScoreColumn
        {
            get { return _showPlayerScoreColumn; }
            set
            {
                Settings.Default.ShowPlayerScoreColumn = value;
                Set(() => ShowPlayerScoreColumn, ref _showPlayerScoreColumn, value);
            }
        }

        public bool ShowEseaRwsColumn
        {
            get { return _showEseaRwsColumn; }
            set
            {
                Settings.Default.ShowEseaRwsColumn = value;
                Set(() => ShowEseaRwsColumn, ref _showEseaRwsColumn, value);
            }
        }

        public bool ShowClutch1v1Column
        {
            get { return _showClutch1v1Column; }
            set
            {
                Settings.Default.ShowClutch1v1Column = value;
                Set(() => ShowClutch1v1Column, ref _showClutch1v1Column, value);
            }
        }

        public bool ShowClutch1v2Column
        {
            get { return _showClutch1v2Column; }
            set
            {
                Settings.Default.ShowClutch1v2Column = value;
                Set(() => ShowClutch1v2Column, ref _showClutch1v2Column, value);
            }
        }

        public bool ShowClutch1v3Column
        {
            get { return _showClutch1v3Column; }
            set
            {
                Settings.Default.ShowClutch1v3Column = value;
                Set(() => ShowClutch1v3Column, ref _showClutch1v3Column, value);
            }
        }

        public bool ShowClutch1v4Column
        {
            get { return _showClutch1v4Column; }
            set
            {
                Settings.Default.ShowClutch1v4Column = value;
                Set(() => ShowClutch1v4Column, ref _showClutch1v4Column, value);
            }
        }

        public bool ShowClutch1v5Column
        {
            get { return _showClutch1v5Column; }
            set
            {
                Settings.Default.ShowClutch1v5Column = value;
                Set(() => ShowClutch1v5Column, ref _showClutch1v5Column, value);
            }
        }

        public bool ShowDurationColumn
        {
            get { return _showDurationColumn; }
            set
            {
                Settings.Default.ShowDurationColumn = value;
                Set(() => ShowDurationColumn, ref _showDurationColumn, value);
            }
        }

        public bool ShowCommentColumn
        {
            get { return _showCommentColumn; }
            set
            {
                Settings.Default.ShowCommentColumn = value;
                Set(() => ShowCommentColumn, ref _showCommentColumn, value);
            }
        }

        public bool ShowClutchCountColumn
        {
            get { return _showClutchCountColumn; }
            set
            {
                Settings.Default.ShowClutchCountColumn = value;
                Set(() => ShowClutchCountColumn, ref _showClutchCountColumn, value);
            }
        }

        public bool ShowTotalDamageHealthColumn
        {
            get { return _showTotalDamageHealthColumn; }
            set
            {
                Settings.Default.ShowTotalDamageHealthColumn = value;
                Set(() => ShowTotalDamageHealthColumn, ref _showTotalDamageHealthColumn, value);
            }
        }

        public bool ShowTotalDamageArmorColumn
        {
            get { return _showTotalDamageArmorColumn; }
            set
            {
                Settings.Default.ShowTotalDamageArmorColumn = value;
                Set(() => ShowTotalDamageArmorColumn, ref _showTotalDamageArmorColumn, value);
            }
        }

        public bool ShowAverageDamageColumn
        {
            get { return _showAverageDamageColumn; }
            set
            {
                Settings.Default.ShowAverageDamageColumn = value;
                Set(() => ShowAverageDamageColumn, ref _showAverageDamageColumn, value);
            }
        }

        public bool ShowBanColumns
        {
            get { return _showBanColumns; }
            set
            {
                Settings.Default.ShowBanColumns = value;
                Set(() => ShowBanColumns, ref _showBanColumns, value);
            }
        }

        public bool ShowKillPerRoundColumn
        {
            get { return _showKillPerRoundColumn; }
            set
            {
                Settings.Default.ShowKillPerRoundColumn = value;
                Set(() => ShowKillPerRoundColumn, ref _showKillPerRoundColumn, value);
            }
        }

        public bool ShowAssistPerRoundColumn
        {
            get { return _showAssistPerRoundColumn; }
            set
            {
                Settings.Default.ShowAssistPerRoundColumn = value;
                Set(() => ShowAssistPerRoundColumn, ref _showAssistPerRoundColumn, value);
            }
        }

        public bool ShowDeathPerRoundColumn
        {
            get { return _showDeathPerRoundColumn; }
            set
            {
                Settings.Default.ShowDeathPerRoundColumn = value;
                Set(() => ShowDeathPerRoundColumn, ref _showDeathPerRoundColumn, value);
            }
        }

        public bool ShowRoundTypeColumn
        {
            get { return _showRoundTypeColumn; }
            set
            {
                Settings.Default.ShowRoundTypeColumn = value;
                Set(() => ShowRoundTypeColumn, ref _showRoundTypeColumn, value);
            }
        }

        public bool ShowSideTroubleColumn
        {
            get { return _showSideTroubleColumn; }
            set
            {
                Settings.Default.ShowSideTroubleColumn = value;
                Set(() => ShowSideTroubleColumn, ref _showSideTroubleColumn, value);
            }
        }

        public bool ShowTeamTroubleColumn
        {
            get { return _showTeamTroubleColumn; }
            set
            {
                Settings.Default.ShowTeamTroubleColumn = value;
                Set(() => ShowTeamTroubleColumn, ref _showTeamTroubleColumn, value);
            }
        }

        public bool ShowTimeDeathColumn
        {
            get { return _showtimeDeathColumn; }
            set
            {
                Settings.Default.ShowTimeDeathColumn = value;
                Set(() => ShowTimeDeathColumn, ref _showtimeDeathColumn, value);
            }
        }

        public bool ShowRoundPlayedColumn
        {
            get { return _showRoundPlayedColumn; }
            set
            {
                Settings.Default.ShowRoundPlayedColumn = value;
                Set(() => ShowRoundPlayedColumn, ref _showRoundPlayedColumn, value);
            }
        }

        public string CsgoExePath
        {
            get { return _csgoExePath; }
            set
            {
                Settings.Default.CsgoExePath = value;
                Set(() => CsgoExePath, ref _csgoExePath, value);
            }
        }

        public string HlaeConfigParentFolderPath
        {
            get { return _hlaeConfigParentFolderPath; }
            set
            {
                Settings.Default.HlaeConfigParentFolderPath = value;
                Set(() => HlaeConfigParentFolderPath, ref _hlaeConfigParentFolderPath, value);
            }
        }

        public string DownloadFolderPath
        {
            get { return _downloadFolderPath; }
            set
            {
                Settings.Default.DownloadFolder = value;
                Set(() => DownloadFolderPath, ref _downloadFolderPath, value);
                _demosService.DownloadFolderPath = value;
            }
        }

        public bool EnableHlae
        {
            get { return _enableHlae; }
            set
            {
                Settings.Default.EnableHlae = value;
                Set(() => EnableHlae, ref _enableHlae, value);
            }
        }

        public bool EnableHlaeConfigParent
        {
            get { return _enableHlaeConfigParent; }
            set
            {
                Settings.Default.EnableHlaeConfigParent = value;
                Set(() => EnableHlaeConfigParent, ref _enableHlaeConfigParent, value);
            }
        }

        public bool LimitStatsSelectedFolder
        {
            get { return _limitStatsSelectedFolder; }
            set
            {
                Settings.Default.LimitStatsFolder = value;
                _cacheService.Filter.Folder = Settings.Default.LimitStatsFolder ? Settings.Default.LastFolder : null;
                Set(() => LimitStatsSelectedFolder, ref _limitStatsSelectedFolder, value);
            }
        }

        public bool IgnoreLaterBan
        {
            get { return _ignoreLaterBan; }
            set
            {
                Settings.Default.IgnoreLaterBan = value;
                Set(() => IgnoreLaterBan, ref _ignoreLaterBan, value);
                _demosService.IgnoreLaterBan = value;
            }
        }

        public long WatchAccountSteamId
        {
            get { return _watchAccountSteamId; }
            set
            {
                Settings.Default.WatchAccountSteamId = value;
                Set(() => WatchAccountSteamId, ref _watchAccountSteamId, value);
            }
        }

        public DateTime DateStatsFrom
        {
            get { return _dateStatsFrom; }
            set
            {
                Settings.Default.DateStatsFrom = value;
                _cacheService.Filter.From = value;
                Set(() => DateStatsFrom, ref _dateStatsFrom, value);
            }
        }

        public DateTime DateStatsTo
        {
            get { return _dateStatsTo; }
            set
            {
                Settings.Default.DateStatsTo = value;
                _cacheService.Filter.To = value;
                Set(() => DateStatsTo, ref _dateStatsTo, value);
            }
        }

        public List<DemoStatus> StatusList
        {
            get { return _statusList; }
            set { Set(() => StatusList, ref _statusList, value); }
        }

        public bool StartBotOnLaunch
        {
            get { return _startBotOnLaunch; }
            set
            {
                Settings.Default.StartBotOnLaunch = value;
                Set(() => StartBotOnLaunch, ref _startBotOnLaunch, value);
            }
        }

        public bool CloseBotOnExit
        {
            get { return _closeBotOnExit; }
            set
            {
                Settings.Default.CloseBotOnExit = value;
                Set(() => CloseBotOnExit, ref _closeBotOnExit, value);
            }
        }

        public long CacheSize
        {
            get { return _cacheSize; }
            set
            {
                Set(() => CacheSize, ref _cacheSize, value);
                RaisePropertyChanged(() => CacheSizeAsString);
            }
        }

        public string CacheSizeAsString =>
            Properties.Resources.CacheSize + ": ~ " + Math.Round(_cacheSize / 1024f / 1024f) + Properties.Resources.MegaByte;

        #endregion

        #region Commands

        /// <summary>
        /// Command to add an account
        /// </summary>
        public RelayCommand AddAccountCommand
        {
            get
            {
                return _addAccountCommand
                       ?? (_addAccountCommand = new RelayCommand(
                           async () =>
                           {
                               // steamid or steam community url link
                               var steamInput = await _dialogService.ShowInputAsync(Properties.Resources.DialogAddAnAccount,
                                   Properties.Resources.DialogEnterSteamId);
                               if (string.IsNullOrEmpty(steamInput))
                               {
                                   return;
                               }

                               Notification = Properties.Resources.NotificationAddingAccount;
                               HasNotification = true;

                               try
                               {
                                   string steamId = await _steamService.GetSteamIdFromUrlOrSteamId(steamInput);
                                   if (!string.IsNullOrEmpty(steamId))
                                   {
                                       Account account = new Account
                                       {
                                           SteamId = steamId,
                                       };
                                       if (AppSettings.IsInternetConnectionAvailable())
                                       {
                                           Suspect player = await _steamService.GetBanStatusForUser(steamId);
                                           account.Name = player.Nickname;
                                       }
                                       else
                                       {
                                           account.Name = steamId;
                                       }

                                       bool added = await _cacheService.AddAccountAsync(account);
                                       if (added)
                                       {
                                           Accounts.Add(account);
                                       }
                                       else
                                       {
                                           await
                                               _dialogService.ShowErrorAsync(Properties.Resources.DialogAccountAlreadyInList,
                                                   MessageDialogStyle.Affirmative);
                                       }
                                   }
                                   else
                                   {
                                       await
                                           _dialogService.ShowErrorAsync(Properties.Resources.DialogErrorInvalidSteamId,
                                               MessageDialogStyle.Affirmative);
                                   }
                               }
                               catch (Exception e)
                               {
                                   Logger.Instance.Log(e);
                                   await
                                       _dialogService.ShowErrorAsync(Properties.Resources.DialogErrorWhileRetrievingPlayerInformation,
                                           MessageDialogStyle.Affirmative);
                               }
                               finally
                               {
                                   HasNotification = false;
                               }
                           }));
            }
        }

        /// <summary>
        /// Command to remove an account
        /// </summary>
        public RelayCommand<Account> RemoveAccountCommand
        {
            get
            {
                return _removeAccountCommand
                       ?? (_removeAccountCommand = new RelayCommand<Account>(
                           async account =>
                           {
                               bool removed = await _cacheService.RemoveAccountAsync(account);
                               if (!removed)
                               {
                                   await _dialogService.ShowErrorAsync(Properties.Resources.DialogErrorRemovingAccount,
                                       MessageDialogStyle.Affirmative);
                                   return;
                               }

                               // if there is no more accounts or the current stats account is removed, we show stats from all accounts
                               if (!Accounts.Any() || SelectedStatsAccount != null && SelectedStatsAccount.SteamId.ToString() == account.SteamId)
                               {
                                   IsShowAllAccounts = true;
                               }

                               // if the current watch account is removed, we reset the displayed watch account
                               if (WatchAccountSteamId.ToString() == account.SteamId)
                               {
                                   SelectedWatchAccount = null;
                               }

                               await _cacheService.RemoveRankInfoAsync(Convert.ToInt64(account.SteamId));
                               Accounts.Remove(account);
                           }, account => SelectedAccount != null));
            }
        }

        /// <summary>
        /// Command to go to the log file
        /// </summary>
        public RelayCommand NavigateToLogFile
        {
            get
            {
                return _navigateToLogFile
                       ?? (_navigateToLogFile = new RelayCommand(
                           async () =>
                           {
                               if (!File.Exists(Logger.Instance.LogFilePath))
                               {
                                   await _dialogService.ShowErrorAsync(Properties.Resources.DialogNoErrorsFile, MessageDialogStyle.Affirmative);
                                   return;
                               }

                               string argument = "/select, \"" + Logger.Instance.LogFilePath + "\"";
                               Process.Start("explorer.exe", argument);
                           }));
            }
        }

        /// <summary>
        /// Command to clear demos data from cache
        /// </summary>
        public RelayCommand ClearDemosDataCacheCommand
        {
            get
            {
                return _clearDemosDataCacheCommand
                       ?? (_clearDemosDataCacheCommand = new RelayCommand(
                           async () =>
                           {
                               var result = await _dialogService.ShowMessageAsync(Properties.Resources.DialogConfirmDeleteDemosData,
                                   MessageDialogStyle.AffirmativeAndNegative);
                               if (result == MessageDialogResult.Negative)
                               {
                                   return;
                               }

                               await _cacheService.ClearDemosFile();
                               await _cacheService.ClearRankInfoAsync();
                               CacheSize = await _cacheService.GetCacheSizeAsync();
                               await _dialogService.ShowMessageAsync(Properties.Resources.DialogDemosDataCleared, MessageDialogStyle.Affirmative);
                           }));
            }
        }

        /// <summary>
        /// Command to export custom data from the cache
        /// </summary>
        public RelayCommand ExportCustomDataCacheCommand
        {
            get
            {
                return _exportCustomDataCacheCommand
                       ?? (_exportCustomDataCacheCommand = new RelayCommand(
                           async () =>
                           {
                               SaveFileDialog saveCustomDataDialog = new SaveFileDialog
                               {
                                   FileName = "backup.json",
                                   Filter = "JSON file (*.json)|*.json",
                               };

                               if (saveCustomDataDialog.ShowDialog() == DialogResult.OK)
                               {
                                   try
                                   {
                                       await _cacheService.CreateBackupCustomDataFile(saveCustomDataDialog.FileName);
                                       await _dialogService.ShowMessageAsync(Properties.Resources.DialogBackupCreated,
                                           MessageDialogStyle.Affirmative);
                                   }
                                   catch (Exception e)
                                   {
                                       Logger.Instance.Log(e);
                                       await _dialogService.ShowErrorAsync(Properties.Resources.DialogErrorCreatingBackupFile,
                                           MessageDialogStyle.Affirmative);
                                   }
                               }
                           }));
            }
        }

        /// <summary>
        /// Command to import custom data to the cache
        /// </summary>
        public RelayCommand ImportCustomDataCacheCommand
        {
            get
            {
                return _importCustomDataCacheCommand
                       ?? (_importCustomDataCacheCommand = new RelayCommand(
                           async () =>
                           {
                               OpenFileDialog fileDialog = new OpenFileDialog
                               {
                                   DefaultExt = ".json",
                                   Filter = "JSON file (.json)|*.json",
                               };

                               bool? result = fileDialog.ShowDialog();

                               if (result == true)
                               {
                                   try
                                   {
                                       Notification = Properties.Resources.NotificationImportingCustomData;
                                       HasNotification = true;
                                       string filename = fileDialog.FileName;

                                       // contains demos with only custom data
                                       List<Demo> demosFromBackup = await _demosService.GetDemosFromBackup(filename);

                                       // Retrieve needed demos information for serialization from headers
                                       List<string> folders = await _cacheService.GetFoldersAsync();
                                       List<Demo> demosHeader = await _demosService.GetDemosHeader(folders);

                                       // Update custom data if the demo has been found
                                       foreach (Demo demo in demosFromBackup)
                                       {
                                           foreach (Demo demoHeader in demosHeader)
                                           {
                                               string oldId = await _demosService.GetOldId(demoHeader);
                                               if (demoHeader.Equals(demo) || demo.Id == oldId)
                                               {
                                                   await _demosService.GetDemoDataAsync(demoHeader);
                                                   demoHeader.Comment = demo.Comment;
                                                   demoHeader.Status = demo.Status;
                                                   await _cacheService.WriteDemoDataCache(demoHeader);
                                                   break;
                                               }
                                           }
                                       }

                                       await _dialogService.ShowMessageAsync(Properties.Resources.DialogCustomDataImported,
                                           MessageDialogStyle.Affirmative);
                                   }
                                   catch (Exception e)
                                   {
                                       Logger.Instance.Log(e);
                                       await _dialogService.ShowErrorAsync(Properties.Resources.DialogErrorImportingCustomData,
                                           MessageDialogStyle.Affirmative);
                                   }
                                   finally
                                   {
                                       HasNotification = false;
                                   }
                               }
                           }));
            }
        }

        /// <summary>
        /// Command to enable HLAE
        /// </summary>
        public RelayCommand EnableHlaeCommand
        {
            get
            {
                return _enableHlaeCommand
                       ?? (_enableHlaeCommand = new RelayCommand(
                           async () =>
                           {
                               string warningMessage = Properties.Resources.DialogEnableHlaeWarning;

                               if (Settings.Default.CsgoExePath == string.Empty)
                               {
                                   warningMessage += Environment.NewLine + Properties.Resources.DialogSelectCsgoPath;
                               }

                               var enable = await _dialogService.ShowMessageAsync(warningMessage, MessageDialogStyle.AffirmativeAndNegative);

                               if (enable == MessageDialogResult.Affirmative)
                               {
                                   bool isCsgoPathSelected = Settings.Default.CsgoExePath != string.Empty;
                                   if (!isCsgoPathSelected)
                                   {
                                       string csgoExecutablePath = _dialogService.ShowSelectCsgoExecutable();
                                       if (string.IsNullOrEmpty(csgoExecutablePath))
                                       {
                                           EnableHlae = false;
                                           return;
                                       }

                                       CsgoExePath = csgoExecutablePath;
                                   }

                                   // check if HLAE is installed and ask to install if it's not
                                   if (!HlaeService.IsHlaeInstalled())
                                   {
                                       var installHlae = await _dialogService.ShowMessageAsync(Properties.Resources.DialogHlaeNotFound,
                                           MessageDialogStyle.AffirmativeAndNegative);
                                       if (installHlae == MessageDialogResult.Affirmative)
                                       {
                                           if (!AppSettings.IsInternetConnectionAvailable())
                                           {
                                               await _dialogService.ShowMessageAsync(Properties.Resources.DialogNoConnexionDetected,
                                                   MessageDialogStyle.Affirmative);
                                               EnableHlae = false;
                                               return;
                                           }

                                           Notification = Properties.Resources.NotificationInstallingHlae;
                                           HasNotification = true;

                                           try
                                           {
                                               await HlaeService.Install();
                                               EnableHlae = true;
                                               await _dialogService.ShowMessageAsync(Properties.Resources.DialogHlaeInstalled, MessageDialogStyle.Affirmative);
                                           }
                                           catch (Exception ex)
                                           {
                                               EnableHlae = false;
                                               Logger.Instance.Log(ex);
                                               string message = ex is InvalidHlaePathException
                                                   ? Properties.Resources.DialogInvalidHlaeExecutablePath
                                                   : Properties.Resources.DialogHlaeInstallationFailed;
                                               await _dialogService.ShowMessageAsync(message, MessageDialogStyle.Affirmative);
                                           }

                                           HasNotification = false;
                                       }
                                       else
                                       {
                                           await _dialogService.ShowMessageAsync(Properties.Resources.DialogHlaeRequired,
                                               MessageDialogStyle.Affirmative);
                                           EnableHlae = false;
                                       }
                                   }
                                   else
                                   {
                                       // check if an HLAE update is available
                                       if (AppSettings.IsInternetConnectionAvailable())
                                       {
                                           bool isUpdateAvailable = await HlaeService.IsUpdateAvailable();
                                           if (isUpdateAvailable)
                                           {
                                               Notification = Properties.Resources.NotificationUpdatingHlae;
                                               HasNotification = true;
                                               try
                                               {
                                                   await HlaeService.Install();
                                                   EnableHlae = true;
                                                   await _dialogService.ShowMessageAsync(Properties.Resources.DialogHlaeUpdated, MessageDialogStyle.Affirmative);
                                               }
                                               catch (Exception ex)
                                               {
                                                   EnableHlae = false;
                                                   Logger.Instance.Log(ex);
                                                   string message = ex is InvalidHlaePathException
                                                       ? Properties.Resources.DialogInvalidHlaeExecutablePath
                                                       : Properties.Resources.DialogHlaeUpdateFailed;
                                                   await _dialogService.ShowMessageAsync(message, MessageDialogStyle.Affirmative);
                                               }

                                               HasNotification = false;
                                           }
                                       }
                                   }
                               }
                               else
                               {
                                   EnableHlae = false;
                               }
                           }));
            }
        }

        /// <summary>
        /// Command to select the csgo.exe location
        /// </summary>
        public RelayCommand SelectCsgoExePathCommand
        {
            get
            {
                return _selectCsgoExePathCommand
                       ?? (_selectCsgoExePathCommand = new RelayCommand(
                           () =>
                           {
                               string csgoExecutablePath = _dialogService.ShowSelectCsgoExecutable();
                               if (!string.IsNullOrEmpty(csgoExecutablePath))
                               {
                                   CsgoExePath = csgoExecutablePath;
                               }
                           }));
            }
        }

        /// <summary>
        /// Command to define the account used for "watch" features
        /// </summary>
        public RelayCommand SetAsWatchAccountCommand
        {
            get
            {
                return _setAsWatchAccountCommand
                       ?? (_setAsWatchAccountCommand = new RelayCommand(
                           () => { SelectedWatchAccount = SelectedAccount; },
                           () => Accounts.Any() && SelectedAccount != null));
            }
        }

        /// <summary>
        /// Command to update the accounts nickname
        /// </summary>
        public RelayCommand SyncAccountsNickname
        {
            get
            {
                return _syncAccountNickname
                       ?? (_syncAccountNickname = new RelayCommand(
                           async () =>
                           {
                               if (!AppSettings.IsInternetConnectionAvailable())
                               {
                                   await _dialogService.ShowNoInternetConnectionAsync();
                                   return;
                               }

                               Notification = Properties.Resources.NotificationSyncingAccountsNickname;
                               HasNotification = true;
                               List<string> steamIdList = Accounts.Select(a => a.SteamId.ToString()).ToList();
                               IEnumerable<Suspect> players = await _steamService.GetBanStatusForUserList(steamIdList);
                               long currentAccountSteamId = Settings.Default.SelectedStatsAccountSteamID;
                               Accounts.Clear();
                               foreach (Suspect player in players)
                               {
                                   Account account = new Account
                                   {
                                       SteamId = player.SteamId,
                                       Name = player.Nickname,
                                   };
                                   await _cacheService.UpdateAccountAsync(account);
                                   Accounts.Add(account);
                               }

                               SelectedStatsAccount = Accounts.FirstOrDefault(a => a.SteamId == currentAccountSteamId.ToString());

                               HasNotification = false;
                           }, () => Accounts.Any()));
            }
        }

        /// <summary>
        /// Command to select the folder where demos will be saved
        /// </summary>
        public RelayCommand SelectDownloadFolderPathCommand
        {
            get
            {
                return _selectDownloadFolderPath
                       ?? (_selectDownloadFolderPath = new RelayCommand(
                           () =>
                           {
                               FolderBrowserDialog folderDialog = new FolderBrowserDialog
                               {
                                   SelectedPath = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)),
                               };

                               DialogResult result = folderDialog.ShowDialog();
                               if (result != DialogResult.OK)
                               {
                                   return;
                               }

                               DownloadFolderPath = Path.GetFullPath(folderDialog.SelectedPath).ToLower();
                           }));
            }
        }

        /// <summary>
        /// Command to delete vdm files
        /// </summary>
        public RelayCommand DeleteVdmFilesCommand
        {
            get
            {
                return _deleteVdmFilesCommand
                       ?? (_deleteVdmFilesCommand = new RelayCommand(
                           async () =>
                           {
                               var confirm = await _dialogService.ShowMessageAsync(Properties.Resources.DialogDeleteVdmFilesConfirmation,
                                   MessageDialogStyle.AffirmativeAndNegative);
                               if (confirm == MessageDialogResult.Affirmative)
                               {
                                   bool result = await _cacheService.DeleteVdmFiles();
                                   if (!result)
                                   {
                                       await _dialogService.ShowErrorAsync(Properties.Resources.DialogErrorDeletingVdmFiles,
                                           MessageDialogStyle.Affirmative);
                                   }
                                   else
                                   {
                                       await _dialogService.ShowMessageAsync(Properties.Resources.DialogVdmFilesDeleted,
                                           MessageDialogStyle.Affirmative);
                                   }
                               }
                           }));
            }
        }

        #endregion

        public SettingsViewModel(
            IDialogService dialogService, ICacheService chacheService, IDemosService demosService,
            ISteamService steamService, IAccountStatsService accountStatsService)
        {
            _dialogService = dialogService;
            _cacheService = chacheService;
            _demosService = demosService;
            _steamService = steamService;
            _accountStatsService = accountStatsService;
            EnableHlaeCustomLocationCommand = new RelayCommand<bool>(async (isChecked) => await EnableHlaeCustomLocation(isChecked));
            UpdateCustomHlaeLocationCommand = new RelayCommand(async () => await UpdateCustomHlaeLocation());
            ResetCustomHlaeLocationCommand = new RelayCommand(ResetHlaeCustomLocation);
            RevealHlaeExecutableCommand = new RelayCommand(async () => await RevealHlaeExecutable());
            EnableHlaeConfigFolderCommand = new RelayCommand(EnableHlaeConfigFolder);
            UpdateHlaeConfigFolderCommand = new RelayCommand(UpdateHlaeConfigFolder);
            EnableFfmpegCustomLocationCommand = new RelayCommand<bool>(async (isChecked) => await EnableFfmpegCustomLocation(isChecked));
            RevealFfmpegExecutableCommand = new RelayCommand(async () => await RevealFfmpegExecutable());
            UpdateCustomFfmpegLocationCommand = new RelayCommand(async () => await UpdateCustomFfmpegLocation());
            ResetCustomFfmpegLocationCommand = new RelayCommand(ResetFfmpegCustomLocation);
            ApplySteamApiKeyCommand = new RelayCommand(ApplySteamApiKey);
            ResetSteamApiKeyCommand = new RelayCommand(ResetSteamApiKey);

            Notification = Properties.Resources.Settings;

            Themes = new List<ComboboxSelector>
            {
                new ComboboxSelector("Dark", Properties.Resources.Dark),
                new ComboboxSelector("Light", Properties.Resources.Light),
            };

            if (IsInDesignMode)
            {
                _dateStatsTo = DateTime.Today.AddDays(30);
                _selectedTheme = new ComboboxSelector("Dark", Properties.Resources.Dark);
            }
            else
            {
                SelectedTheme = Themes.First(t => t.Id == Settings.Default.Theme);
            }

            Languages = new List<ComboboxSelector>();
            foreach (Language language in AppSettings.LANGUAGES.Where(l => l.IsEnabled))
            {
                ComboboxSelector newLanguage = new ComboboxSelector(language.Key, language.Name);
                if (language.Key == Settings.Default.Language)
                {
                    SelectedLanguage = newLanguage;
                }

                Languages.Add(newLanguage);
            }

            MaxConcurrentAnalyzes = new List<ComboboxSelector>();
            for (int number = 1; number <= 8; number++)
            {
                MaxConcurrentAnalyzes.Add(new ComboboxSelector(number.ToString(), number.ToString()));
            }

            SelectedMaxConcurrentAnalyzes = Settings.Default.MaxConcurrentAnalyzes.ToString();

            Application.Current.Dispatcher.Invoke(async () =>
            {
                List<Account> accounts = await _cacheService.GetAccountListAsync();
                Accounts = new ObservableCollection<Account>(accounts);
                StatusList = new List<DemoStatus>(DemoStatus.DefaultStatus);
                SelectedStatsAccount = Accounts.FirstOrDefault(a => a.SteamId == Settings.Default.SelectedStatsAccountSteamID.ToString());
                DownloadFolderPath = Settings.Default.DownloadFolder;
                IgnoreLaterBan = Settings.Default.IgnoreLaterBan;
                CacheSize = await _cacheService.GetCacheSizeAsync();
                _cacheService.Filter.From = Settings.Default.DateStatsFrom;
                _cacheService.Filter.To = Settings.Default.DateStatsTo;
                _cacheService.Filter.Folder = Settings.Default.LimitStatsFolder ? Settings.Default.LastFolder : null;
            });

            Messenger.Default.Register<SettingsFlyoutOpenedMessage>(this, HandleSettingsFlyoutOpenedMessage);
        }

        private async void HandleSettingsFlyoutOpenedMessage(SettingsFlyoutOpenedMessage msg)
        {
            CacheSize = await _cacheService.GetCacheSizeAsync();
        }

        private void RefreshHlaeSettings()
        {
            IsHlaeCustomLocationEnabled = ServicesSettings.Default.IsHlaeCustomLocationEnabled;
            HlaeExecutableLocation = ServicesSettings.Default.HlaeExecutableLocation;
        }

        private async Task EnableHlaeCustomLocation(bool isChecked)
        {
            if (isChecked)
            {
                string executablePath = HlaeExecutableLocation;
                if (executablePath == "")
                {
                    executablePath = _dialogService.ShowSelectHlaeExecutable();
                    if (string.IsNullOrEmpty(executablePath))
                    {
                        IsHlaeCustomLocationEnabled = false;
                        return;
                    }
                }

                try
                {
                    HlaeService.EnableCustomLocation(executablePath);
                }
                catch (Exception ex)
                {
                    await HandleCustomHlaeLocationActivationException(ex);
                }
            }
            else
            {
                HlaeService.DisableCustomLocation();
            }

            RefreshHlaeSettings();
            Messenger.Default.Send(new CustomHlaeLocationChangedMessage());
        }

        private async Task UpdateCustomHlaeLocation()
        {
            string executablePath = _dialogService.ShowSelectHlaeExecutable();
            if (string.IsNullOrEmpty(executablePath))
            {
                return;
            }

            try
            {
                HlaeService.EnableCustomLocation(executablePath);
            }
            catch (Exception ex)
            {
                await HandleCustomHlaeLocationActivationException(ex);
            }

            RefreshHlaeSettings();
            Messenger.Default.Send(new CustomHlaeLocationChangedMessage());
        }

        private void ResetHlaeCustomLocation()
        {
            HlaeService.ResetCustomLocation();
            RefreshHlaeSettings();
            Messenger.Default.Send(new CustomHlaeLocationChangedMessage());
        }

        private async Task HandleCustomHlaeLocationActivationException(Exception ex)
        {
            string message = string.Format(Properties.Resources.UnexpectedErrorOccured, ex.Message);
            if (ex is InvalidHlaeExecutableException)
            {
                message = Properties.Resources.DialogInvalidExecutable;
            }
            else if (ex is FileNotFoundException)
            {
                message = Properties.Resources.DialogExecutableNotFound;
            }
            else if (ex is InvalidHlaePathException)
            {
                message = Properties.Resources.DialogInvalidHlaeExecutablePath;
            }

            await _dialogService.ShowErrorAsync(message, MessageDialogStyle.Affirmative);
        }

        private async Task RevealHlaeExecutable()
        {
            if (!File.Exists(HlaeExecutableLocation))
            {
                await _dialogService.ShowErrorAsync(Properties.Resources.DialogExecutableNotFound, MessageDialogStyle.Affirmative);
                return;
            }

            string argument = "/select, \"" + HlaeExecutableLocation + "\"";
            Process.Start("explorer.exe", argument);
        }

        private void EnableHlaeConfigFolder()
        {
            if (!string.IsNullOrEmpty(Settings.Default.HlaeConfigParentFolderPath))
            {
                EnableHlaeConfigParent = true;
                return;
            }

            string folderPath = _dialogService.ShowSelectFolder();
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            HlaeConfigParentFolderPath = folderPath;
            EnableHlaeConfigParent = true;
        }

        private void UpdateHlaeConfigFolder()
        {
            string folderPath = _dialogService.ShowSelectFolder();
            if (!string.IsNullOrEmpty(folderPath))
            {
                HlaeConfigParentFolderPath = folderPath;
            }
        }

        private async Task EnableFfmpegCustomLocation(bool isChecked)
        {
            if (isChecked)
            {
                string executablePath = FfmpegExecutableLocation;
                if (executablePath == "")
                {
                    executablePath = _dialogService.ShowSelectFfmpegExecutable();
                    if (string.IsNullOrEmpty(executablePath))
                    {
                        IsFfmpegCustomLocationEnabled = false;
                        return;
                    }
                }

                try
                {
                    FFmpegService.EnableCustomLocation(executablePath);
                }
                catch (Exception ex)
                {
                    await HandleCustomFfmpegLocationActivationException(ex);
                }
            }
            else
            {
                FFmpegService.DisableCustomLocation();
            }

            RefreshFfmpegSettings();
            Messenger.Default.Send(new CustomFfmpegLocationChanged());
        }

        private async Task HandleCustomFfmpegLocationActivationException(Exception ex)
        {
            string message = string.Format(Properties.Resources.UnexpectedErrorOccured, ex.Message);
            if (ex is InvalidFfmpegExecutableException)
            {
                message = Properties.Resources.DialogInvalidExecutable;
            }
            else if (ex is FileNotFoundException)
            {
                message = Properties.Resources.DialogExecutableNotFound;
            }

            await _dialogService.ShowErrorAsync(message, MessageDialogStyle.Affirmative);
        }

        private async Task UpdateCustomFfmpegLocation()
        {
            string executablePath = _dialogService.ShowSelectFfmpegExecutable();
            if (string.IsNullOrEmpty(executablePath))
            {
                return;
            }

            try
            {
                FFmpegService.EnableCustomLocation(executablePath);
            }
            catch (Exception ex)
            {
                await HandleCustomFfmpegLocationActivationException(ex);
            }

            RefreshFfmpegSettings();
            Messenger.Default.Send(new CustomFfmpegLocationChanged());
        }

        private async Task RevealFfmpegExecutable()
        {
            if (!File.Exists(FfmpegExecutableLocation))
            {
                await _dialogService.ShowErrorAsync(Properties.Resources.DialogExecutableNotFound, MessageDialogStyle.Affirmative);
                return;
            }

            string argument = "/select, \"" + FfmpegExecutableLocation + "\"";
            Process.Start("explorer.exe", argument);
        }

        private void ResetFfmpegCustomLocation()
        {
            FFmpegService.ResetCustomLocation();
            RefreshFfmpegSettings();
            Messenger.Default.Send(new CustomFfmpegLocationChanged());
        }

        private void RefreshFfmpegSettings()
        {
            IsFfmpegCustomLocationEnabled = ServicesSettings.Default.IsFfmpegCustomLocationEnabled;
            FfmpegExecutableLocation = ServicesSettings.Default.FfmpegExecutableLocation;
        }

        private async void ApplySteamApiKey()
        {
            var url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={SteamApiKey}&steamids=[]";
            var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            var response = client.GetAsync(url).Result;
            if (response.StatusCode != HttpStatusCode.OK)
            {
                await _dialogService.ShowErrorAsync(Properties.Resources.DialogInvalidSteamApiKey, MessageDialogStyle.Affirmative);
            }
            else
            {
                ServicesSettings.Default.SteamApiKey = SteamApiKey;
            }
        }

        private void ResetSteamApiKey()
        {
            ServicesSettings.Default.SteamApiKey = "";
            SteamApiKey = "";
        }
    }
}
