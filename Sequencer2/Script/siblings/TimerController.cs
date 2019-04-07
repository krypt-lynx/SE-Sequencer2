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

    class TimerController : ISerializable
    {
        public const string LOG_CAT = "tmr";
        
        public const float IgnoreDelayLessThen = 0.01f;

        double timePassed = 0;
        double delay = 0;
        double extraTime = 0;
        bool scheduled = false;
        bool firstTick = true;
        DateTime deserializationTime = DateTime.Now;

        public void Serialize(Serializer encoder)
        {
            encoder.Write(timePassed)
                   .Write(delay)
                   .Write(scheduled);
        }

        public void Deserialize(Deserializer decoder)
        {
            timePassed = decoder.ReadDouble();
            delay = decoder.ReadDouble();
            scheduled = decoder.ReadBool();

            ReinitTimer();
        }

        public TimerController()
        {

        }

        void ReinitTimer()
        {
            UpdateFrequencyFlags();
        }
        
        public void ScheduleStart(double delay)
        {
            // todo: test /waitticks

            this.delay = delay;
            this.timePassed = 0;
            scheduled = true;
            UpdateFrequencyFlags();
        }

        public void ScheduleImmidiate()
        {
            Program.Current.Runtime.UpdateFrequency |= UpdateFrequency.Once;
        }

        public void UpdateBefore()
        {
            var totalSeconds = (float)Program.Current.Runtime.TimeSinceLastRun.TotalSeconds;

            if (firstTick)
            {
                extraTime = 1.0f / 60;            
                firstTick = false;
            }

            delay -= (totalSeconds + extraTime);
            timePassed += (totalSeconds + extraTime);
            extraTime = 0;

            Log.WriteFormat(LOG_CAT, (LogLevel)10, "Delay now is {0}", delay); 

            if (delay < IgnoreDelayLessThen)
            {
                delay = 0;
            }

        }

        public void UpdateAfter()
        {
            UpdateFrequencyFlags();
        }


        void UpdateFrequencyFlags()
        {
            var otherFlags = Program.Current.Runtime.UpdateFrequency & ~(UpdateFrequency.Update1 | UpdateFrequency.Update10 | UpdateFrequency.Update100);
            UpdateFrequency result;
            if (scheduled)
            {
                if (delay >= 3.33)
                {
                    result = otherFlags | UpdateFrequency.Update100;
                }
                else if (delay > 0.3)
                {
                    result = otherFlags | UpdateFrequency.Update10;
                }
                else 
                {
                    result = otherFlags | UpdateFrequency.Update1;
                }
            } 
            else
            {
                result = otherFlags;
            }

            Program.Current.Runtime.UpdateFrequency = result;
        }

        public double TimePassed()
        {
            return timePassed;
        }
        
        public bool Timeout()
        {
            return delay <= 0;
        }
        
        internal void CancelStart()
        {
            delay = 0;
            timePassed = 0;
            scheduled = false;
            UpdateFrequencyFlags();
        }    
    }

    #endregion // ingame script end
}
