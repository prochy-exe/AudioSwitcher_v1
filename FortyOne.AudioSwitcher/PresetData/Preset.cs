using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using AudioSwitcher.AudioApi;
using FortyOne.AudioSwitcher.Helpers;

namespace FortyOne.AudioSwitcher.PresetData
{
    public class Preset
    {
        /// <summary>
        ///     Register a hotkey combination of one or more Modifers and a Key.
        /// </summary>
        /// <remarks>
        ///     This method uses the values of the Modifiers and Key properties.  If
        ///     Modifiers == Modifiers.None and Key = Keys.None then any current hotkey combination
        ///     represented by this instance is deactivated.  Calling this method a subsequent time with
        ///     a different combination will cause the current combination to be replaced and the new hotkey
        ///     combination installed.  If the hotkey registration process fails (for example the hotkey combination
        ///     is already registered) then a Win32Exception is thrown.
        /// </remarks>

        public Preset()
        {
        }

        /// <summary>
        ///     The playback deviceID the preset is using
        /// </summary>
        public Guid PlaybackDeviceId { get; set; }

        /// <summary>
        ///     The recording deviceID the preset is using
        /// </summary>
        public Guid RecordingDeviceId { get; set; }

        public Guid PresetId 
        { 
            get
            {
                byte[] playbackBytes = PlaybackDeviceId.ToByteArray();
                byte[] recordingBytes = RecordingDeviceId.ToByteArray();

                byte[] presetGuid = new byte[16];
                for (int x = 0; x < 8; x++)
                {
                    presetGuid[x] = playbackBytes[x];
                    presetGuid[x + 8] = recordingBytes[x];
                }
                return new Guid(presetGuid);
            }
        }

        public int PresetNumber { get; set; }

        public IDevice PlaybackDevice
        {
            get { return AudioDeviceManager.Controller.GetDevice(PlaybackDeviceId); }
        }

        public IDevice RecordingDevice
        {
            get { return AudioDeviceManager.Controller.GetDevice(RecordingDeviceId); }
        }

        public string PlaybackDeviceName
        {
            get
            {
                if (PlaybackDevice == null)
                    return "Unknown Device";
                return PlaybackDevice.FullName;
            }
        }

        public string RecordingDeviceName
        {
            get
            {
                if (RecordingDevice == null)
                    return "Unknown Device";
                return RecordingDevice.FullName;
            }
        }

        public async void SetActive()
        {
            await PlaybackDevice.SetAsDefaultAsync();
            await RecordingDevice.SetAsDefaultAsync();
            if (Program.Settings.DualSwitchMode)
            {
                await PlaybackDevice.SetAsDefaultCommunicationsAsync();
                await RecordingDevice.SetAsDefaultCommunicationsAsync();
            }
            if (Program.Settings.DeviceNotificationsEnabled)
                NotificationHelper.SendToastNotification(PlaybackDeviceName, PlaybackDevice.DeviceType.ToString(),
                    RecordingDeviceName, RecordingDevice.DeviceType.ToString());
        }
    }
}