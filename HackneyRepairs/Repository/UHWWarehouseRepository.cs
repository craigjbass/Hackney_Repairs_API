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

        public async Task<object> GetTagReferencenumber(string hackneyhomesId)
        {
            _logger.LogInformation($"Get Tag Refernce number for {hackneyhomesId}");

            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    _context.Database.OpenConnection();
                    command.CommandText = "GetTagReferenceNumber";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter
                    {
                        DbType = DbType.String,
                        ParameterName = "@hackneyhomeId",
                        Value = hackneyhomesId
                    });


                    var tagReference="";
                    using (var reader =  command.ExecuteReaderAsync().Result)
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            if (reader["key1"] != null)
                            {
                                tagReference = reader["key1"].ToString().Trim();
                            }
                        }
                    }


                    return tagReference;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new UHWWarehouseRepositoryException();
            }
            finally
            {
                _context.Database.CloseConnection();
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
                    string sql = "select short_address as 'ShortAddress', post_code as 'PostCodeValue', prop_ref as 'PropertyReference' from property where level_code = 7 and post_code = '" + postcode + "'";
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
                    string sql = "select short_address as 'ShortAddress', post_code as 'PostCodeValue', ~no_maint as 'Maintainable', prop_ref as 'PropertyReference' from property where level_code = 7 and post_code = '" + reference + "'";
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
    }

    public class UHWWarehouseRepositoryException: Exception
    {
    }

}
