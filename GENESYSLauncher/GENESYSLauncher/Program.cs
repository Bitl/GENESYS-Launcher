/*
 * Created by SharpDevelop.
 * User: Bitl
 * Date: 2/9/2019
 * Time: 6:31 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace GENESYSLauncher
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		private static string Text = "GENESYS Launcher";
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if (args.Length == 0)
			{
				Application.Run(new LauncherForm());
			}
			else
            {
				CommandLineArguments.Arguments CommandLine = new CommandLineArguments.Arguments(args);

				if (CommandLine["hl2ac"] != null)
				{
					if (Launcher.CreateGame(Launcher.GameType.HL2S).ValidateGamePath())
					{
                        if (!Launcher.IsSteamAppInstalled(243730) || !Launcher.IsSteamAppInstalled(243750) || !Launcher.IsSteamAppInstalled(220))
                        {
                            MessageBox.Show("You must own and install a copy of Half-Life 2 or the Source SDK 2013 Base Singleplayer or Multiplayer in order to run " + Launcher.CreateGame(Launcher.GameType.HL2S).Name, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
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
						if (!Launcher.IsSteamAppInstalled(243730) || !Launcher.IsSteamAppInstalled(243750) || (!Launcher.IsSteamAppInstalled(220) && !Launcher.IsSteamAppInstalled(380) && !Launcher.IsSteamAppInstalled(420)))
						{
							MessageBox.Show("You must own and install a copy of Half-Life 2, Half-Life 2 Episode One, and Half-Life 2 Episode Two, or the Source SDK 2013 Base Singleplayer or Multiplayer in order to run " + Launcher.CreateGame(Launcher.GameType.CyberDiver_v1_00).Name, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						else
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
						if (!Launcher.IsSteamAppInstalled(243730) || !Launcher.IsSteamAppInstalled(243750) || (!Launcher.IsSteamAppInstalled(220) && !Launcher.IsSteamAppInstalled(380) && !Launcher.IsSteamAppInstalled(420)))
						{
							MessageBox.Show("You must own and install a copy of Half-Life 2, Half-Life 2 Episode One, and Half-Life 2 Episode Two, or the Source SDK 2013 Base Singleplayer or Multiplayer in order to run " + Launcher.CreateGame(Launcher.GameType.CyberDiver_v1_20j).Name, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						else
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
						if (!Launcher.IsSteamAppInstalled(550))
						{
							MessageBox.Show("You must own and install a copy of Left 4 Dead 2 in order to run " + Launcher.CreateGame(Launcher.GameType.L4DS).Name, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						else
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
