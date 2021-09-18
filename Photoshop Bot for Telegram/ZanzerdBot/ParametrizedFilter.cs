namespace ZanzerdBot
{
    public abstract class ParametrizedFilter<Tparam> : IFilter
       where Tparam : IParameters, new()
    {

        public ParameterInfo[] GetParameters()
        {
            return new Tparam().GetDesсription();
        }

        public Image Process(Image original, double[] values)
        {
            var parameters = new Tparam();
            parameters.Parse(values);
            return Process(original, parameters);
        }
        public abstract Image Process(Image photo, Tparam parameters);
    }
}
