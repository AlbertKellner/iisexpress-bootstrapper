﻿using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace IISExpressBootstrapper.AcceptanceTests
{
    [TestFixture]
    public class IISExpressHostTests
    {
        private IISExpressHost host;
        private IDictionary<string, string> environmentVariables;

        [SetUp]
        public void SetUp()
        {
            environmentVariables = new Dictionary<string, string> { { "Foo1", "Bar1" }, { "Sample2", "It work's!" } };

            host = new IISExpressHost("IISExpressBootstrapper.SampleWebApp", 8088, environmentVariables: environmentVariables);
        }

        [Test]
        public void ShouldRunTheWebApplication()
        {
            var request = (HttpWebRequest)WebRequest.Create("http://localhost:8088/");

            var response = (HttpWebResponse)request.GetResponse();

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            host.Dispose();
        }

        [Test]
        public void ShouldSetEnvironmentVariables()
        {
            TestEnvironmentVariable("Foo1", environmentVariables["Foo1"]);
            TestEnvironmentVariable("Sample2", environmentVariables["Sample2"]);

            host.Dispose();
        }

        private static void TestEnvironmentVariable(string variable, string expected)
        {
            const string url = "http://localhost:8088/Home/EnvironmentVariables?name={0}";

            var request = (HttpWebRequest) WebRequest.Create(string.Format(url, variable));

            var response = (HttpWebResponse) request.GetResponse();
            var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            var content = sr.ReadToEnd();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Be(expected);
        }

        [Test]
        public void ThrowExceptionWhenNotFoundIISExpressPath()
        {
            const string iisExpressPath = @"Z:\Foo\Bar\iis.exe";

            var config = new Configuration { IISExpressPath = iisExpressPath };

            Action action = () => new IISExpressHost(config);

            action.ShouldThrow<IISExpressNotFoundException>();
        }
    }
}
