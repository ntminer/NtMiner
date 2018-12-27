using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace NTMiner {
    public static class ChannelFactory {
        public static readonly BasicHttpBinding BasicHttpBinding = new BasicHttpBinding {
            TransferMode = TransferMode.Streamed,
            SendTimeout = new TimeSpan(0, 30, 0),
            MaxReceivedMessageSize = 10485760,
            Security = { Mode = BasicHttpSecurityMode.None },
            Name = "BasicHttpBinding",
            ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas() {
                MaxArrayLength = 1048576,
                MaxStringContentLength = 1000000
            }
        };

        public static readonly NetTcpBinding NetTcpBinding = new NetTcpBinding {
            TransferMode = TransferMode.Streamed,
            SendTimeout = new TimeSpan(0, 30, 0),
            MaxReceivedMessageSize = 10737418240,
            Security = { Mode = SecurityMode.None },
            Name = "TcpBinding",
            ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas() {
                MaxArrayLength = 1048576,
                MaxStringContentLength = 1000000
            }
        };

        public static T CreateChannel<T>(Binding binding, string serviceHost, int port) {
            ChannelFactory<T> factory = new ChannelFactory<T>(binding, new EndpointAddress(new Uri(new Uri($"http://{serviceHost}:{port}/"), typeof(T).Name)));
            return factory.CreateChannel();
        }
    }
}
