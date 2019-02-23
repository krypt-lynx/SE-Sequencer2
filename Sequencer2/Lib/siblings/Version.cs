using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace Script
{
    #region ingame script start

    class Version {
        uint mj, mn, pt;
        public uint Packed { get; private set; }

        public Version(uint major, uint minor, uint patch)
        {
            mj = major;
            mn = minor;
            pt = patch;
            Pack();
        }

        public Version(uint packed)
        {
            Packed = packed;
            Unpack();
        }

        void Pack()
        {
            Packed = (mj << 20) + (mn << 10) + pt;
        }

        void Unpack()
        {
            mj = (Packed >> 20) & 0x3FF;
            mn = (Packed >> 10) & 0x3FF;
            pt = Packed & 0x3FF;
        }

        public override string ToString()
        {
            return $"{mj}.{mn}.{pt}";
        }

        public override bool Equals(object obj)
        {
            var b = obj as Version;
            return Packed == b?.Packed;
        }

        public override int GetHashCode()
        {
            return (int)Packed;
        }
        
        public static bool operator ==(Version a, Version b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }
            return a.Equals(b);
        }

        public static bool operator !=(Version a, Version b)
        {
            return !(a == b);
        }
    }
    #endregion // ingame script end
}
