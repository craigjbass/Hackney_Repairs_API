using System.Threading.Tasks;
using HackneyRepairs.Models;
using RepairsService;
using System;
using System.Collections.Generic;
using HackneyRepairs.Entities;

namespace HackneyRepairs.Interfaces
{
    public interface IHackneyRepairsService
    {
        Task<IEnumerable<RepairRequestEntity>> GetRepairByPropertyReference(string propertyReference);       
        Task<RepairCreateResponse> CreateRepairAsync(NewRepairRequest request);
        Task<RepairGetResponse> GetRepairRequestByReferenceAsync(RepairRefRequest request);
        Task<WorksOrderListResponse> CreateRepairWithOrderAsync(NewRepairTasksRequest repairRequest);
        Task<DrsOrder> GetWorkOrderDetails(string workOrderReference);
        Task<bool> UpdateRequestStatus(string repairRequestReference);
        Task<TaskListResponse> GetRepairTasksAsync(RepairRefRequest request);
        Task<int?> UpdateUHTVisitAndBlockTrigger(string workOrderReference, DateTime startDate, DateTime endDate, int orderId, int bookingId, string slotDetail);
        Task<WebResponse> IssueOrderAsync(WorksOrderRequest request);
        Task AddOrderDocumentAsync(string documentType, string workOrderReference, int workOrderId, string processComment);
    }
}