﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GENESYSLauncher.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.7.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2.0")]
        public decimal Version {
            get {
                return ((decimal)(this["Version"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CloseWhenGameLaunches {
            get {
                return ((bool)(this["CloseWhenGameLaunches"]));
            }
            set {
                this["CloseWhenGameLaunches"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-sw -game hl2mp -heapsize 512000 -width 1360 -height 768 -windowed -language japa" +
            "nese -ac -io 0 -nesys 0")]
        public string HL2S_LaunchOptions {
            get {
                return ((string)(this["HL2S_LaunchOptions"]));
            }
            set {
                this["HL2S_LaunchOptions"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string HL2S_CustomNESYSHostIP {
            get {
                return ((string)(this["HL2S_CustomNESYSHostIP"]));
            }
            set {
                this["HL2S_CustomNESYSHostIP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool HL2S_CustomNESYSHostIP_Toggle {
            get {
                return ((bool)(this["HL2S_CustomNESYSHostIP_Toggle"]));
            }
            set {
                this["HL2S_CustomNESYSHostIP_Toggle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-sw -game bs09 -heapsize 1024000 -width 1360 -height 768 -noborder -windowed -io " +
            "0 -nesys 0 -language japanese -ac")]
        public string CyberDiver_LaunchOptions {
            get {
                return ((string)(this["CyberDiver_LaunchOptions"]));
            }
            set {
                this["CyberDiver_LaunchOptions"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-arcadeIO_InitializeSkip -game left4dead2 -language japanese -noborder -windowed")]
        public string L4DS_LaunchOptions {
            get {
                return ((string)(this["L4DS_LaunchOptions"]));
            }
            set {
                this["L4DS_LaunchOptions"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool HL2S_ArcadeMenu_Toggle {
            get {
                return ((bool)(this["HL2S_ArcadeMenu_Toggle"]));
            }
            set {
                this["HL2S_ArcadeMenu_Toggle"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("776556647455391766")]
        public long DiscordAppID {
            get {
                return ((long)(this["DiscordAppID"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DiscordIntegration {
            get {
                return ((bool)(this["DiscordIntegration"]));
            }
            set {
                this["DiscordIntegration"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int LastSelectedTabIndex {
            get {
                return ((int)(this["LastSelectedTabIndex"]));
            }
            set {
                this["LastSelectedTabIndex"] = value;
            }
        }
    }
}
