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

    class TimerController
    {
        public const string LOG_CAT = "tmr";

        const float KeensMagicDelay = 0.016f;
        const float MinTimerDelay = 1f;
        const float PresicionCheckDelay = 0.32f;
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

        public TimerController(Deserializer decoder)
        {
            timePassed = (float)decoder.ReadDouble();
            delay = (float)decoder.ReadDouble();

            ReinitTimer();
        }

        public TimerController()
        {

        }

        void ReinitTimer()
        {
            //Log.Write(timePassed);
            //Log.Write(delay);
            if (delay > 0)
            {
                StartTimer(MinTimerDelay);
                extraTime = MinTimerDelay;

                Log.Write(LOG_CAT, LogLevel.Verbose, "Timer is recovered after script reload");
                if (delay < MinTimerDelay)
                {
                    Log.Write(LOG_CAT, LogLevel.Warning, "Requared delay is smaller then possible at current point");
                }
            }
        }

        bool TriggerTimer()
        {
            if (Timer != null && Timer.IsFunctional)
            {
                Timer.ApplyAction("TriggerNow");
                return true;
            }
            else
            {
                Log.Write(LOG_CAT, LogLevel.Error, "No timer or it not functional.");
                return false;
            }
        }

        bool StartTimer(float delay)
        {
            Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "Trigger; delay {0}", delay);
            if (Timer != null && Timer.IsFunctional)
            {
                Timer.SetValue("TriggerDelay", delay);
                Timer.ApplyAction("Start");
                return true;
            }
            else
            {
                Log.Write(LOG_CAT, LogLevel.Error, "No timer or it not functional.");
                return false;
            }
        }

        bool StopTimer()
        {
            if (Timer != null && Timer.IsFunctional)
            {
                Timer.ApplyAction("Stop");
                return true;
            }
            else
            {
                Log.Write(LOG_CAT, LogLevel.Error, "No timer or it not functional.");
                return false;
            }
        }
        
        IMyTimerBlock timer = null;

        public IMyTimerBlock Timer
        {
            get
            {
                if (timer == null || !timer.IsFunctional)
                {
                    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                    Program.Current.GridTerminalSystem.GetBlocksOfType<IMyTimerBlock>(blocks, x => x.IsFunctional && x.CustomName == Program.TimerName);
                    timer = blocks.FirstOrDefault() as IMyTimerBlock;
                }
                return timer;
            }
        }

        public void ScheduleStart(float delay)
        {
            // todo: test /waitticks

            this.delay = delay;
            this.timePassed = 0;
        }

        private void ScheduleStartInternal(bool forceTriggerNow)
        {
            /* // Because Keen. There is no way to measure delay in Save(), so, I need to log it each tick.
            if (!forceTriggerNow && (delay > (MinTimerDelay + PresicionCheckDelay)))
            {
                StartTimer(delay - PresicionCheckDelay);
            }
            else
            {
                TriggerTimer();
            }
            */

            TriggerTimer();
        }

        public void Update()
        {
            var totalSeconds = (float)Program.Current.Runtime.TimeSinceLastRun.TotalSeconds;

            if (firstTick)
            {
                if ((DateTime.Now - deserializationTime).Seconds < 0.25) // if TriggerNow actually was triggered. Rare thing, but happens
                {
                    Log.Write(LOG_CAT, LogLevel.Verbose, "TriggerNow ticked after game load");
                    extraTime = 1.0f / 60;
                }
            }

            delay -= (totalSeconds + extraTime);
            timePassed += (totalSeconds + extraTime);
            extraTime = 0;
            Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "Delay now is {0}", delay);

            if (delay < IgnoreDelayLessThen)
            {
                delay = 0;
            }
        }

        public void ContinueWait(bool forceTriggerNow)
        {
            if (forceTriggerNow || delay >= IgnoreDelayLessThen)
            {
                ScheduleStartInternal(forceTriggerNow);
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

        public bool IsInterupted()
        {
            return false;
        }
    }

    #endregion // ingame script end
}
