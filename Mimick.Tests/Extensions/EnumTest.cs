using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Mimick.Tests.Extensions
{
    [TestFixture]
    public class EnumTest
    {
        [Test]
        public void GetDescriptionShouldReturnNullIfNoAttribute() => Assert.IsNull(Options.None.GetDescription());

        [Test]
        public void GetDescriptionShouldReturnValueIfAttributeExists() => Assert.AreEqual("Take a shower", Options.Shower.GetDescription());

        public enum Options
        {
            None,

            [System.ComponentModel.Description("Take a shower")]
            Shower,

            [System.ComponentModel.Description("Take a bath")]
            Batch,
        }
    }
}
