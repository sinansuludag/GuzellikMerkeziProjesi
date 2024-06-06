using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Collections.Generic;
using System.Data;
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
            OpenConnection();

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
                MessageBox.Show("Hata: " + ex.Message + "\nStack Trace: " + ex.StackTrace);
            }
            finally
            {
                CloseConnection();
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

        //Bu en son eklediğim fonk bu fonk kayıttablosundaki İslem sütununa ekleme yapıyor
        private void kayitTablosunaEkle()
        {
            try
            {
                if (listIslem.Items.Count == 0)
                {
                    MessageBox.Show("Lütfen paket tanımlayınız!");
                }
                else
                {
                    string[] islemDizisi = listIslem.Items.OfType<string>().ToArray();
                    string birlesikIslem = string.Join(":", islemDizisi);
                    string diziBirlestirGuncelle = "";
                    OpenConnection();

                    using (MySqlCommand selectCommand = new MySqlCommand("Select * from dbdanisankayit where DanisanID=@ID", ConnectionAndStaticTools.Connection))
                    {
                        selectCommand.Parameters.AddWithValue("@ID", id);

                        using (MySqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            // Okuma işlemi için sorgunun çalıştığından emin olun
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string[] dizi1 = reader["İslem"].ToString().Split(':');
                                    string[] dizi2;


                                    foreach (string dizi in dizi1)
                                    {
                                        dizi2 = dizi.Split(',');
                                        string diziBirlestir = string.Join(",", dizi2);
                                        diziBirlestirGuncelle += diziBirlestir + ":";
                                    }

                                    diziBirlestirGuncelle += birlesikIslem;
                                }

                                // Mevcut okuyucuyu kapatın
                                reader.Close();

                                // Tek bir Update işlemi için bağlantıyı açın, güncelleme işlemini gerçekleştirin ve bağlantıyı kapatın
                                using (MySqlCommand updateCommand = new MySqlCommand("Update dbdanisankayit set İslem=@İslem where DanisanID=@ID", ConnectionAndStaticTools.Connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@ID", id);
                                    updateCommand.Parameters.AddWithValue("@İslem", diziBirlestirGuncelle);
                                    updateCommand.ExecuteNonQuery();
                                }

                                listIslem.Items.Clear();
                            }
                            else
                            {
                                MessageBox.Show("Belirtilen ID için veri bulunamadı.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                // Her durumda bağlantıyı kapatın
                CloseConnection();
            }


        }

        private void paketListele()
        {
            cbIslem.Items.Clear();
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand("Select * from dbpaketler ORDER BY Paket ASC", ConnectionAndStaticTools.Connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                cbIslem.Items.Add(reader["Paket"].ToString());
            }
            CloseConnection();
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
