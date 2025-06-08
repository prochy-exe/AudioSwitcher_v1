using System;
using System.Windows.Forms;
using AudioSwitcher.AudioApi;
using FortyOne.AudioSwitcher.PresetData;
using FortyOne.AudioSwitcher.Helpers;
using System.Collections.Generic;

namespace FortyOne.AudioSwitcher
{
    public enum PresetFormMode
    {
        Normal,
        Edit
    }

    public partial class PresetForm : Form
    {
        private readonly Preset _preset;
        private readonly PresetFormMode _mode = PresetFormMode.Normal;
        private readonly Guid oldPresetId;
        private DeviceState _deviceStateFilter = DeviceState.Active;

        public PresetForm()
        {
            InitializeComponent();

            _preset = new Preset();

            // Keep in mind how the user wants the devices shown
            if (Program.Settings.ShowDisabledDevices)
                _deviceStateFilter |= DeviceState.Disabled;

            if (Program.Settings.ShowDisconnectedDevices)
                _deviceStateFilter |= DeviceState.Unplugged;

            cmbPlaybackDevices.Items.Clear();
            cmbRecordingDevices.Items.Clear();

            foreach (var ad in AudioDeviceManager.Controller.GetPlaybackDevices(_deviceStateFilter))
                cmbPlaybackDevices.Items.Add(ad);

            foreach (var ad in AudioDeviceManager.Controller.GetCaptureDevices(_deviceStateFilter))
                cmbRecordingDevices.Items.Add(ad);

            cmbPlaybackDevices.DisplayMember = "FullName";
            cmbPlaybackDevices.ValueMember = "ID";
            cmbRecordingDevices.DisplayMember = "FullName";
            cmbRecordingDevices.ValueMember = "ID";
        }

        public PresetForm(Preset pr)
            : this()
        {
            _preset = pr;

            _preset.PlaybackDeviceId = pr.PlaybackDeviceId;
            _preset.RecordingDeviceId = pr.RecordingDeviceId;

            _mode = PresetFormMode.Edit;

            Text = "Edit Preset";
            btnAdd.Text = "Save";

            oldPresetId = _preset.PresetId;
        }

        private void PresetForm_Load(object sender, EventArgs e)
        {

            foreach (var o in cmbPlaybackDevices.Items)
            {
                if (((IDevice)o).Id == _preset.PlaybackDeviceId)
                {
                    cmbPlaybackDevices.SelectedIndex = cmbPlaybackDevices.Items.IndexOf(o);
                    break;
                }
            }

            cmbPlaybackDevices.DisplayMember = "FullName";
            cmbPlaybackDevices.ValueMember = "ID";

            foreach (var o in cmbRecordingDevices.Items)
            {
                if (((IDevice)o).Id == _preset.RecordingDeviceId)
                {
                    cmbRecordingDevices.SelectedIndex = cmbRecordingDevices.Items.IndexOf(o);
                    break;
                }
            }

            cmbRecordingDevices.DisplayMember = "FullName";
            cmbRecordingDevices.ValueMember = "ID";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (_mode == PresetFormMode.Normal && PresetManager.DuplicatePreset(_preset))
                return;

            if (_mode == PresetFormMode.Edit)
            { 
                PresetManager.ModifyPreset(_preset, oldPresetId);
                DialogResult = DialogResult.OK;
                Close();
            }
            else if (PresetManager.AddPreset(_preset))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                //
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cmbPlaybackDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPlaybackDevices.SelectedItem == null)
                return;

            _preset.PlaybackDeviceId = ((IDevice)cmbPlaybackDevices.SelectedItem).Id;
        }

        private void cmbRecordingDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRecordingDevices.SelectedItem == null)
                return;

            _preset.RecordingDeviceId = ((IDevice)cmbRecordingDevices.SelectedItem).Id;
        }

    }
}