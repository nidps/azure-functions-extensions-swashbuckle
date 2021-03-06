﻿using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace TestFunction
{
    [ApiExplorerSettings(GroupName = "testee")]
    public class TestController
    {
        [ProducesResponseType(typeof(TestModel[]), (int) HttpStatusCode.OK)]
        [FunctionName("TestGets")]
        [QueryStringParameter("expand", "it is expand parameter", DataType = typeof(int))]
        public async Task<IActionResult> Gets([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "test")]
            HttpRequest request)
        {
            return new OkObjectResult(new[] {new TestModel(), new TestModel()});
        }

        [ProducesResponseType(typeof(TestModel), (int) HttpStatusCode.OK)]
        [FunctionName("TestGet")]
        public Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "test/{id}")]
            HttpRequest request, int id)
        {
            return Task.FromResult<IActionResult>(new OkObjectResult(new TestModel()));
        }

        [ProducesResponseType(typeof(TestModel), (int) HttpStatusCode.OK)]
        [QueryStringParameter("name", "this is name", DataType = typeof(string), Required = true)]
        [FunctionName("TestGetCat")]
        public Task<IActionResult> GetCat(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "cat/{id}/{testId?}")]
            HttpRequest request, int id, int? testId)
        {
            return Task.FromResult<IActionResult>(new OkObjectResult(new TestModel()));
        }

        [ProducesResponseType(typeof(TestModel), (int) HttpStatusCode.Created)]
        [FunctionName("TestAdd")]
        public Task<IActionResult> Add([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "test")]
            TestModel testModel)
        {
            return Task.FromResult<IActionResult>(new CreatedResult("", testModel));
        }

        [ProducesResponseType(typeof(TestModel), (int) HttpStatusCode.Created)]
        [QueryStringParameter("test", "test", Required = false)]
        [FunctionName("TestAddGet")]
        public async Task<IActionResult> AddAndGet(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "testandget")]
            [RequestBodyType(typeof(TestModel), "testmodel")]
            HttpRequest httpRequest)
        {
            if (httpRequest.Method.ToLower() == "post")
            {
                using (var reader = new StreamReader(httpRequest.Body))
                {
                    var json = await reader.ReadToEndAsync();
                    var testModel = JsonConvert.DeserializeObject<TestModel>(json);
                    return new CreatedResult("", testModel);
                }
            }

            return new OkResult();
        }

        public class TestModel
        {
            /// <summary>
            ///     Id
            /// </summary>
            [Required]
            public int Id { get; set; }

            /// <summary>
            ///     Name
            /// </summary>
            [Required]
            [MaxLength(512)]
            public string Name { get; set; }

            /// <summary>
            ///     Description
            /// </summary>
            [MaxLength(10240)]
            public string Description { get; set; }
        }
    }
}