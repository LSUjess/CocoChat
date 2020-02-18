using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using CocoChat.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CocoChat.Tests.Controllers
{
    [TestClass()]
    public class ChatControllerTests
    {
        private int Setup()
        {
            // Arrange
            ChatController controller = new ChatController();
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            var response = controller.AddChat("tester", "This is an unit test", 2000);
            var jsonString = response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(jsonString.Result);

            response.Dispose();
            return (int) json["id"];
        }

        private void CleanupDB(int id)
        {
            // Arrange
            ChatController controller = new ChatController();
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            controller.Cleanup(id);
        }

        [TestMethod()]
        public void GetByIdTest()
        {
            // Arrange
            ChatController controller = new ChatController();
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
            var id = Setup();

            // Act
            var response = controller.Get(id);
            var contentResult = response.Content;
            // Assert the result  
            Assert.IsNotNull(contentResult);

            if (response != null)
            {
                var jsonString = response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(jsonString.Result);
                Assert.AreEqual("tester", json["username"]);
            }

            CleanupDB(id);
            response.Dispose();
        }
        
        [TestMethod()]
        public void GetByUsernameTest()
        {
            // Arrange
            ChatController controller = new ChatController();
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            // Act
            var id = Setup();
            var response = controller.GetByUsername("tester");
            var contentResult = response.Content;
            // Assert the result  
            Assert.IsNotNull(contentResult);

            if (response != null)
            {
                var jsonString = response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<JArray>(jsonString.Result);
                Assert.AreEqual("This is an unit test", json[0]["text"]);
            }

            CleanupDB(id);
            response.Dispose();
        }
    }
}