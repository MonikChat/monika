using System;
using System.Collections.Generic;
using System.Text;

namespace Monika.Setup
{
    public interface IEnvironment : IEquatable<IEnvironment>
    {
        string EnvironmentName { get; }

        bool IsEnvironment(IEnvironment environment);
    }
}
