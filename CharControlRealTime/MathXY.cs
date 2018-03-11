using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CharControlRealTime
{
   public class MathXY:IDisposable
    {
        public MathXY() { }

        private ZoomAxisXY axis;
        private PositionXY position;

        //scaling the chart
        public ZoomAxisXY GetPointAB(double Axis_AX, double Axis_AY, double Axis_BX, double Axis_BY,  double height, double distancePointAxisX, double distancePointAxisY, double scaleX, double scaleY, double ChangeScaleAxisX, double ChangeScaleAxisY)
        {
            if (double.IsNaN(Axis_AX) || double.IsNaN(Axis_AY) || double.IsNaN(Axis_BX) || double.IsNaN(Axis_BY) || double.IsNaN(height) || double.IsNaN(distancePointAxisX)) return null;
            if (double.IsNaN(distancePointAxisY) || double.IsNaN(scaleX) || double.IsNaN(scaleY) || double.IsNaN(ChangeScaleAxisX) || double.IsNaN(ChangeScaleAxisY)) return null;

            if (double.IsInfinity(Axis_AX) || double.IsInfinity(Axis_AY) || double.IsInfinity(Axis_BX) || double.IsInfinity(Axis_BY) || double.IsInfinity(height) || double.IsInfinity(distancePointAxisX)) return null;
            if (double.IsInfinity(distancePointAxisY) || double.IsInfinity(scaleX) || double.IsInfinity(scaleY) || double.IsInfinity(ChangeScaleAxisX) || double.IsInfinity(ChangeScaleAxisY)) return null;

            double Axis_A_X = 0;
            double Axis_A_Y = 0;
            double Axis_B_X = 0;
            double Axis_B_Y = 0;
  
            Axis_A_X =  Axis_AX / distancePointAxisY;
            Axis_A_Y =  (height - Axis_AY)  / distancePointAxisX;

            Axis_B_X =  Axis_BX / distancePointAxisY;
            Axis_B_Y =  (height - Axis_BY)  / distancePointAxisX;

            //rounding of variables
            if (scaleX >= 1)
            {
                Axis_A_X = Math.Round(Axis_A_X, 1);
                Axis_B_X = Math.Round(Axis_B_X, 1);
            }

            if (scaleY >= 1)
            {
                Axis_A_Y = Math.Round(Axis_A_Y, 1);  
                Axis_B_Y = Math.Round(Axis_B_Y, 1);  
            }

            axis = new ZoomAxisXY()
            {
                //if the scale has changed, add the difference => the situation occurs after the second magnification of the chart, i.e. ChangeScaleAxis
                Axis_AX = ChangeScaleAxisX + Axis_A_X,
                Axis_BX = ChangeScaleAxisX + Axis_B_X,
                Axis_AY = ChangeScaleAxisY + Axis_A_Y,
                Axis_BY = ChangeScaleAxisY + Axis_B_Y
            };

            return axis;
        }

        //drawing a chart
        public PositionXY GetAxisPosition(double X, double Y, double distancePointAxisX, double distancePointAxisY,  double height, double ChangeScaleAxisX, double ChangeScaleAxisY)
        {
            if (double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(distancePointAxisX) || double.IsNaN(distancePointAxisY) || double.IsNaN(height) || (double.IsNaN(ChangeScaleAxisX) || double.IsNaN(ChangeScaleAxisY))) return null;

            if (double.IsInfinity(X) || double.IsInfinity(Y) || double.IsInfinity(distancePointAxisX) || double.IsInfinity(distancePointAxisY)|| double.IsInfinity(height) || (double.IsInfinity(ChangeScaleAxisX) || double.IsInfinity(ChangeScaleAxisY))) return null;

            position = new PositionXY()
            {
                //calculation of coordinates X and Y
                PositionX = distancePointAxisY * X  - (ChangeScaleAxisX * distancePointAxisY),
                PositionY = (ChangeScaleAxisY * distancePointAxisX) - (distancePointAxisX * Y - height)
            };

            return position;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                   if(position  != null) position.Dispose();
                   if(axis      != null) axis.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MathXY() { Dispose(false); }
        #endregion

    }
}
