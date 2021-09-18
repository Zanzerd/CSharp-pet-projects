namespace ZanzerdBot
{
    public struct Pixel
    {
        public double CheckValue(double val)
        {
            //if (val > 1.0 || val < 0.0)  throw new ArgumentException("Значение > 1.0 или < 0.0");

            return val;
        }
        private double r;
        private double g;
        private double b;

        public double R
        {
            get { return r; }

            set
            {
                r = CheckValue(value);
            }
        }

        public double G
        {
            get { return g; }

            set
            {
                g = CheckValue(value);
            }
        }

        public double B
        {
            get { return b; }

            set
            {
                b = CheckValue(value);
            }
        }
        public static Pixel operator *(Pixel p, double d)
        {
            return new Pixel { R = p.R * d, G = p.G * d, B = p.B * d };
        }

        public static Pixel operator *(double d, Pixel p)
        {
            return p * d;
        }

        public static Pixel Trim(Pixel p)
        {
            Pixel p2 = new Pixel { };
            if (p.R > 1.0) p2.R = 1.0;
            else if (p.R < 0.0) p2.R = 0.0;
            else p2.R = p.R;

            if (p.G > 1.0) p2.G = 1.0;
            else if (p.G < 0.0) p2.G = 0.0;
            else p2.G = p.G;

            if (p.B > 1.0) p2.B = 1.0;
            else if (p.B < 0.0) p2.B = 0.0;
            else p2.B = p.B;

            return p2;
        }
    }
}
