#region Usings
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
#endregion

namespace GENESYSLauncher
{
    #region LauncherForm
    public partial class LauncherForm : Form
    {
		public Thread discordThread;
        public bool continueDiscordThreadLoop;
		public bool init = true;

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
			label20.Text = "v" + Properties.Settings.Default.Version.ToString();
			var versionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
			label21.Text = label20.Text + "." + versionInfo.ProductBuildPart.ToString() + "." + versionInfo.ProductPrivatePart.ToString();
			checkBox5.Checked = Settings.ReadBool("CloseWhenGameLaunches");
			checkBox3.Checked = Settings.ReadBool("DiscordIntegration");
			tabControl1.SelectedIndex = Properties.Settings.Default.LastSelectedTabIndex;
			#endregion

			#region hl2 survivor load settings
			textBox4.Text = Settings.ReadString("HL2S_LaunchOptions");
			checkBox1.Checked = Settings.ReadBool("HL2S_ArcadeMenu_Toggle");
			checkBox2.Checked = Settings.ReadBool("HL2S_CustomNESYSHostIP_Toggle");
			textBox5.Text = Settings.ReadString("HL2S_CustomNESYSHostIP");

			if (checkBox1.Checked == false)
			{
				if (textBox4.Text.Contains(" -ac"))
				{
					textBox4.Text = textBox4.Text.Replace(" -ac", "");
				}
			}
			else if (checkBox1.Checked == true)
			{
				if (!textBox4.Text.Contains(" -ac"))
				{
					textBox4.Text = textBox4.Text + " -ac";
				}
			}
			#endregion

			#region cyberdiver load settings
			textBox7.Text = Settings.ReadString("CyberDiver_LaunchOptions");
			#endregion

			#region l4d survivors load settings
			textBox6.Text = Settings.ReadString("L4DS_LaunchOptions");
			#endregion

#if DEBUG
			button8.Visible = true;
			button9.Visible = true;
			button10.Visible = true;
			button11.Visible = true;
			button12.Visible = true;
#else
			tabControl1.Height = 305;
#endif

			//check for the games.
			bool hl2acAvailable = Launcher.CreateGame(Launcher.GameType.HL2S).ValidateGamePath();
            if (!hl2acAvailable)
            {
                tabControl1.TabPages.Remove(tabPage1);
            }
			else
			{
				if (!Launcher.IsSteamAppInstalled(220))
				{
					MessageBox.Show("You must own and install a copy of Half-Life 2 in order to run " + Launcher.CreateGame(Launcher.GameType.HL2S).Name, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					tabControl1.TabPages.Remove(tabPage1);
				}
			}
			bool cdAvailable = Launcher.CreateGame(Launcher.GameType.CyberDiver).ValidateGamePath();
            if (!cdAvailable)
            {
                tabControl1.TabPages.Remove(tabPage2);
            }
			else
			{
				if (!Launcher.IsSteamAppInstalled(220) && !Launcher.IsSteamAppInstalled(380) && !Launcher.IsSteamAppInstalled(420))
				{
					MessageBox.Show("You must own and install a copy of Half-Life 2, Half-Life 2 Episode One, and Half-Life 2 Episode Two in order to run " + Launcher.CreateGame(Launcher.GameType.CyberDiver).Name, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					tabControl1.TabPages.Remove(tabPage2);
				}
			}
			bool l4dsAvailable = Launcher.CreateGame(Launcher.GameType.L4DS).ValidateGamePath();
            if (!l4dsAvailable)
            {
                tabControl1.TabPages.Remove(tabPage3);
            }
			else
			{
				if (!Launcher.IsSteamAppInstalled(550))
				{
					MessageBox.Show("You must own and install a copy of Left 4 Dead 2 in order to run " + Launcher.CreateGame(Launcher.GameType.L4DS).Name, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					tabControl1.TabPages.Remove(tabPage3);
				}
			}

			if (tabControl1.TabPages.Count <= 0)
			{
				MessageBox.Show("There are no GENESYS games installed. The launcher will now close.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				Close();
			}

			switchImage();
			GlobalVars.isConsole = false;
			init = false;

			try
			{
				// Discord Functionality
				if (File.Exists(GlobalVars.DiscordDllPath) && Properties.Settings.Default.DiscordIntegration)
				{
					Console.WriteLine("DISCORD: Loaded!");
					GlobalVars.discord = new Discord.Discord(Properties.Settings.Default.DiscordAppID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
					GlobalVars.discord.SetLogHook(Discord.LogLevel.Debug, (level, message) =>
					{
						Console.WriteLine("Log[{0}] {1}", level, message);
					});
					Launcher.UpdateActivity(Launcher.GameType.None);

					// Pump the event look to ensure all callbacks continue to get fired.
					//https://stackoverflow.com/questions/17142842/infinite-while-loop-with-form-application-c-sharp
					continueDiscordThreadLoop = true;
					discordThread = new Thread(() =>
					{
						try
						{
							while (continueDiscordThreadLoop)
							{
								GlobalVars.discord.RunCallbacks();
								Thread.Sleep(1000 / 60);
							}
						}
						finally
						{
							GlobalVars.discord.Dispose();
						}
					});
					discordThread.IsBackground = true;
					discordThread.Start();
				}
			}
			catch (Exception)
            {

            }
		}

		//close launcher on launch
		void CheckBox5CheckedChanged(object sender, EventArgs e)
		{
			Settings.WriteBool("CloseWhenGameLaunches", checkBox5.Checked);
		}

		private void checkBox3_CheckedChanged(object sender, EventArgs e)
		{
			if (!init)
			{
				Settings.WriteBool("DiscordIntegration", checkBox3.Checked);
				MessageBox.Show("The launcher will now restart to apply this setting.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				Application.Restart();
			}
		}

		void TabControl1IndexChanged(object sender, EventArgs e)
		{
			switchImage();
			Properties.Settings.Default.LastSelectedTabIndex = tabControl1.SelectedIndex;
			Properties.Settings.Default.Save();
		}

		void switchImage()
        {
			if (tabControl1.SelectedTab == tabPage1)
			{
                pictureBox1.Image = imageList3.Images[0];
			}
			else if (tabControl1.SelectedTab == tabPage2)
			{
				pictureBox1.Image = imageList3.Images[1];
			}
			else if (tabControl1.SelectedTab == tabPage3)
			{
				pictureBox1.Image = imageList3.Images[2];
			}
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
			if (checkBox1.Checked == false)
            {
				if (textBox4.Text.Contains(" -ac"))
				{
					textBox4.Text = textBox4.Text.Replace(" -ac", "");
				}
			}
			else if (checkBox1.Checked == true)
            {
				if (!textBox4.Text.Contains(" -ac"))
				{
					textBox4.Text = textBox4.Text + " -ac";
				}
			}

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
			MethodInvoker mi = delegate () { Launcher.LaunchGame(Launcher.GameType.HL2S); };
			this.Invoke(mi);
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
			MethodInvoker mi = delegate () { Launcher.LaunchGame(Launcher.GameType.CyberDiver); };
			this.Invoke(mi);
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
			MethodInvoker mi = delegate () { Launcher.LaunchGame(Launcher.GameType.L4DS); };
			this.Invoke(mi);
		}
        #endregion

        #region General/Debug

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

		private void button9_Click(object sender, EventArgs e)
		{
			Launcher.LaunchGame_Debug(Launcher.GameType.CyberDiver);
		}

		private void button10_Click(object sender, EventArgs e)
		{
			Launcher.LaunchGame_Debug(Launcher.GameType.HL2S);
		}

		private void button11_Click(object sender, EventArgs e)
		{
			Launcher.LaunchGame_Debug(Launcher.GameType.L4DS);
		}
		private void button12_Click(object sender, EventArgs e)
		{
			Launcher.LaunchGame_Debug(Launcher.GameType.None);
		}
        #endregion
    }
    #endregion
}
