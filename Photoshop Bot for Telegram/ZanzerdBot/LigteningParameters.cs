namespace ZanzerdBot
{
    public class LighteningParameters : IParameters
    {
        public double Coefficient { get; set; }
        public ParameterInfo[] GetDesсription()
        {
            return new[]
            {
                new ParameterInfo { Name="Коэффициент", MaxValue=10, MinValue=0, Increment=0.1, DefaultValue=1 }

            };
        }

        public void Parse(double[] parameters)
        {
            Coefficient = parameters[0];
        }
    }
}
