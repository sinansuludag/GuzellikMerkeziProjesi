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
    public partial class OdemeAl : Form
    {

        int id;

        public OdemeAl(int id)
        {
            InitializeComponent();
            this.id = id;
        }

        private void OdemeAl_Load(object sender, EventArgs e)
        {
            txtID.Text=id.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtTutar.Text == "" && cbOdTip.Text == "")
            {
                MessageBox.Show("Lütfen Ödeme tipi ve Tutar alanını doldurunuz");
            }
            else
            {
                try
                {
                    ConnectionAndStaticTools.OpenConnection();

                    MySqlCommand mySqlCommand = new MySqlCommand("Insert into dbalinanodemeler (ID,Tarih,Aciklama,OdemeTipi,Tutar) values (@ID,@Tarih,@Aciklama,@OdemeTipi,@Tutar)", ConnectionAndStaticTools.Connection);
                    mySqlCommand.Parameters.AddWithValue("@ID", id);
                    mySqlCommand.Parameters.AddWithValue("@Tarih", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                    mySqlCommand.Parameters.AddWithValue("@Aciklama", richTxtAciklama.Text);
                    mySqlCommand.Parameters.AddWithValue("@OdemeTipi", cbOdTip.Text);
                    mySqlCommand.Parameters.AddWithValue("@Tutar", float.Parse(txtTutar.Text));

                    mySqlCommand.ExecuteNonQuery();
                    raporaEkle();
                    temizle();
                    MessageBox.Show("Ödeme başarıyla alınmıştır.");

                    DanisanBilgileri danisanBilgileri = new DanisanBilgileri(id);
                    ConnectionAndStaticTools.danisanGetir(danisanBilgileri, id);
                    this.Hide();
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

        private void raporaEkle()
        {
            try
            {
                string[] isimSoyad = danisanIsimSoyadGetir();
                ConnectionAndStaticTools.OpenConnection();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO dbrapor (ID, `Adi Soyadi`, Tarih, OdemeTipi, Tutar) VALUES (@ID, @AdiSoyadi, @Tarih, @OdemeTipi, @Tutar)", ConnectionAndStaticTools.Connection);
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@AdiSoyadi", isimSoyad[0]);
                cmd.Parameters.AddWithValue("@Tarih", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@OdemeTipi", cbOdTip.Text);
                cmd.Parameters.AddWithValue("@Tutar", float.Parse(txtTutar.Text));

                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                MessageBox.Show("Hata:" + e.Message);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }
        }

        private string[] danisanIsimSoyadGetir()
        {
            List<string> isimSoyadListesi = new List<string>();

            try
            {
                ConnectionAndStaticTools.OpenConnection();
                MySqlCommand cmd = new MySqlCommand("SELECT Adi, Soyadi FROM dbdanisankayit WHERE DanisanID = @ID", ConnectionAndStaticTools.Connection);
                cmd.Parameters.AddWithValue("@ID", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string adi = reader["Adi"].ToString();
                    string soyadi = reader["Soyadi"].ToString();
                    string isimSoyad = adi + " " + soyadi;
                    isimSoyadListesi.Add(isimSoyad);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Hata:" + e.Message);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }

            return isimSoyadListesi.ToArray();
        }


        private void temizle()
        {
            txtID.Text = "";
            richTxtAciklama.Text = "";
            cbOdTip.Text = "";
            txtTutar.Text = "";
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
