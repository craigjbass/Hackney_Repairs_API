using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HackneyRepairs.DbContext;
using HackneyRepairs.Entities;
using HackneyRepairs.Interfaces;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

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

        // this may have to return a list of appointments and take the number of them to be returned
        public async Task<DrsAppointmentEntity> GetAppointment(string workOrderReference)
        {
            try
            {
                DrsAppointmentEntity appointment;
                using (var connection = new MySqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    string query = $"SELECT * FROM p_job WHERE name = '{workOrderReference}'";
                    appointment = connection.Query<DrsAppointmentEntity>(query).FirstOrDefault();
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
