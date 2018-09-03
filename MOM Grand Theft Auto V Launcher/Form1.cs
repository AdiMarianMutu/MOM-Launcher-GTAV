using System;
using System.Linq;
using System.Windows.Forms;

using System.IO;
using System.Diagnostics;
using System.Xml;
using Microsoft.Win32;
using System.Windows.Media;
using System.Net;
using System.Text.RegularExpressions;


namespace MOM_Grand_Theft_Auto_V_Launcher {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        #region SCRIPTS
        #region Tab Page
        #region TabPage.Enable
        //EnableTab(tabControl1.TabPages[tabControl1.SelectedIndex = 1], false);
        public static void EnableTab(TabPage page, bool enable) { EnableControls(page.Controls, enable); }
        private static void EnableControls(Control.ControlCollection ctls, bool enable) {
            foreach (Control ctl in ctls) { ctl.Enabled = enable; EnableControls(ctl.Controls, enable); }
        }
        #endregion
        #region TabPage.RemoveDashedRectangle
        private bool skipSelectionChanged = false;
        private void SelectTabWithoutFocus(TabPage tabPage) {
            this.skipSelectionChanged = true;
            var prevFocusedControl = this.ActiveControl;
            if (this.ActiveControl != null) {
                this.tabControl1.SelectedTab = tabPage;
                prevFocusedControl.Focus();
            }
            this.skipSelectionChanged = false;
        }
        #endregion
        #endregion
        #region Enable controls
        private void enableControls(bool enabled) {
            if (enabled == false) {
                update_btn.Enabled = false;
                savechanges_button.Enabled = false;
                groupBox11.Enabled = false;
                steam_radiobutton.Enabled = false;
                retail_radiobutton.Enabled = false;
                ramc_groupbox.Enabled = false;
                ramc_checkunall_btn.Enabled = false;
                groupBox2.Enabled = false;
                backupName_txtbox.Enabled = false;
                makeBackup_button.Enabled = false;
                loadAnotherXML_button.Enabled = false;
                deleteCurrentXML_button.Enabled = false;
                readonly_checkbox.Enabled = false;
                set_fileName_txtbox.Enabled = false;
                applyChanges_button.Enabled = false;
                gtavDir_btn.Enabled = false;
                selfRadio_listb.Enabled = false;
                selfRadioAdd_btn.Enabled = false;
                selfRadioRemove_btn.Enabled = false;
                selfRadioRename_txtbox.Enabled = false;
                selfRadioPlayStop_btn.Enabled = false;
                selfRadioReload_btn.Enabled = false;
                modEnabled_grpb.Enabled = false;
                modDisabled_grpb.Enabled = false;
                modRefreshLists_btn.Enabled = false;
                modDisable_btn.Enabled = false;
                modEnable_btn.Enabled = false;
                modUninstall_btn.Enabled = false;
            } else {
                if (update_btn.Text != "Last version of MOM is already installed!") { update_btn.Enabled = true; }
                savechanges_button.Enabled = true;
                groupBox11.Enabled = true;
                steam_radiobutton.Enabled = true;
                retail_radiobutton.Enabled = true;
                ramc_groupbox.Enabled = true;
                ramc_checkunall_btn.Enabled = true;
                groupBox2.Enabled = true;
                backupName_txtbox.Enabled = true;
                makeBackup_button.Enabled = true;
                loadAnotherXML_button.Enabled = true;
                deleteCurrentXML_button.Enabled = true;
                readonly_checkbox.Enabled = true;
                set_fileName_txtbox.Enabled = true;
                applyChanges_button.Enabled = true;
                gtavDir_btn.Enabled = true;
                selfRadio_listb.Enabled = true;
                selfRadioAdd_btn.Enabled = true;
                selfRadioRemove_btn.Enabled = true;
                selfRadioRename_txtbox.Enabled = true;
                selfRadioPlayStop_btn.Enabled = true;
                selfRadioReload_btn.Enabled = true;
                modEnabled_grpb.Enabled = true;
                modDisabled_grpb.Enabled = true;
                modRefreshLists_btn.Enabled = true;
                if (modDisabledName_listcb.Items.Count > 0) { modDisable_btn.Enabled = true; }
                modEnable_btn.Enabled = true;
                modUninstall_btn.Enabled = true;
            }
        }
        #endregion

        #region [SELF] Check update
        private void selfCheckUpdate(bool updateButtonClick, bool notifyIconClick) {
            try {
                var selfGetFile = new WebClient();
                selfGetFile.DownloadFile("https://www.gta5-mods.com/tools/mom-grand-theft-auto-v-launcher", "selfVersion.temp");

                /* READ FROM THE FILE */
                string selfReadFile_version = File.ReadAllText(Directory.GetCurrentDirectory() + "\\selfVersion.temp");
                Regex selfFindFile_version = new Regex("<span class=\"version\">(.*)</span>");
	            Match match = selfFindFile_version.Match(selfReadFile_version);
                string selfGetNew_version = match.Groups[1].ToString();
                /* ---- */

                var selfUpdateWindow = new Form2();
                //selfToolVersion = "1.0";
                if (selfGetNew_version == selfToolVersion && updateButtonClick == true && notifyIconClick == false) {
                    File.WriteAllText(Directory.GetCurrentDirectory() + @"\updateInfo.temp", selfToolVersion + "\r\n" + selfGetNew_version);
                    selfUpdateWindow.ShowDialog();
                } else if (selfGetNew_version != selfToolVersion && updateButtonClick == false && notifyIconClick == false) {
                    update_noticon.Visible = true;
                    update_noticon.ShowBalloonTip(3500, "MOM Launcher", "New v." + selfGetNew_version + " Available", ToolTipIcon.None);
                } else if (selfGetNew_version != selfToolVersion  && notifyIconClick == true || updateButtonClick == true) {
                    File.WriteAllText(Directory.GetCurrentDirectory() + @"\updateInfo.temp", selfToolVersion + "\r\n" + selfGetNew_version);
                    selfUpdateWindow.ShowDialog();
                }

                File.Delete(Directory.GetCurrentDirectory() + "\\selfVersion.temp");
            } catch { MessageBox.Show("Can't verify the last version of MOM.\r\n(Check your Internet connection)", "Oops! Connection lost...", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }
        #endregion
        #region [SELF] Check 'MOMLauncher.dll' install
        private void selfCheck_dll() {
            if (!Directory.Exists(selfGTAV_dirPath + @"\scripts") && selfFirstLaunch == false) { MessageBox.Show("Please, install 'ScriptHookV' and 'ScriptHookVDotNet'","Critical error!",MessageBoxButtons.OK,MessageBoxIcon.Error); Process.Start("https://it.gta5-mods.com/tools/scripthookv-net"); } else {
                    if (!File.Exists(selfGTAV_dirPath + @"\scripts\MOMLauncher.dll")) {
                        MessageBox.Show("Please put the 'MOMLauncher.dll' mod inside your 'script' folder","MOMLauncher.dll missing",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
        }
        #endregion
        #region [P.P.] CPU Affinity & Integer converter
        private void selfPPCPUAffinity_converter() {
            gta5CPUAffinity_bin = "";
            if (c1_checkbox.Checked == true) { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length, "1"); } else { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length, "0"); }
            if (c2_checkbox.Checked == true) { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length - 1, "1"); } else { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length - 1, "0"); }
            if (c3_checkbox.Checked == true) { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length - 2, "1"); } else { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length - 2, "0"); }
            if (c4_checkbox.Checked == true) { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length - 3, "1"); } else { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length - 3, "0"); }
            if (c5_checkbox.Checked == true) { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length - 4, "1"); } else { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length - 4, "0"); }
            if (c6_checkbox.Checked == true) { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length - 5, "1"); } else { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length - 5, "0"); }
            if (c7_checkbox.Checked == true) { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length - 6, "1"); } else { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length - 6, "0"); }
            if (c8_checkbox.Checked == true) { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length - 7, "1"); } else { gta5CPUAffinity_bin = gta5CPUAffinity_bin.Insert(gta5CPUAffinity_bin.Length - 7, "0"); }
            gta5CPUAffinity_int = 0;
            gta5CPUAffinity_int = Convert.ToInt64(gta5CPUAffinity_bin, 2);
        }
        #endregion
        #region [RAM Cleaner] Get Process Path
        public string selfRAMCleaner_GetProcessPath(string name) {
            Process[] processes = Process.GetProcessesByName(name);

            if (processes.Length > 0) {
                return processes[0].MainModule.FileName;
            } else {
                return string.Empty;
            }
        }
        #endregion
        #region [SELF] Load user settings
        private void selfLoadUserSet() {
            XmlDocument self_file = new XmlDocument();
            self_file.Load(self_filePath);

            XmlNodeList xnl = self_file.SelectNodes("MOM_Grand_Theft_Auto_V_Launcher");
            foreach (XmlNode xn in xnl) {
                selfGTAV_dirPath = xn["GTAV_Directory"].InnerText; 
                gtavdir_txtbox.Text = selfGTAV_dirPath;
                cpupriority_combobox.Text = xn["CPU_Priority"].InnerText;
                gta5CPUAffinity_int = int.Parse(xn["CPU_Affinity_Integer"].InnerText);
                gta5CPUAffinity_bin = xn["CPU_Affinity_Binary"].InnerText;
                selfPP_enableKeySet_txtbox.Text = xn["CPU_P.P._Key"].InnerText;
                stoplauncher_combobox.Text = xn["GTAVLauncherStop"].InnerText;
                autoclose_combobox.Text = xn["SelfStop"].InnerText;
                if (xn["GTAV_Version"].InnerText == "Retail") { retail_radiobutton.Checked = true; } else { steam_radiobutton.Checked = true; }
                if (xn["Notify_Update"].InnerText == "TRUE")  { newUpdate_cbx.Checked = true; } else { newUpdate_cbx.Checked = false; }
                #region RAM Cleaner
                XmlNodeList xnl1 = self_file.SelectNodes("MOM_Grand_Theft_Auto_V_Launcher/RAMCleaner");
                foreach (XmlNode xn1 in xnl1) {
                    if (xn1["explorer.exe"].InnerText == "Checked") { ramc_explorer_checkbox.Checked = true; }
                    if (xn1["chrome.exe"].InnerText == "Checked") { ramc_chrome_checkbox.Checked = true; }
                    if (xn1["Steam.exe"].InnerText == "Checked") { ramc_steam_checkbox.Checked = true; }
                    if (xn1["reader_sl.exe"].InnerText == "Checked") { ramc_reader_sl_checkbox.Checked = true; }
                    if (xn1["jqs.exe"].InnerText == "Checked") { ramc_jqs_checkbox.Checked = true; }
                    if (xn1["AdobeARM.exe"].InnerText == "Checked") { ramc_adobearm_checkbox.Checked = true; }
                    if (xn1["AAMUpdateNotifier.exe"].InnerText == "Checked") { ramc_aamupnotf_checkbox.Checked = true; }
                    if (xn1["Jusched.exe"].InnerText == "Checked") { ramc_jusched_checkbox.Checked = true; }
                    if (xn1["DivXUpdate.exe"].InnerText == "Checked") { ramc_divxupdate_checkbox.Checked = true; }
                    if (xn1["NeroCheck.exe"].InnerText == "Checked") { ramc_nero_checkbox.Checked = true; }
                    if (xn1["HKCMD.exe"].InnerText == "Checked") { ramc_hkcmd_checkbox.Checked = true; }
                    if (xn1["atiptaxx.exe"].InnerText == "Checked") { ramc_ati0_checkbox.Checked = true; }
                    if (xn1["ati2evxx.exe"].InnerText == "Checked") { ramc_ati1_checkbox.Checked = true; }
                    if (xn1["RAVCpl64.exe"].InnerText == "Checked") { ramc_ravcpl_checkbox.Checked = true; }
                    if (xn1["Nwiz.exe"].InnerText == "Checked") { ramc_nwiz_checkbox.Checked = true; }
                    if (xn1["CCC.exe"].InnerText == "Checked") { ramc_ccc_checkbox.Checked = true; }
                    if (xn1["RadeonSettings.exe"].InnerText == "Checked") { ramc_radsett_checkbox.Checked = true; }
                    if (xn1["SearchIndexer.exe"].InnerText == "Checked") { ramc_searchind_checkbox.Checked = true; }
                    if (xn1["SearchUI.exe"].InnerText == "Checked") { ramc_cortana_checkbox.Checked = true; }
                }
                #endregion
            }
        }
        #endregion
        #region [SELF] Create GTA V shortcut
        private void CreateShortcut() {
            string shortcutLocation = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "self_launchgtav" + ".lnk");
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = "MOM Grand Theft Auto V Launcher Shortcut";
            shortcut.TargetPath = gtavdir_txtbox.Text + "\\GTAVLauncher.exe";
            shortcut.WorkingDirectory = Path.GetDirectoryName(gtavdir_txtbox.Text + "\\GTAVLauncher.exe");
            shortcut.Save();
        }
        #endregion
        #region [FIND MODS]
        #region [ENABLED]
        private void selfModsEnabled_find() {
            try {
                #region [MOD FILTER DLL]
                string []selfModFilter_dll = {
                    selfGTAV_dirPath + "\\bink2w64.dll",
                    selfGTAV_dirPath + "\\d3d11.dll",
                    selfGTAV_dirPath + "\\d3dcompiler_46.dll",
                    selfGTAV_dirPath + "\\d3dcsx_46.dll",
                    selfGTAV_dirPath + "\\GFSDK_ShadowLib.win64.dll",
                    selfGTAV_dirPath + "\\GFSDK_TXAA.win64.dll",
                    selfGTAV_dirPath + "\\GFSDK_TXAA_AlphaResolve.win64.dll",
                    selfGTAV_dirPath + "\\socialclub.dll",
                    selfGTAV_dirPath + "\\steam_api_ext64.dll",
                    selfGTAV_dirPath + "\\steam_api64.dll"
                };
                #endregion
                /* gta v folder */
                /* dll */
                if (modEnabledName_listcb.Items.Count > 0) {
                    modEnabledName_listcb.Items.Clear();
                    modEnabledPath_listcb.Items.Clear();
                    modDisable_btn.Enabled = false;
                }

                try
                {
                    foreach (string selfGetModsName_ in Directory.EnumerateFiles(selfGTAV_dirPath, "*.dll")) {
                        for (int i = 0; i <= selfModFilter_dll.Length - 1; i++) {
                            if (selfGetModsName_ != selfModFilter_dll[i] && i == selfModFilter_dll.Length - 1) {
                                string selfMod_ = selfGetModsName_;
                                modEnabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                                modEnabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                            }
                            if (selfGetModsName_ == selfModFilter_dll[i]) { break; }
                        }
                    }
                } catch { }
                /* .rpf */
                try {
                    foreach (string selfGetModsName_ in Directory.EnumerateFiles(selfGTAV_dirPath + @"\mods", "*.rpf", SearchOption.AllDirectories)) {
                        string selfMod_ = selfGetModsName_;
                        modEnabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modEnabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* asi */
                try {
                    foreach (string selfGetModsPath_ in Directory.EnumerateFiles(selfGTAV_dirPath, "*.asi")) {
                        string selfMod_ = selfGetModsPath_;
                        modEnabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modEnabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* scripts folder */
                /* .dll */
                try {
                    foreach (string selfGetModsName_ in Directory.EnumerateFiles(selfGTAV_dirPath + @"\scripts", "*.dll")) {
                        string selfMod_ = selfGetModsName_;
                        modEnabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modEnabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* .cs */
                try {
                    foreach (string selfGetModsPath_ in Directory.EnumerateFiles(selfGTAV_dirPath + @"\scripts", "*.cs")) {
                        string selfMod_ = selfGetModsPath_;
                        modEnabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modEnabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* .vb */
                try {
                    foreach (string selfGetModsPath_ in Directory.EnumerateFiles(selfGTAV_dirPath + @"\scripts", "*.vb")) {
                        string selfMod_ = selfGetModsPath_;
                        modEnabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modEnabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* .lua */
                try {
                    foreach (string selfGetModsPath_ in Directory.EnumerateFiles(selfGTAV_dirPath + @"\scripts", "*.lua")) {
                        string selfMod_ = selfGetModsPath_;
                        modEnabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modEnabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* lua -> addins */
                try {
                    foreach (string selfGetModsPath_ in Directory.EnumerateFiles(selfGTAV_dirPath + @"\scripts\addins", "*.lua")) {
                        string selfMod_ = selfGetModsPath_;
                        modEnabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modEnabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* lua -> libs */
                try {
                    foreach (string selfGetModsPath_ in Directory.EnumerateFiles(selfGTAV_dirPath + @"\scripts\libs", "*.lua")) {
                        string selfMod_ = selfGetModsPath_;
                        modEnabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modEnabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }

                string _pMod;
                for (int i = 0; i < modEnabledPath_listcb.Items.Count; i++) {
                    _pMod = modEnabledPath_listcb.Items[i].ToString();
                    _pMod = _pMod.Replace(selfGTAV_dirPath, "");
                    modEnabledPath_listcb.Items[i] = _pMod;
                }

                modEnabled_grpb.Text = "Mods enabled found [" + modEnabledName_listcb.Items.Count.ToString() + "]";
            } catch { MessageBox.Show("Please verify the Grand Theft Auto V directory path", "MOM can't found any mods installed", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }
        #endregion

        #region [DISABLED]
        private void selfModsDisabled_find() {
            try {
                /* gta v folder */
                /* dll */
                if (modDisabledName_listcb.Items.Count > 0) {
                    modDisabledName_listcb.Items.Clear();
                    modDisabledPath_listcb.Items.Clear();
                    modEnable_btn.Enabled = false;
                }

                try
                {
                    foreach (string selfGetMods_ in Directory.EnumerateFiles(selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]", "*.dll")) {
                        string selfMod_ = selfGetMods_;
                        modDisabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modDisabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* .rpf */
                try {
                    foreach (string selfGetModsName_ in Directory.EnumerateFiles(selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]\\mods", "*.rpf", SearchOption.AllDirectories)) {
                        string selfMod_ = selfGetModsName_;
                        modDisabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modDisabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* asi */
                try {
                    foreach (string selfGetMods_ in Directory.EnumerateFiles(selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]", "*.asi")) {
                        string selfMod_ = selfGetMods_;
                        modDisabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modDisabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* scripts folder */
                /* .dll */
                try {
                    foreach (string selfGetMods_ in Directory.EnumerateFiles(selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]" + @"\scripts", "*.dll")) {
                        string selfMod_ = selfGetMods_;
                        modDisabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modDisabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* .cs */
                try {
                    foreach (string selfGetMods_ in Directory.EnumerateFiles(selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]" + @"\scripts", "*.cs")) {
                        string selfMod_ = selfGetMods_;
                        modDisabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modDisabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* .vb */
                try {
                    foreach (string selfGetMods_ in Directory.EnumerateFiles(selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]" + @"\scripts", "*.vb")) {
                        string selfMod_ = selfGetMods_;
                        modDisabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modDisabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* .lua */
                try {
                    foreach (string selfGetMods_ in Directory.EnumerateFiles(selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]" + @"\scripts", "*.lua")) {
                        string selfMod_ = selfGetMods_;
                        modDisabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modDisabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* lua -> addins */
                try {
                    foreach (string selfGetMods_ in Directory.EnumerateFiles(selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]" + @"\scripts\addins", "*.lua")) {
                        string selfMod_ = selfGetMods_;
                        modDisabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modDisabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }
                /* lua -> libs */
                try {
                    foreach (string selfGetMods_ in Directory.EnumerateFiles(selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]" + @"\scripts\libs", "*.lua")) {
                        string selfMod_ = selfGetMods_;
                        modDisabledName_listcb.Items.Add(selfMod_.Substring(selfMod_.LastIndexOf('\\') + 1));
                        modDisabledPath_listcb.Items.Add(selfMod_.Substring(0, selfMod_.LastIndexOf('\\') + 1));
                    }
                } catch { }

                string _pMod;
                for (int i = 0; i < modDisabledPath_listcb.Items.Count; i++) {
                    _pMod = modDisabledPath_listcb.Items[i].ToString();
                    _pMod = _pMod.Replace(selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]", "");
                    modDisabledPath_listcb.Items[i] = _pMod;
                }

                modDisabled_grpb.Text = "Mods disabled found [" + modDisabledName_listcb.Items.Count.ToString() + "]";
            } catch { MessageBox.Show("Please verify the Grand Theft Auto V directory path", "MOM can't found any mods installed", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }
        #endregion
        #endregion
        #region [SETTINGS.XML TAB] Load settings.xml
        private void set_loadFile(string set_filePath) {
            try {
                XmlDocument set_file = new XmlDocument();
                set_file.Load(set_filePath);

            
                #region <GRAPHICS>
                XmlNode set_fileN0 = set_file.SelectSingleNode("Settings/graphics/Tessellation");
                set_tesselation_numeric.Value = Convert.ToDecimal(set_fileN0.Attributes["value"].Value);

                XmlNode set_fileN1 = set_file.SelectSingleNode("Settings/graphics/LodScale");
                set_lodscale_numeric.Value = Convert.ToDecimal(set_fileN1.Attributes["value"].Value);

                XmlNode set_fileN2 = set_file.SelectSingleNode("Settings/graphics/PedLodBias");
                set_pedlod_numeric.Value = Convert.ToDecimal(set_fileN2.Attributes["value"].Value);

                XmlNode set_fileN3 = set_file.SelectSingleNode("Settings/graphics/VehicleLodBias");
                set_carlod_numeric.Value = Convert.ToDecimal(set_fileN3.Attributes["value"].Value);

                XmlNode set_fileN4 = set_file.SelectSingleNode("Settings/graphics/ShadowQuality");
                set_shadow_numeric.Value = Convert.ToDecimal(set_fileN4.Attributes["value"].Value);

                XmlNode set_fileN5 = set_file.SelectSingleNode("Settings/graphics/ReflectionQuality");
                set_reflqual_numeric.Value = Convert.ToDecimal(set_fileN5.Attributes["value"].Value);

                XmlNode set_fileN6 = set_file.SelectSingleNode("Settings/graphics/ReflectionMSAA");
                set_reflmsaa_numeric.Value = Convert.ToDecimal(set_fileN6.Attributes["value"].Value);

                XmlNode set_fileN7 = set_file.SelectSingleNode("Settings/graphics/SSAO");
                set_ssao_numeric.Value = Convert.ToDecimal(set_fileN7.Attributes["value"].Value);

                XmlNode set_fileN8 = set_file.SelectSingleNode("Settings/graphics/AnisotropicFiltering");
                set_anisofil_numeric.Value = Convert.ToDecimal(set_fileN8.Attributes["value"].Value);

                XmlNode set_fileN9 = set_file.SelectSingleNode("Settings/graphics/MSAA");
                set_msaa_numeric.Value = Convert.ToDecimal(set_fileN9.Attributes["value"].Value);

                XmlNode set_fileN10 = set_file.SelectSingleNode("Settings/graphics/MSAAFragments");
                set_msaafrag_numeric.Value = Convert.ToDecimal(set_fileN10.Attributes["value"].Value);

                XmlNode set_fileN11 = set_file.SelectSingleNode("Settings/graphics/MSAAQuality");
                set_msaaqual_numeric.Value = Convert.ToDecimal(set_fileN11.Attributes["value"].Value);
           
                XmlNode set_fileN12 = set_file.SelectSingleNode("Settings/graphics/TextureQuality");
                set_texturequal_numeric.Value = Convert.ToDecimal(set_fileN12.Attributes["value"].Value);
 
                XmlNode set_fileN13 = set_file.SelectSingleNode("Settings/graphics/ParticleQuality");
                set_partqual_numeric.Value = Convert.ToDecimal(set_fileN13.Attributes["value"].Value);

                XmlNode set_fileN14 = set_file.SelectSingleNode("Settings/graphics/WaterQuality");
                set_waterqual_numeric.Value = Convert.ToDecimal(set_fileN14.Attributes["value"].Value);

                XmlNode set_fileN15 = set_file.SelectSingleNode("Settings/graphics/GrassQuality");
                set_grassqual_numeric.Value = Convert.ToDecimal(set_fileN15.Attributes["value"].Value);

                XmlNode set_fileN16 = set_file.SelectSingleNode("Settings/graphics/ShaderQuality");
                set_shadqual_numeric.Value = Convert.ToDecimal(set_fileN16.Attributes["value"].Value);

                XmlNode set_fileN17 = set_file.SelectSingleNode("Settings/graphics/Shadow_SoftShadows");
                set_shadsoft_numeric.Value = Convert.ToDecimal(set_fileN17.Attributes["value"].Value);

                XmlNode set_fileN18 = set_file.SelectSingleNode("Settings/graphics/UltraShadows_Enabled");
                set_ultrashad_combobox.Text = set_fileN18.Attributes["value"].InnerText;

                XmlNode set_fileN19 = set_file.SelectSingleNode("Settings/graphics/Shadow_ParticleShadows");
                set_shadpart_combobox.Text = set_fileN19.Attributes["value"].InnerText;

                XmlNode set_fileN20 = set_file.SelectSingleNode("Settings/graphics/Shadow_Distance");
                set_shaddist_numeric.Value = Convert.ToDecimal(set_fileN20.Attributes["value"].Value);

                XmlNode set_fileN21 = set_file.SelectSingleNode("Settings/graphics/Shadow_LongShadows");
                set_shadlong_combobox.Text = set_fileN21.Attributes["value"].InnerText;

                XmlNode set_fileN22 = set_file.SelectSingleNode("Settings/graphics/Shadow_SplitZStart");
                set_shadsplitzstart_numeric.Value = Convert.ToDecimal(set_fileN22.Attributes["value"].Value);

                XmlNode set_fileN23 = set_file.SelectSingleNode("Settings/graphics/Shadow_SplitZEnd");
                set_shadsplitzend_numeric.Value = Convert.ToDecimal(set_fileN23.Attributes["value"].Value);

                XmlNode set_fileN24 = set_file.SelectSingleNode("Settings/graphics/Shadow_aircraftExpWeight");
                set_shadaircraft_numeric.Value = Convert.ToDecimal(set_fileN24.Attributes["value"].Value);

                XmlNode set_fileN25 = set_file.SelectSingleNode("Settings/graphics/Shadow_DisableScreenSizeCheck");
                set_shaddisables_combobox.Text = set_fileN25.Attributes["value"].InnerText;

                XmlNode set_fileN26 = set_file.SelectSingleNode("Settings/graphics/Reflection_MipBlur");
                set_reflmipblur_combobox.Text = set_fileN26.Attributes["value"].InnerText;

                XmlNode set_fileN27 = set_file.SelectSingleNode("Settings/graphics/FXAA_Enabled");
                set_fxaa_combobox.Text = set_fileN27.Attributes["value"].InnerText;

                XmlNode set_fileN28 = set_file.SelectSingleNode("Settings/graphics/TXAA_Enabled");
                set_txaa_combobox.Text = set_fileN28.Attributes["value"].InnerText;

                XmlNode set_fileN29 = set_file.SelectSingleNode("Settings/graphics/Lighting_FogVolumes");
                set_lighfog_combobox.Text = set_fileN29.Attributes["value"].InnerText;
 
                XmlNode set_fileN30 = set_file.SelectSingleNode("Settings/graphics/Shader_SSA");
                set_shadssa_combobox.Text = set_fileN30.Attributes["value"].InnerText;

                XmlNode set_fileN31 = set_file.SelectSingleNode("Settings/graphics/DX_Version");
                set_dxver_numeric.Value = Convert.ToDecimal(set_fileN31.Attributes["value"].Value);

                XmlNode set_fileN32 = set_file.SelectSingleNode("Settings/graphics/CityDensity");
                set_citydens_numeric.Value = Convert.ToDecimal(set_fileN32.Attributes["value"].Value);

                XmlNode set_fileN33 = set_file.SelectSingleNode("Settings/graphics/PedVarietyMultiplier");
                set_pedvar_numeric.Value = Convert.ToDecimal(set_fileN33.Attributes["value"].Value);

                XmlNode set_fileN34 = set_file.SelectSingleNode("Settings/graphics/VehicleVarietyMultiplier");
                set_vehicvar_numeric.Value = Convert.ToDecimal(set_fileN34.Attributes["value"].Value);

                XmlNode set_fileN35 = set_file.SelectSingleNode("Settings/graphics/PostFX");
                set_postfx_numeric.Value = Convert.ToDecimal(set_fileN35.Attributes["value"].Value);

                XmlNode set_fileN36 = set_file.SelectSingleNode("Settings/graphics/DoF");
                set_dof_combobox.Text = set_fileN36.Attributes["value"].InnerText;

                XmlNode set_fileN37 = set_file.SelectSingleNode("Settings/graphics/HdStreamingInFlight");
                set_hdstream_combobox.Text = set_fileN37.Attributes["value"].InnerText;

                XmlNode set_fileN38 = set_file.SelectSingleNode("Settings/graphics/MaxLodScale");
                set_maxlod_numeric.Value = Convert.ToDecimal(set_fileN38.Attributes["value"].Value);

                XmlNode set_fileN39 = set_file.SelectSingleNode("Settings/graphics/MotionBlurStrength");
                set_motionblur_numeric.Value = Convert.ToDecimal(set_fileN39.Attributes["value"].Value);
            #endregion
                #region <SYSTEM>
                XmlNode set_fileN40 = set_file.SelectSingleNode("Settings/system/numBytesPerReplayBlock");
                set_bytesperreplay_numeric.Value = Convert.ToDecimal(set_fileN40.Attributes["value"].Value);

                XmlNode set_fileN41 = set_file.SelectSingleNode("Settings/system/numReplayBlocks");
                set_bytesreplay_numeric.Value = Convert.ToDecimal(set_fileN41.Attributes["value"].Value);

                XmlNode set_fileN42 = set_file.SelectSingleNode("Settings/system/maxSizeOfStreamingReplay");
                set_maxstream_numeric.Value = Convert.ToDecimal(set_fileN42.Attributes["value"].Value);

                XmlNode set_fileN43 = set_file.SelectSingleNode("Settings/system/maxFileStoreSize");
                set_maxfile_numeric.Value = Convert.ToDecimal(set_fileN43.Attributes["value"].Value);
            #endregion
                XmlNode set_fileN44 = set_file.SelectSingleNode("Settings/audio/Audio3d");
                set_audio3d_combobox.Text = set_fileN44.Attributes["value"].InnerText;
                #region <VIDEO>
                XmlNode set_fileN45 = set_file.SelectSingleNode("Settings/video/AdapterIndex");
                set_adapterind_numeric.Value = Convert.ToDecimal(set_fileN45.Attributes["value"].Value);

                XmlNode set_fileN46 = set_file.SelectSingleNode("Settings/video/OutputIndex");
                set_outputind_numeric.Value = Convert.ToDecimal(set_fileN46.Attributes["value"].Value);

                XmlNode set_fileN47 = set_file.SelectSingleNode("Settings/video/ScreenWidth");
                set_screenw_numeric.Value = Convert.ToDecimal(set_fileN47.Attributes["value"].Value);

                XmlNode set_fileN48 = set_file.SelectSingleNode("Settings/video/ScreenHeight");
                set_screenh_numeric.Value = Convert.ToDecimal(set_fileN48.Attributes["value"].Value);

                XmlNode set_fileN49 = set_file.SelectSingleNode("Settings/video/RefreshRate");
                set_refreshrate_numeric.Value = Convert.ToDecimal(set_fileN49.Attributes["value"].Value);

                XmlNode set_fileN50 = set_file.SelectSingleNode("Settings/video/Windowed");
                set_wind_numeric.Value = Convert.ToDecimal(set_fileN50.Attributes["value"].Value);

                XmlNode set_fileN51 = set_file.SelectSingleNode("Settings/video/VSync");
                set_vsync_numeric.Value = Convert.ToDecimal(set_fileN51.Attributes["value"].Value);

                XmlNode set_fileN52 = set_file.SelectSingleNode("Settings/video/Stereo");
                set_stereo_numeric.Value = Convert.ToDecimal(set_fileN52.Attributes["value"].Value);

                XmlNode set_fileN53 = set_file.SelectSingleNode("Settings/video/Convergence");
                set_conv_numeric.Value = Convert.ToDecimal(set_fileN53.Attributes["value"].Value);

                XmlNode set_fileN54 = set_file.SelectSingleNode("Settings/video/Separation");
                set_separation_numeric.Value = Convert.ToDecimal(set_fileN54.Attributes["value"].Value);

                XmlNode set_fileN55 = set_file.SelectSingleNode("Settings/video/PauseOnFocusLoss");
                set_pausefoc_numeric.Value = Convert.ToDecimal(set_fileN55.Attributes["value"].Value);

                XmlNode set_fileN56 = set_file.SelectSingleNode("Settings/video/AspectRatio");
                set_aspectratio_numeric.Value = Convert.ToDecimal(set_fileN56.Attributes["value"].Value);
                #endregion
                XmlNode set_fileN57 = set_file.SelectSingleNode("Settings/VideoCardDescription");
                set_videocard_texbox.Text = set_fileN57.InnerText;

                FileAttributes attributes = File.GetAttributes(set_filePath);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) { readonly_checkbox.Checked = true; } else { readonly_checkbox.Checked = false; }
                
                if (set_firstLaunch == false) {
                    MessageBox.Show("File successfully loaded!");
                }
                makeBackup_button.Enabled = true;
                deleteCurrentXML_button.Enabled = true;
                applyChanges_button.Enabled = true;
                set_fileName_txtbox.Text = set_filePath.Remove(0, set_filePath.LastIndexOf("\\") + 1);
            } catch {
                makeBackup_button.Enabled = false;
                applyChanges_button.Enabled = false;
                deleteCurrentXML_button.Enabled = false;
                set_fileName_txtbox.Text = "NO FILE";
                MessageBox.Show("This isn't a GTA V 'settings.xml' file!", "Can't load this file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            set_firstLaunch = false;
        }
        #endregion
        #region [SETTINGS.XML TAB] Save settings.xml
        private void set_saveFile(string set_filePathXML) {
            try {
                XmlDocument set_file = new XmlDocument();
                set_file.Load(set_filePathXML);


                #region <GRAPHICS>
                XmlNode set_fileN0 = set_file.SelectSingleNode("Settings/graphics/Tessellation");
                set_fileN0.Attributes["value"].InnerText = Convert.ToString(set_tesselation_numeric.Value);

                XmlNode set_fileN1 = set_file.SelectSingleNode("Settings/graphics/LodScale");
                set_fileN1.Attributes["value"].InnerText = Convert.ToString(set_lodscale_numeric.Value);

                XmlNode set_fileN2 = set_file.SelectSingleNode("Settings/graphics/PedLodBias");
                set_fileN2.Attributes["value"].InnerText = Convert.ToString(set_pedlod_numeric.Value);

                XmlNode set_fileN3 = set_file.SelectSingleNode("Settings/graphics/VehicleLodBias");
                set_fileN3.Attributes["value"].InnerText = Convert.ToString(set_carlod_numeric.Value);

                XmlNode set_fileN4 = set_file.SelectSingleNode("Settings/graphics/ShadowQuality");
                set_fileN4.Attributes["value"].InnerText = Convert.ToString(set_shadow_numeric.Value);

                XmlNode set_fileN5 = set_file.SelectSingleNode("Settings/graphics/ReflectionQuality");
                set_fileN5.Attributes["value"].InnerText = Convert.ToString(set_reflqual_numeric.Value);

                XmlNode set_fileN6 = set_file.SelectSingleNode("Settings/graphics/ReflectionMSAA");
                set_fileN6.Attributes["value"].InnerText = Convert.ToString(set_reflmsaa_numeric.Value);

                XmlNode set_fileN7 = set_file.SelectSingleNode("Settings/graphics/SSAO");
                set_fileN7.Attributes["value"].InnerText = Convert.ToString(set_ssao_numeric.Value);

                XmlNode set_fileN8 = set_file.SelectSingleNode("Settings/graphics/AnisotropicFiltering");
                set_fileN8.Attributes["value"].InnerText = Convert.ToString(set_anisofil_numeric.Value);

                XmlNode set_fileN9 = set_file.SelectSingleNode("Settings/graphics/MSAA");
                set_fileN9.Attributes["value"].InnerText = Convert.ToString(set_msaa_numeric.Value);

                XmlNode set_fileN10 = set_file.SelectSingleNode("Settings/graphics/MSAAFragments");
                set_fileN10.Attributes["value"].InnerText = Convert.ToString(set_msaafrag_numeric.Value);

                XmlNode set_fileN11 = set_file.SelectSingleNode("Settings/graphics/MSAAQuality");
                set_fileN11.Attributes["value"].InnerText = Convert.ToString(set_msaaqual_numeric.Value);
           
                XmlNode set_fileN12 = set_file.SelectSingleNode("Settings/graphics/TextureQuality");
                set_fileN12.Attributes["value"].InnerText = Convert.ToString(set_texturequal_numeric.Value);
 
                XmlNode set_fileN13 = set_file.SelectSingleNode("Settings/graphics/ParticleQuality");
                set_fileN13.Attributes["value"].InnerText = Convert.ToString(set_partqual_numeric.Value);

                XmlNode set_fileN14 = set_file.SelectSingleNode("Settings/graphics/WaterQuality");
                set_fileN14.Attributes["value"].InnerText = Convert.ToString(set_waterqual_numeric.Value);

                XmlNode set_fileN15 = set_file.SelectSingleNode("Settings/graphics/GrassQuality");
                set_fileN15.Attributes["value"].InnerText = Convert.ToString(set_grassqual_numeric.Value);

                XmlNode set_fileN16 = set_file.SelectSingleNode("Settings/graphics/ShaderQuality");
                set_fileN16.Attributes["value"].InnerText = Convert.ToString(set_shadqual_numeric.Value);

                XmlNode set_fileN17 = set_file.SelectSingleNode("Settings/graphics/Shadow_SoftShadows");
                set_fileN17.Attributes["value"].InnerText = Convert.ToString(set_shadsoft_numeric.Value);

                XmlNode set_fileN18 = set_file.SelectSingleNode("Settings/graphics/UltraShadows_Enabled");
                set_fileN18.Attributes["value"].InnerText = set_ultrashad_combobox.Text;

                XmlNode set_fileN19 = set_file.SelectSingleNode("Settings/graphics/Shadow_ParticleShadows");
                set_fileN19.Attributes["value"].InnerText = set_shadpart_combobox.Text;

                XmlNode set_fileN20 = set_file.SelectSingleNode("Settings/graphics/Shadow_Distance");
                set_fileN20.Attributes["value"].InnerText = Convert.ToString(set_shaddist_numeric.Value);

                XmlNode set_fileN21 = set_file.SelectSingleNode("Settings/graphics/Shadow_LongShadows");
                set_fileN21.Attributes["value"].InnerText = set_shadlong_combobox.Text;

                XmlNode set_fileN22 = set_file.SelectSingleNode("Settings/graphics/Shadow_SplitZStart");
                set_fileN22.Attributes["value"].InnerText = Convert.ToString(set_shadsplitzstart_numeric.Value);

                XmlNode set_fileN23 = set_file.SelectSingleNode("Settings/graphics/Shadow_SplitZEnd");
                set_fileN23.Attributes["value"].InnerText = Convert.ToString(set_shadsplitzend_numeric.Value);

                XmlNode set_fileN24 = set_file.SelectSingleNode("Settings/graphics/Shadow_aircraftExpWeight");
                set_fileN24.Attributes["value"].InnerText = Convert.ToString(set_shadaircraft_numeric.Value);

                XmlNode set_fileN25 = set_file.SelectSingleNode("Settings/graphics/Shadow_DisableScreenSizeCheck");
                set_fileN25.Attributes["value"].InnerText = set_shaddisables_combobox.Text;

                XmlNode set_fileN26 = set_file.SelectSingleNode("Settings/graphics/Reflection_MipBlur");
                set_fileN26.Attributes["value"].InnerText = set_reflmipblur_combobox.Text;

                XmlNode set_fileN27 = set_file.SelectSingleNode("Settings/graphics/FXAA_Enabled");
                set_fileN27.Attributes["value"].InnerText = set_fxaa_combobox.Text;

                XmlNode set_fileN28 = set_file.SelectSingleNode("Settings/graphics/TXAA_Enabled");
                set_fileN28.Attributes["value"].InnerText = set_txaa_combobox.Text;

                XmlNode set_fileN29 = set_file.SelectSingleNode("Settings/graphics/Lighting_FogVolumes");
                set_fileN29.Attributes["value"].InnerText = set_lighfog_combobox.Text;
 
                XmlNode set_fileN30 = set_file.SelectSingleNode("Settings/graphics/Shader_SSA");
                set_fileN30.Attributes["value"].InnerText = set_shadssa_combobox.Text;

                XmlNode set_fileN31 = set_file.SelectSingleNode("Settings/graphics/DX_Version");
                set_fileN31.Attributes["value"].InnerText = Convert.ToString(set_dxver_numeric.Value);

                XmlNode set_fileN32 = set_file.SelectSingleNode("Settings/graphics/CityDensity");
                set_fileN32.Attributes["value"].InnerText = Convert.ToString(set_citydens_numeric.Value);

                XmlNode set_fileN33 = set_file.SelectSingleNode("Settings/graphics/PedVarietyMultiplier");
                set_fileN33.Attributes["value"].InnerText = Convert.ToString(set_pedvar_numeric.Value);

                XmlNode set_fileN34 = set_file.SelectSingleNode("Settings/graphics/VehicleVarietyMultiplier");
                set_fileN34.Attributes["value"].InnerText = Convert.ToString(set_vehicvar_numeric.Value);

                XmlNode set_fileN35 = set_file.SelectSingleNode("Settings/graphics/PostFX");
                set_fileN35.Attributes["value"].InnerText = Convert.ToString(set_postfx_numeric.Value);

                XmlNode set_fileN36 = set_file.SelectSingleNode("Settings/graphics/DoF");
                set_fileN36.Attributes["value"].InnerText = set_dof_combobox.Text;

                XmlNode set_fileN37 = set_file.SelectSingleNode("Settings/graphics/HdStreamingInFlight");
                set_fileN37.Attributes["value"].InnerText = set_hdstream_combobox.Text;

                XmlNode set_fileN38 = set_file.SelectSingleNode("Settings/graphics/MaxLodScale");
                set_fileN38.Attributes["value"].InnerText = Convert.ToString(set_maxlod_numeric.Value);

                XmlNode set_fileN39 = set_file.SelectSingleNode("Settings/graphics/MotionBlurStrength");
                set_fileN39.Attributes["value"].InnerText = Convert.ToString(set_motionblur_numeric.Value);
                #endregion
                #region <SYSTEM>
                XmlNode set_fileN40 = set_file.SelectSingleNode("Settings/system/numBytesPerReplayBlock");
                set_fileN40.Attributes["value"].InnerText = Convert.ToString(set_bytesperreplay_numeric.Value);

                XmlNode set_fileN41 = set_file.SelectSingleNode("Settings/system/numReplayBlocks");
                set_fileN41.Attributes["value"].InnerText = Convert.ToString(set_bytesreplay_numeric.Value);

                XmlNode set_fileN42 = set_file.SelectSingleNode("Settings/system/maxSizeOfStreamingReplay");
                set_fileN42.Attributes["value"].InnerText = Convert.ToString(set_maxstream_numeric.Value);

                XmlNode set_fileN43 = set_file.SelectSingleNode("Settings/system/maxFileStoreSize");
                set_fileN43.Attributes["value"].InnerText = Convert.ToString(set_maxfile_numeric.Value);
                #endregion
                XmlNode set_fileN44 = set_file.SelectSingleNode("Settings/audio/Audio3d");
                set_fileN44.Attributes["value"].InnerText = set_audio3d_combobox.Text;
                #region <VIDEO>
                XmlNode set_fileN45 = set_file.SelectSingleNode("Settings/video/AdapterIndex");
                set_fileN45.Attributes["value"].InnerText = Convert.ToString(set_adapterind_numeric.Value);

                XmlNode set_fileN46 = set_file.SelectSingleNode("Settings/video/OutputIndex");
                set_fileN46.Attributes["value"].InnerText = Convert.ToString(set_outputind_numeric.Value);

                XmlNode set_fileN47 = set_file.SelectSingleNode("Settings/video/ScreenWidth");
                set_fileN47.Attributes["value"].InnerText = Convert.ToString(set_screenw_numeric.Value);

                XmlNode set_fileN48 = set_file.SelectSingleNode("Settings/video/ScreenHeight");
                set_fileN48.Attributes["value"].InnerText = Convert.ToString(set_screenh_numeric.Value);

                XmlNode set_fileN49 = set_file.SelectSingleNode("Settings/video/RefreshRate");
                set_fileN49.Attributes["value"].InnerText = Convert.ToString(set_refreshrate_numeric.Value);

                XmlNode set_fileN50 = set_file.SelectSingleNode("Settings/video/Windowed");
                set_fileN50.Attributes["value"].InnerText = Convert.ToString(set_wind_numeric.Value);

                XmlNode set_fileN51 = set_file.SelectSingleNode("Settings/video/VSync");
                set_fileN51.Attributes["value"].InnerText = Convert.ToString(set_vsync_numeric.Value);

                XmlNode set_fileN52 = set_file.SelectSingleNode("Settings/video/Stereo");
                set_fileN52.Attributes["value"].InnerText = Convert.ToString(set_stereo_numeric.Value);

                XmlNode set_fileN53 = set_file.SelectSingleNode("Settings/video/Convergence");
                set_fileN53.Attributes["value"].InnerText = Convert.ToString(set_conv_numeric.Value);

                XmlNode set_fileN54 = set_file.SelectSingleNode("Settings/video/Separation");
                set_fileN54.Attributes["value"].InnerText = Convert.ToString(set_separation_numeric.Value);

                XmlNode set_fileN55 = set_file.SelectSingleNode("Settings/video/PauseOnFocusLoss");
                set_fileN55.Attributes["value"].InnerText = Convert.ToString(set_pausefoc_numeric.Value);

                XmlNode set_fileN56 = set_file.SelectSingleNode("Settings/video/AspectRatio");
                set_fileN56.Attributes["value"].InnerText = Convert.ToString(set_aspectratio_numeric.Value);
                #endregion
                XmlNode set_fileN57 = set_file.SelectSingleNode("Settings/VideoCardDescription");
                set_fileN57.InnerText = set_videocard_texbox.Text;

                #region Read-only attributes
                FileAttributes attributes = File.GetAttributes(set_filePathXML);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                File.SetAttributes(set_filePathXML, attributes & ~FileAttributes.ReadOnly);
                set_file.Save(set_filePathXML);
                if (readonly_checkbox.Checked == true) { File.SetAttributes(set_filePathXML, FileAttributes.ReadOnly); }
                #endregion
                MessageBox.Show("File successfully saved!");
            } catch { MessageBox.Show("File not saved...", "Sorry!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        #endregion
        #region [SELF RADIO TAB] Load tracks
        private void selfRadio_Load () {
            selfRadio_listb.Items.Clear();
            string[] selfRadioList_getlist = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Rockstar Games\GTA V\User Music\");
            foreach (string selfRadioList_edit in selfRadioList_getlist) {
                string selfRadioList_title = selfRadioList_edit.Remove(0,selfRadioList_edit.LastIndexOf('\\') + 1);

                selfRadio_listb.Items.Add(selfRadioList_title);
            }
            selfRadioTracks_lbl.Text = "Number of tracks: " + selfRadio_listb.Items.Count.ToString();
            if (selfRadio_listb.Items.Count == 0) { selfRadio_listb.Items.Add("Empty"); selfRadioTracks_lbl.Text = "Tracks: 0"; }
            selfRadio_listb.Sorted = true;
        }
            #endregion
        #endregion

        #region Variables
        string selfToolVersion = "4.2b.220618";
        bool selfFirstLaunch = true;
        string self_filePath = Directory.GetCurrentDirectory() + "\\self.xml";
        string set_filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Rockstar Games\GTA V\settings.xml";
        bool set_firstLaunch = true;
        bool self_cancelLaunching = false;
        string selfGTAV_dirPath = "";
        Int64 gta5CPUAffinity_int = 0;
        string gta5CPUAffinity_bin = "";
        bool saveChangeActive_aftergamelaunching = false;
        MediaPlayer selfRadioTrackPlay = new MediaPlayer();
        #endregion

        private void Form1_Load(object sender, EventArgs e) {
            #region [SETTINGS.XML TAB] Panel disable horizontal scrollbar
            panel2.HorizontalScroll.Maximum = 0;
            panel2.AutoScroll = false;
            panel2.VerticalScroll.Visible = false;
            panel2.AutoScroll = true;
            #endregion
            set_loadFile(set_filePath);
            selfGTAV_dirPath = Convert.ToString(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Rockstar Games\GTAV","InstallFolder",null));
            if (selfGTAV_dirPath == "") { selfGTAV_dirPath = Convert.ToString(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Rockstar Games\Grand Theft Auto V","InstallFolder",null)); }
            if (selfGTAV_dirPath == "") { selfGTAV_dirPath = Convert.ToString(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Rockstar Games\Grand Theft Auto V","InstallFolder",null)); }
            if (selfGTAV_dirPath == "") { selfGTAV_dirPath = Convert.ToString(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Rockstar Games\GTAV","InstallFolder",null)); }
            if (selfGTAV_dirPath == "") { selfGTAV_dirPath = Convert.ToString(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Rockstar Games\GTAV","InstallFolder",null)); }

            if (selfGTAV_dirPath == "") { selfGTAV_dirPath = Convert.ToString(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Rockstar Games\Grand Theft Auto V","InstallFolderSteam",null)); }
            if (selfGTAV_dirPath == "") { selfGTAV_dirPath = Convert.ToString(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Rockstar Games\Grand Theft Auto V","InstallFolderSteam",null)); }
            if (selfGTAV_dirPath == "") { selfGTAV_dirPath = Convert.ToString(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Rockstar Games\GTAV","InstallFolderSteam",null)); }
            if (selfGTAV_dirPath == "") { selfGTAV_dirPath = Convert.ToString(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Rockstar Games\GTAV","InstallFolderSteam",null)); }

            gtavdir_txtbox.Text = selfGTAV_dirPath;

            #region Self File
            if (!File.Exists(self_filePath)) {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";
                using (XmlWriter self_file = XmlWriter.Create(self_filePath, settings)) {
                    self_file.WriteStartDocument();

                    self_file.WriteComment("Xxshark888xX (Adriano Mutu) (C)2016-2018 | " + selfToolVersion);
                    self_file.WriteStartElement("MOM_Grand_Theft_Auto_V_Launcher");
                        self_file.WriteStartElement("GTAV_Directory");
                            self_file.WriteString(gtavdir_txtbox.Text);
                        self_file.WriteEndElement();

                        self_file.WriteStartElement("CPU_Priority");
                            self_file.WriteString("Normal");
                        self_file.WriteEndElement();

                        if (Environment.ProcessorCount == 2) {
                            self_file.WriteStartElement("CPU_Affinity_Integer");
                                self_file.WriteString("3");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("CPU_Affinity_Binary");
                                self_file.WriteString("00000011");
                            self_file.WriteEndElement();
                            gta5CPUAffinity_int = 3;
                            gta5CPUAffinity_bin = "00000011";
                        }    

                        if (Environment.ProcessorCount == 4) {
                            self_file.WriteStartElement("CPU_Affinity_Integer");
                                self_file.WriteString("15");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("CPU_Affinity_Binary");
                                self_file.WriteString("00001111");
                            self_file.WriteEndElement();
                            gta5CPUAffinity_int = 15;
                            gta5CPUAffinity_bin = "00001111";
                        }

                        if (Environment.ProcessorCount == 6) {
                            self_file.WriteStartElement("CPU_Affinity_Integer");
                                self_file.WriteString("63");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("CPU_Affinity_Binary");
                                self_file.WriteString("00111111");
                            self_file.WriteEndElement();
                            gta5CPUAffinity_int = 63;
                            gta5CPUAffinity_bin = "00111111";
                        }

                        if (Environment.ProcessorCount == 8) {
                            self_file.WriteStartElement("CPU_Affinity_Integer");
                                self_file.WriteString("255");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("CPU_Affinity_Binary");
                                self_file.WriteString("11111111");
                            self_file.WriteEndElement();
                            gta5CPUAffinity_int = 255;
                            gta5CPUAffinity_bin = "11111111";
                        }

                        self_file.WriteStartElement("CPU_P.P._Key");
                            self_file.WriteString("L");
                        self_file.WriteEndElement();

                        self_file.WriteStartElement("GTAVLauncherStop");
                            self_file.WriteString("No");
                        self_file.WriteEndElement();

                        self_file.WriteStartElement("SelfStop");
                            self_file.WriteString("No");
                        self_file.WriteEndElement();

                        self_file.WriteStartElement("GTAV_Version");
                            self_file.WriteString("Retail");
                        self_file.WriteEndElement();

                        self_file.WriteStartElement("Notify_Update");
                            self_file.WriteString("TRUE");
                        self_file.WriteEndElement();

                        self_file.WriteStartElement("RAMCleaner");

                            self_file.WriteStartElement("explorer.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("chrome.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("Steam.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("reader_sl.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("jqs.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("AdobeARM.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("AAMUpdateNotifier.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("Jusched.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("DivXUpdate.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("NeroCheck.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("HKCMD.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("atiptaxx.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("ati2evxx.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("RAVCpl64.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("Nwiz.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("CCC.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("RadeonSettings.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("SearchIndexer.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                            self_file.WriteStartElement("SearchUI.exe");
                                self_file.WriteString("Unchecked");
                            self_file.WriteEndElement();

                        self_file.WriteEndElement();

                    self_file.WriteEndElement();

                    self_file.WriteEndDocument();
                    self_file.Close();
                }

                cpupriority_combobox.SelectedIndex = 3;
                stoplauncher_combobox.SelectedIndex = 0;
                autoclose_combobox.SelectedIndex = 1;

                #region [ASK FOR GTA V DIRECTORY]
                if (selfGTAV_dirPath == "") {
                    DialogResult gtavdir_btn_result = folderBrowserDialog1.ShowDialog();
                    if (gtavdir_btn_result == DialogResult.OK) {
                        if (File.Exists(folderBrowserDialog1.SelectedPath + "\\GTAVLauncher.exe")) {
                            selfGTAV_dirPath = folderBrowserDialog1.SelectedPath;
                            gtavdir_txtbox.Text = selfGTAV_dirPath;
                            FileVersionInfo getGta5Version = FileVersionInfo.GetVersionInfo(selfGTAV_dirPath + "\\GTA5.exe");
                            string gta5Version = getGta5Version.FileVersion;
                            gta5Info_groupbx.Text = "[Grand Theft Auto V] | v" + gta5Version;
                            selfCheck_dll();
                            savechanges_button.Enabled = true;
                        } else { MessageBox.Show("GTAVLauncher.exe not found! \r\n Try again, by pressing from the right corner button","Sorry!",MessageBoxButtons.OK,MessageBoxIcon.Error); }
                    }
                }
                #endregion
            }
            else {
                selfFirstLaunch = false;
                selfLoadUserSet();
                selfCheck_dll();
            }
                savechanges_button.Enabled = false;
            #endregion
            selfRadio_Load();
            #region [P.P.] Check affinity
            if (gta5CPUAffinity_bin.Substring(7, 1) == "1") { c1_checkbox.Checked = true; }
            if (gta5CPUAffinity_bin.Substring(6, 1) == "1") { c2_checkbox.Checked = true; }
            if (gta5CPUAffinity_bin.Substring(5, 1) == "1") { c3_checkbox.Checked = true; }
            if (gta5CPUAffinity_bin.Substring(4, 1) == "1") { c4_checkbox.Checked = true; }
            if (gta5CPUAffinity_bin.Substring(3, 1) == "1") { c5_checkbox.Checked = true; }
            if (gta5CPUAffinity_bin.Substring(2, 1) == "1") { c6_checkbox.Checked = true; }
            if (gta5CPUAffinity_bin.Substring(1, 1) == "1") { c7_checkbox.Checked = true; }
            if (gta5CPUAffinity_bin.Substring(0, 1) == "1") { c8_checkbox.Checked = true; }
            savechanges_button.Enabled = false;

            #region Check the number of cores
                if (Environment.ProcessorCount == 2) {
                    c3_checkbox.Enabled = false; c3_checkbox.Checked = false;
                    c4_checkbox.Enabled = false; c4_checkbox.Checked = false;
                    c5_checkbox.Enabled = false; c5_checkbox.Checked = false;
                    c6_checkbox.Enabled = false; c6_checkbox.Checked = false;
                    c7_checkbox.Enabled = false; c7_checkbox.Checked = false;
                    c8_checkbox.Enabled = false; c8_checkbox.Checked = false;
                }

                if (Environment.ProcessorCount == 4) {
                    c5_checkbox.Enabled = false; c5_checkbox.Checked = false;
                    c6_checkbox.Enabled = false; c6_checkbox.Checked = false;
                    c7_checkbox.Enabled = false; c7_checkbox.Checked = false;
                    c8_checkbox.Enabled = false; c8_checkbox.Checked = false;
                }
                if (Environment.ProcessorCount == 6) {
                    c7_checkbox.Enabled = false; c7_checkbox.Checked = false;
                    c8_checkbox.Enabled = false; c8_checkbox.Checked = false;
                }
                #endregion
            #endregion
            selfModsEnabled_find();
            selfModsDisabled_find();
            playgtav_button.Enabled = false;
            if (newUpdate_cbx.Checked == true) { updateCheck_timer.Start(); }

            tool_groupbx.Text = "[MOM] | v" + selfToolVersion;
            if (gtavdir_txtbox.Text == "") { gta5Info_groupbx.Text = "[Grand Theft Auto V] | v -//-"; } else {
                FileVersionInfo getGta5Version = FileVersionInfo.GetVersionInfo(gtavdir_txtbox.Text + "\\GTA5.exe");
                string gta5Version = getGta5Version.FileVersion;
                gta5Info_groupbx.Text = "[Grand Theft Auto V] | v" + gta5Version;
            }
        }

        #region [SELF TAB] GTA V directory button click
        private void gtavDir_btn_Click(object sender, EventArgs e) {
            DialogResult gtavdir_btn_result = folderBrowserDialog1.ShowDialog();
            if (gtavdir_btn_result == DialogResult.OK) {
                if (File.Exists(folderBrowserDialog1.SelectedPath + "\\GTAVLauncher.exe")) {
                    selfGTAV_dirPath = folderBrowserDialog1.SelectedPath;
                    gtavdir_txtbox.Text = selfGTAV_dirPath;
                    FileVersionInfo getGta5Version = FileVersionInfo.GetVersionInfo(selfGTAV_dirPath + "\\GTA5.exe");
                    string gta5Version = getGta5Version.FileVersion;
                    gta5Info_groupbx.Text = "[Grand Theft Auto V] | v" + gta5Version;
                    selfCheck_dll();
                } else { MessageBox.Show("GTAVLauncher.exe not found!","Sorry!",MessageBoxButtons.OK,MessageBoxIcon.Error); }
            }   
            waitTime_progressbar.Focus();
        }
        #endregion
        #region [SELF TAB] Save changes button click
        private void savechanges_button_Click(object sender, EventArgs e) {
            if (File.Exists(self_filePath)) { File.Delete(self_filePath); }
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            using (XmlWriter self_file = XmlWriter.Create(self_filePath, settings)) {
                self_file.WriteStartDocument();

                self_file.WriteComment("Xxshark888xX (Adriano Mutu) (C)2018 - " + selfToolVersion);
                self_file.WriteStartElement("MOM_Grand_Theft_Auto_V_Launcher");

                    self_file.WriteStartElement("GTAV_Directory");
                        self_file.WriteString(gtavdir_txtbox.Text);
                    self_file.WriteEndElement();
     
                    self_file.WriteStartElement("CPU_Priority");
                        self_file.WriteString(cpupriority_combobox.Text);
                    self_file.WriteEndElement();

                    selfPPCPUAffinity_converter();
                    self_file.WriteStartElement("CPU_Affinity_Integer");
                        self_file.WriteString(gta5CPUAffinity_int.ToString());
                    self_file.WriteEndElement();

                    self_file.WriteStartElement("CPU_Affinity_Binary");
                        self_file.WriteString(gta5CPUAffinity_bin);
                    self_file.WriteEndElement();

                    self_file.WriteStartElement("CPU_P.P._Key");
                            self_file.WriteString(selfPP_enableKeySet_txtbox.Text);
                    self_file.WriteEndElement();

                    self_file.WriteStartElement("GTAVLauncherStop");
                        self_file.WriteString(stoplauncher_combobox.Text);
                    self_file.WriteEndElement();

                    self_file.WriteStartElement("SelfStop");
                        self_file.WriteString(autoclose_combobox.Text);
                    self_file.WriteEndElement();

                    self_file.WriteStartElement("GTAV_Version");
                       if (retail_radiobutton.Checked == true) { self_file.WriteString("Retail"); } else { self_file.WriteString("Steam"); }
                    self_file.WriteEndElement();

                    self_file.WriteStartElement("Notify_Update");
                       if (newUpdate_cbx.Checked == true) { self_file.WriteString("TRUE"); } else { self_file.WriteString("FALSE"); }
                        self_file.WriteEndElement();

                    self_file.WriteStartElement("RAMCleaner");

                       self_file.WriteStartElement("explorer.exe");
                           self_file.WriteString(Convert.ToString(ramc_explorer_checkbox.CheckState));
                       self_file.WriteEndElement();

                       self_file.WriteStartElement("chrome.exe");
                           self_file.WriteString(Convert.ToString(ramc_chrome_checkbox.CheckState));
                       self_file.WriteEndElement();

                       self_file.WriteStartElement("Steam.exe");
                           self_file.WriteString(Convert.ToString(ramc_steam_checkbox.CheckState));
                       self_file.WriteEndElement();

                       self_file.WriteStartElement("reader_sl.exe");
                           self_file.WriteString(Convert.ToString(ramc_reader_sl_checkbox.CheckState));
                       self_file.WriteEndElement();

                       self_file.WriteStartElement("jqs.exe");
                           self_file.WriteString(Convert.ToString(ramc_jqs_checkbox.CheckState));
                       self_file.WriteEndElement();

                       self_file.WriteStartElement("AdobeARM.exe");
                           self_file.WriteString(Convert.ToString(ramc_adobearm_checkbox.CheckState));
                       self_file.WriteEndElement();

                      self_file.WriteStartElement("AAMUpdateNotifier.exe");
                           self_file.WriteString(Convert.ToString(ramc_aamupnotf_checkbox.CheckState));
                      self_file.WriteEndElement();

                      self_file.WriteStartElement("Jusched.exe");
                           self_file.WriteString(Convert.ToString(ramc_jusched_checkbox.CheckState));
                      self_file.WriteEndElement();

                      self_file.WriteStartElement("DivXUpdate.exe");
                           self_file.WriteString(Convert.ToString(ramc_divxupdate_checkbox.CheckState));
                      self_file.WriteEndElement();

                      self_file.WriteStartElement("NeroCheck.exe");
                           self_file.WriteString(Convert.ToString(ramc_nero_checkbox.CheckState));
                      self_file.WriteEndElement();

                      self_file.WriteStartElement("HKCMD.exe");
                           self_file.WriteString(Convert.ToString(ramc_hkcmd_checkbox.CheckState));
                      self_file.WriteEndElement();

                      self_file.WriteStartElement("atiptaxx.exe");
                           self_file.WriteString(Convert.ToString(ramc_ati0_checkbox.CheckState));
                      self_file.WriteEndElement();

                      self_file.WriteStartElement("ati2evxx.exe");
                           self_file.WriteString(Convert.ToString(ramc_ati1_checkbox.CheckState));
                      self_file.WriteEndElement();

                      self_file.WriteStartElement("RAVCpl64.exe");
                           self_file.WriteString(Convert.ToString(ramc_ravcpl_checkbox.CheckState));
                      self_file.WriteEndElement();

                      self_file.WriteStartElement("Nwiz.exe");
                           self_file.WriteString(Convert.ToString(ramc_nwiz_checkbox.CheckState));
                      self_file.WriteEndElement();

                      self_file.WriteStartElement("CCC.exe");
                           self_file.WriteString(Convert.ToString(ramc_ccc_checkbox.CheckState));
                      self_file.WriteEndElement();

                      self_file.WriteStartElement("RadeonSettings.exe");
                           self_file.WriteString(Convert.ToString(ramc_radsett_checkbox.CheckState));
                      self_file.WriteEndElement();

                      self_file.WriteStartElement("SearchIndexer.exe");
                           self_file.WriteString(Convert.ToString(ramc_searchind_checkbox.CheckState));
                      self_file.WriteEndElement();

                      self_file.WriteStartElement("SearchUI.exe");
                           self_file.WriteString(Convert.ToString(ramc_cortana_checkbox.CheckState));
                      self_file.WriteEndElement();

                   self_file.WriteEndElement();

               self_file.WriteEndElement();

               self_file.WriteEndDocument();
               self_file.Close();
            }
           savechanges_button.Enabled = false;
           waitTime_progressbar.Focus();
        }
        #endregion

        #region [SELF TAB] Play GTA V button click
        private void playgtav_button_Click(object sender, EventArgs e) {
            if (self_cancelLaunching == false) {
                var selfMod_info = File.Create(selfGTAV_dirPath + @"\scripts\ppInfo.momlauncher");
                selfMod_info.Close();
                File.WriteAllText(selfGTAV_dirPath + @"\scripts\ppInfo.momlauncher",cpupriority_combobox.Text + "\r\n" + gta5CPUAffinity_bin + "\r\n" + stoplauncher_combobox.Text + "\r\n" + selfPP_enableKeySet_txtbox.Text);

                waitTime_progressbar.Maximum = 250;
                Process gtav_process = Process.GetProcessesByName("GTA5").FirstOrDefault();
                if (gtav_process == null) {
                    if (steam_radiobutton.Checked == true) { Process.Start("steam://rungameid/271590"); System.Threading.Thread.Sleep(5000); } else {
                        try {
                            if (!File.Exists(Directory.GetCurrentDirectory() + "self_launchgtav.lnk")) { CreateShortcut(); }
                            Process.Start(Directory.GetCurrentDirectory() + "\\self_launchgtav.lnk");
                        } catch { }
                    }
                    Process gtavlauncher_process = Process.GetProcessesByName("GTAVLauncher").FirstOrDefault();
                    if (gtavlauncher_process != null) {
                        playgtav_button.Enabled = false;
                        playgtav_button.Text = "Cancel";
                        self_cancelLaunching = true;
                        if (savechanges_button.Enabled == true) { saveChangeActive_aftergamelaunching = true; } else { saveChangeActive_aftergamelaunching = false; }
                        enableControls(false);

                        timetowait_timer.Interval = 1000;
                        timetowait_timer.Start();
                    } else { MessageBox.Show("Can't execute GTAVLauncher", "GTAVLauncher not executed!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                } else {
                   if (MessageBox.Show("Another instance of GTA5.exe is running now... \r\n\r\nDo you want to continue?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {
                        if (steam_radiobutton.Checked == true) { Process.Start("steam://rungameid/271590"); System.Threading.Thread.Sleep(5000); } else {
                            try {
                                if (!File.Exists(Directory.GetCurrentDirectory() + "self_launchgtav.lnk")) { CreateShortcut(); }
                                Process.Start(Directory.GetCurrentDirectory() + "\\self_launchgtav.lnk");
                            } catch { }
                        }
                        Process gtavlauncher_process = Process.GetProcessesByName("GTAVLauncher").FirstOrDefault();
                        if (gtavlauncher_process != null) {
                            playgtav_button.Enabled = false;
                            playgtav_button.Text = "Cancel";
                            self_cancelLaunching = true;
                            if (savechanges_button.Enabled == true) { saveChangeActive_aftergamelaunching = true; } else { saveChangeActive_aftergamelaunching = false; }
                            enableControls(false);

                            timetowait_timer.Interval = 1000;
                            timetowait_timer.Start();
                        } else { MessageBox.Show("Can't execute GTAVLauncher", "GTAVLauncher not executed!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                }
            } else {
                File.Delete(selfGTAV_dirPath + @"\scripts\ppInfo.momlauncher");
                timetowait_timer.Stop();
                try {
                    Process gtav_process = Process.GetProcessesByName("GTA5").FirstOrDefault();
                    gtav_process.Kill();
                } catch { }
                waitTime_progressbar.Value = 0;
                enableControls(true);
                if (saveChangeActive_aftergamelaunching == false) { savechanges_button.Enabled = false; }
                playgtav_button.Text = "Launch Game";
                self_cancelLaunching = false;
            }
            waitTime_progressbar.Focus();
        }
        #endregion
        #region [PP TAB] RAM Cleaner Check/Uncheck All button click
        private void ramc_checkunall_btn_Click(object sender, EventArgs e) {
            if (ramc_checkunall_btn.Text == "Uncheck all") {
                ramc_explorer_checkbox.Checked = false;
                ramc_chrome_checkbox.Checked = false;
                ramc_steam_checkbox.Checked = false;
                ramc_reader_sl_checkbox.Checked = false;
                ramc_jqs_checkbox.Checked = false;
                ramc_adobearm_checkbox.Checked = false;
                ramc_aamupnotf_checkbox.Checked = false;
                ramc_jusched_checkbox.Checked = false;
                ramc_divxupdate_checkbox.Checked = false;
                ramc_nero_checkbox.Checked = false;
                ramc_hkcmd_checkbox.Checked = false;
                ramc_ati0_checkbox.Checked = false;
                ramc_ati1_checkbox.Checked = false;
                ramc_ravcpl_checkbox.Checked = false;
                ramc_nwiz_checkbox.Checked = false;
                ramc_ccc_checkbox.Checked = false;
                ramc_radsett_checkbox.Checked = false;
                ramc_searchind_checkbox.Checked = false;
                ramc_cortana_checkbox.Checked = false;

                ramc_checkunall_btn.Text = "Check all";
            } else {
                ramc_explorer_checkbox.Checked = true;
                ramc_chrome_checkbox.Checked = true;
                ramc_steam_checkbox.Checked = true;
                ramc_reader_sl_checkbox.Checked = true;
                ramc_jqs_checkbox.Checked = true;
                ramc_adobearm_checkbox.Checked = true;
                ramc_aamupnotf_checkbox.Checked = true;
                ramc_jusched_checkbox.Checked = true;
                ramc_divxupdate_checkbox.Checked = true;
                ramc_nero_checkbox.Checked = true;
                ramc_hkcmd_checkbox.Checked = true;
                ramc_ati0_checkbox.Checked = true;
                ramc_ati1_checkbox.Checked = true;
                ramc_ravcpl_checkbox.Checked = true;
                ramc_nwiz_checkbox.Checked = true;
                ramc_ccc_checkbox.Checked = true;
                ramc_radsett_checkbox.Checked = true;
                ramc_searchind_checkbox.Checked = true;
                ramc_cortana_checkbox.Checked = true;

                ramc_checkunall_btn.Text = "Uncheck all";
            }
            waitTime_progressbar.Focus();
        }
        #endregion
        #region [PP TAB] Enable/Disable Key button click
        private void selfPP_enableKeySet_txtbox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode != Keys.Escape) {
                selfPP_enableKeySet_txtbox.Text = e.KeyCode.ToString();
            }
            waitTime_progressbar.Focus();
        }
        #endregion
        #region [MOD Manager] Split container lose focus
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e) { waitTime_progressbar.Focus(); }
        #endregion
        #region [MOD Manager] Refresh lists button click
        private void modRefreshLists_btn_Click(object sender, EventArgs e) {
            selfModsDisabled_find();
            selfModsEnabled_find();

            modEnabled_grpb.Focus();
        }
        #endregion
        #region [MOD Manager] Sync and buttons controls
        private void modEnabledName_listcb_MouseClick(object sender, MouseEventArgs e) {
            modEnabledPath_listcb.TopIndex = modEnabledName_listcb.TopIndex;
        }

        private void modEnabledName_listcb_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (modEnabledPath_listcb.GetItemChecked(modEnabledName_listcb.SelectedIndex) == false) {
                modEnabledPath_listcb.SetItemCheckState(modEnabledName_listcb.SelectedIndex, CheckState.Checked);
            } else {
                modEnabledPath_listcb.SetItemCheckState(modEnabledName_listcb.SelectedIndex, CheckState.Unchecked);
            }
        }

        private void modDisabledName_listcb_MouseClick(object sender, MouseEventArgs e) {
            modDisabledPath_listcb.TopIndex = modDisabledName_listcb.TopIndex;
        }

        private void modDisabledName_listcb_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (modDisabledPath_listcb.GetItemChecked(modDisabledName_listcb.SelectedIndex) == false) {
                modDisabledPath_listcb.SetItemCheckState(modDisabledName_listcb.SelectedIndex, CheckState.Checked);
            } else {
                modDisabledPath_listcb.SetItemCheckState(modDisabledName_listcb.SelectedIndex, CheckState.Unchecked);
            }
        }

        private void modEnabledName_listcb_MouseUp(object sender, MouseEventArgs e) {
            if (modEnabledName_listcb.CheckedItems.Count > 0) { modDisable_btn.Enabled = true; modUninstall_btn.Enabled = true; } else { modDisable_btn.Enabled = false; modUninstall_btn.Enabled = false; }
        }

        private void modDisabledName_listcb_MouseUp(object sender, MouseEventArgs e) {
            if (modDisabledName_listcb.CheckedItems.Count > 0) { modEnable_btn.Enabled = true; } else { modEnable_btn.Enabled = false; }
        }
        #endregion
        #region [MOD Manager] Disable mods button click
        private void modDisable_btn_Click(object sender, EventArgs e) {
            if (!Directory.Exists(selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]")) {
                Directory.CreateDirectory(selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]");
            }

            int i = 0;
            string[] selfModDisable_path = new string[modEnabledName_listcb.Items.Count];
            string[] selfModDisable_pathDisabled = new string[modEnabledName_listcb.Items.Count];
            string[] selfModDisable_name = new string[modEnabledName_listcb.Items.Count];

            foreach (object modEnabledInfo_path in modEnabledPath_listcb.CheckedItems) {
                selfModDisable_path[i] = selfGTAV_dirPath + modEnabledInfo_path.ToString();
                
                selfModDisable_pathDisabled[i] = selfModDisable_path[i].Substring(selfGTAV_dirPath.Length);

                i++;
            }
            i = 0;
            foreach (object modEnabledInfo_name in modEnabledName_listcb.CheckedItems) {
                selfModDisable_name[i] +=  modEnabledInfo_name.ToString();
                i++;
            }

            modEnabledName_listcb.CheckedItems.OfType<string>().ToList().ForEach(modEnabledName_listcb.Items.Remove);
            for (int ii = modEnabledPath_listcb.CheckedIndices.Count - 1; ii >= 0; ii--) {
                modEnabledPath_listcb.Items.RemoveAt(modEnabledPath_listcb.CheckedIndices[ii]);
            }

            for (int x = 0; x < i; x++) {
                if (!Directory.Exists(selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]" + selfModDisable_pathDisabled[x])) {
                    Directory.CreateDirectory(selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]" + selfModDisable_pathDisabled[x]);
                }
                File.Move(selfModDisable_path[x] + selfModDisable_name[x], selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]" + selfModDisable_pathDisabled[x] + selfModDisable_name[x]);
            }
            modUninstall_btn.Enabled = false;
            selfModsEnabled_find();
            selfModsDisabled_find();
            modEnabled_grpb.Focus();
        }
        #endregion
        #region [MOD Manager] Enable mods button click
        private void modEnable_btn_Click(object sender, EventArgs e) {
            int i = 0;
            string[] selfModEnable_path = new string[modDisabledName_listcb.Items.Count];
            string[] selfModEnable_pathEnabled = new string[modDisabledName_listcb.Items.Count];
            string[] selfModEnable_name = new string[modDisabledName_listcb.Items.Count];
            foreach (object modDisabledInfo_path in modDisabledPath_listcb.CheckedItems) {
                selfModEnable_path[i] = selfGTAV_dirPath + "\\[MOM Launcher Mod Manager]" + modDisabledInfo_path.ToString();
                selfModEnable_pathEnabled[i] = selfModEnable_path[i].Substring(selfGTAV_dirPath.Length + "[MOM Launcher Mod Manager]".Length + 1);

                i++;
            }
            i = 0;
            foreach (object modDisabledInfo_name in modDisabledName_listcb.CheckedItems) {
                selfModEnable_name[i] +=  modDisabledInfo_name.ToString();
                i++;
            }

            modDisabledName_listcb.CheckedItems.OfType<string>().ToList().ForEach(modEnabledName_listcb.Items.Remove);
            for (int ii = modDisabledPath_listcb.CheckedIndices.Count - 1; ii >= 0; ii--) {
                modDisabledPath_listcb.Items.RemoveAt(modDisabledPath_listcb.CheckedIndices[ii]);
            }

            for (int x = 0; x < i; x++) {
                File.Move(selfModEnable_path[x] + selfModEnable_name[x], selfGTAV_dirPath + selfModEnable_pathEnabled[x] + selfModEnable_name[x]);
            }
            selfModsEnabled_find();
            selfModsDisabled_find();
            modEnabled_grpb.Focus();
        }
        #endregion
        #region [MOD Manager] Uninstall mods button click
        private void modUninstall_btn_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure?", "This will permanently delete selected mod(s)", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes ) {
                for (int i = modEnabledName_listcb.Items.Count - 1; i >= 0; i--) {
                    if (modEnabledName_listcb.GetItemChecked(i) == true) {
                        File.Delete(selfGTAV_dirPath + modEnabledPath_listcb.Items[i].ToString() + modEnabledName_listcb.Items[i].ToString());
                    }
                }
            }
            modUninstall_btn.Enabled = false;
            selfModsEnabled_find();
            selfModsDisabled_find();
        }
        #endregion
        #region [SETTINGS.XML TAB] Make a backup button click
        private void makeBackup_button_Click(object sender, EventArgs e) {
            if (backupName_txtbox.Text != "") {
                FolderBrowserDialog set_fileBackup = new FolderBrowserDialog();
                set_fileBackup.Description = "Choose where to save the backup";
                if (set_fileBackup.ShowDialog() == DialogResult.OK) { File.Copy(set_filePath, set_fileBackup.SelectedPath + "\\" + backupName_txtbox.Text + ".xml"); makeBackup_button.Enabled = false; MessageBox.Show("Backup successfully created!"); backupName_txtbox.Text = ""; }
            } else { MessageBox.Show("Please, write a name for the file", "Can't make a backup!", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            readonly_checkbox.Focus();
        }
        #endregion
        #region [SETTINGS.XML TAB] Load another xml button click
        private void loadAnotherXML_button_Click(object sender, EventArgs e) {
            DialogResult xmlFilePath = openFileDialog1.ShowDialog();
            if (xmlFilePath == DialogResult.OK) { set_filePath = openFileDialog1.FileName; set_loadFile(set_filePath); }
            readonly_checkbox.Focus();
        }
        #endregion
        #region [SETTINGS.XML TAB] Delete current xml button click
        private void deleteCurrentXML_button_Click(object sender, EventArgs e) {
            if (MessageBox.Show("If you continue, this file will be permanently deleted!", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                try {
                    FileAttributes attributes = File.GetAttributes(set_filePath);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    File.SetAttributes(set_filePath, attributes & ~FileAttributes.ReadOnly);

                    File.Delete(set_filePath); deleteCurrentXML_button.Enabled = false;
                    makeBackup_button.Enabled = false;
                    applyChanges_button.Enabled = false;
                    MessageBox.Show("File successfully deleted!");
                    set_fileName_txtbox.Text = "NO FILE";
                } catch { MessageBox.Show("Strange, can't delete this file", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            readonly_checkbox.Focus();
        }
        #endregion
        #region [SETTINGS.XML TAB] Apply changs xml button click
        private void applyChanges_button_Click(object sender, EventArgs e) {
            set_saveFile(set_filePath);
            readonly_checkbox.Focus();
        }
        #endregion
        #region [SELF RADIO TAB] Add track button click
        private void selfRadioAdd_btn_Click(object sender, EventArgs e) {
            openFileDialog2.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            DialogResult selfRadioAddTrack_result = openFileDialog2.ShowDialog();

            if (selfRadioAddTrack_result == DialogResult.OK) {
                foreach (string selfTrack in openFileDialog2.FileNames) {
                    string selfRadioAddTrack_edit = selfTrack;
                    selfRadioAddTrack_edit = selfRadioAddTrack_edit.Remove(0,selfRadioAddTrack_edit.LastIndexOf('\\') + 1);

                    if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Rockstar Games\GTA V\User Music\" + selfRadioAddTrack_edit)) {
                        File.Copy(selfTrack, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Rockstar Games\GTA V\User Music\" + selfRadioAddTrack_edit);
                        selfRadio_Load();
                    } else {
                        MessageBox.Show("A track with the same name already exists...", selfRadioAddTrack_edit, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            selfRadioTracks_lbl.Focus();
        }
        #endregion
        #region [SELF RADIO TAB] Remove track button click
        private void selfRadioRemove_btn_Click(object sender, EventArgs e) {
            if (selfRadio_listb.SelectedIndex != -1 && selfRadio_listb.Items.Count >= 1 && selfRadio_listb.SelectedItem.ToString() != "Empty") {
                if (MessageBox.Show("Delete this track?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes) {
                    for (int i = selfRadio_listb.SelectedItems.Count; i > 0; i--) {
                        File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Rockstar Games\GTA V\User Music\" + selfRadio_listb.SelectedItem);
                        selfRadio_listb.Items.RemoveAt(selfRadio_listb.SelectedIndex);
                    }
                    selfRadio_Load();
                }
            }
            selfRadioTracks_lbl.Focus();
        }
        #endregion
        #region [SELF RADIO TAB] Rename track button click
        private void selfRadioRename_btn_Click(object sender, EventArgs e) {
            if (selfRadio_listb.SelectedItem.ToString() != "Empty" && selfRadio_listb.SelectedIndex != -1 && MessageBox.Show("Do you want to rename this track?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                string selfRadioRenameTrack_ext = selfRadio_listb.SelectedItem.ToString();
                selfRadioRenameTrack_ext = selfRadioRenameTrack_ext.Remove(0, selfRadioRenameTrack_ext.LastIndexOf('.'));
                File.Move(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Rockstar Games\GTA V\User Music\" + selfRadio_listb.SelectedItem, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Rockstar Games\GTA V\User Music\" + selfRadioRename_txtbox.Text + selfRadioRenameTrack_ext);
                selfRadioRename_txtbox.Text = null;
                selfRadio_Load();
            }
            selfRadioTracks_lbl.Focus();
        }
        #endregion
        #region [SELF RADIO TAB] Rename textbox check text
        private void selfRadioRename_txtbox_TextChanged(object sender, EventArgs e) {
            if (selfRadioRename_txtbox.Text == "") { selfRadioRename_btn.Enabled = false; } else { selfRadioRename_btn.Enabled = true; }
        }
        #endregion
        #region [SELF RADIO TAB] Play/Stop track button click
        private void selfRadioPlayStop_btn_Click(object sender, EventArgs e) {
            if (selfRadio_listb.SelectedIndex != -1 && selfRadio_listb.Items.Count >= 1 && selfRadio_listb.SelectedItem.ToString() != "Empty" || selfRadioPlayStop_btn.Text == "Stop") {
                if (selfRadioPlayStop_btn.Text == "Play Track") {
                    selfRadioTrackPlay.Open(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Rockstar Games\GTA V\User Music\" + selfRadio_listb.SelectedItem));
                    selfRadioTrackPlay.Play();
                    selfRadioPlayStop_btn.Text = "Stop";
                    selfRadioRemove_btn.Enabled = false;
                    selfRadioRename_txtbox.Text = selfRadio_listb.SelectedItem.ToString();
                    selfRadioRename_txtbox.ReadOnly = true;
                    selfRadioRename_btn.Enabled = false;
                } else { selfRadioTrackPlay.Stop(); selfRadioTrackPlay.Close(); selfRadioPlayStop_btn.Text = "Play Track"; selfRadioRemove_btn.Enabled = true;
                    selfRadioRename_txtbox.Text = null;
                    selfRadioRename_txtbox.ReadOnly = false;
                }
            }
            selfRadioTracks_lbl.Focus();
        }
        #endregion
        #region [SELF RADIO TAB] Reload track list button click
        private void selfRadioReload_btn_Click(object sender, EventArgs e) {
            selfRadio_Load(); selfRadioTracks_lbl.Focus();

            selfRadioAdd_btn.Enabled = true;
            selfRadioRename_txtbox.Enabled = true;
            if (selfRadioRename_txtbox.Text != "" && selfRadioPlayStop_btn.Text == "Play") { selfRadioRename_btn.Enabled = true; }
            selfRadioPlayStop_btn.Enabled = true;
        }
        
        #endregion
        #region [SELF RADIO TAB] Track list check track selected
        private void selfRadio_listb_SelectedIndexChanged(object sender, EventArgs e) {
            if (selfRadio_listb.SelectedItems.Count > 1) {
                selfRadioAdd_btn.Enabled = false;
                selfRadioRename_txtbox.Enabled = false;
                selfRadioRename_btn.Enabled = false;
                selfRadioPlayStop_btn.Enabled = false;
            } else {
                selfRadioAdd_btn.Enabled = true;
                selfRadioRename_txtbox.Enabled = true;
                if (selfRadioRename_txtbox.Text != "" && selfRadioPlayStop_btn.Text == "Play") { selfRadioRename_btn.Enabled = true; }
                selfRadioPlayStop_btn.Enabled = true;
            }
        }
        #endregion

        #region Apply changes to the GTAV Launcher process
        private void timetowait_timer_Tick(object sender, EventArgs e) {
            if (playgtav_button.Enabled == false) { playgtav_button.Enabled = true; }
            if (waitTime_progressbar.Value != waitTime_progressbar.Maximum) { waitTime_progressbar.Value++; } else if (!File.Exists(selfGTAV_dirPath + @"\scripts\gtav.momlauncher")) { waitTime_progressbar.Maximum += 250; }
            if (File.Exists(selfGTAV_dirPath + @"\scripts\gtav.momlauncher")) {
                waitTime_progressbar.Value = waitTime_progressbar.Maximum;
                Process gtav_process = Process.GetProcessesByName("GTA5").First();
                #region GTAV.exe Set Priority
                switch (cpupriority_combobox.SelectedIndex) {
                    case 0:
                        gtav_process.PriorityClass = ProcessPriorityClass.RealTime;
                        break;
                    case 1:
                        gtav_process.PriorityClass = ProcessPriorityClass.High;
                        break;
                    case 2:
                        gtav_process.PriorityClass = ProcessPriorityClass.AboveNormal;
                        break;
                    case 3:
                        gtav_process.PriorityClass = ProcessPriorityClass.Normal;
                        break;
                    case 4:
                        gtav_process.PriorityClass = ProcessPriorityClass.BelowNormal;
                        break;
                    case 5:
                        gtav_process.PriorityClass = ProcessPriorityClass.Idle;
                        break;
                 }
                #endregion            
                Process gtavLauncher_process = Process.GetProcessesByName("GTAVLauncher").First();
                if (stoplauncher_combobox.Text == "No") { gtavLauncher_process.PriorityClass = ProcessPriorityClass.Idle; }
                selfPPCPUAffinity_converter();
                int gtav_affinity = Convert.ToInt32(gta5CPUAffinity_int);
                gtav_process.ProcessorAffinity = (IntPtr)gtav_affinity;
                if (stoplauncher_combobox.SelectedIndex == 0) { gtavLauncher_process.Kill(); }
                File.Delete(selfGTAV_dirPath + @"\scripts\gtav.momlauncher");
                

                if (autoclose_combobox.SelectedIndex == 0) { Application.Exit(); } else { timetowait_timer.Stop(); waitTime_progressbar.Value = 0;
                    enableControls(true);
                    if (saveChangeActive_aftergamelaunching == false) { savechanges_button.Enabled = false; }
                    self_cancelLaunching = false;
                    playgtav_button.Text = "Launch Game";

                    #region RAM Cleaner
                    #region [RAM Cleaner] Processes Path
                    string ramc_steam_path = selfRAMCleaner_GetProcessPath("Steam");
                    string ramc_reader_sl_path = selfRAMCleaner_GetProcessPath("reader_sl");
                    string ramc_jqs_path = selfRAMCleaner_GetProcessPath("jqs");
                    string ramc_adobearm_path = selfRAMCleaner_GetProcessPath("AdobeARM");
                    string ramc_aamupnotf_path = selfRAMCleaner_GetProcessPath("AAM Update Notifier");
                    string ramc_jusched_path = selfRAMCleaner_GetProcessPath("Jusched");
                    string ramc_divxupdate_path = selfRAMCleaner_GetProcessPath("DivXUpdate");
                    string ramc_nero_path = selfRAMCleaner_GetProcessPath("NeroCheck");
                    string ramc_hkcmd_path = selfRAMCleaner_GetProcessPath("HKCMD");
                    string ramc_ati0_path = selfRAMCleaner_GetProcessPath("atiptaxx");
                    string ramc_ati1_path = selfRAMCleaner_GetProcessPath("ati2evxx");
                    string ramc_ravcpl_path = selfRAMCleaner_GetProcessPath("RAVCpl64");
                    string ramc_nwiz_path = selfRAMCleaner_GetProcessPath("Nwiz");
                    string ramc_ccc_path = selfRAMCleaner_GetProcessPath("CCC");
                    string ramc_radeonsettings_path = selfRAMCleaner_GetProcessPath("RadeonSettings");
                    string ramc_searchindex_path = selfRAMCleaner_GetProcessPath("SearchIndexer");
                    string ramc_cortana_path = selfRAMCleaner_GetProcessPath("SearchUI");
                    #endregion

                    bool gtaWaitExit = false;
                    RegistryKey regKey_explorer = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default).OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon", RegistryKeyPermissionCheck.ReadWriteSubTree);
                    if (ramc_explorer_checkbox.Checked == true) {
                        gtaWaitExit = true;
                        #region Disable 'explorer' registery key
                        if (regKey_explorer.GetValue("AutoRestartShell").ToString() == "1") { regKey_explorer.SetValue("AutoRestartShell", 0, RegistryValueKind.DWord); }
                        #endregion
                        Process.GetProcessesByName("explorer").First().Kill();
                    }
                    if (ramc_chrome_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process[] chrome_tasks = Process.GetProcessesByName("chrome");
                            foreach (Process i in chrome_tasks) { i.Kill(); }
                        } catch { }
                    }
                    if (ramc_steam_checkbox.Checked == true && ramc_steam_checkbox.Enabled == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("Steam").First().Kill();
                        } catch { }
                    }
                    if (ramc_reader_sl_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("reader_sl").First().Kill();
                        } catch { }
                    }
                    if (ramc_jqs_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("jqs").First().Kill();
                        } catch { }
                    }
                    if (ramc_adobearm_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("AdobeARM").First().Kill();
                        } catch { }
                    }
                    if (ramc_aamupnotf_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("AAM Update Notifier").First().Kill();
                        } catch { }
                    }
                    if (ramc_jusched_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("Jusched").First().Kill();
                        } catch { }
                    }
                    if (ramc_divxupdate_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("DivXUpdate").First().Kill();
                        } catch { }
                    }
                    if (ramc_nero_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("NeroCheck").First().Kill();
                        } catch { }
                    }
                    if (ramc_hkcmd_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("HKCMD").First().Kill();
                        } catch { }
                    }
                    if (ramc_ati0_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("atiptaxx").First().Kill();
                        } catch { }
                    }
                    if (ramc_ati1_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("ati2evxx").First().Kill();
                        } catch { }
                    }
                    if (ramc_ravcpl_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("RAVCpl64").First().Kill();
                        } catch { }
                    }
                    if (ramc_nwiz_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("Nwiz").First().Kill();
                        } catch { }
                    }
                    if (ramc_ccc_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("CCC").First().Kill();
                        } catch { }
                    }
                    if (ramc_radsett_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("RadeonSettings").First().Kill();
                        } catch { }
                    }
                    if (ramc_searchind_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("SearchIndexer").First().Kill();
                        } catch { }
                    }
                    if (ramc_cortana_checkbox.Checked == true) {
                        try {
                            gtaWaitExit = true;
                            Process.GetProcessesByName("SearchUI").First().Kill();
                            Process.GetProcessesByName("SearchIndexer").First().Kill();
                            System.Threading.Thread.Sleep(1000);
                            Directory.Move(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\SystemApps\Microsoft.Windows.Cortana_cw5n1h2txyewy", Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\SystemApps\Microsoft.Windows.Cortana_cw5n1h2txyewy_MOMLauncher");
                        } catch { }
                    }

                    if (gtaWaitExit == true) {
                        Hide();
                        gtav_process.WaitForExit();
                        Show();
                        if (ramc_explorer_checkbox.Checked == true) {
                            #region Enable 'explorer' registery key
                            if (regKey_explorer.GetValue("AutoRestartShell").ToString() == "0") { regKey_explorer.SetValue("AutoRestartShell", 1, RegistryValueKind.DWord); }
                            #endregion
                            Process.Start("explorer");
                        }
                        if (ramc_steam_checkbox.Checked == true && ramc_steam_checkbox.Enabled == true) {
                            try { Process.Start(ramc_steam_path); } catch { }
                        }
                        if (ramc_reader_sl_checkbox.Checked == true) {
                            try { Process.Start(ramc_reader_sl_path); } catch { }
                        }
                        if (ramc_jqs_checkbox.Checked == true) {
                            try { Process.Start(ramc_jqs_path); } catch { }
                        }
                        if (ramc_adobearm_checkbox.Checked == true) {
                            try { Process.Start(ramc_adobearm_path); } catch { }
                        }
                        if (ramc_aamupnotf_checkbox.Checked == true) {
                            try { Process.Start(ramc_aamupnotf_path); } catch { }
                        }
                        if (ramc_jusched_checkbox.Checked == true) {
                            try { Process.Start(ramc_jusched_path); } catch { }
                        }
                        if (ramc_divxupdate_checkbox.Checked == true) {
                            try { Process.Start(ramc_divxupdate_path); } catch { }
                        }
                        if (ramc_nero_checkbox.Checked == true) {
                            try { Process.Start(ramc_nero_path); } catch { }
                        }
                        if (ramc_hkcmd_checkbox.Checked == true) {
                            try { Process.Start(ramc_hkcmd_path); } catch { }
                        }
                        if (ramc_ati0_checkbox.Checked == true) {
                            try { Process.Start(ramc_ati0_path); } catch { }
                        }
                        if (ramc_ati1_checkbox.Checked == true) {
                            try { Process.Start(ramc_ati1_path); } catch { }
                        }
                        if (ramc_ravcpl_checkbox.Checked == true) {
                            try { Process.Start(ramc_ravcpl_path); } catch { }
                        }
                        if (ramc_nwiz_checkbox.Checked == true) {
                            try { Process.Start(ramc_nwiz_path); } catch { }
                        }
                        if (ramc_ccc_checkbox.Checked == true) {
                            try { Process.Start(ramc_ccc_path); } catch { }
                        }
                        if (ramc_radsett_checkbox.Checked == true) {
                            try { Process.Start(ramc_radeonsettings_path); } catch { }
                        }
                        if (ramc_searchind_checkbox.Checked == true) {
                            try { Process.Start(ramc_searchindex_path); } catch { }
                        }
                        if (ramc_cortana_checkbox.Checked == true) {
                            try {
                                Directory.Move(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\SystemApps\Microsoft.Windows.Cortana_cw5n1h2txyewy_MOMLauncher", Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\SystemApps\Microsoft.Windows.Cortana_cw5n1h2txyewy");
                                System.Threading.Thread.Sleep(1000);
                                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\SystemApps\Microsoft.Windows.Cortana_cw5n1h2txyewy\SearchUI.exe");
                                Process.Start("SearchIndexer"); } catch { }
                        }
                    }
                    #endregion
                }
            }
        }
        #endregion

        #region Links
        private void update_btn_Click(object sender, EventArgs e) {
            selfCheckUpdate(true, false);
            waitTime_progressbar.Focus();

        }
        private void steam_lnk_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("http://steamcommunity.com/id/Xxshark888xX/");
        }
        #endregion
        #region Lose focus buttons etc.
        private void tabControl1_TabIndexChanged(object sender, EventArgs e) {
            if (skipSelectionChanged) { return; }
            this.SelectTabWithoutFocus(this.tabControl1.TabPages[0]);
        }

        #endregion
        #region Check if are some changes
        private void gtavdir_txtbox_TextChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void gtavdir_txtbox_TextChanged_1(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void cpupriority_combobox_TextChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void cpuaffinity_textbox_TextChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void selfPP_enableKeySet_txtbox_TextChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void stoplauncher_combobox_SelectedIndexChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void autoclose_combobox_SelectedIndexChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; if (autoclose_combobox.Text == "Yes") { ramc_groupbox.Enabled = false; } else { ramc_groupbox.Enabled = true; } }
        private void steam_radiobutton_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; if (steam_radiobutton.Checked == true) { ramc_steam_checkbox.Enabled = false; } else { ramc_steam_checkbox.Enabled = true; } }
        private void retail_radiobutton_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void newUpdate_cbx_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        #region [CPU Affinity]
        private void c1_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void c2_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void c3_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void c4_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void c5_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void c6_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void c7_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void c8_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        #endregion
        #region RAMCleaner
        private void ramc_explorer_checkbox_CheckedChanged_1(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_chrome_checkbox_CheckedChanged_1(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_steam_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_reader_sl_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_jqs_checkbox_CheckedChanged(object sender, EventArgs e) {savechanges_button.Enabled = true; }
        private void ramc_adobearm_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_aamupnotf_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_jusched_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_divxupdate_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_nero_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_hkcmd_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_ati0_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_ati1_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_ravcpl_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_nwiz_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_ccc_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_radsett_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_searchind_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        private void ramc_cortana_checkbox_CheckedChanged(object sender, EventArgs e) { savechanges_button.Enabled = true; }
        #endregion
        #endregion


        #region Check if changes are saved
        protected override void OnFormClosing(FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing) {
                if (savechanges_button.Enabled == true) {
                    if (MessageBox.Show("Do you want to exit without saving?", "Unsaved changes!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) {
                        e.Cancel = true;
                    }
                }
            }
            Application.Exit();
        }
        #endregion

        #region Check for new updates
        private void updateCheck_timer_Tick(object sender, EventArgs e) {
            updateCheck_timer.Stop();
            playgtav_button.Enabled = true;
            selfCheckUpdate(false, false);
        }
        #endregion

        private void update_noticon_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left && newUpdate_cbx.Checked == true) {
                update_noticon.Visible = false;
                selfCheckUpdate(false, true);
            }
        }
    }
}