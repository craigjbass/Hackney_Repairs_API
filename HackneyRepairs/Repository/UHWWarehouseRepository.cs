using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HackneyRepairs.DbContext;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;
using HackneyRepairs.PropertyService;
using Microsoft.EntityFrameworkCore;

namespace HackneyRepairs.Repository
{
    public class UHWWarehouseRepository :IUHWWarehouseRepository
    {
        private UHWWarehouseDbContext _context;
        private ILoggerAdapter<UHWWarehouseRepository> _logger;

        public UHWWarehouseRepository(UHWWarehouseDbContext context, ILoggerAdapter<UHWWarehouseRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PropertyLevelModel> GetPropertyLevelInfo(string reference)
        {
            _logger.LogInformation($"Getting propertiy hierarchical info for: {reference}");
            try
            {
                using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    var query = $@"
                        SELECT 
                            property.prop_ref AS 'PropertyReference',
                            property.level_code AS 'LevelCode',
                            property.major_ref AS 'MajorReference',
                            lulevel.lu_desc AS 'Description', 
                            property.address1 AS 'Address',
                            property.post_code AS 'PostCode'
                        FROM 
                            StagedDB.dbo.property
                        INNER 
                            JOIN lulevel ON property.level_code = lulevel.lu_ref 
                        WHERE 
                            prop_ref = '{reference}'";

                    var result = connection.Query<PropertyLevelModel>(query).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new UHWWarehouseRepositoryException();
            }
        }

        public async Task<PropertySummary[]> GetPropertyListByPostCode(string postcode)
        {
            _logger.LogInformation($"Getting properties for postcode {postcode}");
            try
            {
                using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    string query = $@"
                        SELECT 
                            short_address AS 'ShortAddress',
                            post_code AS 'PostCodeValue', 
                            prop_ref AS 'PropertyReference' 
                        FROM 
                            property 
                        WHERE 
                            level_code = 7 AND post_code = '{postcode}'";
                    var properties = connection.Query<PropertySummary>(query).ToArray();
                    return properties;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new UHWWarehouseRepositoryException();
            }
        }

        public async Task<PropertyDetails> GetPropertyDetailsByReference(string reference)
        {
            _logger.LogInformation($"Getting details for property {reference}");
            try
            {
                using (SqlConnection connnection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    string query = $@"
                        SELECT 
                            short_address AS 'ShortAddress',
                            post_code AS 'PostCodeValue',
                            ~no_maint AS 'Maintainable', 
                            prop_ref AS 'PropertyReference' 
                        FROM 
                            property 
                        WHERE 
                            prop_ref = '{reference}'";
                    var property = connnection.Query<PropertyDetails>(query).FirstOrDefault();
                    return property;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new UHWWarehouseRepositoryException();
            }
        }

        public async Task<PropertyDetails> GetPropertyBlockByReference(string reference)
        {
            _logger.LogInformation($"Getting details for property {reference}");
            try
            {
                using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    string query = $@"
                        SELECT 
                            short_address AS 'ShortAddress',
                            post_code AS 'PostCodeValue',
                            ~no_maint AS 'Maintainable', 
                            prop_ref AS 'PropertyReference'
                        FROM 
                            property
                        WHERE 
                            prop_ref = (SELECT u_block 
                                        FROM property 
                                        WHERE prop_ref = '{reference}')";
                    var property = connection.Query<PropertyDetails>(query).FirstOrDefault();
                    return property;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new UHWWarehouseRepositoryException();
            }
        }

        public async Task<PropertyDetails> GetPropertyEstateByReference(string reference)
        {
            _logger.LogInformation($"Getting details for property {reference}");
            try
            {
                using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    string query = $@"
                        SELECT 
                            short_address AS 'ShortAddress',
                            post_code AS 'PostCodeValue',
                            ~no_maint AS 'Maintainable',
                            prop_ref AS 'PropertyReference'
                        FROM 
                            property 
                        WHERE 
                            prop_ref = (SELECT u_estate 
                                        FROM property 
                            WHERE prop_ref = '{reference}')";
                    var property = connection.Query<PropertyDetails>(query).FirstOrDefault();
                    return property;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new UHWWarehouseRepositoryException();
            }
        }

		public async Task<IEnumerable<UHWorkOrder>> GetWorkOrderByPropertyReference(string propertyReference)
        {
            IEnumerable<UHWorkOrder> workOrders;
            try
            {
                using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
					string query = $@"set dateformat ymd;
                                    SELECT
                                       LTRIM(RTRIM(wo.wo_ref)) AS WorkOrderReference,
                                       LTRIM(RTRIM(r.rq_ref)) AS RepairRequestReference,
                                       r.rq_problem AS ProblemDescription,
                                       wo.created AS Created,
                                       wo.est_cost AS EstimatedCost,
                                       wo.act_cost AS ActualCost,
                                       wo.completed AS CompletedOn,
                                       wo.date_due AS DateDue,
                                       LTRIM(RTRIM(wo.wo_status)) AS WorkOrderStatus,
                                       LTRIM(RTRIM(wo.u_dlo_status)) AS DLOStatus,
                                       LTRIM(RTRIM(wo.u_servitor_ref)) AS ServitorReference,
                                       LTRIM(RTRIM(wo.prop_ref)) AS PropertyReference,
                                       LTRIM(RTRIM(t.job_code)) AS SORCode,
                                       LTRIM(RTRIM(tr.trade_desc)) AS Trade

                                    FROM
                                       rmworder wo
                                       INNER JOIN rmreqst r ON wo.rq_ref = r.rq_ref
                                       INNER JOIN rmtask t ON wo.rq_ref = t.rq_ref
                                       INNER JOIN rmtrade tr ON t.trade = tr.trade
                                       WHERE wo.created < '{GetCutoffTime()}' AND wo.prop_ref = '{propertyReference}'AND t.task_no = 1;";
					workOrders = connection.Query<UHWorkOrder>(query);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new UHWWarehouseRepositoryException();
            }
			return workOrders;
        }

        public async Task<IEnumerable<Note>> GetNoteFeed(int noteId, string noteTarget, int size)
        {
            try
            {
                using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    string query = $@"set dateformat ymd;
                    SELECT TOP {size}
                        LTRIM(RTRIM(work_order.wo_ref)) AS WorkOrderReference,
                        note.NDate AS LoggedAt,
                        note.UserID AS LoggedBy,
                        note.NoteText As [Text],
                        note.NoteID AS NoteId
                    FROM 
                        StagedDBW2.dbo.W2ObjectNote AS note
                    INNER JOIN
                        StagedDB.dbo.rmworder AS work_order ON note.KeyNumb = work_order.rmworder_sid
                    WHERE 
                        KeyObject in ('{noteTarget}') AND NoteID > {noteId} 
                        AND note.NDate < '{GetCutoffTime()}'
                    ORDER BY NoteID";

                    var notes = connection.Query<Note>(query);
                    return notes;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new UHWWarehouseRepositoryException();
            }
        }

        public async Task<IEnumerable<string>> GetDistinctNoteKeyObjects()
        {
            try
            {
                using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    string query = @"
                        SELECT DISTINCT LOWER([KeyObject])
                        FROM StagedDBW2.dbo.[W2ObjectNote]";
                    var keyObjets = connection.Query<string>(query);
                    return keyObjets;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new UHWWarehouseRepositoryException();
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

    public class UHWWarehouseRepositoryException : Exception { }
}
