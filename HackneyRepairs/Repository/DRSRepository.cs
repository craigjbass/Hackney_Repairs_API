using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HackneyRepairs.DbContext;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace HackneyRepairs.Repository
{
	public class DRSRepository : IDRSRepository
    {
        private DRSDbContext _context;
        private ILoggerAdapter<DRSRepository> _logger;
		
        public DRSRepository(DRSDbContext context, ILoggerAdapter<DRSRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<DetailedAppointment>> GetAppointmentByWorkOrderReference(string workOrderReference)
		{
            List<DetailedAppointment> appointments;
            _logger.LogInformation($"Getting appointment details from DRS for {workOrderReference}");

            try
            {
                using (var connection = new MySqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    string query = $@"
                        SELECT
                            c_job.GLOBALCURRENTTIMEWINDOW_START AS BeginDate,
                            c_job.GLOBALCURRENTTIMEWINDOW_END AS EndDate,
                            c_job.status AS Status,
                            c_job.ASSIGNEDWORKERS AS AssignedWorker,
                            c_job.priority AS Priority,
                            c_job.CREATIONDATE AS CreationDate,
                            c_job.BD_APPOINTMENT_REASON AS Comment,
                            s_worker.MOBILEPHONE AS Mobilephone,
                            'DRS' AS SourceSystem,
                            'DLO' AS SittingAt
                        FROM
                            s_serviceorder 
                        INNER JOIN c_job ON c_job.PARENTID = s_serviceorder.USERID
                        INNER JOIN s_worker ON c_job.assignedworkersids = s_worker.userid
                        WHERE
                            s_serviceorder.NAME = '{workOrderReference}'";

                    appointments = connection.Query<DetailedAppointment>(query).ToList();
                }
                return appointments;
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
