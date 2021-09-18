namespace ZanzerdBot
{
        public interface IParameters
        {
            ParameterInfo[] GetDesсription();
            void Parse(double[] parameters);
        }
}
