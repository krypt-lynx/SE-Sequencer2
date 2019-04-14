using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    #region ingame script start

    class Transmitter
    {
        private List<IMyTerminalBlock> Antennas;

        public Transmitter(List<IMyTerminalBlock> antennas)
        {
            this.Antennas = antennas;
        }

        public void Transmit(string text, string targetString)
        {
            MyTransmitTarget target = ParseTarget(targetString);
            bool allTransmitted = true;

            foreach (var antenna in Antennas)
            {
                if (antenna is IMyRadioAntenna)
                {
                    allTransmitted = ((IMyRadioAntenna)antenna).TransmitMessage(text, target);
                }
                else if (antenna is IMyLaserAntenna)
                {
                    var laserAntenna = (IMyLaserAntenna)antenna;
                    if (laserAntenna.Status == MyLaserAntennaStatus.Connected)
                    {
                        allTransmitted = ((IMyLaserAntenna)antenna).TransmitMessage(text);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a transmit target object, by looking at individual tokens
        /// in the argument and OR-gating them.
        /// </summary>
        /// <param name="targetName">The string to be examined. </param>
        /// <returns>A valid target from MyTransmitTarget if the target string contained only valid elements, else MyTransmitTarget.None.</returns>
        private MyTransmitTarget ParseTarget(string targetName)
        {
            MyTransmitTarget targetGroup = MyTransmitTarget.None;

            string[] tokens = targetName.Split(new char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);

            targetGroup = tokens.Aggregate(MyTransmitTarget.None, (r, t) =>
            {
                MyTransmitTarget e;
                if (Enum.TryParse(t, true, out e))
                {
                    r |= e;
                }
                else
                {
                    Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Warning, "{0} is not a valid target.", t);
                }
                return r;
            });

            return targetGroup;
        }
    }

    #endregion // ingame script end
}
