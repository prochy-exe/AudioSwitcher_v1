using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AudioSwitcher.AudioApi;
using FortyOne.AudioSwitcher.Configuration;
using FortyOne.AudioSwitcher.HotKeyData;

namespace FortyOne.AudioSwitcher.PresetData
{
    public static class PresetManager
    {
        private static readonly List<Preset> _presets = new List<Preset>();
        public static BindingList<Preset> Presets = new BindingList<Preset>();

        static PresetManager()
        {
            LoadPresets();
            RefreshPresets();
        }

        public static void ClearAll()
        {

            Program.Settings.Presets = "";
            LoadPresets();
            RefreshPresets();
        }

        public static List<Preset> GetPresets()
        {
            try
            {

                var presets = new List<Preset>();

                var presetData = Program.Settings.Presets;
                if (string.IsNullOrEmpty(presetData))
                {
                    return presets;
                }

                var entries = presetData.Split(new[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < entries.Length; i++)
                {
                    var pr = new Preset();

                    var devices = entries[i].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    pr.PlaybackDeviceId = new Guid(devices[0].ToString());
                    pr.RecordingDeviceId = new Guid(devices[1].ToString());
                    pr.PresetNumber = i + 1;

                    presets.Add(pr);
                }
                return presets;
            }
            catch (Exception ex)
            {
                // Log or inspect the exception details
                Console.WriteLine($"Exception caught: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Optionally, you can log more details about the exception type
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return new List<Preset>();
            }
        }

        public static Preset GetPresetByIndex(int presetNumber)
        {
            try
            {

                var presetData = Program.Settings.Presets;

                var entries = presetData.Split(new[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < entries.Length; i++)
                {
                    if (i == presetNumber)
                    {
                        var pr = new Preset();

                        var devices = entries[i].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        pr.PlaybackDeviceId = new Guid(devices[0].ToString());
                        pr.RecordingDeviceId = new Guid(devices[1].ToString());
                        pr.PresetNumber = i + 1;

                        return pr;
                    }
                }
                return new Preset();
            }
            catch (Exception ex)
            {
                // Log or inspect the exception details
                Console.WriteLine($"Exception caught: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Optionally, you can log more details about the exception type
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return new Preset();
            }
        }

        public static Preset GetPresetByGuid(Guid lookupPresetGuid)
        {
            try
            {

                var presetData = Program.Settings.Presets;

                var entries = presetData.Split(new[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < entries.Length; i++)
                {
                    var pr = new Preset();

                    var devices = entries[i].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    pr.PlaybackDeviceId = new Guid(devices[0].ToString());
                    pr.RecordingDeviceId = new Guid(devices[1].ToString());
                    pr.PresetNumber = i + 1;

                    if (pr.PresetId == lookupPresetGuid)
                        return pr;
                }
                return new Preset();
            }
            catch (Exception ex)
            {
                // Log or inspect the exception details
                Console.WriteLine($"Exception caught: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Optionally, you can log more details about the exception type
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return new Preset();
            }
        }

        public static Boolean IsPreset(Guid lookupPresetGuid)
        {
            try
            {

                var presetData = Program.Settings.Presets;

                var entries = presetData.Split(new[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < entries.Length; i++)
                {
                    var pr = new Preset();

                    var devices = entries[i].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    pr.PlaybackDeviceId = new Guid(devices[0].ToString());
                    pr.RecordingDeviceId = new Guid(devices[1].ToString());
                    pr.PresetNumber = i + 1;

                    if (pr.PresetId == lookupPresetGuid)
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log or inspect the exception details
                Console.WriteLine($"Exception caught: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Optionally, you can log more details about the exception type
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        public static void LoadPresets()
        {
            try
            {

                _presets.Clear();

                var presetData = Program.Settings.Presets;
                if (string.IsNullOrEmpty(presetData))
                {
                    RefreshPresets();
                    return;
                }

                var entries = presetData.Split(new[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < entries.Length; i++)
                {
                    var pr = new Preset();
                    var devices = entries[i].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    pr.PlaybackDeviceId = new Guid(devices[0].ToString());
                    pr.RecordingDeviceId = new Guid(devices[1].ToString());
                    pr.PresetNumber = i + 1;

                    _presets.Add(pr);
                }
            }
            catch (Exception ex)
            {
                // Log or inspect the exception details
                Console.WriteLine($"Exception caught: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Optionally, you can log more details about the exception type
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                Program.Settings.Presets = "";
            }
        }

        public static void SavePresets()
        {
            var presetData = "";
            foreach (var pr in _presets)
            {
                presetData += "[" + pr.PlaybackDeviceId + "," + pr.RecordingDeviceId + "]";
            }
            Program.Settings.Presets = presetData;

            RefreshPresets();
        }

        public static bool AddPreset(Preset pr)
        {
            //Check that there is no duplicate
            if (DuplicatePreset(pr))
                return false;

            _presets.Add(pr);

            SavePresets();
            return true;
        }

        public static void RefreshPresets()
        {
            Presets.Clear();
            var filterInvalid = !Program.Settings.ShowUnknownDevicesInHotkeyList;
            IEnumerable<Preset> presetList = _presets;
            if (filterInvalid)
                presetList = presetList.Where(x => x.PlaybackDeviceId != null && x.PlaybackDeviceId != null);
            
            foreach (var p in presetList)
            {
                Presets.Add(p);
            }
        }

        public static bool DuplicatePreset(Preset pr)
        {
            return _presets.Any(p => pr.PlaybackDeviceId == p.PlaybackDeviceId && pr.RecordingDeviceId == p.RecordingDeviceId);
        }

        public static void DeletePreset(Preset pr)
        {
            //Ensure its unregistered
            _presets.Remove(pr);
            HotKeyManager.DeleteHotKeyByGuid(pr.PresetId);
            SavePresets();
        }

        public static void ModifyPreset(Preset pr, Guid oldPresetGuid)
        {
            Preset oldPreset = GetPresetByGuid(oldPresetGuid);
            for (int i = 0; i < _presets.Count; i++)
            {
                if (_presets[i] == oldPreset)
                {
                    _presets[i] = pr;
                    break;
                }
            }
            SavePresets();
            RefreshPresets();
            HotKeyManager.UpdateHotKeyGuid(oldPresetGuid, pr.PresetId);
        }

        public static Preset GetNextPreset(IDevice defaultPlayback, IDevice defaultRecording)
        {
            var i = 0;
            foreach (var pr in _presets)
            {
                if (pr.PlaybackDevice == defaultPlayback && pr.RecordingDevice == defaultRecording)
                    break;
                i++;
            }
            i = i + 1;
            if (i < _presets.Count)
            {
                return _presets[i];
            }
            else
            {
                return _presets[0];
            }
        }
    }
}