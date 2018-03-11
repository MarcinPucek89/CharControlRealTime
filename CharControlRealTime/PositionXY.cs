using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CharControlRealTime
{
   public class PositionXY: IDisposable
    {
        public PositionXY() { }

        //position variables, i.e. changing co-ordinates to positional variables
        public double PositionX { get; set; }
        public double PositionY { get; set; }

        #region IDisposable Support
        private bool disposedValue = false; 
 
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!double.IsInfinity(PositionX) || !double.IsNaN(PositionX)) PositionX = 0;
                    if (!double.IsInfinity(PositionY) || !double.IsNaN(PositionY)) PositionY = 0;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PositionXY() { Dispose(false); }
        #endregion
    }
}
