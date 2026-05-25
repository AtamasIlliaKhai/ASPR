namespace ASPR_Practice1
{
    internal class GraphicalPoint
    {
        public double X1 { get; private set; }
        public double X2 { get; private set; }

        public GraphicalPoint(double x1, double x2)
        {
            X1 = x1;
            X2 = x2;
        }

        public override string ToString()
        {
            return "(" + X1.ToString("0.####") + "; " + X2.ToString("0.####") + ")";
        }
    }
}