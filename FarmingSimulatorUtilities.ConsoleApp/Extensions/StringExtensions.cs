using System.Text.RegularExpressions;

namespace FarmingSimulatorUtilities.ConsoleApp.Extensions
{
    public static class StringExtensions
    {
        private const string EmailRegex = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$";

        public static bool IsEmail(this string input)
        {
            var regex = new Regex(EmailRegex);
            return regex.Match(input).Success;
        }
    }
}