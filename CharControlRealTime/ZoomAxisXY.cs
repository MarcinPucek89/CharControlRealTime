using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CharControlRealTime
{
   public class ZoomAxisXY:IDisposable
    {
        public ZoomAxisXY() {  }

        //variables for plotting a "square", i.e. when selecting the data to be enlarged
        public double Axis_AX { get; set; }
        public double Axis_AY { get; set; }
        public double Axis_BX { get; set; }
        public double Axis_BY { get; set; }

        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue){
                if (disposing){
                    if (!double.IsInfinity(Axis_AX) || !double.IsNaN(Axis_AX)) Axis_AX = 0;
                    if (!double.IsInfinity(Axis_AY) || !double.IsNaN(Axis_AY)) Axis_AY = 0;
                    if (!double.IsInfinity(Axis_BX) || !double.IsNaN(Axis_BX)) Axis_BX = 0;
                    if (!double.IsInfinity(Axis_BY) || !double.IsNaN(Axis_BY)) Axis_BY = 0;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ZoomAxisXY() { Dispose(false); }
        #endregion
    }
}
