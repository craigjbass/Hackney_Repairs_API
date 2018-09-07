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
            var propertyLevelInfo = new PropertyLevelModel();
            var connectionString = Environment.GetEnvironmentVariable("UhWarehouseDb");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var query = @"SELECT property.prop_ref AS 'PropertyReference', property.level_code AS 'LevelCode',
                    property.major_ref AS 'MajorReference', lulevel.lu_desc AS 'Description', 
                    property.address1 AS 'Address', property.post_code AS 'PostCode'
                    FROM StagedDB.dbo.property
                    INNER JOIN lulevel ON property.level_code = lulevel.lu_ref 
                    WHERE prop_ref = '" + reference + "'";

                    var queryResult = connection.Query<PropertyLevelModel>(query).FirstOrDefault();
                    return queryResult;
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
            List<PropertySummary> properties = new List<PropertySummary>();
            _logger.LogInformation($"Getting properties for postcode {postcode}");
            string CS = Environment.GetEnvironmentVariable("UhWarehouseDb");
            if (CS == null)
            {
                CS = ConfigurationManager.ConnectionStrings["UhWarehouseDb"].ConnectionString;
            }
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    string sql = @"SELECT short_address AS 'ShortAddress', post_code AS 'PostCodeValue', 
                    prop_ref AS 'PropertyReference' 
                    FROM property 
                    WHERE level_code = 7 
                    AND post_code = '" + postcode + "'";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr != null & dr.HasRows)
                    {
                        properties = dr.MapToList<PropertySummary>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return properties.ToArray();
        }

        public async Task<PropertyDetails> GetPropertyDetailsByReference(string reference)
        {
            var property = new PropertyDetails();
            _logger.LogInformation($"Getting details for property {reference}");
            string CS = Environment.GetEnvironmentVariable("UhWarehouseDb");
            if (CS == null)
            {
                CS = ConfigurationManager.ConnectionStrings["UhWarehouseDb"].ConnectionString;
            }
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    string sql = @"SELECT short_address AS 'ShortAddress', post_code AS 'PostCodeValue', ~no_maint AS 'Maintainable', 
                    prop_ref AS 'PropertyReference' 
                    FROM property 
                    WHERE prop_ref = '" + reference + "'";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr != null & dr.HasRows)
                    {
                        property = dr.MapToList<PropertyDetails>().FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return property;
        }

        public Task<PropertyDetails> GetPropertyBlockByReference(string reference)
        {
            var property = new PropertyDetails();
            _logger.LogInformation($"Getting details for property {reference}");
            string CS = Environment.GetEnvironmentVariable("UhWarehouseDb");
            if (CS == null)
            {
                CS = ConfigurationManager.ConnectionStrings["UhWarehouseDb"].ConnectionString;
            }
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    string sql = @"SELECT short_address AS 'ShortAddress', post_code AS 'PostCodeValue', ~no_maint AS 'Maintainable', 
                    prop_ref AS 'PropertyReference' FROM property WHERE prop_ref 
                    = (SELECT u_block FROM property WHERE prop_ref = '" + reference + "')";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr != null & dr.HasRows)
                    {
                        property = dr.MapToList<PropertyDetails>().FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return Task.Run(() => property);
        }

        public Task<PropertyDetails> GetPropertyEstateByReference(string reference)
        {
            var property = new PropertyDetails();
            _logger.LogInformation($"Getting details for property {reference}");
            string CS = Environment.GetEnvironmentVariable("UhWarehouseDb");
            if (CS == null)
            {
                CS = ConfigurationManager.ConnectionStrings["UhWarehouseDb"].ConnectionString;
            }
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    string sql = @"SELECT short_address AS 'ShortAddress', post_code AS 'PostCodeValue', ~no_maint AS 'Maintainable', 
                    prop_ref AS 'PropertyReference' FROM property WHERE prop_ref 
                    = (SELECT u_estate FROM property WHERE prop_ref = '" + reference + "')";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr != null & dr.HasRows)
                    {
                        property = dr.MapToList<PropertyDetails>().FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return Task.Run(() => property);
        }
    }

    public class UHWWarehouseRepositoryException : Exception { }
}
