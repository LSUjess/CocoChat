using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Web.Http;
using CocoChat.Models;
using Newtonsoft.Json.Linq;

namespace CocoChat.Controllers
{
    public class ChatController : ApiController
    {
        [Route("api/chats")]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] {"Please add an Id or Username."};
        }

        // GET api/chats/1581955292
        [Route("api/chats/{id:int}")]
        [HttpGet]
        public HttpResponseMessage Get([FromUri()] int id)
        {
            try
            {
                var crud = new AwsController();

                var results = crud.RetrieveChatById(id);
                if (results.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
                else
                {
                    return Request.CreateResponse("Chats not found.");
                }
            }
            catch
            {
                return Request.CreateResponse("Something went wrong");
            }
        }

        // GET api/chats/testcat
        [Route("api/chats/{username}")]
        [HttpGet]
        public HttpResponseMessage GetByUsername([FromUri()] string username)
        {
            try
            {
                var crud = new AwsController();

                var results = crud.RetrieveChatByUsername(username);
                if (results.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }

                return Request.CreateResponse("Chats not found.");
            }
            catch
            {
                return Request.CreateResponse("Something went wrong");
            }

        
        }
        
        [HttpPost]
        [Route("api/chats/{username}/{text}/{timeout}")]
        public HttpResponseMessage  AddChat([FromUri()] string username, [FromUri()] string text,
            [FromUri()] int timeout)
        {
            try
            {
                var crud = new AwsController();
                var chat = new Chat
                {
                    text = text,
                    username = username,
                    timeout = timeout
                };

                var id = crud.CreateChatItem(chat);
                if (id > 0)
                {
                    var jsonObject = new JObject();
                    jsonObject.Add("id", id);

                    return Request.CreateResponse(HttpStatusCode.Created, jsonObject);
                }
                else
                {
                    return Request.CreateResponse("Something went wrong");
                }
            }
            catch
            {
                return Request.CreateResponse("Something went wrong");
            }

        
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        public void Cleanup(int id)
        {
            var crud = new AwsController();

            crud.CleanupDbUnitTester(id);
        }
    }
}