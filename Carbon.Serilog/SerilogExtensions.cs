using Carbon.Common;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Enrichers.Sensitive;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Carbon.Serilog
{
    /// <summary>
    /// Extends Serilog logging library
    /// </summary>
    public static class SerilogExtensions
    {
        /// <summary>
        /// Retrieve masking operator list using their class name which contained in the <see cref="SerilogExtensions"/> class.
        /// </summary>
        /// <param name="nameList">Masking operator names which inherits from <see cref="IMaskingOperator"/></param>
        /// <returns>Matched list of <see cref="IMaskingOperator"/>s</returns>
        /// <exception cref="ArgumentException">Throws if given name is not a known Operator by the extension.</exception>
        public static List<IMaskingOperator> GetMatchingMaskingOperators(params string[] nameList)
        {
            var operatorList = new List<IMaskingOperator>();
            if (nameList != null && nameList.Any())
            {
                foreach (var operatorName in nameList)
                {
                    var classAddress = $"{typeof(SerilogExtensions).FullName}+{operatorName}";
                    Type type = Type.GetType(classAddress);

                    if (type == null)
                        throw new ArgumentException($"Given Masking Operator Name is not valid! - {operatorName}");


                    operatorList.Add((IMaskingOperator)Activator.CreateInstance(type));
                }
            }
            return operatorList;
        }
        /// <summary>
        /// Masks IpV4 and IPV6 adderesses using regex.
        /// </summary>
        public class IPMaskingOperator : RegexMaskingOperator
        {
            private const string _regex = @"(\b(?:(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d\d?)(?::\d{0,5})?\b)|(([0-9A-Fa-f]{1,4}:){7}[0-9A-Fa-f]{1,4}|(\d{1,3}\.){3}\d{1,3})";
            public IPMaskingOperator() : base(_regex, RegexOptions.IgnoreCase | RegexOptions.Compiled)
            {
            }

            protected override bool ShouldMaskMatch(Match match)
            {
                if (IPAddress.TryParse(match.Value, out _) || IPAddress.TryParse(match.Value.Substring(0, match.Value.LastIndexOf(':')), out _))
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Masks Windows & Unix paths as well as files using regex.
        /// </summary>
        public class PathMaskingOperator : RegexMaskingOperator
        {
            private const string _regex = @"((((\\|\/).*)+\.[\w:]+))|(((\\|\/).*)+)|([\w]:(\/|\\)?(((((\\|\/).*)+\.[\w]+))|(((\\|\/).*)+)))";
            public PathMaskingOperator() : base(_regex, RegexOptions.IgnoreCase | RegexOptions.Compiled)
            {
            }

            protected override bool ShouldMaskMatch(Match match)
            {
                if (match.Value.IndexOfAny(Path.GetInvalidPathChars()) == -1)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Masks different type of URLs as well as site addresses using regex.
        /// </summary>
        public class URLMaskingOperator : RegexMaskingOperator
        {
            private const string _regex = @"((http|ftp|https|mqtt|mqtts):\/\/([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?)|(^((localhost)|((?!-)[A-Za-z0-9-]{1,63}(?<!-)\.)+[A-Za-z]{2,253})$)";
            public URLMaskingOperator() : base(_regex, RegexOptions.IgnoreCase | RegexOptions.Compiled)
            {
            }

            protected override bool ShouldMaskMatch(Match match)
            {
                if (Uri.IsWellFormedUriString(match.Value, UriKind.Absolute))
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Masks GUID using regex
        /// </summary>
        public class GuidMaskingOperator : RegexMaskingOperator
        {
            private const string _regex = @"\b[A-F0-9a-f]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12}\b";
            public GuidMaskingOperator() : base(_regex, RegexOptions.IgnoreCase | RegexOptions.Compiled)
            {
            }
        }

        /// <summary>
        /// Masks Json property values with given property name. Indenting and "," is prohibited for usage.
        /// Use <see cref="Operators"/> property for accessing operator list.
        /// </summary>
        /// <remarks>Creates a <see cref="PropertyMaskingOperator"/> for every given property name.</remarks>
        public class PropertyMaskingOperatorCapsule
        {
            /// <summary>
            /// Masking operators for given properties
            /// </summary>
            public List<PropertyMaskingOperator> Operators { get; }
            /// <summary>
            /// Creates a <see cref="PropertyMaskingOperator"/> for every given property name.
            /// </summary>
            /// <param name="properties">Json properties to be masked.</param>
            public PropertyMaskingOperatorCapsule(params string[] properties)
            {
                Operators = new List<PropertyMaskingOperator>();
                if (properties != null)
                {
                    var propRegex = string.Join("|", properties);
                    Operators.Add(new PropertyMaskingOperator("(\"?(" + propRegex + @")""?\s*(:)\s*((((\{+.*)(?=(\}))))|(([^,\n\r\{\}])*(?!\{))))(?![^,\n\r\{\}])"));
                }
            }

            /// <summary>
            /// Masking operator for a property. Uses regex for masking.
            /// </summary>
            public class PropertyMaskingOperator : RegexMaskingOperator
            {
                public PropertyMaskingOperator(string regex) : base(regex, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)
                {
                }
                protected override string PreprocessInput(string input)
                {
                    if (input.Contains("\n"))
                    {
                        input = input.Replace("\n", " ");
                    }
                    if (input.Contains("\r"))
                    {
                        input = input.Replace("\r", " ");
                    }
                    return input;
                }
                protected override string PreprocessMask(string mask, Match match)
                {

                    var parts = match.Value.Split(':');
                    return parts[0] + ":" + mask;
                }
            }
        }


        public static ILogger CreateLogger(IConfiguration configuration)
        {
            var serilogSettings = configuration.GetSection("Serilog").Get<SerilogSettings>();

            if (serilogSettings == null)
                throw new ArgumentNullException(nameof(serilogSettings), "Serilog settings cannot be empty!");

            ILogger logger;
            if (serilogSettings.SensitiveDataMasking != null)
            {

                Console.WriteLine($"Sensitive log masking enabled in Serilog settings");
                logger = new LoggerConfiguration().ReadFrom.Configuration(configuration)
                    .Enrich.WithSensitiveDataMasking(options =>
                    {
                        options.MaskingOperators.AddRange(new PropertyMaskingOperatorCapsule(serilogSettings.SensitiveDataMasking.PropertyNames).Operators);
                        options.MaskingOperators.AddRange(GetMatchingMaskingOperators(serilogSettings.SensitiveDataMasking.Operators));
                        options.MaskProperties.AddRange(serilogSettings.SensitiveDataMasking.PropertyNames);
                    })
                    .CreateLogger();

            }
            else
                logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            return logger;
        }
    }

}