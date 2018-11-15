using System;
using System.Data;
using System.Data.SqlClient;
using HackneyRepairs.DbContext;
using Microsoft.EntityFrameworkCore;

namespace HackneyRepairs.Tests
{
    class UniversalHousingSimulator<T> where T : Microsoft.EntityFrameworkCore.DbContext
    {
        static private string _connectionString = "Data Source=127.0.0.1;Initial Catalog=uhsimulator;Pooling=false;user id=sa;password=Rooty-Tooty";
        public object context;

        public UniversalHousingSimulator()
        {
            var optionsBuilder = new DbContextOptionsBuilder<T>().UseSqlServer(_connectionString);
            var options = optionsBuilder.Options;
            context = Activator.CreateInstance(typeof(T), new[] { options });
        }

        static public bool IsRunning()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        public void InsertTrade(string trade = "TRA", string trade_desc = "Description")
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO rmtrade (trade_desc, trade) VALUES (@trade_description, @trade)";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add("@trade", SqlDbType.Char, 3).Value = trade;
                command.Parameters.Add("@trade_description", SqlDbType.Char, 25).Value = trade_desc;

                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void InsertTask(string trade = "TRA", string rq_ref = "00000002", string job_code = "HIST0001", int task_no = 1)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO rmtask (job_code, rq_ref, trade, task_no) VALUES (@job_code, @rq_ref, @trade, @task_no)";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add("@rq_ref", SqlDbType.Char, 8).Value = rq_ref;
                command.Parameters.Add("@job_code", SqlDbType.Char, 8).Value = job_code;
                command.Parameters.Add("@trade", SqlDbType.Char, 3).Value = trade;
                command.Parameters.Add("@task_no", SqlDbType.Int, 2).Value = task_no;

                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void InsertRequest(string rq_ref = "00000002", string rq_problem = "some problem")
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO rmreqst(rq_ref, rq_problem) VALUES (@rq_ref, @rq_problem)";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add("@rq_ref", SqlDbType.Char, 8).Value = rq_ref;
                command.Parameters.Add("@rq_problem", SqlDbType.Text).Value = rq_problem;

                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void InsertWorkOrder(string wo_ref = "00000002", string created = "2079-01-01 00:00:00.000", double est_cost = 0.0, double act_cost = 0.0, string completed = "2079-01-01 00:00:00.000", string wo_status = null, string u_dlo_status = null, string u_servitor_ref = null, string prop_ref = null, string rq_ref = null, string date_due = "2079-01-01 00:00:00.000")
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO rmworder (wo_ref, created, est_cost, act_cost, completed, wo_status, u_dlo_status, u_servitor_ref, prop_ref, rq_ref, date_due) VALUES (@wo_ref, @created, @est_cost, @act_cost, @completed, @wo_status, @u_dlo_status, @u_servitor_ref, @prop_ref, @rq_ref, @date_due)";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add("@wo_ref", SqlDbType.Char, 10).Value = wo_ref;
                command.Parameters.Add("@created", SqlDbType.DateTime).Value = created ?? (object)DBNull.Value;
                command.Parameters.Add("@est_cost", SqlDbType.Decimal, 2).Value = est_cost;
                command.Parameters.Add("@act_cost", SqlDbType.Decimal, 2).Value = act_cost;
                command.Parameters.Add("@completed", SqlDbType.SmallDateTime).Value = completed ?? (object)DBNull.Value;
                command.Parameters.Add("@wo_status", SqlDbType.Char, 3).Value = wo_status ?? (object)DBNull.Value;
                command.Parameters.Add("@u_dlo_status", SqlDbType.Char, 3).Value = u_dlo_status ?? (object)DBNull.Value;
                command.Parameters.Add("@u_servitor_ref", SqlDbType.Char, 50).Value = u_servitor_ref ?? (object)DBNull.Value;
                command.Parameters.Add("@prop_ref", SqlDbType.Char, 12).Value = prop_ref ?? (object)DBNull.Value;
                command.Parameters.Add("@rq_ref", SqlDbType.Char, 8).Value = rq_ref ?? (object)DBNull.Value;
                command.Parameters.Add("@date_due", SqlDbType.SmallDateTime).Value = date_due;

                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void Reset()
        {
            string query = "TRUNCATE TABLE rmtrade;" +
                "TRUNCATE TABLE rmtask;" +
                "TRUNCATE TABLE rmreqst;" +
                "TRUNCATE TABLE rmworder;";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                connection.Close();
            }
        }
    }
}
