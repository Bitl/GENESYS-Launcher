/*
 * Created by SharpDevelop.
 * User: Bitl
 * Date: 2/9/2019
 * Time: 6:31 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GENESYSLauncher
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		private static string Text = "GENESYS Launcher";
		private static void ValidateHL2S()
		{
			string[] check1 = { "Source SDK Base 2013 Singleplayer" };
			if (!Launcher.CheckFolders(check1))
			{
				string[] check2 = { "Source SDK Base 2013 Multiplayer" };
				if (!Launcher.CheckFolders(check2))
				{
					string[] check3 = { "Half-Life 2" };
					if (!Launcher.CheckFolders(check3))
					{
						MessageBox.Show("You must own and install a copy of Half-Life 2 or the Source SDK 2013 Base Singleplayer or Multiplayer in order to run " + Launcher.CreateGame(Launcher.GameType.HL2S).Name, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else
					{
						Console.WriteLine("FOUND: HL2");
						GlobalVars.HL2SAvail = true;
					}
				}
				else
				{
					Console.WriteLine("FOUND: SDK 2013 MP");
					GlobalVars.HL2SAvail = true;
				}
			}
			else
			{
				Console.WriteLine("FOUND: SDK 2013 SP");
				GlobalVars.HL2SAvail = true;
			}
		}

		private static void ValidateCD()
		{
			string[] check1 = { "Source SDK Base 2013 Singleplayer" };
			if (!Launcher.CheckFolders(check1))
			{
				string[] check2 = { "Source SDK Base 2013 Multiplayer" };
				if (!Launcher.CheckFolders(check2))
				{
					string[] check3 = { "Half-Life 2", "Half-Life 2/episodic", "Half-Life 2/ep2" };
					if (!Launcher.CheckFolders(check3))
					{
						MessageBox.Show("You must own and install a copy of Half-Life 2, Half-Life 2 Episode One, and Half-Life 2 Episode Two, or the Source SDK 2013 Base Singleplayer or Multiplayer in order to run " + Launcher.CreateGame(Launcher.GameType.CyberDiver_Main).Name, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else
					{
						Console.WriteLine("FOUND: HL2 and EPISODES");
						GlobalVars.CDAvail = true;
					}
				}
				else
				{
					Console.WriteLine("FOUND: SDK 2013 MP");
					GlobalVars.CDAvail = true;
				}
			}
			else
			{
				Console.WriteLine("FOUND: SDK 2013 SP");
				GlobalVars.CDAvail = true;
			}
		}

		private static void ValidateL4DS()
		{
			string[] check4 = { "Left 4 Dead 2" };
			if (!Launcher.CheckFolders(check4))
			{
				MessageBox.Show("You must own and install a copy of Left 4 Dead 2 in order to run " + Launcher.CreateGame(Launcher.GameType.L4DS).Name, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				Console.WriteLine("FOUND: L4D2");
				GlobalVars.L4DSAvail = true;
			}
		}

		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			[DllImport("user32.dll")]
			static extern IntPtr SetForegroundWindow(IntPtr hWnd);

			[DllImport("user32.dll")]
            static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

			if (string.IsNullOrWhiteSpace(Settings.ReadString("SteamAppsDir")))
            {
                MessageBox.Show("Please define your steamapps/common folder", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

				using (var fbd = new FolderBrowserDialog())
				{
					DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath) && fbd.SelectedPath.Contains("common"))
                    {
                        Settings.WriteString("SteamAppsDir", fbd.SelectedPath);
					}
					else
                    {
						Process currentProcess = Process.GetCurrentProcess();
						IntPtr hWnd = currentProcess.MainWindowHandle;
						if (hWnd != IntPtr.Zero)
						{
							SetForegroundWindow(hWnd);
							ShowWindow(hWnd, 5);
						}

						MessageBox.Show("Invalid steamapps/common folder. Please relaunch the application and try again.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						Environment.Exit(1);
					}
				}
			}

			if (Launcher.CreateGame(Launcher.GameType.HL2S).ValidateGamePath())
			{
				ValidateHL2S();
			}

			if (Launcher.CreateGame(Launcher.GameType.CyberDiver_v1_00).ValidateGamePath() || Launcher.CreateGame(Launcher.GameType.CyberDiver_v1_20j).ValidateGamePath())
			{
				ValidateCD();
			}

			if (Launcher.CreateGame(Launcher.GameType.L4DS).ValidateGamePath())
			{
				ValidateL4DS();
			}

			if (args.Length == 0)
			{
				Process currentProcess = Process.GetCurrentProcess();
				IntPtr hWnd = currentProcess.MainWindowHandle;
				if (hWnd != IntPtr.Zero)
				{
					SetForegroundWindow(hWnd);
					ShowWindow(hWnd, 5);
				}
				Application.Run(new LauncherForm());
			}
			else
            {
				CommandLineArguments.Arguments CommandLine = new CommandLineArguments.Arguments(args);

				if (CommandLine["hl2ac"] != null)
				{
					if (Launcher.CreateGame(Launcher.GameType.HL2S).ValidateGamePath())
					{
						if (GlobalVars.HL2SAvail)
						{
							Launcher.LaunchGame(Launcher.GameType.HL2S);
						}
					}
					else
					{
						MessageBox.Show("The game cannot be launched because it cannot be found.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
					}

					Application.Exit();
				}
				else if (CommandLine["cdv100"] != null)
				{
					if (Launcher.CreateGame(Launcher.GameType.CyberDiver_v1_00).ValidateGamePath())
					{
						if (GlobalVars.CDAvail)
						{
							Launcher.LaunchGame(Launcher.GameType.CyberDiver_v1_00);
						}
					}
					else
					{
						MessageBox.Show("The game cannot be launched because it cannot be found.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
					}

					Application.Exit();
				}
				else if (CommandLine["cdv120j"] != null)
				{
					if (Launcher.CreateGame(Launcher.GameType.CyberDiver_v1_20j).ValidateGamePath())
					{
						if (GlobalVars.CDAvail)
						{
							Launcher.LaunchGame(Launcher.GameType.CyberDiver_v1_20j);
						}
					}
					else
					{
						MessageBox.Show("The game cannot be launched because it cannot be found.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
					}

					Application.Exit();
				}
				else if (CommandLine["l4ds"] != null)
				{
					if (Launcher.CreateGame(Launcher.GameType.L4DS).ValidateGamePath())
					{
						if (GlobalVars.L4DSAvail)
						{
							Launcher.LaunchGame(Launcher.GameType.L4DS);
						}
					}
					else
					{
						MessageBox.Show("The game cannot be launched because it cannot be found.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
					}

					Application.Exit();
				}
				else if (CommandLine["csneo"] != null)
				{
					if (!string.IsNullOrWhiteSpace(Settings.ReadString("CSNEO_InstallDir")))
					{
						if (Launcher.CreateGame(Launcher.GameType.CSNEO).ValidateGamePath())
						{
							Launcher.LaunchGame(Launcher.GameType.CSNEO);
						}
						else
						{
							MessageBox.Show("The game cannot be launched because it cannot be found.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
						}
					}
					else
					{
						MessageBox.Show("A path has not been defined for CS:NEO. Please load up a path for the game in the launcher.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}

					Application.Exit();
				}
				else if (CommandLine["debug"] != null)
                {
					GlobalVars.isDebug = true;
					Application.Run(new LauncherForm());
				}
			}
		}
	}
}
