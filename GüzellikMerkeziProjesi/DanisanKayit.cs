using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GüzellikMerkeziProjesi
{
    public partial class DanisanKayit : Form
    {
        
        public DanisanKayit()
        {
            InitializeComponent();
        }

        private void DanisanKayit_Load(object sender, EventArgs e)
        {
            paketListele();
            txtAdi.Focus();
        }

        private void btnAnasayfa1_Click(object sender, EventArgs e)
        {
            Anasayfa anasayfa = new Anasayfa();
            anasayfa.Show();
            this.Hide();
        }

        private void temizle()
        {
            txtAdi.Text = "";
            txtSoyadi.Text = "";
            txtTel.Text = "";
            cbCinsiyet.Text = "";
            txtReferans.Text = "";
            listIslem.Items.Clear();
            txtSeans.Text = "";
            txtUcret.Text = "";
        }
        private void AltForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; // Formun kapatılmasını engelleyin
            Anasayfa anasayfa = new Anasayfa();
            anasayfa.Show();
            this.Hide(); // Alt formu gizleyin veya kapatın
        }


        private bool TelefonNumarasiGecerliMi(string telefon)
        {
            // Telefon numarasının uzunluğunu kontrol et
            if (telefon.Length != 11)
            {
                MessageBox.Show("Telefon numarası 11 haneli olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Telefon numarasının sadece sayılardan oluştuğunu kontrol et
            foreach (char karakter in telefon)
            {
                if (!char.IsDigit(karakter))
                {
                    MessageBox.Show("Telefon numarası sadece rakamlardan oluşmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private void txtTutar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Enter tuşunu bastırmayı engelle
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            ConnectionAndStaticTools.OpenConnection();

            try
            {

                string telefonNumarasi = txtTel.Text;

                string[] islemDizisi = listIslem.Items.OfType<string>().ToArray();
                string birlesikIslem = string.Join(":", islemDizisi);

                if (string.IsNullOrWhiteSpace(txtAdi.Text) || string.IsNullOrWhiteSpace(txtSoyadi.Text) || string.IsNullOrWhiteSpace(txtTel.Text) || string.IsNullOrWhiteSpace(cbCinsiyet.Text) || string.IsNullOrWhiteSpace(birlesikIslem))
                {
                    MessageBox.Show("Lütfen gerekli alanları doldurunuz!");
                    return;
                }
                // Telefon numarasının geçerli olup olmadığını kontrol et
                if (!TelefonNumarasiGecerliMi(telefonNumarasi))
                {
                    return;
                }

                MySqlCommand cmdDanisan = new MySqlCommand("INSERT INTO dbdanisankayit (Adi, Soyadi, Telefon, Cinsiyet, Referans, İslem) VALUES (@Adi, @Soyadi, @Telefon, @Cinsiyet, @Referans, @İslem)", ConnectionAndStaticTools.Connection);
                cmdDanisan.Parameters.AddWithValue("@Adi", txtAdi.Text);
                cmdDanisan.Parameters.AddWithValue("@Soyadi", txtSoyadi.Text);
                cmdDanisan.Parameters.AddWithValue("@Telefon", txtTel.Text);
                cmdDanisan.Parameters.AddWithValue("@Cinsiyet", cbCinsiyet.Text);
                cmdDanisan.Parameters.AddWithValue("@Referans", txtReferans.Text);
                cmdDanisan.Parameters.AddWithValue("@İslem", birlesikIslem);

                cmdDanisan.ExecuteNonQuery();

                int danisanID;

                if (int.TryParse(cmdDanisan.LastInsertedId.ToString(), out danisanID))
                {
                    paketBilgisineGonder(danisanID);
                    MessageBox.Show("Kayıtlar başarıyla eklenmiştir.");
                    temizle();
                }
                else
                {
                    MessageBox.Show("DanisanID alınamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message + "\nStack Trace: " + ex.StackTrace);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }
        }

        public void paketBilgisineGonder(int danisanId)
        {
            ConnectionAndStaticTools.OpenConnection();

            try
            {
                bool seansMi = true;
                bool kacinciSeKontol=true;
                string[] islemDizisi = listIslem.Items.OfType<string>().ToArray();
                string birlesikIslem = string.Join(":", islemDizisi);

                int sayac = 1;
                int kacinciGelis = 0;

                string[] islemDizisi2 = birlesikIslem.Split(':');
                string[] sonIslem;


                foreach (string paketDizi in islemDizisi2)
                {   

                    sonIslem = paketDizi.Split(',');
                    string islem = sonIslem[0];
                    int seans = Convert.ToInt32(sonIslem[1]);
                    float ucret = float.Parse(sonIslem[2]);
                    for (int i = 1; i <= seans; i++)
                    {
                        kacinciGelis++;
                        string seansKontrol= $"{sayac}.Seans";
                       
                        
                        if(seansMi==true && kacinciSeKontol==true)
                        {
                            seansKontrol = $"{sayac}.Seans";

                        }
                        else if(seansMi==false && kacinciSeKontol == true)
                        {
                            seansKontrol = $"{sayac}.Kontrol";
                            sayac++;    
                        }
                        if (i == seans)
                        {
                            sayac = 1;
                        }

                        if (i % 2 == 0)
                        {
                            kacinciSeKontol = false;
                        }
                        else
                        {
                            kacinciSeKontol = true;
                        }
                        seansMi = true ? false : true;

                        MySqlCommand cmdPaket = new MySqlCommand("INSERT INTO dbpaketbilgisi (ID, KacinciGelis,`Seans/Kontrol`, Aciklama, Durum, Tutar) VALUES (@ID,@KacinciGelis, @SeansKontrol, @Aciklama, @Durum, @Tutar)", ConnectionAndStaticTools.Connection);
                        cmdPaket.Parameters.AddWithValue("@ID", danisanId);
                        cmdPaket.Parameters.AddWithValue("@KacinciGelis", kacinciGelis);
                        cmdPaket.Parameters.AddWithValue("@SeansKontrol", seansKontrol);
                        cmdPaket.Parameters.AddWithValue("@Aciklama", $"{islem}{i}");
                        cmdPaket.Parameters.AddWithValue("@Durum", "Bekliyor");
                        cmdPaket.Parameters.AddWithValue("@Tutar", ucret / seans);

                        cmdPaket.ExecuteNonQuery();

                        
                    }

                }

                temizle();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message + "\nStack Trace: " + ex.StackTrace);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }
        }



        private void paketListele()
        {
            cbIslem.Items.Clear();
            ConnectionAndStaticTools.OpenConnection();
            MySqlCommand cmd = new MySqlCommand("Select * from dbpaketler ORDER BY Paket ASC", ConnectionAndStaticTools.Connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                cbIslem.Items.Add(reader["Paket"].ToString());
            }
            ConnectionAndStaticTools.CloseConnection();
        }

     

        private void paketListesiTemizle()
        {
            txtSeans.Text = "";
            txtUcret.Text = "";
        }

        private void btnEkle1_Click(object sender, EventArgs e)
        {
            if (cbIslem.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir işlem seçiniz!");
            }
            else
            {
                PaketBilgisi paketBilgisi;
                if (float.TryParse(txtSeans.Text, out float seans) && float.TryParse(txtUcret.Text, out float ucret))
                {
                    paketBilgisi = new PaketBilgisi(cbIslem.SelectedItem.ToString(), seans, ucret);
                    listIslem.Items.Add(paketBilgisi.ToString());
                    paketListesiTemizle();
                    cbIslem.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir sayı giriniz");
                }

            }
        }

       
    }
}
