using Moq;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi;
using System;

namespace FortyOne.AudioSwitcher.Helpers
{
    public static class DeviceMockHelper
    {
        public static Mock<IDevice> CreateFakePlaybackDeviceMock()
        {
            var fakeDeviceMock = new Mock<IDevice>();

            // Setup the properties you need for the fake device
            fakeDeviceMock.SetupGet(d => d.Name).Returns("Cycle Playback Devices");
            fakeDeviceMock.SetupGet(d => d.FullName).Returns("Cycle Playback Devices");
            fakeDeviceMock.SetupGet(d => d.InterfaceName).Returns("Cycle Playback Devices Interface");
            fakeDeviceMock.SetupGet(d => d.IconPath).Returns("%windir%\\system32\\mmres.dll,-3030");
            fakeDeviceMock.SetupGet(d => d.Id).Returns(new Guid("00000000-0000-0000-0000-000000000000"));
            fakeDeviceMock.SetupGet(d => d.Volume).Returns(50.0);
            fakeDeviceMock.SetupGet(d => d.IsMuted).Returns(false);
            fakeDeviceMock.SetupGet(d => d.State).Returns(DeviceState.Active);
            fakeDeviceMock.SetupGet(d => d.DeviceType).Returns(DeviceType.Playback);

            // Internally store whether this device is the default
            bool isDefault = false;

            // Mocking the read-only property `IsDefaultDevice` using a callback to simulate the default state
            fakeDeviceMock.SetupGet(d => d.IsDefaultDevice).Returns(() => isDefault);

            // Simulating SetAsDefault() method by using Callback to set `isDefault` to true
            fakeDeviceMock.Setup(d => d.SetAsDefault()).Callback(() => isDefault = true);

            return fakeDeviceMock;
        }

        public static Mock<IDevice> CreateFakeRecordingDeviceMock()
        {
            var fakeDeviceMock = new Mock<IDevice>();

            // Setup the properties you need for the fake device
            fakeDeviceMock.SetupGet(d => d.Name).Returns("Cycle Recording Devices");
            fakeDeviceMock.SetupGet(d => d.FullName).Returns("Cycle Recording Devices");
            fakeDeviceMock.SetupGet(d => d.InterfaceName).Returns("Cycle Recording Devices Interface");
            fakeDeviceMock.SetupGet(d => d.IconPath).Returns("%windir%\\system32\\mmres.dll,-3030");
            fakeDeviceMock.SetupGet(d => d.Id).Returns(new Guid("00000000-0000-0000-0000-000000000001"));
            fakeDeviceMock.SetupGet(d => d.Volume).Returns(50.0);
            fakeDeviceMock.SetupGet(d => d.IsMuted).Returns(false);
            fakeDeviceMock.SetupGet(d => d.State).Returns(DeviceState.Active);
            fakeDeviceMock.SetupGet(d => d.DeviceType).Returns(DeviceType.Capture);

            // Internally store whether this device is the default
            bool isDefault = false;

            // Mocking the read-only property `IsDefaultDevice` using a callback to simulate the default state
            fakeDeviceMock.SetupGet(d => d.IsDefaultDevice).Returns(() => isDefault);

            // Simulating SetAsDefault() method by using Callback to set `isDefault` to true
            fakeDeviceMock.Setup(d => d.SetAsDefault()).Callback(() => isDefault = true);

            return fakeDeviceMock;
        }

        public static Mock<IDevice> CreateFakeCyclePresetDeviceMock()
        {
            var fakeDeviceMock = new Mock<IDevice>();

            // Setup the properties you need for the fake device
            fakeDeviceMock.SetupGet(d => d.Name).Returns("Cycle Presets");
            fakeDeviceMock.SetupGet(d => d.FullName).Returns("Cycle Presets");
            fakeDeviceMock.SetupGet(d => d.InterfaceName).Returns("Cycle Presets Interface");
            fakeDeviceMock.SetupGet(d => d.IconPath).Returns("%windir%\\system32\\mmres.dll,-3030");
            fakeDeviceMock.SetupGet(d => d.Id).Returns(new Guid("00000000-0000-0000-0000-000000000002"));
            fakeDeviceMock.SetupGet(d => d.Volume).Returns(50.0);
            fakeDeviceMock.SetupGet(d => d.IsMuted).Returns(false);
            fakeDeviceMock.SetupGet(d => d.State).Returns(DeviceState.Active);
            fakeDeviceMock.SetupGet(d => d.DeviceType).Returns(DeviceType.Playback);

            // Internally store whether this device is the default
            bool isDefault = false;

            // Mocking the read-only property `IsDefaultDevice` using a callback to simulate the default state
            fakeDeviceMock.SetupGet(d => d.IsDefaultDevice).Returns(() => isDefault);

            // Simulating SetAsDefault() method by using Callback to set `isDefault` to true
            fakeDeviceMock.Setup(d => d.SetAsDefault()).Callback(() => isDefault = true);

            return fakeDeviceMock;
        }


        public static Mock<IDevice> CreateFakePresetDeviceMock(int presetNumber, Guid presetGuid)
        {
            var fakeDeviceMock = new Mock<IDevice>();

            // Setup the properties you need for the fake device
            fakeDeviceMock.SetupGet(d => d.Name).Returns($"Preset {presetNumber}");
            fakeDeviceMock.SetupGet(d => d.FullName).Returns($"Preset {presetNumber}");
            fakeDeviceMock.SetupGet(d => d.InterfaceName).Returns($"Preset {presetNumber} interface");
            fakeDeviceMock.SetupGet(d => d.IconPath).Returns("%windir%\\system32\\mmres.dll,-3030");
            fakeDeviceMock.SetupGet(d => d.Id).Returns(presetGuid);
            fakeDeviceMock.SetupGet(d => d.Volume).Returns(50.0);
            fakeDeviceMock.SetupGet(d => d.IsMuted).Returns(false);
            fakeDeviceMock.SetupGet(d => d.State).Returns(DeviceState.Active);
            fakeDeviceMock.SetupGet(d => d.DeviceType).Returns(DeviceType.Capture);

            // Internally store whether this device is the default
            bool isDefault = false;

            // Mocking the read-only property `IsDefaultDevice` using a callback to simulate the default state
            fakeDeviceMock.SetupGet(d => d.IsDefaultDevice).Returns(() => isDefault);

            // Simulating SetAsDefault() method by using Callback to set `isDefault` to true
            fakeDeviceMock.Setup(d => d.SetAsDefault()).Callback(() => isDefault = true);

            return fakeDeviceMock;
        }

    }
}