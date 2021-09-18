using System;
using System.Collections.Generic;
using System.Text;

namespace ZanzerdBot
{
    public class EmptyParameters : IParameters
    {
            public ParameterInfo[] GetDesсription()
            {
                return new ParameterInfo[0];
            }

            public void Parse(double[] parameters)
            {

            }
    }
}
