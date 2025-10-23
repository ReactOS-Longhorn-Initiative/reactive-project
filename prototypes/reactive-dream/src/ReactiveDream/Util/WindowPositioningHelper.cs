using System;

namespace ReactiveDream
{
    public readonly struct WindowBoundsAxis
    {
        public readonly double Position;
        public readonly double Size;
        public WindowBoundsAxis(double position, double size)
        {
            Position = position;
            Size = size;
        }

        public override string ToString()
            => $"(axis {Position}, {Size})";
    }


    public static class WindowPositioningHelper
    {
        public static WindowBoundsAxis GetValidWindowPosForAxis(WindowBoundsAxis vmRequested, WindowBoundsAxis addWindowParams, double workingAreaSize)
        {
            double retPos = vmRequested.Position;
            double retSize = vmRequested.Size;


            double addWindowParamPos = addWindowParams.Position;
            if (IsDoubleSane(addWindowParamPos) && (addWindowParamPos >= 0))
                retPos = addWindowParamPos;


            double addWindowParamSize = addWindowParams.Size;
            if (IsDoubleSane(addWindowParamSize) && (addWindowParamSize > 0))
                retSize = addWindowParamSize;


            if (retPos < 0)
                retPos = workingAreaSize + retPos;


            return new(retPos, retSize);
        }


        static bool IsDoubleSane(double d)
        {
            if (double.IsNaN(d))
                return false;
            else if (double.IsInfinity(d))
                return false;
            else
                return true;
        }
    }
}