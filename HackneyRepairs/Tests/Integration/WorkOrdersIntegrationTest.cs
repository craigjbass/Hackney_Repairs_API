using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using HackneyRepairs.Entities;
using System.Collections.Generic;
using System.Linq;
using HackneyRepairs.Models;

namespace HackneyRepairs.Tests.Integration
{
    public class WorkOrdersIntegrationTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public WorkOrdersIntegrationTest()
        {
            _server = new TestServer(new WebHostBuilder()
            .UseStartup<TestStartup>());
            _client = _server.CreateClient();
        }

        #region GetWorkOrder endpoint
        [Fact]
        public async Task return_a_200_result_with_workOrder_json_for_valid_request_by_reference()
        {
            var result = await _client.GetAsync("v1/work_orders/12345678");
            var jsonResult = await result.Content.ReadAsStringAsync();
			var workOrder = JsonConvert.DeserializeObject<UHWorkOrder>(jsonResult);

			Assert.IsType<UHWorkOrder>(workOrder);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_404_result_for_no_workorder_matching_reference()
        {
            var result = await _client.GetAsync("v1/work_orders/9999999999");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
		#endregion

		#region GetWorkOrder Notes endpoint
		public async Task return_a_200_result_with_notes_json_for_valid_request_by_reference()
        {
            var result = await _client.GetAsync("v1/work_orders/12345678/notes");
            var jsonResult = await result.Content.ReadAsStringAsync();
			var notes = JsonConvert.DeserializeObject<NotesEntity>(jsonResult);

			Assert.IsType<IEnumerable<NotesEntity>>(notes);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

		[Fact]
        public async Task return_a_404_result_when_no_notes_found_for_workorder()
        {
            var result = await _client.GetAsync("v1/work_orders/9999999999");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        #endregion

		#region GetWorkOrderByPropertyReference endpoint
		[Fact]
        public async Task return_a_200_result_with_list_workOrders_json_for_valid_request()
        {
            var result = await _client.GetAsync("v1/work_orders?propertyreference=12345678");
            var jsonresult = await result.Content.ReadAsStringAsync();
			var workOrder = JsonConvert.DeserializeObject<List<UHWorkOrderBase>>(jsonresult).ToList();

			Assert.IsType<List<UHWorkOrderBase>>(workOrder);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_404_result_for_no_results()
        {
            var result = await _client.GetAsync("v1/work_orders?propertyreference=9999999999");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
        #endregion
    }
}
