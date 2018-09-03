using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using HackneyRepairs.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using HackneyRepairs.Entities;
using Dapper;
using HackneyRepairs.Models;

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

        public async Task<IEnumerable<Note>> GetNotesByWorkOrderReference(string workOrderReference)
        {
            IEnumerable<Note> notes;
            try
            {
                using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    string query = "";
                    string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    switch (env)
                    {

                        case "Production":
                            query = $@"SELECT
                                    note.NoteText AS Text, note.NDate as LoggedAt,
                                    note.UserID as LoggedBy
                                    FROM
                                    uhwlive.dbo.W2ObjectNote AS note
                                    INNER JOIN uhtlive.dbo.rmworder AS work_order
                                    on note.KeyNumb = work_order.rmworder_sid
                                    where work_order.wo_ref = '{workOrderReference}'";
                            break;
                        default:
                            query = $@"SELECT
                                    note.NoteText AS Text, note.NDate as LoggedAt,
                                    note.UserID as LoggedBy
                                    FROM
                                    uhwdev.dbo.W2ObjectNote AS note
                                    INNER JOIN uhtdev.dbo.rmworder AS work_order
                                    on note.KeyNumb = work_order.rmworder_sid
                                    where work_order.wo_ref = '{workOrderReference}'";
                            break;
                    }
                    notes = connection.Query<Note>(query);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new UhtRepositoryException();
            }
            return notes;
        }

        public async Task<IEnumerable<DetailedNote>> GetRecentNotes(string noteId, int? remainingCount)
        {
            IEnumerable<DetailedNote> notes;
            try
            {
                using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    if (remainingCount == null)
                    {
                        remainingCount = 50;
                    }

                    string environmentDbWord = "";
                    string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                    switch (env)
                    {
                        case "Production":
                            environmentDbWord = "live";
                            break;
                        default:
                            environmentDbWord = "dev";
                            break;
                    }

                    var query = $@"set dateformat ymd;
                        SELECT TOP {remainingCount}
                            work_order.wo_ref AS WorkOrderReference,
                            note.NDate AS LoggedAt,
                            note.UserID AS LoggedBy,
                            note.NoteText AS [Text],
                            note.NoteID AS NoteId
                        FROM 
                            uhw{environmentDbWord}.dbo.W2ObjectNote AS note
                        INNER JOIN
                            uht{environmentDbWord}.dbo.rmworder AS work_order ON note.KeyNumb = work_order.rmworder_sid
                        WHERE
                            note.KeyObject IN ('UHOrder', 'UHOrderNA') AND note.NoteID > {noteId}
                            AND work_order.created > '{GetCutoffTime()}'
                        ORDER BY NoteID";

                    notes = connection.Query<DetailedNote>(query);
                    return notes;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new UhwRepositoryException();
            }
        }

        public static string GetCutoffTime()
        {
            DateTime now = DateTime.Now;
            DateTime dtCutoff = new DateTime(now.Year, now.Month, now.Day, 23, 0, 0);
            dtCutoff = dtCutoff.AddDays(-1);
            return dtCutoff.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
    public class UhwRepositoryException : Exception {}
}


