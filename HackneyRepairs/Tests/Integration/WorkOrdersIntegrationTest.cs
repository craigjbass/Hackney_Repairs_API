﻿using System.Threading.Tasks;
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
            _server = new TestServer(new WebHostBuilder().UseStartup<TestStartup>());
            _client = _server.CreateClient();
        }

        #region GET Work order by work order reference tests
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
			var result = await _client.GetAsync("v1/work_orders/0");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
        #endregion

		#region GET All work orders by property reference tests
        [Fact]
        public async Task when_making_a_request_without_a_property_reference_returns_a_400()
        {
            var result = await _client.GetAsync("v1/work_orders");

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task when_making_a_valid_request_returns_a_200_result_with_a_list_of_work_orders()
        {
            var result = await _client.GetAsync("v1/work_orders?propertyreference=12345678");
            var jsonresult = await result.Content.ReadAsStringAsync();
            var workOrder = JsonConvert.DeserializeObject<List<UHWorkOrderBase>>(jsonresult).ToList();

            Assert.IsType<List<UHWorkOrderBase>>(workOrder);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task when_making_a_valid_query_with_multiple_property_references_returns_a_200_result_with_a_list_of_work_orders()
        {
            var result = await _client.GetAsync("v1/work_orders?propertyreference=12345678&propertyreference=56781234");
            var jsonresult = await result.Content.ReadAsStringAsync();
            var workOrder = JsonConvert.DeserializeObject<List<UHWorkOrderBase>>(jsonresult).ToList();

            Assert.IsType<List<UHWorkOrderBase>>(workOrder);
        }

        [Fact]
        public async Task when_one_property_is_missing_still_returns_a_200_result_with_a_list_of_work_orders()
        {
            var result = await _client.GetAsync("v1/work_orders?propertyreference=12345678&propertyreference=0");
            var jsonresult = await result.Content.ReadAsStringAsync();
            var workOrder = JsonConvert.DeserializeObject<List<UHWorkOrderBase>>(jsonresult).ToList();

            Assert.IsType<List<UHWorkOrderBase>>(workOrder);
        }

        [Fact]
        public async Task returns_an_empty_list_with_status_200_when_no_workorders_are_found()
        {
            var result = await _client.GetAsync("v1/work_orders?propertyreference=9999999999");
            var jsonResult = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("[]", jsonResult);
        }

		[Fact]
        public async Task returns_an_empty_list_with_status_200_when_no_properties_are_found()
        {
            var result = await _client.GetAsync("v1/work_orders?propertyreference=0");
            var jsonResult = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("[]", jsonResult);
        }
        #endregion

        #region GET Work order notes test
        [Fact]
        public async Task return_a_200_result_with_notes_json_for_valid_request_by_reference()
        {
            var result = await _client.GetAsync("v1/work_orders/12345678/notes");
            var jsonResult = await result.Content.ReadAsStringAsync();
            var notes = JsonConvert.DeserializeObject<List<NotesEntity>>(jsonResult);

            Assert.IsType<List<NotesEntity>>(notes);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_an_empty_list_when_no_notes_found_for_workorder()
        {
            var result = await _client.GetAsync("v1/work_orders/9999999999/notes");
			var jsonResult = await result.Content.ReadAsStringAsync();
			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("[]", jsonResult);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

		[Fact]
        public async Task return_404_when_workorder_not_found()
        {
            var result = await _client.GetAsync("v1/work_orders/00/notes");
            var jsonResult = await result.Content.ReadAsStringAsync();
			Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
        #endregion

        #region GET work order feed tests
        [Fact]
        public async Task return_a_200_result_with_list_workOrders_json_for_valid_request_in_feed()
        {
            var result = await _client.GetAsync("v1/work_orders/feed?startId=12345678");
            var jsonResult = await result.Content.ReadAsStringAsync();
            var workOrders = JsonConvert.DeserializeObject<List<UHWorkOrderFeed>>(jsonResult).ToList();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.IsType<List<UHWorkOrderFeed>>(workOrders);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_200_result_with_empty_array_when_no_results()
        {
            var result = await _client.GetAsync("v1/work_orders/feed?startId=99999999");
            var jsonResult = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("[]", jsonResult);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_500_result_when_exception_is_thrown_in_service()
        {
            var result = await _client.GetAsync("v1/work_orders/feed?startID=11550853");
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }
        #endregion
    }
}
