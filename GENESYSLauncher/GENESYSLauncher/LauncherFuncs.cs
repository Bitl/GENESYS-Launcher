/*
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
        }

        public static void WriteBool(string setting, bool value)
        {
            Properties.Settings.Default[setting] = value;
            Properties.Settings.Default.Save();
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
			CyberDiver,
			L4DS
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
					CommandLine = "-sw -game hl2mp -heapsize 512000 -width 1360 -height 768 -windowed " + (Settings.ReadBool("HL2S_ArcadeMenu_Toggle") ? " -ac" : "") + " -io 0 -nesys 0 " + Settings.ReadString("HL2S_LaunchOptions");
                    Info = @"This game runs at a 1360x788 resolution.
If you have a larger monitor resolution, you might need to change your monitor resolution in order to fit this game on your screen properly.
Game Instructions:
To start the game, press F3 2 times on your keyboard.
To navigate the interface, use the arrow keys to move around the interface, and press F2 to select an option.
To exit the game, type 'exit' into the Debug Console window.";
                    Image = "HL2AC_Large";
                    break;
              		
          			case GameType.CyberDiver:
              		Name = "Cyber Diver";
					EXEName = "hl2.exe";
					CommandLine = "-sw -game bs09 -heapsize 1024000 -width 1360 -height 768 -noborder -windowed -ac -io 0 -nesys 0 " + Settings.ReadString("CyberDiver_LaunchOptions");
                    Info = @"This game runs at a 1360x788 resolution.
If you have a larger monitor resolution, you might need to change your monitor resolution in order to fit this game on your screen properly.
Game Instructions:
To start the game, press F3 2 times on your keyboard.
To navigate the interface, use the arrow keys to move around the interface, and press F2 to select an option.
To exit the game, close the game window.";
                    Image = "CD_Large";
                    break;
              		
              		case GameType.L4DS:
              		Name = "Left 4 Dead Survivors";
					EXEName = "left4dead2.exe";
					CommandLine ="-arcadeIO_InitializeSkip -game left4dead2 -language japanese -noborder -windowed " + Settings.ReadString("L4DS_LaunchOptions");
                    Info = @"This game runs at a 1920x1080 resolution by default, but it can be changed with the -w and h launch options.
Game Instructions:
Left 4 Dead Survivors uses an interface that supports Mouse and Keyboard.
To exit the game, close the game window.
Customization Instructions:
To customize your character, use the '-console' launch option, then type the command 'customAvatar_controller' into the console and hit Enter/Return. 
After customizing your character, press 'Reload' then 'Start' to get into the game.";
                    Image = "L4DS_Large";
                    break;
              		
          			default:
              		Name = "";
					EXEName = "";
					CommandLine = "";
                    Info = "";
                    Image = "";
              		break;
      			}
			}

            public string GetGamePath()
            {
                return GlobalVars.GamePath + "\\" + Name + "\\" + EXEName;
            }

            public bool ValidateGamePath()
            {
                return File.Exists(GetGamePath());
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

        public static void LaunchGame(GameType gameToLaunch, Discord.Discord discord)
		{
            var gameClass = CreateGame(gameToLaunch);

            if (gameClass.ValidateGamePath())
            {
                var processInfo = new ProcessStartInfo();
                processInfo.WorkingDirectory = Path.GetDirectoryName(gameClass.GetGamePath());
                processInfo.FileName = gameClass.EXEName;
                processInfo.Arguments = gameClass.CommandLine;
                var proc = Process.Start(processInfo);

                if (File.Exists(GlobalVars.DiscordDllPath))
                {
                    discord = new Discord.Discord(Properties.Settings.Default.DiscordAppID, (System.UInt64)Discord.CreateFlags.Default);
                    var activityManager = discord.GetActivityManager();
                    activityManager.UpdateActivity(Launcher.UpdateRichPresense(gameClass.Type), (res) =>
                    {
                        if (res == Discord.Result.Ok)
                        {
                            Debug.WriteLine("DISCORD: Everything is fine!");
                        }
                    });
                }
            }
		}

        public static long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        public static Discord.Activity UpdateRichPresense(GameType gameToLaunch)
        {
            var gameClass = CreateGame(gameToLaunch);

            var activityTimestamp = new Discord.ActivityTimestamps();
            activityTimestamp.Start = UnixTimeNow();

            var activityAssets = new Discord.ActivityAssets();
            activityAssets.LargeImage = gameClass.Image;
            activityAssets.LargeText = gameClass.Name;

            switch (gameClass.Type)
            {
                case GameType.HL2S:
                case GameType.CyberDiver:
                case GameType.L4DS:
                    return new Discord.Activity
                    {
                        State = "Idle",
                        Details = gameClass.Name + ": In Game",
                        Timestamps = activityTimestamp,
                    };
                case GameType.None:
                default:
                    return new Discord.Activity
                    {
                        State = "Idle",
                        Details = "In Launcher",
                        Timestamps = activityTimestamp,
                    };
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
	}
	
	#endregion
}
