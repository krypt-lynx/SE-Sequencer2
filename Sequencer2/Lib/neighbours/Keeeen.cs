using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace Script
{

    /* #override
     * TrimComments : true
     */
    #region ingame script start

    /* not used
    public interface ICloneable
    {
        object Clone();
    } */

    /* not used
    public class Tuple<T1>
    {
        public T1 Item1 { get; private set; }

        public Tuple(T1 t1)
        {
            Item1 = t1;
        }
    }
    */

    public class NotImplementedException : Exception { }

    public class Tuple<T1, T2>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }

        public Tuple(T1 t1, T2 t2)
        {
            Item1 = t1;
            Item2 = t2;
        }
    }

    /* not used
    public class Tuple<T1, T2, T3>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }

        public Tuple(T1 t1, T2 t2, T3 t3)
        {
            Item1 = t1;
            Item2 = t2;
            Item3 = t3;
        }
    }
    */
    #endregion // ingame script end
}
