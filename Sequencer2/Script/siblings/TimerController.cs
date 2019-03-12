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

        float timePassed = 0;
        float delay = 0;
        float extraTime = 0;
        bool firstTick = true;
        DateTime deserializationTime = DateTime.Now;

        public void Serialize(Serializer encoder)
        {
            encoder.Write(timePassed)
                   .Write(delay);
        }

        public void Deserialize(Deserializer decoder)
        {
            timePassed = decoder.ReadFloat();
            delay = decoder.ReadFloat();

            ReinitTimer();
        }

        public TimerController() { }

        void ReinitTimer()
        {
            UpdateFrequencyFlags();
            Program.Current.Runtime.UpdateFrequency |= UpdateFrequency.Once;
        }
        
        public void ScheduleStart(float delay)
        {
            // todo: test /waitticks

            this.delay = delay;
            this.timePassed = 0;
            UpdateFrequencyFlags();
        }

        public void Update()
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

            UpdateFrequencyFlags();
        }

        void UpdateFrequencyFlags()
        {
            if (delay >= 3.33)
            {
                Program.Current.Runtime.UpdateFrequency = UpdateFrequency.Update100;
            }
            else if (delay > 0.3)
            {
                Program.Current.Runtime.UpdateFrequency = UpdateFrequency.Update10;
            }
            else if (delay > 0)
            {
                Program.Current.Runtime.UpdateFrequency = UpdateFrequency.Update1;
            }
        }

        public float TimePassed()
        {
            return timePassed;
        }
        
        public bool Timeout()
        {
            return delay <= 0;
        }
        
        internal void CancelStart()
        {
            this.delay = 0;
            this.timePassed = 0;
            UpdateFrequencyFlags();
        }    
    }

    #endregion // ingame script end
}
