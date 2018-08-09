using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HackneyRepairs.DbContext;
using HackneyRepairs.Entities;
using HackneyRepairs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HackneyRepairs.Repository
{
    public class DrsRepository
    {
        private DrsDbContext _context;
        private ILoggerAdapter<DrsRepository> _logger;

        public DrsRepository(DrsDbContext context, ILoggerAdapter<DrsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DrsAppointment> GetDrsAppointment(string workOrderReference)
        {
            try
            {
                DrsAppointment appointment;
                using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    string query = $"SELECT * FROM p_job WHERE name = '{workOrderReference}'";
                    appointment = connection.Query<DrsAppointment>(query).FirstOrDefault();
                }
                return appointment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new DrsRepositoryException();
            }
        }
    }

    public class DrsRepositoryException : Exception {}
}
