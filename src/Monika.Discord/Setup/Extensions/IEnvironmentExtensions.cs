namespace Monika.Setup
{
    static class IEnvironmentExtensions
    {
        public static bool IsDevelopment(this IEnvironment env)
        {
            return env.IsEnvironment(Environment.Development);
        }

        public static bool IsEnvironment(this IEnvironment env,
            string environmentString)
        {
            foreach (var environment in environmentString.Split(','))
            {
                if (env.EnvironmentName == environment)
                    return true;
            }

            return false;
        }
    }
}
