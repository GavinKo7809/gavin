using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlconn = null;
        private string connstr =
            "SERVER=" + ConfigurationManager.AppSettings["ip"] + "," + ConfigurationManager.AppSettings["PORT"] +
            ";DATABASE=" + ConfigurationManager.AppSettings["DBNAME"] +
            ";UID=" + ConfigurationManager.AppSettings["USERID"] +
            ";PASSWORD=" + ConfigurationManager.AppSettings["USERPASSWORD"];

        public Form1()
        {
            InitializeComponent();
        }

        private void ConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                sqlconn = new SqlConnection(connstr);
                sqlconn.Open();
                MessageBox.Show("DB 연결 성공");
                Console.WriteLine("[알림} DB 연결 성공");
            }
            catch(Exception ex)
            {
                MessageBox.Show("에러발생" + ex.ToString());
                Console.WriteLine("[오류]오류내용" +  ex.ToString());
            }

            DataSet dsLoca = new DataSet();
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                string sql = "SELECT DISTINCT LOCATIONS FROM INVENTORY_LIST";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                adapter.Fill(dsLoca, "INVENTORY_LIST");

                LocationsComboBox.DataSource = dsLoca.Tables[0];
                LocationsComboBox.DisplayMember = "LOCATIONS";
                LocationsComboBox.SelectedIndex = -1;
            }

            DataSet dsEquip = new DataSet();
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                string sql = "SELECT DISTINCT EQUIPMENT FROM INVENTORY_LIST";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                adapter.Fill(dsEquip, "INVENTORY_LIST");

                EquipmentComboBox.DataSource = dsEquip.Tables[0];
                EquipmentComboBox.DisplayMember = "EQUIPMENT";
                EquipmentComboBox.SelectedIndex = -1;
            }
        }

        private void DisconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if(sqlconn != null)
                {
                    sqlconn.Close();
                    MessageBox.Show("DB 연결 해제");
                    Console.WriteLine("[알림} DB 연결 해제");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("에러발생" + ex.ToString());
                Console.WriteLine("[오류]오류내용" + ex.ToString());
            }
        }

        private void AllSearchBtn_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();

            if (sqlconn == null)
            {
                MessageBox.Show("DB 연결 필요");
                Console.WriteLine("[알림} DB 연결 필요");
            }
            if (sqlconn != null)
            {
                using (SqlConnection conn = new SqlConnection(connstr))
                {
                    string sql =
                     "SELECT LOCATIONS AS 장소," +
                     "EQUIPMENT AS 설비," +
                     "CLASSIFYING AS 구분," +
                     "PRODUCTNAME AS 품명," +
                     "PRODUCTINFO AS 규격," +
                     "PRODUCTSUM AS 수량," +
                     "CHANGER AS 최종수정자," +
                     "MODIFYDATE AS 최종변경일," +
                     "HISTORY AS 내역 FROM INVENTORY_LIST";
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(ds, "INVENTORY_LIST");
                }
                SearchDataGrid.DataSource = ds.Tables[0];
            }
        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            string[] strSearch = { };
            string[] strIndex = { };
            StringBuilder sbSearch = new StringBuilder();
            DataSet ds = new DataSet();

            strIndex = new string[6] { "LOCATIONS", "EQUIPMENT", "CLASSIFYING", "CHANGER", "PRODUCTNAME", "PRODUCTINFO" };

            strSearch = new string[6]
            {
                LocationsComboBox.Text,
                EquipmentComboBox.Text,
                ClassifyingTextBox.Text,
                ChangerTextBox.Text,
                ProductNameTextBox.Text,
                ProductInfoTextBox.Text
            };

            bool bWhere = true;
            bool bAnd = false;

            for (int i = 0; i < strSearch.Length; i++) 
            {
                if (strSearch[i] == string.Empty) continue;

                if (bWhere) sbSearch.Append(" WHERE ");
                if (bAnd) sbSearch.Append(" AND ");

                sbSearch.Append(strIndex[i]);

                if (i > 1) sbSearch.Append(" LIKE ");
                else sbSearch.Append(" =");

                if (i > 1) sbSearch.Append(" '%" + strSearch[i] + "%'");
                else sbSearch.Append(" '" + strSearch[i] + "'");

                bWhere = false;
                bAnd = true;
            }

            string strSearchWord = sbSearch.ToString();

            if (strSearchWord != string.Empty) sbSearch.Append(";");

            using (SqlConnection conn = new SqlConnection(connstr))
            {
                string sql =
                    "SELECT LOCATIONS AS 장소," +
                    "EQUIPMENT AS 설비," +
                    "CLASSIFYING AS 구분," +
                    "PRODUCTNAME AS 품명," +
                    "PRODUCTINFO AS 규격," +
                    "PRODUCTSUM AS 수량," +
                    "CHANGER AS 최종수정자," +
                    "MODIFYDATE AS 최종변경일," +
                    "HISTORY AS 내역 FROM INVENTORY_LIST" + strSearchWord;
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                adapter.Fill(ds, "INVENTORY_LIST");
            }

            SearchDataGrid.DataSource = ds.Tables[0];

        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            LocationsComboBox.SelectedIndex = -1;
            EquipmentComboBox.SelectedIndex = -1;
        }
    }
}
