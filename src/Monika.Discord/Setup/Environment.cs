using System;
using System.Collections.Generic;
using System.Text;

namespace Monika.Setup
{
    struct Environment : IEnvironment
    {
        public static Environment Development
            => new Environment("Development");
        public static Environment Staging
            => new Environment("Staging");
        public static Environment Production
            => new Environment("Production");

        public string EnvironmentName { get; }

        public Environment(string environmentName)
        {
            EnvironmentName = environmentName;
        }

#region Overrides
        public bool Equals(IEnvironment other)
        {
            if (other == null)
                return false;

            return other.EnvironmentName == EnvironmentName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IEnvironment);
        }

        public override int GetHashCode()
            => EnvironmentName.GetHashCode();

        public override string ToString()
        {
            return EnvironmentName;
        }
#endregion

        public bool IsEnvironment(IEnvironment environment)
            => Equals(environment);
    }
}
