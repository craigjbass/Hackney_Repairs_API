using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using HackneyRepairs.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using HackneyRepairs.Entities;
using Dapper;

namespace HackneyRepairs.Repository
{
	public class UhwRepository : IUhwRepository
	{
		private UhwDbContext _context;

		private ILoggerAdapter<UhwRepository> _logger;
		public UhwRepository(UhwDbContext context, ILoggerAdapter<UhwRepository> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task AddOrderDocumentAsync(string documentType, string workOrderReference, int workOrderId, string processComment)
		{
			_logger.LogInformation($"Starting process to add repair request document to UH for work order {workOrderReference}");
			try
			{
				using (var command = _context.Database.GetDbConnection().CreateCommand())
				{
					_context.Database.OpenConnection();
					command.CommandText = "usp_StartHackneyProcessV2";
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.Add(new SqlParameter
					{
						DbType = DbType.String,
						ParameterName = "@docTypeCode",
						Value = documentType
					});
					command.Parameters.Add(new SqlParameter
					{
						DbType = DbType.String,
						ParameterName = "@WorkOrderRef",
						Value = workOrderReference
					});
					command.Parameters.Add(new SqlParameter
					{
						DbType = DbType.Int32,
						ParameterName = "@WorkOrderId",
						Value = workOrderId
					});
					command.Parameters.Add(new SqlParameter
					{
						DbType = DbType.String,
						ParameterName = "@ProcessComment",
						Value = processComment
					});
					await command.ExecuteNonQueryAsync();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw new UhwRepositoryException();
			}
			finally
			{
				_context.Database.CloseConnection();
			}
		}      

		public async Task<IEnumerable<NotesEntity>> GetNotesByWorkOrderReference(string workOrderReference)
		{
			IEnumerable<NotesEntity> notes;
			try
			{
				using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
				{
					string query = "";
					string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
					switch (env)
					{
						case "Development":
							query = $@"SELECT note.*
                                    FROM
                                    uhwdev.dbo.W2ObjectNote AS note
                                    INNER JOIN uhtdev.dbo.rmworder AS work_order
                                    on note.KeyNumb = work_order.rmworder_sid
                                    where work_order.wo_ref = '{workOrderReference}'";
							break;
						case "Test":
							query = $@"SELECT note.*
                                    FROM
                                    uhwtest.dbo.W2ObjectNote AS note
                                    INNER JOIN uhttest.dbo.rmworder AS work_order
                                    on note.KeyNumb = work_order.rmworder_sid
                                    where work_order.wo_ref = '{workOrderReference}'";
							break;
						case "Production":
							query = $@"SELECT note.*
                                    FROM
                                    uhwlive.dbo.W2ObjectNote AS note
                                    INNER JOIN uhtlive.dbo.rmworder AS work_order
                                    on note.KeyNumb = work_order.rmworder_sid
                                    where work_order.wo_ref = '{workOrderReference}'";
							break;
					}
					notes = connection.Query<NotesEntity>(query);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw new UhtRepositoryException();
			}
			return notes;
		}
	}
    public class UhwRepositoryException : Exception
    {
    }
}


