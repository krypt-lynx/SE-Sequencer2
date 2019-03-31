#define Simulation
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    partial class Program 
    {
#if !Simulation

        #region ingame script start

        Exception lastException = null;
        public static MyGridProgram Current;
        bool tryWriteErrorOnce = true;

        public void IsolatedRun(Action work)
        {
            if (lastException == null)
            {
                try
                {
                    Current = this;
                    Log.NewFrame();

                    work();

                    Current = null;
                }
                catch (Exception e)
                {
                    lastException = e;
                }
            }

            if (lastException != null)
            {
                EchoException();
                return;
            }
        }

        private void EchoException()
        {
            try
            {
                WE("Exception handled!");
                WE("Please, make screenshot of this message and report the issue to developer");
                WE("To recover recompile the script");
                WE(lastException.GetType().Name);
                WE(lastException.Message);
                WE(lastException.StackTrace);
            }
            catch { }
            tryWriteErrorOnce = false;
        }

        void WE(string message)
        {
            if (tryWriteErrorOnce)
            {
                Log.Write(message);
            } else
            {
                Echo(message);
            }
        }

        #endregion // ingame script end

#else
        Exception lastException = null;
        public static MyGridProgram Current;

        public void IsolatedRun(Action work)
        {
            Current = this;
            Log.NewFrame();

            work();

            Current = null;
        }
#endif
    }

}
