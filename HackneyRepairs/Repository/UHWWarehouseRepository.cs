using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
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
                    string sql = "SELECT SHORT_ADDRESS AS 'ShortAddress', POST_CODE AS 'PostCodeValue', PROP_REF AS 'PropertyReference' FROM PROPERTY WHERE LELEL_CODE = 7 AND POST_CODE = '" + postcode + "'";
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
                    string sql = "SELECT SHORT_ADDRESS AS 'ShortAddress', POST_CODE AS 'PostCodeValue', ~NO_MAINT AS 'Maintainable', PROP_REF AS 'PropertyReference' FROM PROPERTY WHERE PROP_REF = '" + reference + "'";
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
                    string sql = "SELECT SHORT_ADDRESS AS 'ShortAddress', POST_CODE AS 'PostCodeValue', ~NO_MAINT AS 'Maintainable', PROP_REF AS 'PropertyReference' FROM PROPERTY WHERE PROP_REF";
                    sql += " = (SELECT U_BLOCK FROM PROPERTY WHERE PROP_REF = '" + reference + "')";
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
                    string sql = "SELECT SHORT_ADDRESS AS 'ShortAddress', POST_CODE AS 'PostCodeValue', ~NO_MAINT AS 'Maintainable', PROP_REF AS 'PropertyReference' FROM PROPERTY WHERE PROP_REF";
                    sql += " = (SELECT U_ESTATE FROM PROPERTY WHERE PROP_REF = '" + reference + "')";
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
