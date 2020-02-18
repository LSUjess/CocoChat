using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using CocoChat.Models;
using Newtonsoft.Json.Linq;

namespace CocoChat.Controllers
{
    public class AwsController : Controller
    {
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);

        private static string tableName = "CocoChats";
        public int CreateChatItem(Chat chat)
        {
            Table chatTable = Table.LoadTable(client, tableName);

            var id = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var chatDoc = new Document();
            chatDoc["id"] = id;
            chatDoc["username"] = chat.username;
            chatDoc["text"] = chat.text;
            chatDoc["timeout"] = chat.timeout;
            chatDoc["expiration_date"] = id + chat.timeout;

            chatTable.PutItem(chatDoc);

            return id;
        }
        public JObject RetrieveChatById(int id)
        {
            Table chatTable = Table.LoadTable(client, tableName);
            GetItemOperationConfig config = new GetItemOperationConfig
            {
                AttributesToGet = new List<string> { "username", "text", "expiration_date" },
                ConsistentRead = true
            };
            Document document = chatTable.GetItem(id, config);

            if (document != null)
            {
                var jsonObject = ParseDocument(document);

                // calculate timeout with expiration date
                var returnJson = new JObject();
                returnJson.Add("username", (string)jsonObject.SelectToken("username"));
                returnJson.Add("text", (string)jsonObject.SelectToken("text"));
                var expiration = (int)jsonObject.SelectToken("expiration_date");
                var expDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiration);
                returnJson.Add("expiration_date", expDate);

                return returnJson;
            }

            return null;
        }
        public List<JObject> RetrieveChatByUsername(string username)
        {
            Table chatTable = Table.LoadTable(client, tableName);

            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition("username", ScanOperator.Equal, username);
            var now = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            scanFilter.AddCondition("expiration_date", ScanOperator.GreaterThan, now);

            ScanOperationConfig config = new ScanOperationConfig()
            {
                Filter = scanFilter,
                Select = SelectValues.SpecificAttributes,
                AttributesToGet = new List<string> { "id", "text" }
            };

            Search search = chatTable.Scan(config);

            var chatList = new List<JObject>();
            JObject jsonObject;
            List<Document> documentList;
            do
            {
                documentList = search.GetNextSet();

                foreach (var document in documentList)
                {
                    jsonObject = ParseDocument(document);
                    chatList.Add(jsonObject);
                }

                UpdateRead(chatList);

                return chatList;
            }
            while (!search.IsDone);
        }
        private async void UpdateRead(List<JObject> chatList)
        {
            Table chatTable = Table.LoadTable(client, tableName);

            var now = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            foreach (var chat in chatList)
            {
                var chatDoc = new Document();
                chatDoc["id"] = (int)chat.GetValue("id");
                chatDoc["expiration_date"] = now;

                chatTable.UpdateItem(chatDoc);
            }
        }
        public async void CleanupDbUnitTester(int id)
        {
            Table chatTable = Table.LoadTable(client, tableName);

            DeleteItemOperationConfig config = new DeleteItemOperationConfig
            {
                // Return the deleted item.
                ReturnValues = ReturnValues.AllOldAttributes
            };
            Document document = chatTable.DeleteItem(id, config);
        }
        private JObject ParseDocument(Document document)
        {
            var jsonObject = new JObject();

            foreach (var attribute in document.GetAttributeNames())
            {
                string stringValue = null;
                var value = document[attribute];
                if (value is Primitive)
                    stringValue = value.AsPrimitive().Value.ToString();
                else if (value is PrimitiveList)
                    stringValue = string.Join(",", (from primitive
                            in value.AsPrimitiveList().Entries
                                                    select primitive.Value).ToArray());
                jsonObject.Add(attribute, stringValue);
            }

            return jsonObject;
        }


    }
}