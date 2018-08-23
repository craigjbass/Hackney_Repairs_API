using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;

namespace HackneyRepairs.Repository
{
	public class DRSRepository : IDRSRepository
    {
		public DRSRepository()
		{
			
		}

		public Task<IEnumerable<DetailedAppointment>> GetAppointmentByWorkOrderReference(string workOrderReference)
		{
			// Query
			//SELECT
            //    c_job.GLOBALCURRENTTIMEWINDOW_START AS BeginDate,
            //    c_job.GLOBALCURRENTTIMEWINDOW_END AS EndDate,
            //    c_job.status AS "Status",
            //    c_job.ASSIGNEDWORKERS AS AssignedWorker,
            //    c_job.priority AS Priority,
            //    s_worker.MOBILEPHONE AS Mobilephone
            //FROM
                //s_serviceorder
                //INNER JOIN c_job ON c_job.PARENTID = s_serviceorder.USERID
                //INNER JOIN s_worker ON c_job.assignedworkersids = s_worker.userid
                //WHERE
                    //qs_serviceorder.NAME = 01912967
			throw new NotImplementedException();
		}
	}
}
