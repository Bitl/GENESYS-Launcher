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

            #region csneo load settings
            textBox1.Text = Settings.ReadString("CSNEO_InstallDir");
			checkBox4.Checked = Settings.ReadBool("CSNEO_ShowTab");

			if (!Settings.ReadBool("CSNEO_ShowTab"))
            {
				tabControl1.TabPages.Remove(tabPage4);
			}
			#endregion

			if (GlobalVars.isDebug == true)
			{
				button8.Visible = true;
				button9.Visible = true;
				button10.Visible = true;
				button11.Visible = true;
                button12.Visible = true;
                button17.Visible = true;
				button16.Visible = true;
			}
			else
			{
				tabControl1.Height = 305;
			}

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
					MessageBox.Show("You must own and install a copy of Half-Life 2 in order to run " + Launcher.CreateGame(Launcher.GameType.HL2S).Name, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					tabControl1.TabPages.Remove(tabPage1);
				}
			}

			bool cdv1Available = Launcher.CreateGame(Launcher.GameType.CyberDiver_v1_00).ValidateGamePath();
            if (!cdv1Available)
            {
				bool cdv12Available = Launcher.CreateGame(Launcher.GameType.CyberDiver_v1_20j).ValidateGamePath();
				if (!cdv12Available)
				{
					tabControl1.TabPages.Remove(tabPage2);
				}
				else
				{
					if (!Launcher.IsSteamAppInstalled(220) && !Launcher.IsSteamAppInstalled(380) && !Launcher.IsSteamAppInstalled(420))
					{
						MessageBox.Show("You must own and install a copy of Half-Life 2, Half-Life 2 Episode One, and Half-Life 2 Episode Two in order to run " + Launcher.CreateGame(Launcher.GameType.CyberDiver_v1_20j).Name, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						tabControl1.TabPages.Remove(tabPage2);
					}
				}
            }
			else
			{
				if (!Launcher.IsSteamAppInstalled(220) && !Launcher.IsSteamAppInstalled(380) && !Launcher.IsSteamAppInstalled(420))
				{
					MessageBox.Show("You must own and install a copy of Half-Life 2, Half-Life 2 Episode One, and Half-Life 2 Episode Two in order to run " + Launcher.CreateGame(Launcher.GameType.CyberDiver_v1_00).Name, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
					MessageBox.Show("You must own and install a copy of Left 4 Dead 2 in order to run " + Launcher.CreateGame(Launcher.GameType.L4DS).Name, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					tabControl1.TabPages.Remove(tabPage3);
				}
			}

			if (tabControl1.TabPages.Count <= 0)
			{
				MessageBox.Show("There are no GENESYS games installed. The launcher will now close.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
					// Pump the event look to ensure all callbacks continue to get fired.
					//https://stackoverflow.com/questions/17142842/infinite-while-loop-with-form-application-c-sharp
					continueDiscordThreadLoop = true;
					discordThread = new Thread(() =>
					{
						Console.WriteLine("DISCORD: Loaded!");
						GlobalVars.discord = new Discord.Discord(Properties.Settings.Default.DiscordAppID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
						GlobalVars.discord.SetLogHook(Discord.LogLevel.Debug, (level, message) =>
						{
							Console.WriteLine("Log[{0}] {1}", level, message);
						});
						Launcher.UpdateActivity(Launcher.GameType.None);

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
				MessageBox.Show("The launcher will now restart to apply this setting.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            else if (tabControl1.SelectedTab == tabPage4)
            {
                pictureBox1.Image = imageList3.Images[3];
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
					MessageBox.Show("Host redirection completed!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
					break;
				case NESYSRedirection.NESYSRedirectionStatus.FailedToRedirect:
					MessageBox.Show("ERROR: You have not ran the launcher as administator or the launcher cannot find the hosts file", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;
				case NESYSRedirection.NESYSRedirectionStatus.AlreadyRedirected:
					MessageBox.Show("You already did the host redirection!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
					break;
				default:
					MessageBox.Show("oh look, nothing!", Text, MessageBoxButtons.OK, MessageBoxIcon.Question);
					break;
			}
		}

        //hl2 survivor launch button
        void Button1Click(object sender, EventArgs e)
        {
			if (Launcher.CreateGame(Launcher.GameType.HL2S).ValidateGamePath())
			{
				MethodInvoker mi = delegate () { Launcher.LaunchGame(Launcher.GameType.HL2S); };
				Invoke(mi);
			}
			else
			{
				MessageBox.Show("The game cannot be launched because it cannot be found.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
			}
		}

		private void button10_Click(object sender, EventArgs e)
		{
			Launcher.LaunchGame_Debug(Launcher.GameType.HL2S);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Launcher.ShowGameInfo(Launcher.GameType.HL2S);
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
			bool cdv1Available = Launcher.CreateGame(Launcher.GameType.CyberDiver_v1_00).ValidateGamePath();
			bool cdv12Available = Launcher.CreateGame(Launcher.GameType.CyberDiver_v1_20j).ValidateGamePath();

			if (cdv1Available && cdv12Available)
			{
				var result = MessageBox.Show("The launcher detects that you have both versions of Cyber Diver available. Press yes to launch v1.20j, or press no to launch v1.00.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
				if (result == DialogResult.Yes)
				{
					MethodInvoker mi = delegate () { Launcher.LaunchGame(Launcher.GameType.CyberDiver_v1_20j); };
					Invoke(mi);
				}
				else if (result == DialogResult.No)
				{
					MethodInvoker mi = delegate () { Launcher.LaunchGame(Launcher.GameType.CyberDiver_v1_00); };
					Invoke(mi);
				}
			}
			else
            {
				if (cdv1Available)
                {
					MethodInvoker mi = delegate () { Launcher.LaunchGame(Launcher.GameType.CyberDiver_v1_00); };
					Invoke(mi);
				}
				else if (cdv12Available)
                {
					MethodInvoker mi = delegate () { Launcher.LaunchGame(Launcher.GameType.CyberDiver_v1_20j); };
					Invoke(mi);
				}
				else
                {
					MessageBox.Show("The game cannot be launched because it cannot be found.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
				}
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			Launcher.ShowGameInfo(Launcher.GameType.CyberDiver_Main);
		}

		private void button9_Click(object sender, EventArgs e)
		{
			Launcher.LaunchGame_Debug(Launcher.GameType.CyberDiver_v1_00);
		}

		private void button17_Click(object sender, EventArgs e)
		{
			Launcher.LaunchGame_Debug(Launcher.GameType.CyberDiver_v1_20j);
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
            if (Launcher.CreateGame(Launcher.GameType.L4DS).ValidateGamePath())
            {
                MethodInvoker mi = delegate () { Launcher.LaunchGame(Launcher.GameType.L4DS); };
                Invoke(mi);
            }
			else
			{
				MessageBox.Show("The game cannot be launched because it cannot be found.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			Launcher.ShowGameInfo(Launcher.GameType.L4DS);
		}

		private void button11_Click(object sender, EventArgs e)
		{
			Launcher.LaunchGame_Debug(Launcher.GameType.L4DS);
		}
		#endregion

		#region CS NEO
		private void button15_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Please provide the location to your TeknoParrot folder.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

			using (var fbd = new FolderBrowserDialog())
			{
				DialogResult result = fbd.ShowDialog();

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
                    Settings.WriteString("CSNEO_InstallDir", fbd.SelectedPath);
					textBox1.Text = Settings.ReadString("CSNEO_InstallDir");
				}
			}
		}

		private void button13_Click(object sender, EventArgs e)
		{
			Launcher.ShowGameInfo(Launcher.GameType.CSNEO);
		}

		private void button16_Click(object sender, EventArgs e)
		{
			Launcher.LaunchGame_Debug(Launcher.GameType.CSNEO);
		}

		private void button14_Click(object sender, EventArgs e)
		{
            if (!string.IsNullOrWhiteSpace(Settings.ReadString("CSNEO_InstallDir")))
            {
				if (Launcher.CreateGame(Launcher.GameType.CSNEO).ValidateGamePath())
				{
					MethodInvoker mi = delegate () { Launcher.LaunchGame(Launcher.GameType.CSNEO); };
					Invoke(mi);
				}
				else
				{
					MessageBox.Show("The game cannot be launched because it cannot be found.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
				}
			}
            else
            {
				MessageBox.Show("A path has not been defined for CS:NEO. Please load up a path for the game.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void checkBox4_CheckedChanged(object sender, EventArgs e)
		{
			Settings.WriteBool("CSNEO_ShowTab", checkBox4.Checked);

			if (Settings.ReadBool("CSNEO_ShowTab") && !tabControl1.TabPages.Contains(tabPage4))
			{
				tabControl1.TabPages.Add(tabPage4);
			}
			else if (!Settings.ReadBool("CSNEO_ShowTab") && tabControl1.TabPages.Contains(tabPage4))
			{
				tabControl1.TabPages.Remove(tabPage4);
			}
		}
		#endregion

		#region General/Debug
		void Button8Click(object sender, EventArgs e)
        {
			var game = Launcher.CreateGame(Launcher.GameType.HL2S);
			string processPath = game.GetGamePath() + " " + game.CommandLine + (game.ValidateGamePath() ? "" : " (Game unavailable: It cannot be found in the games directory.)");

			var game2 = Launcher.CreateGame(Launcher.GameType.CyberDiver_v1_00);
            string processPath2 = game2.GetGamePath() + " " + game2.CommandLine + (game2.ValidateGamePath() ? "" : " (Game unavailable: It cannot be found in the games directory.)");

			var game21 = Launcher.CreateGame(Launcher.GameType.CyberDiver_v1_20j);
			string processPath21 = game2.GetGamePath() + " " + game2.CommandLine + (game2.ValidateGamePath() ? "" : " (Game unavailable: It cannot be found in the games directory.)");

			var game3 = Launcher.CreateGame(Launcher.GameType.L4DS);
			string processPath3 = game3.GetGamePath() + " " + game3.CommandLine + (game3.ValidateGamePath() ? "" : " (Game unavailable: It cannot be found in the games directory.)");

			MessageBox.Show("HL2 Survivor: " + processPath + "\n\nCyber Diver v1: " + processPath2 + "\n\nCyber Diver v1.2: " + processPath21 + "\n\nL4D Survivors: " + processPath3);
        }

		private void button12_Click(object sender, EventArgs e)
		{
			Launcher.LaunchGame_Debug(Launcher.GameType.None);
		}
        #endregion
    }
    #endregion
}
