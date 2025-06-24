using System;
using Avalonia;
using Avalonia.Media;

namespace ReactiveDream
{
    public static class GeometryHelper
    {
        static readonly Point _POINT_ZERO = new(0, 0);


        public static Rect ToRectWithXY(this Size size, Point xy)
            => new(xy, size);
        public static Rect ToRectWithX0Y0(this Size size)
            => size.ToRectWithXY(_POINT_ZERO);


        public static Rect WithXY(this Rect rect, double x, double y)
            => rect.WithXY(new(x, y));

        public static Rect WithXY(this Rect rect, Point xy)
            => new(_POINT_ZERO, xy);

        public static Rect WithX0Y0(this Rect rect)
            => rect.WithXY(_POINT_ZERO);
    }
}