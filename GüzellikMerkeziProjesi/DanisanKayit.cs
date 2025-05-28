using System;
using System.Data;
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
           

            try
            {
                ConnectionAndStaticTools.ExecuteWithConnection(conn =>
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

                    MySqlCommand cmdDanisan = new MySqlCommand("INSERT INTO dbdanisankayit (Adi, Soyadi, Telefon, Cinsiyet, Referans, İslem) VALUES (@Adi, @Soyadi, @Telefon, @Cinsiyet, @Referans, @İslem)", conn);
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
                });

                
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("DANISAN KAYDET Veritabanı hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("DANISAN KAYDET Hata: " + ex.Message);
            }
        }

        public void paketBilgisineGonder(int danisanId)
        {
            

            try
            {
                ConnectionAndStaticTools.ExecuteWithConnection(conn =>
                {
                    bool seansMi = true;
                    bool kacinciSeKontol = true;
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
                            string seansKontrol = $"{sayac}.Seans";


                            if (seansMi == true && kacinciSeKontol == true)
                            {
                                seansKontrol = $"{sayac}.Seans";

                            }
                            else if (seansMi == false && kacinciSeKontol == true)
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

                            MySqlCommand cmdPaket = new MySqlCommand("INSERT INTO dbpaketbilgisi (ID, KacinciGelis,`Seans/Kontrol`, Aciklama, Durum, Tutar) VALUES (@ID,@KacinciGelis, @SeansKontrol, @Aciklama, @Durum, @Tutar)", conn);
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

                });
                
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("PAKET BİLGİSİNE GONDER Veritabanı hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PAKET BİLGİSİNE GONDER Hata: " + ex.Message );
            }

        }



        private void paketListele()
        {
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;

            try
            {
                // ComboBox'u temizleyin
                cbIslem.Items.Clear();

                // Bağlantıyı aç
                ConnectionAndStaticTools.ExecuteWithConnection(conn =>
                {
                    // SQL komutunu tanımla
                    cmd = new MySqlCommand("SELECT * FROM dbpaketler ORDER BY Paket ASC", conn);

                    // Veri okuyucu kullanımı
                    reader = cmd.ExecuteReader();

                    // Okuma işlemi sırasında hata kontrolü
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // "Paket" kolonundan veri al ve ComboBox'a ekle
                            cbIslem.Items.Add(reader["Paket"].ToString());
                        }
                    }
                    else
                    {
                        // Hiç veri bulunamadıysa uyarı ver
                        MessageBox.Show("Veritabanında paket bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                });

               
            }
            catch (MySqlException sqlEx)
            {
                // MySQL hatalarını yakala
                MessageBox.Show($"DANISAN KAYIT PAKET LISTELE Veritabanı hatası: {sqlEx.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Diğer hataları yakala
                MessageBox.Show($"DANISAN KAYIT PAKET LISTELE Beklenmeyen bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Veri okuyucuyu kapat
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                // Komutu kapat
                if (cmd != null)
                {
                    cmd.Dispose();
                }

            }
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
                    MessageBox.Show("Lütfen geçerli bir seans veya ücret sayısı giriniz");
                }

            }
        }

       
    }
}
