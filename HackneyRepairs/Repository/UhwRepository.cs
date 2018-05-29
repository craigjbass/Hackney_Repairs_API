using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HackneyRepairs.DbContext;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Data.SqlClient;

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

    }

    public class UhwRepositoryException : Exception
    {
    }
}


