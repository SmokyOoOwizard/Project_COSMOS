namespace COSMOS.Core
{
    public struct Rect
    {
        public double X;
        public double Y;
        public double Height;
        public double Width;

        public double Left { get { return X; } }
        public double Right { get { return X + Width; } }
        public double Top { get { return Y + Height; } }
        public double Down { get { return Y; } }

        public Rect(double x, double y, double height, double width)
        {
            X = x;
            Y = y;
            Height = height;
            Width = width;
        }
        public bool Intersect(Rect rect)
        {
            return !(rect.Left > Right || rect.Right < Left || rect.Top < Down || rect.Down > Top);
        }
        public bool Inside(Rect rect)
        {
            return (rect.Left > Left && rect.Right < Right && rect.Top < Top && rect.Down > Down);
        }
        public Rect GetLTQuad()
        {
            return new Rect(X, Y + Height / 2, Height / 2, Width / 2);
        }
        public Rect GetLBQuad()
        {
            return new Rect(X, Y, Height / 2, Width / 2);
        }
        public Rect GetRTQuad()
        {
            return new Rect(X + Width / 2, Y + Height / 2, Height / 2, Width / 2);
        }
        public Rect GetRBQuad()
        {
            return new Rect(X + Width / 2, Y, Height / 2, Width / 2);
        }
    }
}
