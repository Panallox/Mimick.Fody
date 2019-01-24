using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes
{
    /// <summary>
    /// A class containing properties and methods which should be populated with values.
    /// </summary>
    public class ValueAttributes
    {
        /// <summary>
        /// Gets a number processed from the associated value attribute sourced from an application configuration document.
        /// </summary>
        [Value("{AppConfig.Number}")]
        public int AppConfigNumber { get; set; }

        /// <summary>
        /// Gets a string processed from the associated value attribute sourced from an application configuration document.
        /// </summary>
        [Value("{AppConfig.String}")]
        public string AppConfigString { get; set; }

        /// <summary>
        /// Gets a complex number processed from the associated value attribute.
        /// </summary>
        [Value("20 + 100 / 2 - 40 / 4 + (10 + 5 * 4)")]
        public int ComplexNumber { get; set; }

        /// <summary>
        /// Gets a complex string processed from the associated value attribute.
        /// </summary>
        [Value("'Number ' + (10 + 10 - 20 / 2 + 20 * 4)")]
        public string ComplexString { get; set; }

        /// <summary>
        /// Gets a computed number processed from the associated value attribute.
        /// </summary>
        [Value("10 * 5")]
        public int ComputedNumber { get; set; }

        /// <summary>
        /// Gets a computed string processed from the associated value attribute.
        /// </summary>
        [Value("'Test ' + 1 + ' Value'")]
        public string ComputedString { get; set; }

        /// <summary>
        /// Gets a configured number processed from the associated value attribute.
        /// </summary>
        [Value("{Configured.Method}")]
        public int ConfiguredNumber { get; set; }

        /// <summary>
        /// Gets a configured string processed from the associated value attribute.
        /// </summary>
        [Value("{Configured.Property}")]
        public string ConfiguredString { get; set; }

        /// <summary>
        /// Gets a configured number processed from the associated value attribute.
        /// </summary>
        [Value("{factory.Number}")]
        public int FactoryNumber { get; set; }

        /// <summary>
        /// Gets a configured string processed from the associated value attribute.
        /// </summary>
        [Value("{factory.String}")]
        public string FactoryString { get; set; }

        /// <summary>
        /// Gets a number processed from the associated value attribute sourced from a JSON document.
        /// </summary>
        [Value("{Values.JsonNumber}")]
        public int JsonNumber { get; set; }

        /// <summary>
        /// Gets a string processed from the associated value attribute sourced from a JSON document.
        /// </summary>
        [Value("{Values.JsonString}")]
        public string JsonString { get; set; }

        /// <summary>
        /// Gets a simple number processed from the associated value attribute.
        /// </summary>
        [Value("10")]
        public int SimpleNumber { get; set; }

        /// <summary>
        /// Gets a simple string processed from the associated value attribute.
        /// </summary>
        [Value("Test")]
        public string SimpleString { get; set; }

        /// <summary>
        /// Gets a number processed from the associated value attribute sourced from an XML document.
        /// </summary>
        [Value("{//Configurations/Attribute/@Value}")]
        public int XmlAttributeNumber { get; set; }

        /// <summary>
        /// Gets a string processed from the associated value attribute sourced from an XML document.
        /// </summary>
        [Value("{//Configurations/Element}")]
        public string XmlElementString { get; set; }

        /// <summary>
        /// Gets a number processed from the associated value attribute sourced from a YAML document.
        /// </summary>
        [Value("{Configurations.Number}")]
        public int YamlNumber { get; set; }

        /// <summary>
        /// Gets a string processed from the associated value attribute sourced from a YAML document.
        /// </summary>
        [Value("{Configurations.Text}")]
        public string YamlString { get; set; }

        /// <summary>
        /// Gets a value using a computed value from the parameter value attribute.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The computer number.</returns>
        public int GetComputedNumber([Value("10 + 50 / 2 - 100 / 5")] int number = 0) => number * 4;

        /// <summary>
        /// Gets a value using a computed value from the parameter value attribute.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The computed text.</returns>
        public string GetComputedString([Value("'Testing ' + (100 / 5 + 10)")] string text = null) => $"{text} Result";

        /// <summary>
        /// Gets a value using a computed value from the parameter value attribute asynchronously.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The computer number.</returns>
        public async Task<int> GetComputedNumberAsync([Value("10 + 50 / 2 - 100 / 5")] int number = 0) => await Task.Run(() => number * 4);

        /// <summary>
        /// Gets a value using a computed value from the parameter value attribute asynchronously.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The computed text.</returns>
        public async Task<string> GetComputedStringAsync([Value("'Testing ' + (100 / 5 + 10)")] string text = null) => await Task.Run(() => $"{text} Result");
    }
    
    /// <summary>
    /// A configuration class which provides configuration values based on member return values.
    /// </summary>
    [Configuration]
    public class ValueConfigurationProvider
    {
        /// <summary>
        /// Gets a configuration property value.
        /// </summary>
        [Provide("Configured.Property")]
        public string FromProperty => "Hello";

        /// <summary>
        /// Gets a configuration method value.
        /// </summary>
        /// <returns></returns>
        [Provide("Configured.Method")]
        public int FromMethod() => 123456;
    }
}
