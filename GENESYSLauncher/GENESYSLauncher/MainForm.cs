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
using System.Drawing;

namespace GENESYSLauncher
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		#region Core Launcher Code
		
		//on load
		void MainFormLoad(object sender, EventArgs e)
		{
			#region core launcher
			label20.Text = Convert.ToString(Settings.ReadVal("Version"));
			label21.Text = Application.ProductVersion;
			checkBox5.Checked = Convert.ToBoolean(Settings.ReadVal("CloseWhenGameLaunches"));
			#endregion
			
			#region hl2 survivor load settings
			textBox4.Text = Settings.ReadVal("HL2S_LaunchOptions");
			checkBox1.Checked = Convert.ToBoolean(Settings.ReadVal("HL2S_ArcadeMenu_Toggle"));
			checkBox2.Checked = Convert.ToBoolean(Settings.ReadVal("HL2S_CustomNESYSHostIP_Toggle"));
			textBox5.Text = Settings.ReadVal("HL2S_CustomNESYSHostIP");
			#endregion
			
			#region cyberdiver load settings
			textBox7.Text = Settings.ReadVal("CyberDiver_LaunchOptions");
			checkBox4.Checked = Convert.ToBoolean(Settings.ReadVal("CyberDiver_ArcadeMenu_Toggle"));
			#endregion
			
			#region l4d survivors load settings
			textBox6.Text = Settings.ReadVal("L4DS_LaunchOptions");
			#endregion
		}
		
		
		//close launcher on launch
		void CheckBox5CheckedChanged(object sender, EventArgs e)
		{
			Settings.WriteVal("CloseWhenGameLaunches",Convert.ToString(checkBox5.Checked));
		}
		
		#endregion
		
		#region HL2 Survivor
		
		//hl2 survivor launch options
		void TextBox4TextChanged(object sender, EventArgs e)
		{
			Settings.WriteVal("HL2S_LaunchOptions",textBox4.Text);
		}
		
		//hl2 survivor arcade interface toggle
		void CheckBox1CheckedChanged(object sender, EventArgs e)
		{
			Settings.WriteVal("HL2S_ArcadeMenu_Toggle",Convert.ToString(checkBox1.Checked));
		}
		
		//hl2 survivor custom NESYS host IP toggle
		void CheckBox2CheckedChanged(object sender, EventArgs e)
		{
			Settings.WriteVal("HL2S_CustomNESYSHostIP_Toggle",Convert.ToString(checkBox2.Checked));
			
			if (checkBox2.Checked)
			{
				textBox5.ReadOnly = true;
				textBox5.Enabled = false;
			}
			else
			{
				textBox5.ReadOnly = false;
				textBox5.Enabled = true;
			}
		}
		
		//hl2 survivor custom NESYS host IP
		void TextBox5TextChanged(object sender, EventArgs e)
		{
			Settings.WriteVal("HL2S_CustomNESYSHostIP",textBox5.Text);
		}
		
		//hl2 survivor nesys host redirection
		void Button3Click(object sender, EventArgs e)
		{
			var status = NESYSRedirection.SetUpNESYSHostRedirection();
			
			switch(status)
			{
				case NESYSRedirection.NESYSRedirectionStatus.Redirected:
					label8.Text = "Host redirection completed!";
            		label8.ForeColor = Color.Lime;
				break;
				case NESYSRedirection.NESYSRedirectionStatus.FailedToRedirect:
					label8.Text = "ERROR: You have not ran the launcher as administator or the launcher cannot find the hosts file.";
            		label8.ForeColor = Color.Red;
				break;
				case NESYSRedirection.NESYSRedirectionStatus.AlreadyRedirected:
					label8.Text = "You already did the host redirection!";
            		label8.ForeColor = Color.Yellow;
				break;
				default:
					label8.Text = "oh look, nothing!";
					var rnd = new Random();
            		label8.ForeColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));;
            	break;
			}
		}
		
		//hl2 survivor game information button
		void Button2Click(object sender, EventArgs e)
		{
			//soontm
		}
		
		//hl2 survivor launch button
		void Button1Click(object sender, EventArgs e)
		{
			bool shouldclose = Launcher.LaunchGame(Launcher.GameType.HL2S);
			
			if(shouldclose)
			{
				Close();
			}
		}
		
		#endregion
		
		#region Cyber Diver
		
		//cyber diver launch options
		void TextBox7TextChanged(object sender, EventArgs e)
		{
			Settings.WriteVal("CyberDiver_LaunchOptions",textBox7.Text);
		}
		
		//cyber diver arcade interface toggle
		void CheckBox4CheckedChanged(object sender, EventArgs e)
		{
			Settings.WriteVal("CyberDiver_ArcadeMenu_Toggle",Convert.ToString(checkBox4.Checked));
		}
		
		//cyber diver game information
		void Button5Click(object sender, EventArgs e)
		{
			//soontm
		}
		
		//cyber diver launch
		void Button6Click(object sender, EventArgs e)
		{
			bool shouldclose = Launcher.LaunchGame(Launcher.GameType.CyberDiver);
			
			if(shouldclose)
			{
				Close();
			}
		}
		
		#endregion
		
		#region L4D Survivors
		
		//l4d survivors launch options
		void TextBox6TextChanged(object sender, EventArgs e)
		{
			Settings.WriteVal("L4DS_LaunchOptions",textBox6.Text);
		}
		
		//l4d survivors game information
		void Button4Click(object sender, EventArgs e)
		{
			//soontm
		}
		
		// l4d survivors launch
		void Button7Click(object sender, EventArgs e)
		{
			bool shouldclose = Launcher.LaunchGame(Launcher.GameType.L4DS);
			
			if(shouldclose)
			{
				Close();
			}
		}
		#endregion
		
		void Button8Click(object sender, EventArgs e)
		{
			var game = new Launcher.Game();
			game.Type = Launcher.GameType.HL2S;
			game.GetGameFromGameType();
			string processPath = GlobalVars.GamePath + "\\" + game.Name + "\\" + game.EXEName;
			MessageBox.Show(processPath + " " + game.CommandLine);
			
			var game2 = new Launcher.Game();
			game2.Type = Launcher.GameType.CyberDiver;
			game2.GetGameFromGameType();
			string processPath2 = GlobalVars.GamePath + "\\" + game2.Name + "\\" + game2.EXEName;
			MessageBox.Show(processPath2 + " " + game2.CommandLine);
			
			var game3 = new Launcher.Game();
			game3.Type = Launcher.GameType.L4DS;
			game3.GetGameFromGameType();
			string processPath3 = GlobalVars.GamePath + "\\" + game3.Name + "\\" + game3.EXEName;
			MessageBox.Show(processPath3 + " " + game3.CommandLine);
		}
	}
}
