﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Entities;
using HackneyRepairs.Models;

namespace HackneyRepairs.Interfaces
{
    public interface IUhtRepository
    {
        Task<DrsOrder> GetWorkOrderDetails(string workOrderReference);
        Task<bool> UpdateRequestStatus(string repairRequestReference);
        Task<int?> UpdateVisitAndBlockTrigger(string workOrderReference, DateTime startDate, DateTime endDate, int orderId, int bookingId, string slotDetail);
        Task<bool> GetMaintainableFlag(string propertyReference);
		Task<UHWorkOrder> GetWorkOrder(string workOrderReference);
		Task<IEnumerable<UHWorkOrderBase>> GetWorkOrderByPropertyReference(string propertyId);
		Task<IEnumerable<RepairRequestBase>> GetRepairRequests(string propertyReference);
		Task<IEnumerable<UhtAppointmentEntity>> GetAppointmentsByWorkOrderReference(string workOrderReference);
    }
}
