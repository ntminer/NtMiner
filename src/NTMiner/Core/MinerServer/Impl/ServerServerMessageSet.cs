using NTMiner.MinerServer;
using NTMiner.ServerMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTMiner.Core.MinerServer.Impl {
    public class ServerServerMessageSet : IServerMessageSet {
        public int Count => throw new NotImplementedException();

        public void Add(string provider, string messageType, string content) {
            throw new NotImplementedException();
        }

        public List<IServerMessage> GetServerMessages(DateTime timeStamp) {
            throw new NotImplementedException();
        }

        public void Clear() {
            throw new NotImplementedException();
        }

        public IEnumerator<IServerMessage> GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
