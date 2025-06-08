﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FortyOne.AudioSwitcher.Configuration;
using FortyOne.AudioSwitcher.PresetData;

namespace FortyOne.AudioSwitcher.HotKeyData
{
    public static class HotKeyManager
    {
        private static readonly List<HotKey> _hotkeys = new List<HotKey>();
        public static BindingList<HotKey> HotKeys = new BindingList<HotKey>();

        static HotKeyManager()
        {
            LoadHotKeys();
            RefreshHotkeys();
        }

        public static event EventHandler HotKeyPressed;

        public static void ClearAll()
        {
            foreach (var hk in _hotkeys)
            {
                hk.UnregisterHotkey();
            }

            Program.Settings.HotKeys = "";
            LoadHotKeys();
            RefreshHotkeys();
        }

        public static void LoadHotKeys()
        {
            try
            {
                foreach (var hk in _hotkeys)
                {
                    hk.UnregisterHotkey();
                }

                _hotkeys.Clear();

                var hotkeydata = Program.Settings.HotKeys;
                if (string.IsNullOrEmpty(hotkeydata))
                {
                    RefreshHotkeys();
                    return;
                }

                var entries = hotkeydata.Split(new[] { ",", "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < entries.Length; i++)
                {
                    var key = int.Parse(entries[i++]);
                    var modifiers = int.Parse(entries[i++]);
                    var hk = new HotKey();

                    var r = new Regex(ConfigurationSettings.GUID_REGEX);
                    var matches = r.Matches(entries[i]);
                    if (matches.Count == 0)
                        continue;
                    hk.DeviceId = new Guid(matches[0].ToString());

                    hk.Modifiers = (Modifiers)modifiers;
                    hk.Key = (Keys)key;
                    _hotkeys.Add(hk);
                    hk.HotKeyPressed += hk_HotKeyPressed;
                    hk.RegisterHotkey();
                }
            }
            catch
            {
                Program.Settings.HotKeys = "";
            }
        }

        private static void hk_HotKeyPressed(object sender, EventArgs e)
        {
            if (HotKeyPressed != null)
                HotKeyPressed(sender, e);
        }

        public static void SaveHotKeys()
        {
            var hotkeydata = "";
            foreach (var hk in _hotkeys)
            {
                hotkeydata += "[" + (int)hk.Key + "," + (int)hk.Modifiers + "," + hk.DeviceId + "]";
            }
            Program.Settings.HotKeys = hotkeydata;

            RefreshHotkeys();
        }

        public static bool AddHotKey(HotKey hk)
        {
            //Check that there is no duplicate
            if (DuplicateHotKey(hk))
                return false;

            hk.HotKeyPressed += hk_HotKeyPressed;
            hk.RegisterHotkey();

            if (!hk.IsRegistered)
                return false;

            _hotkeys.Add(hk);

            SaveHotKeys();
            return true;
        }

        public static void RefreshHotkeys()
        {
            HotKeys.Clear();
            var filterInvalid = !Program.Settings.ShowUnknownDevicesInHotkeyList;
            IEnumerable<HotKey> hotkeyList = _hotkeys;
            if (filterInvalid)
                hotkeyList = hotkeyList.Where(x => x.Device != null);
            
            foreach (var k in hotkeyList)
            {
                HotKeys.Add(k);
            }
        }

        public static bool DuplicateHotKey(HotKey hk)
        {
            return _hotkeys.Any(k => hk.Key == k.Key && hk.Modifiers == k.Modifiers);
        }

        public static void DeleteHotKey(HotKey hk)
        {
            //Ensure its unregistered
            hk.UnregisterHotkey();
            _hotkeys.Remove(hk);
            SaveHotKeys();
        }

        public static void DeleteHotKeyByGuid(Guid deviceGuid)
        {
            foreach (var hk in _hotkeys)
            {
                if (hk.DeviceId == deviceGuid)
                {
                    DeleteHotKey(hk);
                    break;
                }
            }
        }

        public static void UpdateHotKeyGuid(Guid oldGuid, Guid newGuid)
        {
            for (int i = 0; i < _hotkeys.Count; i++)
            {
                if (_hotkeys[i].DeviceId == oldGuid)
                {
                    _hotkeys[i].DeviceId = newGuid;
                    break; // Exit the loop as we've updated the preset
                }
            }
            SaveHotKeys();
            RefreshHotkeys();
        }
    }
}