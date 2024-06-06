using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Tsp;

namespace GüzellikMerkeziProjesi
{
    public partial class Anasayfa : Form
    {

        public Anasayfa()
        {
            InitializeComponent();
        }

        private void Anasayfa_Load(object sender, EventArgs e)
        {
            paketListele();
        }

       

        private void btnDanisanBul_Click(object sender, EventArgs e)
        {
            DanisanBul danisanBul = new DanisanBul();
            danisanBul.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DanisanKayit danisanKayit = new DanisanKayit();
            danisanKayit.Show();
            this.Hide();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {

            try
            {
                if (txtPaketEkle.Text == "")
                {
                    MessageBox.Show("Lütfen bir paket ismi giriniz!");
                    return;
                }

                ConnectionAndStaticTools.OpenConnection();

                // Veritabanında aynı paket ismiyle kayıtlı bir satır olup olmadığını kontrol ediyoruz
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM dbpaketler WHERE Paket = @Paket", ConnectionAndStaticTools.Connection);
                checkCmd.Parameters.AddWithValue("@Paket", txtPaketEkle.Text);
                int existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                // Eğer belirli bir paket ismiyle kayıtlı satır varsa, kullanıcıya uyarı ver
                if (existingCount > 0)
                {
                    MessageBox.Show("Bu paket zaten mevcut!");
                    return;
                }

                // Belirli bir paket ismiyle kayıtlı satır yoksa yeni paketi ekleyebiliriz
                MySqlCommand insertCmd = new MySqlCommand("INSERT INTO dbpaketler (Paket) VALUES (@Paket)", ConnectionAndStaticTools.Connection);
                insertCmd.Parameters.AddWithValue("@Paket", txtPaketEkle.Text);
                insertCmd.ExecuteNonQuery();

                MessageBox.Show("Paket başarıyla eklendi!");
                temizle();
                paketListele();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }
        }

        private void paketListele()
        {
            cbPakListe.Items.Clear();
            ConnectionAndStaticTools.OpenConnection(); 
            MySqlCommand cmd = new MySqlCommand("Select * from dbpaketler ORDER BY Paket ASC", ConnectionAndStaticTools.Connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            { 
                cbPakListe.Items.Add(reader["Paket"].ToString());
            }
            ConnectionAndStaticTools.CloseConnection();         }

        private void temizle()
        {
            txtPaketEkle.Text = "";
            txtPaketEkle.Focus();
        }

        private void silToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbPakListe.SelectedItem != null)
                {
                    string selectedValue = cbPakListe.SelectedItem.ToString();
                    ConnectionAndStaticTools.OpenConnection();

                    MySqlCommand cmd = new MySqlCommand("DELETE FROM dbpaketler WHERE Paket = @Paket", ConnectionAndStaticTools.Connection);
                    cmd.Parameters.AddWithValue("@Paket", selectedValue);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Paket başarıyla silindi!");
                    txtPaketEkle.Focus();
                    cbPakListe.Items.Remove(selectedValue);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Kasa kasa = new Kasa();
            kasa.Show();
            this.Hide();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            YedekleKontrol yedekleKontrol = new YedekleKontrol();
            yedekleKontrol.ShowDialog();
        }



    }
}
