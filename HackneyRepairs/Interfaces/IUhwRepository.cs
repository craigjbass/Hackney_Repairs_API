using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Models;

namespace HackneyRepairs.Interfaces
{
    public interface IUhwRepository
    {
        Task AddOrderDocumentAsync(string documentType, string workOrderReference, int workOrderId, string processComment);
    }
}
