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
            for (int i = 0; i < tokens.Length; i++)
            {
                switch (tokens[i])
                {
                    case "none":
                        targetGroup = targetGroup | MyTransmitTarget.None;
                        break;
                    case "owned":
                        targetGroup = targetGroup | MyTransmitTarget.Owned;
                        break;
                    case "ally":
                        targetGroup = targetGroup | MyTransmitTarget.Ally;
                        break;
                    case "neutral":
                        targetGroup = targetGroup | MyTransmitTarget.Neutral;
                        break;
                    case "enemy":
                        targetGroup = targetGroup | MyTransmitTarget.Enemy;
                        break;
                    case "everyone":
                        targetGroup = targetGroup | MyTransmitTarget.Everyone;
                        break;
                    case "default":
                        targetGroup = targetGroup | MyTransmitTarget.Default;
                        break;
                    default:
                        throw new FormatException(tokens[i] + " is not a valid target.");
                }
            }

            return targetGroup;
        }
    }

    #endregion // ingame script end
}
