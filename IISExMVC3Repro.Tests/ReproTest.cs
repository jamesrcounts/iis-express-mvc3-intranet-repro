using System;
using System.Net;
using ApprovalTests.Asp;
using ApprovalTests.Asp.Mvc;
using ApprovalTests.Reporters;
using IISExMVC3Repro.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IISExMVC3Repro.Tests
{
    [TestClass]
    [UseReporter(typeof(DiffReporter))]
    public class ReproTest
    {
        public ReproTest()
        {
            PortFactory.MvcPort = 2609;
        }

        /// <summary>
        /// MvcApprovals fails with 401.2 because no user id was sent to IIS Express.
        /// See logs at ...\Documents\IISExpress\Logs
        /// </summary>
        [TestMethod]
        public void ApproveIndex()
        {
            try
            {
                MvcApprovals.VerifyMvcPage(new HomeController().Index);
                Assert.Fail("Expected Exception.");
            }
            catch (Exception ex)
            {
                ApprovalTests.Approvals.Verify(ex.Message);
            }
        }

        /// <summary>
        /// IIS Express returns 401.2 because WebClient fails to send any user id.
        /// See logs at ...\Documents\IISExpress\Logs
        /// </summary>
        [TestMethod]
        public void WebClientWithDefaultSettings()
        {
            try
            {
                new WebClient().DownloadString(@"http://localhost:2609/Home/Index");
                Assert.Fail("Expected Exception.");
            }
            catch (WebException ex)
            {
                ApprovalTests.Approvals.Verify(ex.Message);
            }
        }

        /// <summary>
        /// WebClient sends user id and sucessfully retrieves the page from IIS Express
        /// </summary>
        [TestMethod]
        public void WebClientWithCredentials()
        {
            var client = new WebClient() { UseDefaultCredentials = true };
            var result = client.DownloadString(@"http://localhost:2609/Home/Index");
            ApprovalTests.Approvals.Verify(result);
        }
    }
}