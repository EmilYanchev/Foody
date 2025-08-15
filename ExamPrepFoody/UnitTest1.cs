using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;

namespace ExamPrepFoody
{
    [TestFixture]
    public class ApiTests 
    {
        private RestClient _client;
        private static string createdFoodId;
        private static string baseUrl = "http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:86";

        [OneTimeSetUp]
        public void Setup()
        {
            string token = GetJwtToken("emil1998", "emil@1998");
            var options = new RestClientOptions(baseUrl)
            {
                Authenticator = new JwtAuthenticator(token)
            };
            _client = new RestClient(options); 
        }

        private string GetJwtToken(string username, string password)
        {
            var loginClient = new RestClient(baseUrl);
            var request = new RestRequest("/api/User/Authentication", Method.Post);
            request.AddJsonBody(new { username, password });
            var response = loginClient.Execute(request);
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            return json.GetProperty("accessToken").GetString() ?? string.Empty;
        }

        [Test, Order(1)]
        public void CreateFood_ShouldReturnCreated()
        {
            var food = new
            {
                Name = "New Food",
                Description = "New food description",
                Url = ""
            };
            
            var request = new RestRequest("/api/Food/Create", Method.Post);
            request.AddJsonBody(food);
            var response = _client.Execute(request);
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            createdFoodId = json.GetProperty("foodId").GetString() ?? string.Empty;
            
        }

        [Test, Order(2)]
        public void EditFood_ShouldReturnUpdated()
        {
            var changes = new[]
            {
                new {path = "/name", op ="replace", value ="updatedRandomFood" }
            };
            var request = new RestRequest($"/api/Food/Edit/{createdFoodId}", Method.Patch);
            request.AddJsonBody(changes);
            var response = _client.Execute(request);
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(json.GetProperty("msg").GetString(), Is.EqualTo("Successfully edited"));
            
        }

        [Test, Order(3)]
        public void GetAllFoods_ShouldReturnAllFoods()
        {
            var request = new RestRequest("/api/Food/All", Method.Get);
            var response = _client.Execute(request);
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var foods = JsonSerializer.Deserialize<List<JsonElement>>(response.Content);
            Assert.That(foods, Is.Not.Empty);
        }

        [Test, Order(4)]
        public void DeleteFood_ShouldReturnDeleted()
        {
            var request = new RestRequest($"/api/Food/Delete/{createdFoodId}", Method.Delete);
            var response = _client.Execute(request);
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            Assert.That(json.GetProperty("msg").GetString(), Is.EqualTo("Deleted successfully!"));
        }

        [Test, Order(5)]
        public void CreateFood_WithOutRequiredField_ShouldReturnBadRequest()
        {
            var food = new
            {
                Name = "",
                Description = "",
                Url = ""
            };
            
            var request = new RestRequest("/api/Food/Create", Method.Post);
            request.AddJsonBody(food);
            var response = _client.Execute(request);
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test, Order(6)]
        public void EditFood_ThatIsNonExistingFood_ShouldReturnNotFound()
        {
            string fakeId = "12341";
            var changes = new[]
            {
                new {path = "/name", op ="replace", value ="updatedRandomFood" }
            };
            var request = new RestRequest($"/api/Food/Edit/{fakeId}", Method.Patch);
            request.AddJsonBody(changes);
            var response = _client.Execute(request);
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            
           Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
           Assert.That(json.GetProperty("msg").GetString(), Is.EqualTo("No food revues..."));
        }

        [Test, Order(7)]
        public void DeleteFood_WithNotExistingId_ShouldReturnBadRequest()
        {
            string fakeId = "12341";
            var request = new RestRequest($"/api/Food/Delete/{fakeId}", Method.Delete);
            var response = _client.Execute(request);
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            Assert.That(json.GetProperty("msg").GetString(), Is.EqualTo("Unable to delete this food revue!"));
            
            
            
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _client?.Dispose();
        }
    }
}