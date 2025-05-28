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
                if (string.IsNullOrWhiteSpace(txtPaketEkle.Text))
                {
                    MessageBox.Show("Lütfen bir paket ismi giriniz!");
                    return;
                }

                ConnectionAndStaticTools.ExecuteWithConnection(conn =>
                {
                    MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM dbpaketler WHERE Paket = @Paket", conn);
                    checkCmd.Parameters.AddWithValue("@Paket", txtPaketEkle.Text);
                    int existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (existingCount > 0)
                    {
                        MessageBox.Show("Bu paket zaten mevcut!");
                        return;
                    }

                    MySqlCommand insertCmd = new MySqlCommand("INSERT INTO dbpaketler (Paket) VALUES (@Paket)", conn);
                    insertCmd.Parameters.AddWithValue("@Paket", txtPaketEkle.Text);
                    insertCmd.ExecuteNonQuery();

                    MessageBox.Show("Paket başarıyla eklendi!");
                    temizle();
                    paketListele();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("PAKET EKLEME Hata oluştu: " + ex.Message);
            }
        }

        private void paketListele()
        {
            cbPakListe.Items.Clear();
            try
            {
                ConnectionAndStaticTools.ExecuteWithConnection(conn =>
                {
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM dbpaketler ORDER BY Paket ASC", conn);

                   MySqlDataReader reader = cmd.ExecuteReader();
                    
                        while (reader.Read())
                        {
                            cbPakListe.Items.Add(reader["Paket"].ToString());
                        }
                    
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("PAKET LİSTELEME hata oluştu: " + ex.Message);
            }
        }

        private void temizle()
        {
            txtPaketEkle.Text = "";
            txtPaketEkle.Focus();
        }

        //Paket Silme
        private void silToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbPakListe.SelectedItem != null)
                {
                    string selectedValue = cbPakListe.SelectedItem.ToString();

                    ConnectionAndStaticTools.ExecuteWithConnection(conn =>
                    {
                        MySqlCommand cmd = new MySqlCommand("DELETE FROM dbpaketler WHERE Paket = @Paket", conn);
                        cmd.Parameters.AddWithValue("@Paket", selectedValue);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Paket başarıyla silindi!");
                        cbPakListe.Items.Remove(selectedValue);
                    });
                }
                else
                {
                    MessageBox.Show("Lütfen silmek istediğiniz bir paketi seçin.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PAKET SİLME hata oluştu: " + ex.Message);
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
