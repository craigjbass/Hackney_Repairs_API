using System;
using System.Threading.Tasks;
using HackneyRepairs.Entities;

namespace HackneyRepairs.Interfaces
{
    public interface IDrsRepository
    {
        Task<DrsAppointmentEntity> GetDrsAppointment(string workOrderReference);
    }
}
