using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CharControlRealTime
{
   public class AxisXY : IDisposable,IComparable
    {
        public AxisXY() { }

        //variable coordinates for plotting the graph
        public double Axis_X { get; set; }
        public double Axis_Y { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is AxisXY _AxisXY)
                return this.Axis_Y.CompareTo(_AxisXY.Axis_Y);
            else
                throw new AggregateException("Object is not a AxisY");


        }
        
        #region IDisposable Support

        public void Dispose(){ GC.SuppressFinalize(this); }

        ~AxisXY() { Dispose(); }
        #endregion
        
    }
}
