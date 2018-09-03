#region [REFERENCES]
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
#endregion
public class MOMLauncher : Script {

    #region [OWN SCRIPTS]
    private void selfGetCharacter_Name() {
        if (Game.Player.Character.Model == PedHash.Michael) { selfCharName = "Michael"; }
        if (Game.Player.Character.Model == PedHash.Franklin) { selfCharName = "Franklin"; }
        if (Game.Player.Character.Model == PedHash.Trevor) { selfCharName = "Trevor"; }
    }
    private void PP_getGTAVProcesses() {
        Process[] selfGetProcesses = Process.GetProcesses();
        foreach (Process _proc in selfGetProcesses) {
                if (_proc.ProcessName == "GTA5") { PP_GTAV = Process.GetProcessById(_proc.Id); }
                if (selfGTAVLauncher_stopped == "No" && _proc.ProcessName == "GTAVLauncher") { PP_GTAVLauncher = Process.GetProcessById(_proc.Id); }
            }
        Wait(2500);
    }
    #endregion
    #region [VARIABLES]
    bool PP_first = false;
    bool PP_enabled = false;
    bool PP_enabledNotify = false;
    Process PP_GTAV;
    Process PP_GTAVLauncher;

    
    string PP_priority = "";
    string PP_affinityBin;
    Int64 PP_affinityInt64;
    int PP_affinityInt;
    string selfGTAVLauncher_stopped;
    Keys PP_IngameEnable;

    string selfCharName;
    #endregion

    public MOMLauncher() {
        Tick += onTick;
        KeyDown += onKeyDown;
    }

    void onTick(object sender, EventArgs e) {
            if (PP_first == false && File.Exists(@"scripts\ppInfo.momlauncher")) {
            if (Game.Player.Character.IsWalking == true) {
                PP_first = true;
                var selfMOMFile = File.Create(@"scripts\gtav.momlauncher");
                selfMOMFile.Close();
                using (StreamReader selfFile = new StreamReader(@"scripts\ppInfo.momlauncher")) {
                    PP_priority = selfFile.ReadLine();
                    PP_affinityBin = selfFile.ReadLine();
                    selfGTAVLauncher_stopped = selfFile.ReadLine();
                    PP_IngameEnable = (Keys)Enum.Parse(typeof(Keys), selfFile.ReadLine());
                    selfFile.Close();
                    selfFile.Dispose();
                }
                File.Delete(@"scripts\ppInfo.momlauncher");

                PP_getGTAVProcesses();
                if (PP_GTAV.PriorityClass.ToString() != PP_priority) {
                    BigMessageThread.MessageInstance.ShowColoredShard("[MOM Launcher]","Something bad has happened, the Performance Process has failed!",HudColor.HUD_COLOUR_BLACK,HudColor.HUD_COLOUR_RED, 10 * 1000);
                }  else {
                    BigMessageThread.MessageInstance.ShowColoredShard("[MOM Launcher]","Performance Process successfully executed",HudColor.HUD_COLOUR_BLACK,HudColor.HUD_COLOUR_GREEN, 10 * 1000);
                    PP_enabled = true;
                }
            } else {
                selfGetCharacter_Name();
                BigMessageThread.MessageInstance.ShowColoredShard("[MOM Launcher]","Performance Process ready!\r\nMove " + selfCharName + " to start the P.P.",HudColor.HUD_COLOUR_BLACK,HudColor.HUD_COLOUR_ORANGE);
            }
        }

        if (PP_enabledNotify == true) {
            if (PP_enabled == true) {
                BigMessageThread.MessageInstance.ShowColoredShard("[MOM Launcher]","Performance Process enabled",HudColor.HUD_COLOUR_BLACK,HudColor.HUD_COLOUR_GREEN, 5 * 1000);
            } else {
                BigMessageThread.MessageInstance.ShowColoredShard("[MOM Launcher]","Performance Process disabled",HudColor.HUD_COLOUR_BLACK,HudColor.HUD_COLOUR_RED, 5 * 1000);
            }
            PP_enabledNotify = false;
        }
    }

    void onKeyDown(object sender, KeyEventArgs e) {
        if (e.KeyCode == PP_IngameEnable) {
            PP_enabledNotify = true;
            if (PP_enabled == false) {
                #region [SET PRIORITY]
                switch (PP_priority) {
                    case "Real Time":
                        PP_GTAV.PriorityClass = ProcessPriorityClass.RealTime;
                        break;
                    case "High":
                        PP_GTAV.PriorityClass = ProcessPriorityClass.High;
                        break;
                    case "Above Normal":
                        PP_GTAV.PriorityClass = ProcessPriorityClass.AboveNormal;
                        break;
                    case "Normal":
                        PP_GTAV.PriorityClass = ProcessPriorityClass.Normal;
                        break;
                    case "Below Normal":
                        PP_GTAV.PriorityClass = ProcessPriorityClass.BelowNormal;
                        break;
                    case "Low":
                        PP_GTAV.PriorityClass = ProcessPriorityClass.Idle;
                        break;
                }
                #endregion
                PP_affinityInt64 = Convert.ToInt64(PP_affinityBin, 2);
                PP_affinityInt = Convert.ToInt32(PP_affinityInt64);
                PP_GTAV.ProcessorAffinity = (IntPtr)PP_affinityInt;
                if (selfGTAVLauncher_stopped == "No") { PP_GTAVLauncher.PriorityClass = ProcessPriorityClass.Idle; }
                PP_enabled = true;
            } else {
                PP_GTAV.PriorityClass = ProcessPriorityClass.Normal;
                #region [SET NORMAL AFFINITY]
                if (Environment.ProcessorCount == 2) { PP_GTAV.ProcessorAffinity = (IntPtr)3; }
                if (Environment.ProcessorCount == 4) { PP_GTAV.ProcessorAffinity = (IntPtr)15; }
                if (Environment.ProcessorCount == 6) { PP_GTAV.ProcessorAffinity = (IntPtr)63; }
                if (Environment.ProcessorCount == 8) { PP_GTAV.ProcessorAffinity = (IntPtr)255; }
                #endregion
                if (selfGTAVLauncher_stopped == "No") { PP_GTAVLauncher.PriorityClass = ProcessPriorityClass.Normal; }
                PP_enabled = false;
            }
        }
    }
}