﻿/*
 * Created by SharpDevelop.
 * User: Bitl
 * Date: 2/9/2019
 * Time: 8:49 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Configuration;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace GENESYSLauncher
{
	#region NESYS Redirection
	
	public static class NESYSRedirection
	{
		public enum NESYSRedirectionStatus
		{
			None,
			AlreadyRedirected,
			FailedToRedirect,
			Redirected
		}
		
		public static NESYSRedirectionStatus SetUpNESYSHostRedirection()
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts");
            string hostsText = File.ReadAllText(path);
            if (UacHelper.IsProcessElevated == true)
            {
            	if (hostsText.Contains("bg3test.cg.taito.co.jp"))
            	{
            		return NESYSRedirectionStatus.AlreadyRedirected;
            	}
            	else
            	{
            		var writer = new StreamWriter(path, true);
            		writer.Write(Environment.NewLine);
            		writer.Write("##Survivor host redirection");
            		writer.Write(Environment.NewLine);
            		bool customiptoggle = Settings.ReadBool("HL2S_CustomNESYSHostIP_Toggle");
            		string customip = Settings.ReadString("HL2S_CustomNESYSHostIP");
            		if (customiptoggle == false)
					{
            			writer.Write("127.0.0.1    bg3test.cg.taito.co.jp");
            		}
            		else
            		{
            			writer.Write(customip + "    bg3test.cg.taito.co.jp");
            		}
            		writer.Dispose();
            		return NESYSRedirectionStatus.Redirected;
           	 	}
            }
            else
            {
            	return NESYSRedirectionStatus.FailedToRedirect;
            }
		}
	}
	
	#endregion
	
	#region Settings Class
	
	public static class Settings
	{
		public static string ReadString(string setting)
		{
            return Properties.Settings.Default[setting].ToString();
		}

        public static bool ReadBool(string setting)
        {
            return Convert.ToBoolean(Properties.Settings.Default[setting]);
        }

        public static void WriteString(string setting, string value)
		{
            Properties.Settings.Default[setting] = value;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        public static void WriteBool(string setting, bool value)
        {
            Properties.Settings.Default[setting] = value;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }
    }

    #endregion

    #region Launcher Class

    public static class Launcher
    {
        public enum GameType
        {
            None,
            HL2S,
            CyberDiver_Main,
            CyberDiver_v1_00,
            CyberDiver_v1_20j,
            L4DS,
            CSNEO
        }

        public class Game
        {
            public string Name { get; set; }
            public string EXEName { get; set; }
            public string CommandLine { get; set; }
            public GameType Type { get; set; }
            public string Info { get; set; }
            public string Image { get; set; }

            public Game()
            {
                Type = GameType.None;
            }

            public void GetGameFromGameType()
            {
                switch (Type)
                {
                    case GameType.HL2S:
                        Name = "Half-Life 2 Survivor";
                        EXEName = "hl2.exe";
                        CommandLine = Settings.ReadString("HL2S_LaunchOptions");
                        Info = @"This game runs at a 1360x788 resolution.
If you have a larger monitor resolution, you might need to change your monitor resolution in order to fit this game on your screen properly.
Game Instructions:
To start the game, press F3 2 times on your keyboard.
To navigate the interface, use the arrow keys to move around the interface, and press F2 to select an option.
To exit the game, type 'exit' into the Debug Console window.";
                        Image = "hl2ac_large";
                        break;

                    case GameType.CyberDiver_Main:
                        Name = "Cyber Diver";
                        EXEName = "";
                        CommandLine = "";
                        Info = @"This game runs at a 1360x788 resolution.
If you have a larger monitor resolution, you might need to change your monitor resolution in order to fit this game on your screen properly.
Game Instructions:
To start the game, press F3 2 times on your keyboard.
To navigate the interface, use the arrow keys to move around the interface, and press F2 to select an option.
To exit the game, close the game window.";
                        Image = "";
                        break;

                    case GameType.CyberDiver_v1_00:
                        Name = "Cyber Diver v1.00";
                        EXEName = "hl2.exe";
                        CommandLine = Settings.ReadString("CyberDiver_LaunchOptions");
                        Info = "";
                        Image = "cd_large";
                        break;

                    case GameType.CyberDiver_v1_20j:
                        Name = "Cyber Diver v1.20j";
                        EXEName = "hl2.exe";
                        CommandLine = Settings.ReadString("CyberDiver_LaunchOptions");
                        Info = "";
                        Image = "cd_large";
                        break;

                    case GameType.L4DS:
                        Name = "Left 4 Dead Survivors";
                        EXEName = "left4dead2.exe";
                        CommandLine = Settings.ReadString("L4DS_LaunchOptions");
                        Info = @"This game runs at a 1920x1080 resolution by default, but it can be changed with the -w and h launch options.
Game Instructions:
Left 4 Dead Survivors uses an interface that supports Mouse and Keyboard.
To exit the game, close the game window.
Customization Instructions:
To customize your character, use the '-console' launch option, then type the command 'customAvatar_controller' into the console and hit Enter/Return. 
After customizing your character, press 'Reload' then 'Start' to get into the game.";
                        Image = "l4ds_large";
                        break;

                    case GameType.CSNEO:
                        Name = "Counter-Strike NEO";
                        EXEName = "TeknoParrotUi.exe";
                        CommandLine = "";
                        Info = @"Once the game has fully loaded, access the console with the ALT+F7 keys, and enter the following command: 'login 13' (without quotes). 
Feel free to change the numbers to whatever you like; 33, 69, 71 and such. 
Click on 'OK' on the prompt and enter any username. 
Then click on the [決定] button, followed by the [はい] button and wait for several seconds. 
The main menu should appear. 
For the sake of simplicity, open the console again with the ALT+F7 key and enter the map (e.g. map neo_06collision). 
Once the map loads, access the console once again and enter the bot_add command to add bots to the team.";
                        Image = "csneo_large";
                        break;

                    default:
                        Name = "Launcher";
                        EXEName = "";
                        CommandLine = "";
                        Info = "";
                        Image = "launcher_large";
                        break;
                }
            }

            public string GetTechnicalName()
            {
                return Name.Replace(" ", "").Replace("-", "").Replace(".", "");
            }

            public string GetGamePath()
            {
                string returnVal = "";
                if (Type != GameType.CSNEO)
                {
                    returnVal = GlobalVars.GamePath + "\\" + Name + "\\" + EXEName;
                }
                else
                {
                    returnVal = Settings.ReadString("CSNEO_InstallDir") + "\\" + EXEName;
                }

                Console.WriteLine(GetTechnicalName() + ".GetGamePath() returns " + returnVal);
                return returnVal;
            }

            public bool ValidateGamePath()
            {
                bool returnVal = File.Exists(GetGamePath());
                Console.WriteLine(GetTechnicalName() + ".ValidateGamePath() returns " + returnVal);
                return returnVal;
            }
        }

        public static Game CreateGame(GameType gameToLaunch)
        {
            var gameClass = new Game();
            gameClass.Type = gameToLaunch;
            gameClass.GetGameFromGameType();
            return gameClass;
        }

        public static void ShowGameInfo(GameType gameType)
        {
            var game = CreateGame(gameType);
            MessageBox.Show(game.Info, game.Name + " Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void UpdateActivity(GameType gameToLaunch)
        {
            try
            {
                if (File.Exists(GlobalVars.DiscordDllPath) && Properties.Settings.Default.DiscordIntegration && GlobalVars.discord != null && !GlobalVars.isConsole)
                {
                    var gameClass = CreateGame(gameToLaunch);
                    var activityManager = GlobalVars.discord.GetActivityManager();

                    string launcherState = "In Game";
                    string launcherDetails = gameClass.Name;
                    string smallImage = "launcher_large";
                    string smallText = launcherState;

                    if (gameClass.Type == GameType.None)
                    {
                        launcherState = "In Launcher";
                        launcherDetails = "";
                        smallImage = "";
                        smallText = "";
                    }

                    var activity = new Discord.Activity
                    {
                        State = launcherState,
                        Details = launcherDetails,
                        Timestamps = {
                            Start = UnixTimeNow()
                        },
                        Assets = {
                            LargeImage = gameClass.Image,
                            LargeText = gameClass.Name,
                            SmallImage = smallImage,
                            SmallText = smallText
                        },
                        Instance = true
                    };

                    activityManager.UpdateActivity(activity, result =>
                    {
                        Console.WriteLine("DISCORD: Rich Presense for " + gameClass.Name);
                        Console.WriteLine("DISCORD: Update Activity: " + result);
                        if (result == Discord.Result.Ok)
                        {
                            Console.WriteLine("DISCORD: Everything is fine!");
                        }
                        else if (result == Discord.Result.InternalError)
                        {
                            Console.WriteLine("DISCORD: Error when connecting!");
                        }
                    });
                }
            }
            catch (Exception)
            {

            }
        }

        public static void LaunchGame(GameType gameToLaunch)
        {
            if (Settings.ReadBool("CloseWhenGameLaunches"))
            {
                foreach (Form form in Application.OpenForms)
                {
                    if (form.GetType() == typeof(LauncherForm))
                    {
                        if (form.Visible == true)
                        {
                            if (form.InvokeRequired)
                            {
                                form.Invoke(new MethodInvoker(delegate
                                {
                                    form.Visible = false;
                                }));
                            }
                        }
                    }
                }
            }

            var gameClass = CreateGame(gameToLaunch);

            if (gameClass.ValidateGamePath())
            {
                try
                {
                    UpdateActivity(gameToLaunch);
                }
                catch (Exception)
                {

                }
                var processInfo = new ProcessStartInfo();
                processInfo.WorkingDirectory = Path.GetDirectoryName(gameClass.GetGamePath());
                processInfo.FileName = gameClass.EXEName;
                processInfo.Arguments = gameClass.CommandLine;
                //add event on process close
                var proc = Process.Start(processInfo);
                proc.EnableRaisingEvents = true;
                proc.Exited += Proc_Exited;
            }
        }

        private static void Proc_Exited(object sender, EventArgs e)
        {
            if (Settings.ReadBool("CloseWhenGameLaunches"))
            {
                foreach (Form form in Application.OpenForms)
                {
                    if (form.GetType() == typeof(LauncherForm))
                    {
                        if (form.Visible == false)
                        {
                            if (form.InvokeRequired)
                            {
                                form.Invoke(new MethodInvoker(delegate
                                {
                                    form.Visible = true;
                                }));
                            }
                        }
                    }
                }
            }

            UpdateActivity(GameType.None);
        }

        public static void LaunchGame_Debug(GameType gameToLaunch)
        {
            var gameClass = CreateGame(gameToLaunch);
            UpdateActivity(gameToLaunch);
        }

        public static long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        public static bool IsSteamAppInstalled(string GameFolder)
        {
            if (Directory.Exists(Settings.ReadString("SteamAppsDir") + "\\" + GameFolder))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckFolders(string[] folders)
        {
            int folderNum = folders.Length;
            int validFolderNum = 0;

            foreach (string s in folders)
            {
                if (IsSteamAppInstalled(s))
                {
                    validFolderNum++;
                }
            }

            if (validFolderNum == folderNum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
	
	#endregion
	
	#region UAC Helper
	
	public static class UacHelper
	{
    private const string uacRegistryKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
    private const string uacRegistryValue = "EnableLUA";

    private static uint STANDARD_RIGHTS_READ = 0x00020000;
    private static uint TOKEN_QUERY = 0x0008;
    private static uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation, uint TokenInformationLength, out uint ReturnLength);

    public enum TOKEN_INFORMATION_CLASS
    {
        TokenUser = 1,
        TokenGroups,
        TokenPrivileges,
        TokenOwner,
        TokenPrimaryGroup,
        TokenDefaultDacl,
        TokenSource,
        TokenType,
        TokenImpersonationLevel,
        TokenStatistics,
        TokenRestrictedSids,
        TokenSessionId,
        TokenGroupsAndPrivileges,
        TokenSessionReference,
        TokenSandBoxInert,
        TokenAuditPolicy,
        TokenOrigin,
        TokenElevationType,
        TokenLinkedToken,
        TokenElevation,
        TokenHasRestrictions,
        TokenAccessInformation,
        TokenVirtualizationAllowed,
        TokenVirtualizationEnabled,
        TokenIntegrityLevel,
        TokenUIAccess,
        TokenMandatoryPolicy,
        TokenLogonSid,
        MaxTokenInfoClass
    }

    public enum TOKEN_ELEVATION_TYPE
    {
        TokenElevationTypeDefault = 1,
        TokenElevationTypeFull,
        TokenElevationTypeLimited
    }

    public static bool IsUacEnabled
    {
        get
        {
            RegistryKey uacKey = Registry.LocalMachine.OpenSubKey(uacRegistryKey, false);
            bool result = uacKey.GetValue(uacRegistryValue).Equals(1);
            return result;
        }
    }

    public static bool IsProcessElevated
    {
        get
        {
            if (IsUacEnabled)
            {
                IntPtr tokenHandle;
                if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_READ, out tokenHandle))
                {
                    throw new ApplicationException("Could not get process token.  Win32 Error Code: " + Marshal.GetLastWin32Error());
                }

                TOKEN_ELEVATION_TYPE elevationResult = TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;

                int elevationResultSize = Marshal.SizeOf((int)elevationResult);
                uint returnedSize = 0;
                IntPtr elevationTypePtr = Marshal.AllocHGlobal(elevationResultSize);

                bool success = GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenElevationType, elevationTypePtr, (uint)elevationResultSize, out returnedSize);
                if (success)
                {
                    elevationResult = (TOKEN_ELEVATION_TYPE)Marshal.ReadInt32(elevationTypePtr);
                    bool isProcessAdmin = elevationResult == TOKEN_ELEVATION_TYPE.TokenElevationTypeFull;
                    return isProcessAdmin;
                }
                else
                {
                    throw new ApplicationException("Unable to determine the current elevation.");
                }
            }
            else
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                bool result = principal.IsInRole(WindowsBuiltInRole.Administrator);
                return result;
            }
        }
    }
	}
	
	#endregion
	
	#region Global Vars
	
	public static class GlobalVars
	{
		public static string RootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string DiscordDllPath = RootPath + "\\" + Discord.Constants.DllName + ".dll";
        public static string GamePath = RootPath + "\\games";
        public static Discord.Discord discord = null;
        public static bool isConsole = true;
        public static bool isDebug = false;
        public static bool CDAvail = false;
        public static bool HL2SAvail = false;
        public static bool L4DSAvail = false;
    }

    #endregion
}
