using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace SYBD_PRUD
{
    public partial class Form1 : Form
    {
        public DataSet ds;
        public SqlDataAdapter adapter;
        public SqlCommandBuilder commandBuilder;
        string connectionString = @"Data Source=name of PC;Initial Catalog=name of catalog;Integrated Security=True";
        string sql = "SELECT * FROM catalog of sql_ex ORDER BY id";
        public Form1()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sql, connection);

                ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            DataRow row = ds.Tables[0].NewRow(); // добавляем новую строку в DataTable
            ds.Tables[0].Rows.Add(row);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sql, connection);
                commandBuilder = new SqlCommandBuilder(adapter);
                adapter.InsertCommand = new SqlCommand("sp_CreateUser", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("name cols"SqlDbType.NVarChar, 50, "Name of table cols")
                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@Id", SqlDbType.NVarChar, 50, "Id");
                parameter.Direction = ParameterDirection.Output;

                adapter.Update(ds);
            }
        }

        private void removeButton_Click(object sender, EventArgs e)//delete empty rows
        {
            // удаляем выделенные строки из dataGridView1
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
            }
        }

        private void emailButton_Click(object sender, EventArgs e)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress("here must be your email addres");
            mail.To.Add("here too");
            mail.IsBodyHtml = true;
            mail.Subject = "any";

            string mailBody = "<table width='100%' style='border:Solid 1px Black;'>"; ;

            foreach (DataGridViewRow row in dataGridView1.Rows)//uuse html table for create table`s view of info
            {
                mailBody += "<tr>";
                foreach (DataGridViewCell cell in row.Cells)
                {
                    mailBody += "<td>" + cell.Value + "</td>";
                }
                {
                    mailBody += "</tr>";
                }
            }
            mailBody += "</table>";
            mail.Body = mailBody;

            var client = new SmtpClient("smtp.gmail.com", 587);//it is google`s domain
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("here must be your login of @mail", "here must be your password of @mail");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(mail);
        }
    }
}
