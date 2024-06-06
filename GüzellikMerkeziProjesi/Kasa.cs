using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GüzellikMerkeziProjesi
{
    public partial class Kasa : Form
    {
       
        public Kasa()
        {
            InitializeComponent();
        }

        private void Kasa_Load(object sender, EventArgs e)
        {
            
            raporListesiniGetir();
            toplamOdemeGetir();
        }

        private void AltForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; // Formun kapatılmasını engelleyin
            Anasayfa anasayfa = new Anasayfa();
            anasayfa.Show();
            this.Hide(); // Alt formu gizleyin veya kapatın
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // SQL sorgusunu oluştur
                string query = "SELECT * FROM dbrapor WHERE Tarih BETWEEN @BaslangicTarihi AND @BitisTarihi";

                // MySqlDataAdapter nesnesini oluştur ve sorguyu belirt
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(query, ConnectionAndStaticTools.Connection);

                // Parametreleri ekleyerek filtreleme yap
                mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@BaslangicTarihi", DateTime.Parse(basDataTimePic.Text));
                mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@BitisTarihi", DateTime.Parse(bitisDataTimePic.Text));

                // Verileri al ve bir DataTable'e doldur
                DataTable dt = new DataTable();
                mySqlDataAdapter.Fill(dt);

                // Verileri DataGridView'e aktar
                dataGridView1.DataSource = dt;
                toplamOdemeGetir();
                // Eğer kayıt yoksa kullanıcıya bilgi ver
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Belirtilen tarih aralığında kayıt bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toplamOdemeGetir()
        {
            try
            {
                ConnectionAndStaticTools.OpenConnection();
                MySqlCommand mySqlCommand = new MySqlCommand("Select Tutar from dbrapor where Tarih BETWEEN @BaslangicTarihi AND @BitisTarihi", ConnectionAndStaticTools.Connection);
                mySqlCommand.Parameters.AddWithValue("@BaslangicTarihi", DateTime.Parse(basDataTimePic.Text));
                mySqlCommand.Parameters.AddWithValue("@BitisTarihi", DateTime.Parse(bitisDataTimePic.Text));

                MySqlDataReader reader = mySqlCommand.ExecuteReader();

                float toplam = 0;
                while (reader.Read())
                {
                    float tutar = float.Parse(reader["Tutar"].ToString());
                    toplam+= tutar;
                }
                txtTutar.Text=toplam.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Hata:" + ex.Message);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }
        }

        private void raporListesiniGetir()
        {
            try
            {
                // SQL sorgusunu oluştur
                string query = "SELECT * FROM dbrapor where Tarih BETWEEN @BaslangicTarihi AND @BitisTarihi ";

                // MySqlDataAdapter nesnesini oluştur ve sorguyu belirt
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(query, ConnectionAndStaticTools.Connection);
                mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@BaslangicTarihi", DateTime.Parse(basDataTimePic.Text));
                mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@BitisTarihi", DateTime.Parse(bitisDataTimePic.Text));


                // Verileri al ve bir DataTable'e doldur
                DataTable dt = new DataTable();
                mySqlDataAdapter.Fill(dt);

                // Verileri DataGridView'e aktar
                dataGridView1.DataSource = dt;
                dataGridView1.BackgroundColor = Color.White;
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 139, 139);
                //Sıralamayı kaldır
                dataGridView1.Columns["ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Tarih"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Adi Soyadi"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["OdemeTipi"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Tutar"].SortMode = DataGridViewColumnSortMode.NotSortable;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            Anasayfa anasayfa = new Anasayfa();
            anasayfa.Show();
            this.Hide();
        }
    }
}
