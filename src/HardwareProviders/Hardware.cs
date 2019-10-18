using System;
using System.Collections.Generic;
using System.Security.Permissions;

namespace HardwareProviders {
    public abstract class Hardware {
        private readonly List<Sensor> active = new List<Sensor>();

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        static Hardware() {
            EnsureHook();
        }

        protected static void EnsureHook() {
            Ring0.Open();
            Opcode.Open();

            AppDomain.CurrentDomain.DomainUnload += (sender, args) => {
                Opcode.Close();
                Ring0.Close();
            };
        }

        protected Hardware(string name) {
            Name = name;
        }

        protected Hardware(string name, string identifier) {
            Identifier = identifier;
            Name = name;
        }

        public string Name { get; protected set; }

        public string Identifier { get; protected set; }

        public abstract void Update();

        public void ActivateSensor(Sensor sensor) {
            active.Add(sensor);
        }

        public void DeactivateSensor(Sensor sensor) {
            active.Remove(sensor);
        }
    }
}