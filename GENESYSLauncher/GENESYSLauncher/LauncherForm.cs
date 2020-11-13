#region Usings
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
#endregion

namespace GENESYSLauncher
{
    #region LauncherForm
    public partial class LauncherForm : Form
    {
		public Discord.Discord discord;

		#region Constructor
		public LauncherForm()
        {
            InitializeComponent();
        }
        #endregion

        #region Core Launcher Code

        //on load
        void MainFormLoad(object sender, EventArgs e)
		{
			#region core launcher settings
			label20.Text = Settings.ReadString("Version");
			var versionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
			label21.Text = Settings.ReadString("Version") + "." + versionInfo.ProductBuildPart.ToString() + "." + versionInfo.ProductPrivatePart.ToString();
			checkBox5.Checked = Settings.ReadBool("CloseWhenGameLaunches");
			#endregion

			#region hl2 survivor load settings
			textBox4.Text = Settings.ReadString("HL2S_LaunchOptions");
			checkBox1.Checked = Settings.ReadBool("HL2S_ArcadeMenu_Toggle");
			checkBox2.Checked = Settings.ReadBool("HL2S_CustomNESYSHostIP_Toggle");
			textBox5.Text = Settings.ReadString("HL2S_CustomNESYSHostIP");
			#endregion

			#region cyberdiver load settings
			textBox7.Text = Settings.ReadString("CyberDiver_LaunchOptions");
			#endregion

			#region l4d survivors load settings
			textBox6.Text = Settings.ReadString("L4DS_LaunchOptions");
			#endregion

#if DEBUG
			button8.Visible = true;
#else
			//check for the games.
			bool hl2acAvailable = Launcher.CreateGame(Launcher.GameType.HL2S).ValidateGamePath();
			if (!hl2acAvailable) { tabControl1.TabPages.Remove(tabPage1); }
			bool cdAvailable = Launcher.CreateGame(Launcher.GameType.CyberDiver).ValidateGamePath();
			if (!cdAvailable) { tabControl1.TabPages.Remove(tabPage2); }
			bool l4dsAvailable = Launcher.CreateGame(Launcher.GameType.L4DS).ValidateGamePath();
			if (!l4dsAvailable) { tabControl1.TabPages.Remove(tabPage3); }

			if (tabControl1.TabPages.Count <= 0)
            {
				MessageBox.Show("There are no GENESYS games installed. The launcher will now close.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				Close();
            }
#endif
			// Discord Functionality
			if (File.Exists(GlobalVars.DiscordDllPath))
			{
				Console.WriteLine("DISCORD: Loaded!");
				discord = new Discord.Discord(Properties.Settings.Default.DiscordAppID, (System.UInt64)Discord.CreateFlags.Default);
				discord.RunCallbacks();
				var activityManager = discord.GetActivityManager();
				activityManager.UpdateActivity(Launcher.UpdateRichPresense(Launcher.GameType.None), (res) =>
				{
					if (res == Discord.Result.Ok)
					{
						Console.WriteLine("DISCORD: Everything is fine!");
					}
					else if (res == Discord.Result.ServiceUnavailable)
					{
						Console.WriteLine("DISCORD: Error when connecting!");
					}
				});
			}
		}

		void MainFormClosing(object sender, FormClosingEventArgs e)
        {
			discord.Dispose();
        }

		//close launcher on launch
		void CheckBox5CheckedChanged(object sender, EventArgs e)
		{
			Settings.WriteBool("CloseWhenGameLaunches", checkBox5.Checked);
		}

		#endregion

		#region HL2 Survivor

		//hl2 survivor launch options
		void TextBox4TextChanged(object sender, EventArgs e)
		{
			Settings.WriteString("HL2S_LaunchOptions", textBox4.Text);
		}

		//hl2 survivor arcade interface toggle
		void CheckBox1CheckedChanged(object sender, EventArgs e)
		{
			Settings.WriteBool("HL2S_ArcadeMenu_Toggle", checkBox1.Checked);
		}

		//hl2 survivor custom NESYS host IP toggle
		void CheckBox2CheckedChanged(object sender, EventArgs e)
		{
			Settings.WriteBool("HL2S_CustomNESYSHostIP_Toggle", checkBox2.Checked);

			if (checkBox2.Checked)
			{
				textBox5.ReadOnly = false;
				textBox5.Enabled = true;
			}
			else
			{
				textBox5.ReadOnly = true;
				textBox5.Enabled = false;
			}
		}

		//hl2 survivor custom NESYS host IP
		void TextBox5TextChanged(object sender, EventArgs e)
		{
			Settings.WriteString("HL2S_CustomNESYSHostIP", textBox5.Text);
		}

		//hl2 survivor nesys host redirection
		void Button3Click(object sender, EventArgs e)
		{
			var status = NESYSRedirection.SetUpNESYSHostRedirection();

			switch (status)
			{
				case NESYSRedirection.NESYSRedirectionStatus.Redirected:
					MessageBox.Show("Host redirection completed!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
					break;
				case NESYSRedirection.NESYSRedirectionStatus.FailedToRedirect:
					MessageBox.Show("ERROR: You have not ran the launcher as administator or the launcher cannot find the hosts file", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;
				case NESYSRedirection.NESYSRedirectionStatus.AlreadyRedirected:
					MessageBox.Show("You already did the host redirection!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
					break;
				default:
					MessageBox.Show("oh look, nothing!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Question);
					break;
			}
		}

		//hl2 survivor launch button
		void Button1Click(object sender, EventArgs e)
		{
			Launcher.LaunchGame(Launcher.GameType.HL2S, discord);

			if (Settings.ReadBool("CloseWhenGameLaunches"))
			{
				Visible = false;
			}
		}

		#endregion

		#region Cyber Diver

		//cyber diver launch options
		void TextBox7TextChanged(object sender, EventArgs e)
		{
			Settings.WriteString("CyberDiver_LaunchOptions", textBox7.Text);
		}

		//cyber diver launch
		void Button6Click(object sender, EventArgs e)
		{
			Launcher.LaunchGame(Launcher.GameType.CyberDiver, discord);

			if (Settings.ReadBool("CloseWhenGameLaunches"))
			{
				Visible = false;
			}
		}

		#endregion

		#region L4D Survivors

		//l4d survivors launch options
		void TextBox6TextChanged(object sender, EventArgs e)
		{
			Settings.WriteString("L4DS_LaunchOptions", textBox6.Text);
		}

		// l4d survivors launch
		void Button7Click(object sender, EventArgs e)
		{
			Launcher.LaunchGame(Launcher.GameType.L4DS, discord);

			if (Settings.ReadBool("CloseWhenGameLaunches"))
			{
				Visible = false;
			}
		}
        #endregion

        #region General

        void Button8Click(object sender, EventArgs e)
        {
			var game = Launcher.CreateGame(Launcher.GameType.HL2S);
			string processPath = game.GetGamePath() + " " + game.CommandLine + (game.ValidateGamePath() ? "" : " (Game unavailable: It cannot be found in the games directory.)");

			var game2 = Launcher.CreateGame(Launcher.GameType.CyberDiver);
			string processPath2 = game2.GetGamePath() + " " + game2.CommandLine + (game2.ValidateGamePath() ? "" : " (Game unavailable: It cannot be found in the games directory.)");

			var game3 = Launcher.CreateGame(Launcher.GameType.L4DS);
			string processPath3 = game3.GetGamePath() + " " + game3.CommandLine + (game3.ValidateGamePath() ? "" : " (Game unavailable: It cannot be found in the games directory.)");

			MessageBox.Show("HL2 Survivor: " + processPath + "\n\nCyber Diver: " + processPath2 + "\n\nL4D Survivors: " + processPath3);
        }

		private void button2_Click(object sender, EventArgs e)
		{
			Launcher.ShowGameInfo(Launcher.GameType.HL2S);
		}

        private void button4_Click(object sender, EventArgs e)
        {
			Launcher.ShowGameInfo(Launcher.GameType.CyberDiver);
		}

        private void button5_Click(object sender, EventArgs e)
        {
			Launcher.ShowGameInfo(Launcher.GameType.L4DS);
		}
        #endregion
    }
    #endregion
}
