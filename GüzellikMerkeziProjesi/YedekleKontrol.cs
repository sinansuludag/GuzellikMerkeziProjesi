using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MySql.Data.MySqlClient.MySqlBackup;

namespace GüzellikMerkeziProjesi
{
    public partial class YedekleKontrol : Form
    {
        public YedekleKontrol()
        {
            InitializeComponent();
            txtKullaniciAdi.Focus();
        }



        private void btnYedekleGiris_Click(object sender, EventArgs e)
        {
            try
            {
                string kullaniciAdi = txtKullaniciAdi.Text;
                string parola = txtSifre.Text;

                string connectionString = ConfigurationManager.ConnectionStrings["Baglanti"].ConnectionString;
                var builder = new MySqlConnectionStringBuilder(connectionString);
                string appConfigUsername = builder.UserID;
                string appConfigPassword = builder.Password;
                string veritabaniAdi = builder.Database;

                if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(parola))
                {
                    MessageBox.Show("Kullanıcı adı veya parola boş olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (kullaniciAdi != appConfigUsername || parola != appConfigPassword)
                {
                    MessageBox.Show("Kullanıcı adı veya parola yanlış.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Temizle();
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "SQL Dosyaları (*.sql)|*.sql",
                    FileName = $"{veritabaniAdi}_yedek.sql"
                };

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                string yedekDosyaYolu = saveFileDialog.FileName;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Create the MySqlBackup object
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        MySqlBackup mb = new MySqlBackup(command);

                        // Export the database to a temporary file
                        string tempFilePath = Path.GetTempFileName();
                        mb.ExportToFile(tempFilePath);

                        // Prepend CREATE DATABASE and USE commands to the temporary file content
                        string createDatabaseCommand = $"CREATE DATABASE IF NOT EXISTS `{veritabaniAdi}`;\nUSE `{veritabaniAdi}`;\n\n";
                        File.WriteAllText(yedekDosyaYolu, createDatabaseCommand + File.ReadAllText(tempFilePath));

                        // Delete the temporary file
                        File.Delete(tempFilePath);
                    }
                }

                MessageBox.Show("Veritabanı yedekleme tamamlandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                Anasayfa anasayfa = new Anasayfa();
                anasayfa.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Temizle()
        {
            txtSifre.Text = "";
            txtKullaniciAdi.Text = "";
            txtKullaniciAdi.Focus();
        }

        private void txtSifre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Enter tuşunu bastırmayı engelle
            }
        }

        private void txtKullaniciAdi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Enter tuşunu bastırmayı engelle
            }
        }
    }
}
