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
					Launcher.LaunchGame(Launcher.GameType.HL2S);
                }
				else if (CommandLine["cd"] != null)
				{
					Launcher.LaunchGame(Launcher.GameType.CyberDiver);
				}
				else if (CommandLine["l4ds"] != null)
				{
					Launcher.LaunchGame(Launcher.GameType.L4DS);
				}

				Application.Exit();
			}
		}
		
	}
}
