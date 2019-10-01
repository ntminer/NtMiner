/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2009-2011 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

namespace HardwareProviders.Board.LPC
{
    internal class F718XX : ISuperIO
    {
        // Hardware Monitor
        private const byte ADDRESS_REGISTER_OFFSET = 0x05;
        private const byte DATA_REGISTER_OFFSET = 0x06;

        private const byte PWM_VALUES_OFFSET = 0x2D;

        // Hardware Monitor Registers
        private const byte VOLTAGE_BASE_REG = 0x20;
        private const byte TEMPERATURE_CONFIG_REG = 0x69;
        private const byte TEMPERATURE_BASE_REG = 0x70;

        private readonly ushort address;

        private readonly byte[] FAN_PWM_REG =
            {0xA3, 0xB3, 0xC3, 0xD3};

        private readonly byte[] FAN_TACHOMETER_REG =
            {0xA0, 0xB0, 0xC0, 0xD0};

        public F718XX(Chip chip, ushort address)
        {
            this.address = address;
            Chip = chip;

            Voltages = new float?[chip == Chip.F71858 ? 3 : 9];
            Temperatures = new float?[chip == Chip.F71808E ? 2 : 3];
            Fans = new float?[chip == Chip.F71882 || chip == Chip.F71858 ? 4 : 3];
            Controls = new float?[chip == Chip.F71878AD ? 3 : 0];
        }

        public byte? ReadGPIO(int index)
        {
            return null;
        }

        public void WriteGPIO(int index, byte value)
        {
        }

        public void SetControl(int index, byte? value)
        {
            if (index < Controls.Length)
                WriteByte(FAN_PWM_REG[index], value ?? 128);
        }

        public Chip Chip { get; }

        public float?[] Voltages { get; }

        public float?[] Temperatures { get; }

        public float?[] Fans { get; }

        public float?[] Controls { get; }

        public void Update()
        {
            if (!Ring0.WaitIsaBusMutex(10))
                return;

            for (var i = 0; i < Voltages.Length; i++)
                if (Chip == Chip.F71808E && i == 6)
                {
                    // 0x26 is reserved on F71808E
                    Voltages[i] = 0;
                }
                else
                {
                    int value = ReadByte((byte) (VOLTAGE_BASE_REG + i));
                    Voltages[i] = 0.008f * value;
                }

            for (var i = 0; i < Temperatures.Length; i++)
                switch (Chip)
                {
                    case Chip.F71858:
                    {
                        var tableMode = 0x3 & ReadByte(TEMPERATURE_CONFIG_REG);
                        int high =
                            ReadByte((byte) (TEMPERATURE_BASE_REG + 2 * i));
                        int low =
                            ReadByte((byte) (TEMPERATURE_BASE_REG + 2 * i + 1));
                        if (high != 0xbb && high != 0xcc)
                        {
                            var bits = 0;
                            switch (tableMode)
                            {
                                case 0:
                                    bits = 0;
                                    break;
                                case 1:
                                    bits = 0;
                                    break;
                                case 2:
                                    bits = (high & 0x80) << 8;
                                    break;
                                case 3:
                                    bits = (low & 0x01) << 15;
                                    break;
                            }

                            bits |= high << 7;
                            bits |= (low & 0xe0) >> 1;
                            var value = (short) (bits & 0xfff0);
                            Temperatures[i] = value / 128.0f;
                        }
                        else
                        {
                            Temperatures[i] = null;
                        }
                    }
                        break;
                    default:
                    {
                        var value = (sbyte) ReadByte((byte) (
                            TEMPERATURE_BASE_REG + 2 * (i + 1)));
                        if (value < sbyte.MaxValue && value > 0)
                            Temperatures[i] = value;
                        else
                            Temperatures[i] = null;
                    }
                        break;
                }

            for (var i = 0; i < Fans.Length; i++)
            {
                var value = ReadByte(FAN_TACHOMETER_REG[i]) << 8;
                value |= ReadByte((byte) (FAN_TACHOMETER_REG[i] + 1));

                if (value > 0)
                    Fans[i] = value < 0x0fff ? 1.5e6f / value : 0;
                else
                    Fans[i] = null;
            }

            for (var i = 0; i < Controls.Length; i++)
                Controls[i] = 100 * ReadByte((byte) (PWM_VALUES_OFFSET + i)) / 256.0f;

            Ring0.ReleaseIsaBusMutex();
        }

        private byte ReadByte(byte register)
        {
            Ring0.WriteIoPort(
                (ushort) (address + ADDRESS_REGISTER_OFFSET), register);
            return Ring0.ReadIoPort((ushort) (address + DATA_REGISTER_OFFSET));
        }

        private void WriteByte(byte register, byte value)
        {
            Ring0.WriteIoPort(
                (ushort) (address + ADDRESS_REGISTER_OFFSET), register);
            Ring0.WriteIoPort((ushort) (address + DATA_REGISTER_OFFSET), value);
        }
    }
}