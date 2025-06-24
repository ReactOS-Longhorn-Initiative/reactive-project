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
            if (double.IsNormal(addWindowParamPos) && (addWindowParamPos >= 0))
                retPos = addWindowParamPos;


            double addWindowParamSize = addWindowParams.Size;
            if (double.IsNormal(addWindowParamSize) && (addWindowParamSize > 0))
                retSize = addWindowParamSize;


            if (retPos < 0)
                retPos = workingAreaSize + retPos;


            return new(retPos, retSize);
        }
    }
}