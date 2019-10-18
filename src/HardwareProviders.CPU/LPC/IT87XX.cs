/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2009-2015 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

using System;

namespace HardwareProviders.Board.LPC
{
    internal class IT87XX : ISuperIO
    {
        // Consts
        private const byte ITE_VENDOR_ID = 0x90;

        // Environment Controller
        private const byte ADDRESS_REGISTER_OFFSET = 0x05;
        private const byte DATA_REGISTER_OFFSET = 0x06;

        // Environment Controller Registers    
        private const byte CONFIGURATION_REGISTER = 0x00;
        private const byte TEMPERATURE_BASE_REG = 0x29;
        private const byte VENDOR_ID_REGISTER = 0x58;
        private const byte FAN_TACHOMETER_DIVISOR_REGISTER = 0x0B;
        private const byte FAN_TACHOMETER_16BIT_REGISTER = 0x0C;
        private const byte VOLTAGE_BASE_REG = 0x20;

        private readonly ushort address;

        private readonly ushort addressReg;
        private readonly ushort dataReg;
        private readonly byte[] FAN_PWM_CTRL_REG = {0x15, 0x16, 0x17};
        private readonly byte[] FAN_PWM_DUTY_REG = {0x63, 0x6b, 0x73};

        private readonly byte[] FAN_TACHOMETER_EXT_REG =
            {0x18, 0x19, 0x1a, 0x81, 0x83, 0x4c};

        private readonly byte[] FAN_TACHOMETER_REG =
            {0x0d, 0x0e, 0x0f, 0x80, 0x82, 0x4c};

        private readonly bool[] fansDisabled = new bool[0];

        private readonly ushort gpioAddress;
        private readonly int gpioCount;
        private readonly bool has16bitFanCounter;
        private readonly bool hasNewerAutopwm;

        private readonly byte[] initialFanPwmControl = new byte[3];
        private readonly byte[] initialFanPwmControlMode = new byte[3];

        private readonly bool[] restoreDefaultFanPwmControlRequired = new bool[3];
        private readonly byte version;

        private readonly float voltageGain;

        public IT87XX(Chip chip, ushort address, ushort gpioAddress, byte version)
        {
            this.address = address;
            Chip = chip;
            this.version = version;
            addressReg = (ushort) (address + ADDRESS_REGISTER_OFFSET);
            dataReg = (ushort) (address + DATA_REGISTER_OFFSET);
            this.gpioAddress = gpioAddress;

            // Check vendor id
            bool valid;
            var vendorId = ReadByte(VENDOR_ID_REGISTER, out valid);
            if (!valid || vendorId != ITE_VENDOR_ID)
                return;

            // Bit 0x10 of the configuration register should always be 1
            var config = ReadByte(CONFIGURATION_REGISTER, out valid);
            if ((config & 0x10) == 0 && chip != Chip.IT8665E)
                return;
            if (!valid)
                return;

            // IT8686E has more sensors
            if (chip == Chip.IT8686E)
            {
                Voltages = new float?[10];
                Temperatures = new float?[5];
                Fans = new float?[5];
                fansDisabled = new bool[5];
                Controls = new float?[3];
            }
            else if (chip == Chip.IT8665E)
            {
                Voltages = new float?[10];
                Temperatures = new float?[6];
                Fans = new float?[6];
                fansDisabled = new bool[6];
                Controls = new float?[3];
            }
            else
            {
                Voltages = new float?[9];
                Temperatures = new float?[3];
                Fans = new float?[chip == Chip.IT8705F ? 3 : 5];
                fansDisabled = new bool[chip == Chip.IT8705F ? 3 : 5];
                Controls = new float?[3];
            }

            // IT8620E, IT8628E, IT8721F, IT8728F, IT8772E and IT8686E use a 12mV resultion 
            // ADC, all others 16mV
            if (chip == Chip.IT8620E || chip == Chip.IT8628E || chip == Chip.IT8721F
                || chip == Chip.IT8728F || chip == Chip.IT8771E || chip == Chip.IT8772E || chip == Chip.IT8686E)
                voltageGain = 0.012f;
            else if (chip == Chip.IT8665E)
                voltageGain = 0.0109f;
            else
                voltageGain = 0.016f;

            // older IT8705F and IT8721F revisions do not have 16-bit fan counters
            if (chip == Chip.IT8705F && version < 3 ||
                chip == Chip.IT8712F && version < 8)
                has16bitFanCounter = false;
            else
                has16bitFanCounter = true;

            if (chip == Chip.IT8620E) hasNewerAutopwm = true;

            // Disable any fans that aren't set with 16-bit fan counters
            if (has16bitFanCounter)
            {
                int modes = ReadByte(FAN_TACHOMETER_16BIT_REGISTER, out valid);

                if (!valid)
                    return;

                if (Fans.Length >= 5)
                {
                    fansDisabled[3] = (modes & (1 << 4)) == 0;
                    fansDisabled[4] = (modes & (1 << 5)) == 0;
                }

                if (Fans.Length >= 6) fansDisabled[5] = (modes & (1 << 2)) == 0;
            }

            // Set the number of GPIO sets
            switch (chip)
            {
                case Chip.IT8712F:
                case Chip.IT8716F:
                case Chip.IT8718F:
                case Chip.IT8726F:
                    gpioCount = 5;
                    break;
                case Chip.IT8720F:
                case Chip.IT8721F:
                    gpioCount = 8;
                    break;
                case Chip.IT8620E:
                case Chip.IT8628E:
                case Chip.IT8705F:
                case Chip.IT8728F:
                case Chip.IT8771E:
                case Chip.IT8772E:
                    gpioCount = 0;
                    break;
            }
        }

        public byte? ReadGPIO(int index)
        {
            if (index >= gpioCount)
                return null;

            return Ring0.ReadIoPort((ushort) (gpioAddress + index));
        }

        public void WriteGPIO(int index, byte value)
        {
            if (index >= gpioCount)
                return;

            Ring0.WriteIoPort((ushort) (gpioAddress + index), value);
        }

        public void SetControl(int index, byte? value)
        {
            if (index < 0 || index >= Controls.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (!Ring0.WaitIsaBusMutex(10))
                return;

            if (value.HasValue)
            {
                SaveDefaultFanPwmControl(index);

                if (hasNewerAutopwm)
                {
                    var ctrlValue = ReadByte(FAN_PWM_CTRL_REG[index], out var valid);

                    if (valid)
                    {
                        var isOnAutoControl = (ctrlValue & (1 << 7)) > 0;
                        if (isOnAutoControl)
                        {
                            // Set to manual speed control
                            ctrlValue &= byte.MaxValue ^ (1 << 7);
                            WriteByte(FAN_PWM_CTRL_REG[index], ctrlValue);
                        }
                    }

                    // set speed
                    WriteByte(FAN_PWM_DUTY_REG[index], value.Value);
                }
                else
                {
                    // set output value
                    WriteByte(FAN_PWM_CTRL_REG[index], (byte) (value.Value >> 1));
                }
            }
            else
            {
                RestoreDefaultFanPwmControl(index);
            }

            Ring0.ReleaseIsaBusMutex();
        }

        public Chip Chip { get; }

        public float?[] Voltages { get; } = new float?[0];

        public float?[] Temperatures { get; } = new float?[0];

        public float?[] Fans { get; } = new float?[0];

        public float?[] Controls { get; } = new float?[0];

        public void Update()
        {
            if (!Ring0.WaitIsaBusMutex(10))
                return;

            for (var i = 0; i < Voltages.Length; i++)
            {
                bool valid;

                var value =
                    voltageGain * ReadByte((byte) (VOLTAGE_BASE_REG + i), out valid);

                if (!valid)
                    continue;
                if (value > 0)
                    Voltages[i] = value;
                else
                    Voltages[i] = null;
            }

            for (var i = 0; i < Temperatures.Length; i++)
            {
                bool valid;
                var value = (sbyte) ReadByte(
                    (byte) (TEMPERATURE_BASE_REG + i), out valid);
                if (!valid)
                    continue;

                if (value < sbyte.MaxValue && value > 0)
                    Temperatures[i] = value;
                else
                    Temperatures[i] = null;
            }

            if (has16bitFanCounter)
                for (var i = 0; i < Fans.Length; i++)
                {
                    if (fansDisabled[i])
                        continue;
                    bool valid;
                    int value = ReadByte(FAN_TACHOMETER_REG[i], out valid);
                    if (!valid)
                        continue;
                    value |= ReadByte(FAN_TACHOMETER_EXT_REG[i], out valid) << 8;
                    if (!valid)
                        continue;

                    if (value > 0x3f)
                        Fans[i] = value < 0xffff ? 1.35e6f / (value * 2) : 0;
                    else
                        Fans[i] = null;
                }
            else
                for (var i = 0; i < Fans.Length; i++)
                {
                    bool valid;
                    int value = ReadByte(FAN_TACHOMETER_REG[i], out valid);
                    if (!valid)
                        continue;

                    var divisor = 2;
                    if (i < 2)
                    {
                        int divisors = ReadByte(FAN_TACHOMETER_DIVISOR_REGISTER, out valid);
                        if (!valid)
                            continue;
                        divisor = 1 << ((divisors >> (3 * i)) & 0x7);
                    }

                    if (value > 0)
                        Fans[i] = value < 0xff ? 1.35e6f / (value * divisor) : 0;
                    else
                        Fans[i] = null;
                }

            for (var i = 0; i < Controls.Length; i++)
                if (hasNewerAutopwm)
                {
                    bool valid;
                    var value = ReadByte(FAN_PWM_DUTY_REG[i], out valid);
                    if (!valid)
                        continue;

                    var ctrlValue = ReadByte(FAN_PWM_CTRL_REG[i], out valid);
                    if (!valid)
                        continue;

                    if ((ctrlValue & 0x80) > 0)
                        Controls[i] = null;
                    else
                        Controls[i] = (float) Math.Round(value * 100.0f / 0xFF);
                }
                else
                {
                    bool valid;
                    var value = ReadByte(FAN_PWM_CTRL_REG[i], out valid);
                    if (!valid)
                        continue;

                    if ((value & 0x80) > 0)
                        Controls[i] = null;
                    else
                        Controls[i] = (float) Math.Round((value & 0x7F) * 100.0f / 0x7F);
                }

            Ring0.ReleaseIsaBusMutex();
        }

        private byte ReadByte(byte register, out bool valid)
        {
            Ring0.WriteIoPort(addressReg, register);
            var value = Ring0.ReadIoPort(dataReg);
            valid = register == Ring0.ReadIoPort(addressReg);
            return value;
        }

        private bool WriteByte(byte register, byte value)
        {
            Ring0.WriteIoPort(addressReg, register);
            Ring0.WriteIoPort(dataReg, value);
            return register == Ring0.ReadIoPort(addressReg);
        }

        private void SaveDefaultFanPwmControl(int index)
        {
            if (hasNewerAutopwm)
            {
                if (!restoreDefaultFanPwmControlRequired[index])
                {
                    initialFanPwmControlMode[index] =
                        ReadByte(FAN_PWM_CTRL_REG[index], out _);

                    initialFanPwmControl[index] =
                        ReadByte(FAN_PWM_DUTY_REG[index], out _);
                }
            }
            else
            {
                if (!restoreDefaultFanPwmControlRequired[index])
                    initialFanPwmControl[index] =
                        ReadByte(FAN_PWM_CTRL_REG[index], out _);
            }

            restoreDefaultFanPwmControlRequired[index] = true;
        }

        private void RestoreDefaultFanPwmControl(int index)
        {
            if (hasNewerAutopwm)
            {
                if (restoreDefaultFanPwmControlRequired[index])
                {
                    WriteByte(FAN_PWM_CTRL_REG[index], initialFanPwmControlMode[index]);
                    WriteByte(FAN_PWM_DUTY_REG[index], initialFanPwmControl[index]);
                    restoreDefaultFanPwmControlRequired[index] = false;
                }
            }
            else
            {
                if (restoreDefaultFanPwmControlRequired[index])
                {
                    WriteByte(FAN_PWM_CTRL_REG[index], initialFanPwmControl[index]);
                    restoreDefaultFanPwmControlRequired[index] = false;
                }
            }
        }
    }
}