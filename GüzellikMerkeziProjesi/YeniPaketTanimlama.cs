using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text;
namespace GüzellikMerkeziProjesi
{
    public partial class YeniPaketTanimlama : Form
    {


        int id;

        public YeniPaketTanimlama(int id)
        {
            InitializeComponent();
            this.id = id;
        }

        private void YeniPaketTanimlama_Load(object sender, EventArgs e)
        {
            paketListele();
        }

       

        private void button1_Click(object sender, EventArgs e)
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

        private void paketListesiTemizle()
        {
            txtSeans.Text = "";
            txtUcret.Text = "";
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
          
            ConnectionAndStaticTools.OpenConnection();

            try
            {

                bool seansMi = true;
                bool kacinciSeKontol = true;
                string[] islemDizisi = listIslem.Items.OfType<string>().ToArray();
                string birlesikIslem = string.Join(":", islemDizisi);
                int kacinciGelis = 0;

                int sayac = 1;

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

                        MySqlCommand cmdPaket = new MySqlCommand("INSERT INTO dbpaketbilgisi (ID,KacinciGelis,`Seans/Kontrol`, Aciklama, Durum, Tutar) VALUES (@ID,@KacinciGelis, @SeansKontrol, @Aciklama, @Durum, @Tutar)", ConnectionAndStaticTools.Connection);
                        cmdPaket.Parameters.AddWithValue("@ID", id);
                        cmdPaket.Parameters.AddWithValue("@KacinciGelis", kacinciGelis);
                        cmdPaket.Parameters.AddWithValue("@SeansKontrol", seansKontrol);
                        cmdPaket.Parameters.AddWithValue("@Aciklama", $"{islem}{i}");
                        cmdPaket.Parameters.AddWithValue("@Durum", "Bekliyor");
                        cmdPaket.Parameters.AddWithValue("@Tutar", ucret / seans);

                        cmdPaket.ExecuteNonQuery();


                    }
                    kacinciGelis = 0;
                }

                kayitTablosunaEkle();
                MessageBox.Show("Paket başarıyla tanımlanmıştır.");
                DanisanBilgileri danisanBilgileri = new DanisanBilgileri(id);
                ConnectionAndStaticTools.danisanGetir(danisanBilgileri, id);
                this.Hide();

            }
            catch (Exception ex)
            {

                MessageBox.Show(" Tüm alanları doldurunuz: " + ex.Message);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }

        }

        private int GetNextKacinciGelis()
        {
            int kacinciGelis = 500;

            try
            {
                // Veritabanından en büyük KacinciGelis değerini al
                ConnectionAndStaticTools.OpenConnection();
                MySqlCommand command = new MySqlCommand("SELECT MAX(KacinciGelis) FROM dbpaketbilgisi", ConnectionAndStaticTools.Connection);
                object result = command.ExecuteScalar();
                ConnectionAndStaticTools.CloseConnection();

                // Eğer sonuç null değilse ve 100'den küçükse, başlangıç değeri olarak 100 kullan
                if (result != DBNull.Value && result != null)
                {
                    int maxValue = Convert.ToInt32(result);
                    kacinciGelis = maxValue < 500 ? 100 : maxValue + 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("GET NEXT KACINCI GELIS Hata: " + ex.Message);
            }

            return kacinciGelis;
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

        private void kayitTablosunaEkle()
        {
            try
            {
                if (listIslem.Items.Count == 0)
                {
                    MessageBox.Show("Lütfen paket tanımlayınız!");
                    return;
                }

                string[] islemDizisi = listIslem.Items.OfType<string>().ToArray();
                string birlesikIslem = string.Join(":", islemDizisi);
                StringBuilder diziBirlestirGuncelle = new StringBuilder();

                ConnectionAndStaticTools.OpenConnection();

                using (MySqlCommand selectCommand = new MySqlCommand("SELECT * FROM dbdanisankayit WHERE DanisanID = @ID", ConnectionAndStaticTools.Connection))
                {
                    selectCommand.Parameters.AddWithValue("@ID", id);

                    using (MySqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string[] dizi1 = reader["İslem"].ToString().Split(':');
                                foreach (string dizi in dizi1)
                                {
                                    string[] dizi2 = dizi.Split(',');
                                    string diziBirlestir = string.Join(",", dizi2);
                                    diziBirlestirGuncelle.Append(diziBirlestir + ":");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Belirtilen ID için veri bulunamadı.");
                            return;
                        }
                    } // `reader` burada kapanacak
                }

                // Mevcut işlemi yeni işlemle birleştiriyoruz
                diziBirlestirGuncelle.Append(birlesikIslem);

                // Veritabanını güncelliyoruz
                using (MySqlCommand updateCommand = new MySqlCommand("UPDATE dbdanisankayit SET İslem = @İslem WHERE DanisanID = @ID", ConnectionAndStaticTools.Connection))
                {
                    updateCommand.Parameters.AddWithValue("@ID", id);
                    updateCommand.Parameters.AddWithValue("@İslem", diziBirlestirGuncelle.ToString());
                    updateCommand.ExecuteNonQuery();
                }

                // İşlem listesini temizliyoruz
                listIslem.Items.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("YENI PAKET TANIMLAMA KAYIT TABLOSUNA EKLE Hata: " + ex.Message);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }
        }



        private void paketListele()
        {
            cbIslem.Items.Clear();
            MySqlDataReader reader = null;

            try
            {
                // Veritabanı bağlantısını açıyoruz
                ConnectionAndStaticTools.OpenConnection();

                // Veritabanından veri çekmek için komut oluşturuyoruz
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM dbpaketler ORDER BY Paket ASC", ConnectionAndStaticTools.Connection);

                // Komutun sonuçlarını okuyoruz
                reader = cmd.ExecuteReader();

                // Verileri combo box'a ekliyoruz
                while (reader.Read())
                {
                    cbIslem.Items.Add(reader["Paket"].ToString());
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıya bilgi veriyoruz
                MessageBox.Show("YENİ PAKET TANIMLA PAKET LISTELE Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Veritabanı reader'ını kapatıyoruz
                if (reader != null)
                {
                    reader.Close();
                }

                // Veritabanı bağlantısını kapatıyoruz
                ConnectionAndStaticTools.CloseConnection();
            }
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
