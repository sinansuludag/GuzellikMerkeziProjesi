using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GüzellikMerkeziProjesi
{
    public partial class TutarGuncelle : Form
    {
        int id;

        public TutarGuncelle(int id)
        {
            InitializeComponent();
            this.id = id;
        }

        public void OpenConnection()
        {
            if (ConnectionAndStaticTools.Connection.State == System.Data.ConnectionState.Closed)
            {
                ConnectionAndStaticTools.Connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (ConnectionAndStaticTools.Connection.State == System.Data.ConnectionState.Open)
            {
                ConnectionAndStaticTools.Connection.Close();
            }
        }

        private void TutarGuncelle_Load(object sender, EventArgs e)
        {
            DanisanBilgileri danisanBilgileri = new DanisanBilgileri(id);
            txtId.Text =id.ToString();
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (txtTutar.Text == "")
            {
                MessageBox.Show("Lütfen tutar giriniz");
            }
            else
            {
                if (float.TryParse(txtTutar.Text, out float yeniTutar))
                {
                    // Veritabanında tutarı güncelle
                    OpenConnection();
                    MySqlCommand mySqlCommand = new MySqlCommand("UPDATE dbalinanodemeler SET Tutar = @Tutar WHERE ID = @ID and Tarih=@Tarih", ConnectionAndStaticTools.Connection);
                    mySqlCommand.Parameters.AddWithValue("@ID", id);
                    mySqlCommand.Parameters.AddWithValue("@Tutar", yeniTutar);
                    mySqlCommand.Parameters.AddWithValue("@Tarih", dtpickerTarih.Value.ToString("yyyy-MM-dd"));
                    mySqlCommand.ExecuteNonQuery();
                    CloseConnection();
                    MessageBox.Show("Güncelleme başarıyla yapılmıştır.");

                    // DanisanBilgileri formunu aç ve güncellenmiş bilgileri göster
                    DanisanBilgileri danisanBilgileri = new DanisanBilgileri(id);
                    ConnectionAndStaticTools.danisanGetir(danisanBilgileri, id);

                    // Bu formu gizle
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir tutar giriniz.");
                }
            }
            
        }


        private void AltForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Form kapanmasını engelle
            e.Cancel = true;

            // DanisanBilgileri formunu aç
            DanisanBilgileri danisanBilgileri = new DanisanBilgileri(id);
            ConnectionAndStaticTools.danisanGetir(danisanBilgileri, id);
            this.Hide(); // Alt formu gizle
        }

        private void txtTutar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Enter tuşunu bastırmayı engelle
            }
        }
    }
}
