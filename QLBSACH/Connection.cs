using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLBSACH
{
    internal class Connection : DataTable
    {
        // Biến toàn cục
        SqlConnection connection;
        SqlDataAdapter adapter;
        SqlCommand command;

        // Lấy chuỗi kết nối
        public string ConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder["Server"] = ".\\SQLEXPRESS";
            builder["Database"] = "QLBSACH";
            builder["User ID"] = "sa";
            builder["Password"] = "123456";
            builder["Column Encryption Setting"] = "Enabled";
            return builder.ConnectionString;
        }

        // Lấy chuỗi kết nối cho người dùng cụ thể
        public string ConnectionString(string username, string passowrd)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder["Server"] = ".\\SQLEXPRESS";
            builder["Database"] = "QLBSACH";
            builder["User ID"] = username;
            builder["Password"] = passowrd;
            builder["Column Encryption Setting"] = "Enabled";
            return builder.ConnectionString;
        }

        // Mở kết nối dành cho sa
        public bool OpenConnection()
        {
            try
            {
                if (connection == null)
                    connection = new SqlConnection(ConnectionString());
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                return true;
            }
            catch
            {
                connection.Close();
                return false;
            }
        }

        // Mở kết nối dành cho người dùng cụ thể
        public bool OpenConnection(string username, string passowrd)
        {
            try
            {
                if (connection == null)
                    connection = new SqlConnection(ConnectionString(username, passowrd));
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                return true;
            }
            catch
            {
                connection.Close();
                return false;
            }
        }
        // Thực thi câu lệnh Select
        public void Fill(SqlCommand selectCommand)
        {
            command = selectCommand;
            try
            {
                command.Connection = connection;

                adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;

                this.Clear();
                adapter.Fill(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể thực thi câu lệnh SQL này!\nLỗi: " + ex.Message, "Lỗi truy vấn", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Thực thi câu lệnh Insert, Update, Delete
        // Thực thi trên toàn DataGridView
        public int Update()
        {
            int result = 0;
            SqlTransaction transaction = null;
            try
            {
                transaction = connection.BeginTransaction();

                command.Connection = connection;
                command.Transaction = transaction;

                adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                result = adapter.Update(this);
                transaction.Commit();
            }
            catch (Exception e)
            {
                if (transaction != null)
                    transaction.Rollback();
                MessageBox.Show("Không thể thực thi câu lệnh SQL này!\nLỗi: " + e.Message, "Lỗi truy vấn", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        // Thực thi trên một câu lệnh đơn
        public int Update(SqlCommand insertUpdateDeleteCommand)
        {
            int result = 0;
            SqlTransaction transaction = null;
            try
            {
                transaction = connection.BeginTransaction();

                insertUpdateDeleteCommand.Connection = connection;
                insertUpdateDeleteCommand.Transaction = transaction;
                result = insertUpdateDeleteCommand.ExecuteNonQuery();

                this.AcceptChanges();
                transaction.Commit();
            }
            catch (Exception e)
            {
                if (transaction != null)
                    transaction.Rollback();
                MessageBox.Show("Không thể thực thi câu lệnh SQL này!\nLỗi: " + e.Message, "Lỗi truy vấn", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
    }
}