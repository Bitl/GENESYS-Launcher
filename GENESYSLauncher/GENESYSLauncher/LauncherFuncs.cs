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
            		bool customiptoggle = Convert.ToBoolean(Settings.ReadVal("HL2S_CustomNESYSHostIP_Toggle"));
            		string customip = Convert.ToString(Settings.ReadVal("HL2S_CustomNESYSHostIP"));
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
		public static string ReadVal(string setting)
		{
			return ConfigurationManager.AppSettings[setting];
		}
		
		public static void WriteVal(string setting, string value)
		{
			Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);

			config.AppSettings.Settings.Remove(setting);
			config.AppSettings.Settings.Add(setting, value);

			config.Save(ConfigurationSaveMode.Modified);
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
			
			public Game()
    		{
				Type = GameType.None;
			}
			
			public void GetGameFromGameType()
			{
				switch (Type)
      			{
          			case GameType.HL2S:
              		Name = "Half-Life 2 Survivor Ver2.0";
					EXEName = "hl2.exe";
					CommandLine = "-sw -game hl2mp -heapsize 512000 -width 1360 -height 768"
						+ (Convert.ToBoolean(Settings.ReadVal("HL2S_ArcadeMenu_Toggle")) ? " -ac" : "") 
						+ " -nesys 0 " 
						+ Settings.ReadVal("HL2S_LaunchOptions");
              		break;
              		
          			case GameType.CyberDiver:
              		Name = "Cyber Diver";
					EXEName = "hl2.exe";
					CommandLine = "-sw -game bs09 -heapsize 1024000 -width 1360 -height 768" 
						+ (Convert.ToBoolean(Settings.ReadVal("CyberDiver_ArcadeMenu_Toggle")) ? " -ac" : "")
						+ " -nesys 0 " 
						+ Settings.ReadVal("CyberDiver_LaunchOptions");
              		break;
              		
              		case GameType.L4DS:
              		Name = "Left 4 Dead Survivors";
					EXEName = "left4dead2.exe";
					CommandLine ="-arcadeIO_InitializeSkip -game left4dead2 -language japanese -noborder -windowed " 
						+ Settings.ReadVal("L4DS_LaunchOptions");
              		break;
              		
          			default:
              		Name = "";
					EXEName = "";
					CommandLine = "";
              		break;
      			}
			}
		}
		
		public static bool LaunchGame(GameType gameToLaunch)
		{
			var gameClass = new Game();
			gameClass.Type = gameToLaunch;
			gameClass.GetGameFromGameType();
			string processPath = GlobalVars.GamePath + "\\" + gameClass.Name + "\\";
			
			var processInfo = new ProcessStartInfo();
			processInfo.WorkingDirectory = Path.GetDirectoryName(processPath);
			processInfo.FileName = gameClass.EXEName;
			processInfo.Arguments = gameClass.CommandLine;
			var proc = Process.Start(processInfo);
			
			if (Convert.ToBoolean(Settings.ReadVal("CloseWhenGameLaunches")))
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
		public static string GamePath = RootPath + "\\games";
	}
	
	#endregion
}
