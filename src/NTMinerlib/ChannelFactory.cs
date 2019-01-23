using System;
using System.ServiceModel;

namespace NTMiner {
    public static class ChannelFactory {
        private static readonly TimeSpan Timeout = new TimeSpan(hours: 0, minutes: 0, seconds: 30);
        public static readonly BasicHttpBinding BasicHttpBinding = new BasicHttpBinding {
            TransferMode = TransferMode.Streamed,
            SendTimeout = Timeout,
            ReceiveTimeout = Timeout,
            CloseTimeout = Timeout,
            OpenTimeout = Timeout,
            MaxReceivedMessageSize = 10485760,
            Security = { Mode = BasicHttpSecurityMode.None },
            Name = "BasicHttpBinding",
            ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas() {
                MaxArrayLength = 1048576,
                MaxStringContentLength = 1000000
            }
        };

        public static T CreateChannel<T>(string serviceHost, int port) {
            ChannelFactory<T> factory = new ChannelFactory<T>(BasicHttpBinding, new EndpointAddress(new Uri(new Uri($"http://{serviceHost}:{port}/"), typeof(T).Name)));
            return factory.CreateChannel();
        }
    }
}
