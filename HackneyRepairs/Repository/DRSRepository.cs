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

		public async Task<DetailedAppointment> GetCurrentAppointmentByWorkOrderReference(string workOrderReference)
        {
			DetailedAppointment appointment;
            _logger.LogInformation($"Getting current appointment details from DRS for {workOrderReference}");

            try
            {
                using (var connection = new MySqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    string query = $@"
                        SELECT
                            s_job.GLOBALCURRENTTIMEWINDOW_START AS BeginDate,
                            s_job.GLOBALCURRENTTIMEWINDOW_END AS EndDate,
                            s_job.status AS Status,
                            s_job.ASSIGNEDWORKERS AS AssignedWorker,
                            s_job.priority AS Priority,
                            s_job.CREATIONDATE AS CreationDate,
                            s_job.BD_APPOINTMENT_REASON AS Comment,
                            s_worker.MOBILEPHONE AS Mobilephone,
                            'DRS' AS SourceSystem
                        FROM
                            s_serviceorder 
                        INNER JOIN s_job ON s_job.PARENTID = s_serviceorder.USERID
                        INNER JOIN s_worker ON s_job.assignedworkersids = s_worker.userid
                        WHERE
                            s_serviceorder.NAME = '{workOrderReference}'
                            AND s_job.status = 'planned'";

					appointment = connection.Query<DetailedAppointment>(query).FirstOrDefault();
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
