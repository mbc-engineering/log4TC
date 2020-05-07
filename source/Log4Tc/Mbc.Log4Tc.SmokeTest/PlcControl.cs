using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwinCAT;
using TwinCAT.Ads;

namespace Mbc.Log4Tc.SmokeTest
{
    public class PlcControl : IDisposable
    {
        private readonly AdsSession _session;
        private readonly IAdsConnection _connection;

        public PlcControl(AmsAddress addr)
        {
            _session = new AdsSession(addr);
            _connection = (IAdsConnection) _session.Connect();
        }

        public void Dispose()
        {
            _connection.Disconnect();
        }

        public void Reset()
        {
            var state = _connection.ReadState();

            var stateInfo = new StateInfo
            {
                AdsState = AdsState.Reset,
                DeviceState = 0,
            };
            _connection.WriteControl(stateInfo);
        }

        public void Start()
        {
            var stateInfo = new StateInfo
            {
                AdsState = AdsState.Run,
                DeviceState = 0,
            };
            _connection.WriteControl(stateInfo);
        }

        public void Stop()
        {
            var state = _connection.ReadState();
            if (state.AdsState == AdsState.Run)
            {
                var stateInfo = new StateInfo
                {
                    AdsState = AdsState.Stop,
                    DeviceState = 0,
                };
                _connection.WriteControl(stateInfo);
            }
        }
    }
}
