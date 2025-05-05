using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;

namespace GüzellikMerkeziProjesi
{
    public partial class DanisanBilgileri : Form
    {
        int id;
        List<string> silinenList = new List<string>();
        List<string> diziList = new List<string>();
        

        public DanisanBilgileri(int id)
        {
            InitializeComponent();
            this.id = id;
        }
      

        private void AltForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; // Formun kapatılmasını engelleyin
            Anasayfa anasayfa = new Anasayfa();
            anasayfa.Show();
            this.Hide(); // Alt formu gizleyin veya kapatın
        }


        public void notlariListele(int danisanID)
        {
            try
            {
                // Bağlantıyı açık olduğundan emin olun
                ConnectionAndStaticTools.OpenConnection();

                // Veritabanı bağlantısı ve komut oluşturuluyor
                MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM dbnotlar WHERE ID = @ID", ConnectionAndStaticTools.Connection);
                adapter.SelectCommand.Parameters.AddWithValue("@ID", danisanID);

                DataTable dt = new DataTable();

                // Veritabanından veri alınıyor
                adapter.Fill(dt);

                // Veriler DataGridView'e aktarılıyor
                dataGridView2.DataSource = dt;
            }
            catch (MySqlException sqlEx)
            {
                // Veritabanı hatalarını yakalama
                MessageBox.Show($"NOTLARI LISTELE Veritabanı hatası: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // Genel hataları yakalama
                MessageBox.Show($"NOTLARI LISTELE  hata oluştu: {ex.Message}");
            }
            finally
            {
                // Bağlantıyı kapatıyoruz
                ConnectionAndStaticTools.CloseConnection();
            }
        }




        public void seansBilgileriniGetir(int danisanID)
        {
            try
            {
                // Veritabanı bağlantısını açıyoruz
                ConnectionAndStaticTools.OpenConnection();

                // Veritabanından verileri al
                DataTable dt = new DataTable();
                using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter("SELECT * from dbpaketbilgisi where ID=@DanisanID", ConnectionAndStaticTools.Connection))
                {
                    // Parametreyi ekleyerek sorguyu çalıştırıyoruz
                    mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@DanisanID", danisanID);
                    mySqlDataAdapter.Fill(dt);
                }

                // Veri kaynağını DataGridView'e bağla
                dataGridView1.DataSource = dt;

                // Görüntülenmeyecek sütunları gizle
                dataGridView1.Columns["KacinciGelis"].Visible = false;

                // Readonly özelliğini ayarla
                dataGridView1.Columns["ID"].ReadOnly = true;
                dataGridView1.Columns["Seans/Kontrol"].ReadOnly = true;
                dataGridView1.Columns["Aciklama"].ReadOnly = true;
                dataGridView1.Columns["Tutar"].ReadOnly = true;

                // DataGridView'in arka plan rengini ve seçili hücrenin arka plan rengini ayarla
                dataGridView1.BackgroundColor = Color.White;
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 139, 139);

                // İstenen sütunların genişliklerini belirleme
                dataGridView1.Columns["ID"].Width = 50;
                dataGridView1.Columns["Tarih"].Width = 80;
                dataGridView1.Columns["Seans/Kontrol"].Width = 80;
                dataGridView1.Columns["Durum"].Width = 90;
                dataGridView1.Columns["Tutar"].Width = 70;
                dataGridView1.Columns["Aciklama"].Width = 370;

                // Sıralamayı engelle
                dataGridView1.Columns["ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Tarih"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Seans/Kontrol"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Durum"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Tutar"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Aciklama"].SortMode = DataGridViewColumnSortMode.NotSortable;

            }
            catch (MySqlException sqlEx)
            {
                // MySQL veritabanı hatası
                MessageBox.Show("SEANS BİLGİLERİ GETİR Veritabanı hatası: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                // Genel hatalar
                MessageBox.Show("SEANS BILGILERI GETIR hata oluştu: " + ex.Message);
            }
            finally
            {
                // Bağlantıyı kapatıyoruz
                ConnectionAndStaticTools.CloseConnection();
            }
        }


        //Gelinen seans için o satırın arka plan rengi kırmızı olsun
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name.Equals("Durum", StringComparison.CurrentCultureIgnoreCase))
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                DataGridViewCell cell = row.Cells[e.ColumnIndex];

                if (e.Value != null && e.Value.ToString().Equals("Geldi", StringComparison.CurrentCultureIgnoreCase))
                {
                    foreach (DataGridViewCell rowCell in row.Cells)
                    {
                        rowCell.Style.BackColor = Color.Red;
                    }
                }
                else
                {
                    // Eğer "Geldi" değilse, satırın orijinal arka plan rengini geri yükle
                    if (row.DefaultCellStyle.BackColor != dataGridView1.DefaultCellStyle.BackColor)
                    {
                        foreach (DataGridViewCell rowCell in row.Cells)
                        {
                            rowCell.Style.BackColor = row.DefaultCellStyle.BackColor;
                        }
                    }
                }
            }
        }


        public void notBilgisiniGetir(int danisanID)
        {
            try
            {
                // Veritabanı bağlantısını açma
                ConnectionAndStaticTools.OpenConnection();

                // MySqlDataAdapter ile veriyi çekme işlemi
                using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter("SELECT * from dbnotlar where ID=@DanisanID", ConnectionAndStaticTools.Connection))
                {
                    mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@DanisanID", danisanID);

                    // DataTable ile veriyi doldurma
                    DataTable dt = new DataTable();
                    mySqlDataAdapter.Fill(dt);

                    // DataGridView'e veri kaynağını bağlama
                    dataGridView2.DataSource = dt;

                    // DataGridView özelliklerini ayarlama
                    dataGridView2.BackgroundColor = Color.White;
                    dataGridView2.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 139, 139);

                    // Sıralamayı kaldırma
                    dataGridView2.Columns["ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView2.Columns["Notlar"].SortMode = DataGridViewColumnSortMode.NotSortable;

                    // Hücre genişliklerini ayarlama
                    dataGridView2.Columns["ID"].Width = 50;
                    dataGridView2.Columns["Notlar"].Width = 600;
                }
            }
            catch (MySqlException ex)
            {
                // MySQL hatası
                MessageBox.Show("NOT BILGISINI GETIR Veritabanı hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Genel hata
                MessageBox.Show("NOT BILGISINI GETIR Hata: " + ex.Message);
            }
            finally
            {
                // Bağlantıyı kapatma
                ConnectionAndStaticTools.CloseConnection();
            }
        }


        public void alinanOdemeBilgisiniGetir(int danisanID)
        {
            try
            {
                // Veritabanı bağlantısını açma
                ConnectionAndStaticTools.OpenConnection();

                // MySqlDataAdapter ile veriyi çekme işlemi
                using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter("SELECT * from dbalinanodemeler where ID=@DanisanID", ConnectionAndStaticTools.Connection))
                {
                    mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@DanisanID", danisanID);

                    // DataTable ile veriyi doldurma
                    DataTable dt = new DataTable();
                    mySqlDataAdapter.Fill(dt);

                    // DataGridView'e veri kaynağını bağlama
                    dataGridView3.DataSource = dt;
                    dataGridView3.BackgroundColor = Color.White;
                    dataGridView3.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 139, 139);

                    // Sıralamayı kaldırma
                    dataGridView3.Columns["ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView3.Columns["Tarih"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView3.Columns["Aciklama"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView3.Columns["OdemeTipi"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView3.Columns["Tutar"].SortMode = DataGridViewColumnSortMode.NotSortable;

                    // Hücre genişliklerini ayarlama
                    dataGridView3.Columns["ID"].Width = 50;
                    dataGridView3.Columns["Tarih"].Width = 90;
                    dataGridView3.Columns["Aciklama"].Width = 500;
                    dataGridView3.Columns["OdemeTipi"].Width = 90;
                    dataGridView3.Columns["Tutar"].Width = 60;
                }
            }
            catch (MySqlException ex)
            {
                // MySQL hatası
                MessageBox.Show("ALINAN ODEME BILGISI Veritabanı hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Genel hata
                MessageBox.Show("ALINAN ODEME BILGISI Hata: " + ex.Message);
            }
            finally
            {
                // Bağlantıyı kapatma
                ConnectionAndStaticTools.CloseConnection();
            }
        }



        public void DanisanBilgileriniGoster(int danisanId)
        {
            try
            {
                // Veritabanı bağlantısını açma
                ConnectionAndStaticTools.OpenConnection();

                // MySQL komutu oluşturma
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM dbdanisankayit WHERE DanisanID = @DanisanId", ConnectionAndStaticTools.Connection);
                cmd.Parameters.AddWithValue("@DanisanId", danisanId);

                // Veriyi okuma
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Verileri arayüze yazma
                    txtId.Text = reader["DanisanID"].ToString();
                    txtAdi.Text = reader["Adi"].ToString();
                    txtSoyadi.Text = reader["Soyadi"].ToString();
                    txtTel.Text = reader["Telefon"].ToString();
                    txtCinsiyet.Text = reader["Cinsiyet"].ToString();
                    txtReferans.Text = reader["Referans"].ToString();

                    // İşlem ücretlerini hesaplama
                    string[] diziUcret = reader["İslem"].ToString().Split(':');
                    float toplam = 0;
                    foreach (string d in diziUcret)
                    {
                        string[] diziUcret2 = d.Split(',');
                        if (diziUcret2.Length == 3)
                        {
                            if (float.TryParse(diziUcret2[2], out float ucret)) // Ucretin geçerli olup olmadığını kontrol et
                            {
                                toplam += ucret;
                            }
                        }
                    }

                    // Toplam ücreti arayüze yazma
                    txtTopTutar.Text = toplam.ToString();

                    // Ödeme bilgisini hesaplama
                    float alOdeme = alinanOdemeTutari(danisanId);
                    if (toplam - alOdeme <= 0)
                    {
                        txtKalanOdeme.Text = "0";
                        txtAlOdeme.Text = alOdeme.ToString();
                    }
                    else
                    {
                        txtKalanOdeme.Text = (toplam - alOdeme).ToString();
                        txtAlOdeme.Text = alOdeme.ToString();
                    }
                }
                else
                {
                    // Danışman bulunamadıysa hata mesajı
                    MessageBox.Show("Danışan bulunamadı.");
                }
            }
            catch (MySqlException ex)
            {
                // MySQL hatası
                MessageBox.Show("DANISAN BILGILERINI GOSTER Veritabanı hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Genel hata
                MessageBox.Show("DANISAN BILGILERINI GOSTER Hata: " + ex.Message);
            }
            finally
            {
                // Bağlantıyı kapatma
                ConnectionAndStaticTools.CloseConnection();
            }
        }


        public float alinanOdemeTutari(int danisanID)
        {
            float toplam = 0f;
            try
            {
                // Her işlem için yeni bir bağlantı açıyoruz
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Baglanti"].ConnectionString))
                {
                    connection.Open();  // Bağlantıyı aç

                    // Veritabanından verileri almak için sorgu oluştur
                    using (MySqlCommand mySqlCommand = new MySqlCommand("SELECT * FROM dbalinanodemeler WHERE ID = @DanisanId", connection))
                    {
                        mySqlCommand.Parameters.AddWithValue("@DanisanId", danisanID);

                        // MySqlDataReader kullanarak verileri çek
                        using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Tutar değeri string olarak alınıyor
                                string tutarStr = reader["Tutar"].ToString();

                                // string değeri float'a dönüştür
                                if (float.TryParse(tutarStr, out float tutar))
                                {
                                    toplam += tutar;  // Eğer geçerli bir float ise, toplamı artır
                                }
                                else
                                {
                                    // Eğer dönüşüm başarısız olursa, loglama yapılabilir veya kullanıcıya mesaj gösterilebilir
                                    MessageBox.Show($"Geçersiz tutar değeri: {tutarStr}");
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException mySqlEx)
            {
                // MySQL hatalarını burada yakalayabilirsiniz
                MessageBox.Show("ALINAN ODEME TUTARI Veritabanı Hatası: " + mySqlEx.Message);
            }
            catch (Exception ex)
            {
                // Diğer hataları burada yakalayabilirsiniz
                MessageBox.Show("ALINAN ODEME TUTARI Hata: " + ex.Message);
            }

            return toplam;
        }





        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            danisanBilgisiniGüncelle();
            Anasayfa anasayfa = new Anasayfa();
            anasayfa.Show();
            this.Hide();
        }

        private void temizle()
        {
            txtId.Text = "";
            txtAdi.Text = "";
            txtSoyadi.Text = "";
            txtTel.Text = "";
            txtCinsiyet.Text = "";
            txtReferans.Text = "";
            txtTopTutar.Text = "";
            txtAlOdeme.Text = "";
            txtKalanOdeme.Text = "";

            // DataGridView'leri temizleme
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            dataGridView3.DataSource = null;
        }

        private void danisanBilgisiniGüncelle()
        {
            try
            {
                try
                {
                    // Veritabanı bağlantısını aç
                    ConnectionAndStaticTools.OpenConnection();

                    // SQL güncelleme komutunu oluştur
                    MySqlCommand cmd = new MySqlCommand(
                        "Update dbdanisankayit set Adi=@Adi, Soyadi=@Soyadi, Telefon=@Telefon, Cinsiyet=@Cinsiyet, Referans=@Referans where DanisanID=@ID",
                        ConnectionAndStaticTools.Connection);

                    // Parametreleri ekle
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Adi", txtAdi.Text);
                    cmd.Parameters.AddWithValue("@Soyadi", txtSoyadi.Text);
                    cmd.Parameters.AddWithValue("@Telefon", txtTel.Text);
                    cmd.Parameters.AddWithValue("@Cinsiyet", txtCinsiyet.Text);
                    cmd.Parameters.AddWithValue("@Referans", txtReferans.Text);

                    // Sorguyu çalıştır
                     cmd.ExecuteNonQuery();              
                }
                catch (MySqlException mysqlEx)
                {
                    // Veritabanı hatalarını yakala
                    MessageBox.Show("DANISMAN BILGISI GUNCELLE Veritabanı hatası: " + mysqlEx.Message);
                }
                catch (Exception ex)
                {
                    // Diğer hataları yakala
                    MessageBox.Show("DANISMAN BILGISI GUNCELLE Hata: " + ex.Message);
                }
                finally
                {
                    // Bağlantıyı kapat
                    ConnectionAndStaticTools.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                // Genel hata kontrolü (genellikle try-catch'in dışı için gereksiz olabilir, burada başka bir hata yakalanabilir)
                MessageBox.Show("DANISMAN BILGISI GUNCELLE GENEL hata oluştu: " + ex.Message);
            }
        }

        // Danışan kayıt silme
        private void btnSil_Click(object sender, EventArgs e)
        {
            // Kullanıcıya silme işlemi için onay mesajı göster
            DialogResult result = MessageBox.Show("Danışana ait tüm verileri kalıcı olarak silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Kullanıcının seçimine göre işlemi gerçekleştir
            if (result == DialogResult.Yes)
            {
                try
                {
                    // Veritabanı bağlantısını aç
                    ConnectionAndStaticTools.OpenConnection();

                    // SQL komutunu oluştur
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM dbdanisankayit WHERE DanisanID = @ID", ConnectionAndStaticTools.Connection);
                    cmd.Parameters.AddWithValue("@ID", id);

                    // Silme işlemini gerçekleştir
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Silme başarılı ise
                        temizle();
                        MessageBox.Show("Kayıt başarıyla silindi.");
                        Anasayfa anasayfa = new Anasayfa();
                        anasayfa.Show();
                        this.Hide();
                    }
                    else
                    {
                        // Eğer silinen hiçbir kayıt yoksa
                        MessageBox.Show("Silinecek kayıt bulunamadı.");
                    }
                }
                catch (MySqlException mysqlEx)
                {
                    // Veritabanı hatası ile ilgili özel hata mesajı
                    MessageBox.Show("DANISAN KAYIT SILME Veritabanı hatası: " + mysqlEx.Message);
                }
                catch (Exception ex)
                {
                    // Diğer hatalar ile ilgili genel hata mesajı
                    MessageBox.Show("DANISAN KAYIT SILME Hata: " + ex.Message);
                }
                finally
                {
                    // Bağlantıyı kapat
                    ConnectionAndStaticTools.CloseConnection();
                }
            }
            else
            {
                // Kullanıcı "Hayır" seçeneğini seçerse burada yapılacak işlemler
                MessageBox.Show("Silme işlemi iptal edildi.");
            }
        }


        public void taninanPaketListele(int danismaID)
        {
            try
            {
                // ComboBox'ı temizle
                cbTaninanPaket.Items.Clear();

                // Veritabanı bağlantısını aç
                ConnectionAndStaticTools.OpenConnection();

                // SQL komutunu oluştur
                MySqlCommand cmd = new MySqlCommand("SELECT İslem FROM dbdanisankayit WHERE DanisanID=@ID ORDER BY İslem ASC", ConnectionAndStaticTools.Connection);
                cmd.Parameters.AddWithValue("@ID", danismaID);

                // Veritabanından veri oku
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string[] dizi1 = reader["İslem"].ToString().Split(':');
                            string[] dizi2;
                            for (int i = 0; i < dizi1.Length; i++)
                            {
                                dizi2 = dizi1[i].Split(',');
                                if (dizi2.Length > 0 && !string.IsNullOrEmpty(dizi2[0]))
                                {
                                    cbTaninanPaket.Items.Add(dizi2[0]);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Eğer veri yoksa, ComboBox'a herhangi bir şey eklenmez
                        MessageBox.Show("Bu danışanın tanınan paketi bulunmamaktadır.");
                    }
                }
            }
            catch (MySqlException mysqlEx)
            {
                // Veritabanı hataları için özel hata mesajı
                MessageBox.Show("TANINAN PAKET LISTELE Veritabanı hatası: " + mysqlEx.Message);
            }
            catch (Exception ex)
            {
                // Diğer hatalar için genel hata mesajı
                MessageBox.Show("TANINAN PAKET LISTELE Hata: " + ex.Message);
            }
            finally
            {
                // Veritabanı bağlantısını kapat
                ConnectionAndStaticTools.CloseConnection();
            }
        }


        public void dizidenElemanSilme(ref string[] array, string elementToRemove)
        {
            List<string> geciciList = new List<string>(array);
            int indexToRemove = geciciList.IndexOf(elementToRemove);

            if (indexToRemove != -1)
            {
                geciciList.RemoveAt(indexToRemove); // Öğeyi kaldır
                if (indexToRemove < geciciList.Count) // Eğer kaldırılan öğe listenin son öğesi değilse
                {
                    // Kaldırılan öğeden sonraki öğenin başındaki ":" karakterini kaldır
                    string sonrakiEleman = geciciList[indexToRemove];
                    if (sonrakiEleman.StartsWith(":"))
                    {
                        geciciList[indexToRemove] = sonrakiEleman.Substring(1);
                    }
                }
            }

            array = geciciList.ToArray();
        }


        public void taninanPaketSil()
        {
            try
            {
                if (cbTaninanPaket.SelectedItem != null)
                {
                    string selectedValue = cbTaninanPaket.SelectedItem.ToString();
                    cbTaninanPaket.Items.Clear();

                    // Veritabanı bağlantısını aç
                    ConnectionAndStaticTools.OpenConnection();

                    // Danışanın işlem bilgilerini çekmek için komut oluştur
                    MySqlCommand cmd = new MySqlCommand("SELECT İslem FROM dbdanisankayit WHERE DanisanID = @ID", ConnectionAndStaticTools.Connection);
                    cmd.Parameters.AddWithValue("@ID", id);

                    // Veritabanından veri okuma işlemi
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] dizi1 = reader["İslem"].ToString().Split(':');
                            string[] dizi2;

                            // Okunan her öğeyi işleyelim
                            for (int i = 0; i < dizi1.Length; i++)
                            {
                                dizi2 = dizi1[i].Split(',');

                                // Eğer seçilen paketle eşleşiyorsa, onu sil
                                if (selectedValue == dizi2[0])
                                {
                                    silinenList.Add(dizi1[i]);
                                    dizidenElemanSilme(ref dizi1, dizi1[i]);

                                    // Geriye kalan öğeleri diziList'e ekle
                                    foreach (var item in dizi1)
                                    {
                                        diziList.Add(item);
                                    }
                                }
                            }
                        }
                    }

                    // Veritabanı bağlantısını kapat
                    ConnectionAndStaticTools.CloseConnection();

                    // Paket bilgilerini güncelle
                    taninanPaketguncelle();
                    paketBilgisiGuncelle(id);
                }
                else
                {
                    MessageBox.Show("Lütfen bir öğe seçin.");
                }
            }
            catch (MySqlException ex)
            {
                // MySQL hatalarını burada yakalayabilirsiniz
                MessageBox.Show("TANINAN PAKET SILME MySQL Hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıya mesaj göster
                MessageBox.Show("TANINAN PAKET SILME Hata: " + ex.Message);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }
        }


        public void taninanPaketguncelle()
        {
            try
            {
                // Veritabanı bağlantısını aç
                ConnectionAndStaticTools.OpenConnection();

                // Diziyi listeye dönüştür ve veritabanına göndermek için hazırlık yap
                string[] dizi = diziList.ToArray();

                // SQL komutunu hazırla
                MySqlCommand cmd = new MySqlCommand("UPDATE dbdanisankayit SET İslem = @İslem WHERE DanisanID = @ID", ConnectionAndStaticTools.Connection);

                // Parametreleri ekle
                cmd.Parameters.AddWithValue("@ID", id);
                string birlestirilmisDizi = string.Join(":", dizi);
                cmd.Parameters.AddWithValue("@İslem", birlestirilmisDizi);

                // Komutu çalıştır
                int etk = cmd.ExecuteNonQuery();

                // Etkili bir işlem varsa kullanıcıya başarı mesajı göster
                if (etk > 0)
                {
                    MessageBox.Show("Paket başarıyla silinmiştir");
                }
                else
                {
                    MessageBox.Show("Paket silinirken bir sorun oluştu.");
                }

                // ComboBox'taki seçili öğeyi temizle
                cbTaninanPaket.Text = "";
            }
            catch (MySqlException ex)
            {
                // MySQL hatalarını burada yakalayabilirsiniz
                MessageBox.Show("TANINAN PAKET GUNCELLEME Veritabani Hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Diğer genel hataları burada yakalayabilirsiniz
                MessageBox.Show("TANINAN PAKET Hata: " + ex.Message);
            }
            finally
            {
                // Bağlantıyı kapatmayı unutma
                ConnectionAndStaticTools.CloseConnection();
            }
        }


        public void paketBilgisiGuncelle(int danisanId)
        {
            // Veritabanı bağlantısını aç
            ConnectionAndStaticTools.OpenConnection();

            try
            {
                // Silinen paket bilgilerini içeren listeyi diziye dönüştür
                string[] dizi = silinenList.ToArray();

                // Eğer dizi boş değilse
                if (dizi.Length > 0)
                {
                    string[] sonIslem;
                    int sayac = 1;

                    // Dizi üzerinde döngü başlat
                    foreach (string paketDizi in dizi)
                    {
                        // Paketi virgülle ayır
                        sonIslem = paketDizi.Split(',');

                        // Eğer dizinin uzunluğu beklenen öğe sayısına sahip değilse
                        if (sonIslem.Length != 3)
                        {
                            MessageBox.Show("Paket dizisi beklenen öğe sayısına sahip değil.");
                            continue; // Bir sonraki pakete geç
                        }

                        string islem = sonIslem[0]; // İlk eleman işlem ismi
                        int seans = Convert.ToInt32(sonIslem[1]); // İkinci eleman seans sayısı

                        // Seans sayısına göre işlem yap
                        for (int i = 1; i <= seans; i++)
                        {
                            // Eğer son seansa ulaşıldıysa sayaç sıfırlanacak
                            if (i == seans)
                            {
                                sayac = 1;
                            }
                            sayac++;

                            // SQL komutunu oluştur
                            MySqlCommand cmdPaket = new MySqlCommand("DELETE FROM dbpaketbilgisi WHERE ID = @ID AND Aciklama = @Aciklama", ConnectionAndStaticTools.Connection);
                            cmdPaket.Parameters.AddWithValue("@ID", danisanId);
                            cmdPaket.Parameters.AddWithValue("@Aciklama", $"{islem}{i}");

                            try
                            {
                                // SQL komutunu çalıştır
                                cmdPaket.ExecuteNonQuery();
                            }
                            catch (MySqlException sqlEx)
                            {
                                // MySQL hatası yakalama
                                MessageBox.Show($"PAKET BILGISI GUNCELLE MySQL Hatası: {sqlEx.Message}");
                            }
                            catch (Exception ex)
                            {
                                // Diğer genel hatalar
                                MessageBox.Show($"PAKET BILGISI GUNCELLE Hata: {ex.Message}");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Silinen paket listesi boş.");
                }
            }
            catch (Exception ex)
            {
                // Genel hata yönetimi
                MessageBox.Show($"PAKET BILGISI GUNCELLE Genel Hata: {ex.Message}");
            }
            finally
            {
                // Bağlantıyı kapat
                ConnectionAndStaticTools.CloseConnection();
            }
        }


        private void label16_Click(object sender, EventArgs e)
        {
            NotOlustur notOlustur = new NotOlustur(id);
            notOlustur.Show();
            this.Hide();
            
        }

        private void label15_Click(object sender, EventArgs e)
        {
            OdemeAl odemeAl = new OdemeAl(id);
            odemeAl.Show();
            this.Hide();
        }



        private void notListesiGetir()
        {
            try
            {
                ConnectionAndStaticTools.OpenConnection();

                // Veritabanı sorgusu için adapter
                using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter("SELECT * FROM dbnotlar WHERE ID = @DanisanID", ConnectionAndStaticTools.Connection))
                {
                    // Parametreyi ekle
                    mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@DanisanID", id);

                    // DataTable oluşturulacak ve veri ile doldurulacak
                    DataTable dt = new DataTable();
                    mySqlDataAdapter.Fill(dt);

                    // DataGridView'e veri aktar
                    dataGridView2.DataSource = dt;
                }
            }
            catch (MySqlException sqlEx)
            {
                // Veritabanı hatası varsa yakala
                MessageBox.Show("NOT LISTESI GETIR Veritabanı hatası: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                // Diğer tüm hataları yakala
                MessageBox.Show("NOT LISTESI GETIR Hata: " + ex.Message);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }
        }


        private void alinanOdemelerListesiGetir()
        {
            try
            {
                ConnectionAndStaticTools.OpenConnection();

                // MySqlDataAdapter ile verileri çekiyoruz
                using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter("SELECT * FROM dbalinanodemeler WHERE ID = @DanisanID", ConnectionAndStaticTools.Connection))
                {
                    // Parametreyi ekle
                    mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@DanisanID", id);

                    // DataTable oluşturulacak ve veri ile doldurulacak
                    DataTable dt = new DataTable();
                    mySqlDataAdapter.Fill(dt);

                    // DataGridView'e veri aktar
                    dataGridView3.DataSource = dt;

                    // "Aciklama" sütununu düzenlenebilir yap
                    dataGridView3.Columns["Aciklama"].ReadOnly = false;
                }
            }
            catch (MySqlException sqlEx)
            {
                // Veritabanı hatası varsa yakala
                MessageBox.Show("ALINAN ODEMELER LISTESI GETIR Veritabanı hatası: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                // Diğer tüm hataları yakala
                MessageBox.Show("ALINAN ODEMELER LISTESI GETIR Hata: " + ex.Message);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }
        }


        private void lblYeniPaketTanimla_Click(object sender, EventArgs e)
        {
            YeniPaketTanimlama yeniPaketTanimlama = new YeniPaketTanimlama(id);
            yeniPaketTanimlama.Show();
            this.Hide();
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 2) // Tarih sütunu (2)
            {
                string cellValue = e.FormattedValue.ToString().Trim(); // Hücreyi temizle

                // Hücre değeri boşsa geçerli olduğunu varsayalım
                if (string.IsNullOrEmpty(cellValue))
                {
                    return; // Eğer boşsa işlem yapma
                }

                DateTime yeniTarih;

                // DateTime.TryParse kullanarak geçerli tarih formatını kontrol et
                bool isValidDate = DateTime.TryParse(cellValue, out yeniTarih);

                if (!isValidDate)
                {
                    // Geçersiz tarih girildi, hata mesajı göster
                    MessageBox.Show("Lütfen geçerli bir tarih girin. Örneğin: 01/01/2024.");
                    e.Cancel = true; //İletisim kutusunun cıkmasını engelle
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 2) // Tarih sütunu (index 2)
                {
                    var cellValue = dataGridView1.Rows[e.RowIndex].Cells["Tarih"].Value;

                    // Eğer hücre boş bırakılmışsa, veritabanında tarihi null olarak güncelle
                    if (cellValue == DBNull.Value || cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
                    {
                        int satirId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ID"].Value);
                        int kacinciGelis = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["KacinciGelis"].Value);
                        string aciklama = dataGridView1.Rows[e.RowIndex].Cells["Aciklama"].Value?.ToString() ?? string.Empty;

                        // Veritabanında tarihi null olarak güncelle
                        GuncelleTarihSatir(satirId, null, kacinciGelis, aciklama);
                        return;
                    }

                    DateTime yeniTarih;
                    bool isValidDate = DateTime.TryParse(cellValue.ToString().Trim(), out yeniTarih);

                    if (isValidDate)
                    {
                        int satirId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ID"].Value);
                        int kacinciGelis = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["KacinciGelis"].Value);
                        string aciklama = dataGridView1.Rows[e.RowIndex].Cells["Aciklama"].Value?.ToString() ?? string.Empty;

                        // Geçerli tarih formatıysa veritabanını güncelle
                        GuncelleTarihSatir(satirId, yeniTarih, kacinciGelis, aciklama);
                    }
                    else
                    {
                        MessageBox.Show("Lütfen geçerli bir tarih formatı girin.");
                        dataGridView1.Rows[e.RowIndex].Cells["Tarih"].Value = DBNull.Value; // Hücreyi temizle
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("CELL END EDIT Veritabanı hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CELL END EDIT Hata: " + ex.Message);
            }
        }


        private void GuncelleTarihSatir(int satirId, DateTime? yeniTarih, int KacinciGelis, string aciklama)
        {
            try
            {
                ConnectionAndStaticTools.OpenConnection();

                string updateQuery = "UPDATE dbpaketbilgisi SET Tarih = @YeniTarih WHERE ID = @ID AND KacinciGelis = @KacinciGelis AND Aciklama = @Aciklama";

                using (MySqlCommand cmd = new MySqlCommand(updateQuery, ConnectionAndStaticTools.Connection))
                {
                    // Eğer tarih null ise, parametreyi DBNull olarak ayarla
                    if (yeniTarih.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@YeniTarih", yeniTarih.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@YeniTarih", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@ID", satirId);
                    cmd.Parameters.AddWithValue("@KacinciGelis", KacinciGelis);
                    cmd.Parameters.AddWithValue("@Aciklama", aciklama);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException sqlEx)
            {
                MessageBox.Show("GUNCELLE TARIH SATIR Veritabanı hatası: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("GUNCELLE TARIH SATIR Hata: " + ex.Message);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }
        }



        private void durumGuncelle(int satirId, string yeniDurum, int KacinciGelis, string aciklama)
        {
            try
            {
                ConnectionAndStaticTools.OpenConnection();

                // Veritabanında güncelleme sorgusu
                string updateQuery = "UPDATE dbpaketbilgisi SET Durum = @YeniDurum WHERE ID = @ID and KacinciGelis=@KacinciGelis and Aciklama=@Aciklama";
                using (MySqlCommand cmd = new MySqlCommand(updateQuery, ConnectionAndStaticTools.Connection))
                {
                    // Parametreleri ekle
                    cmd.Parameters.AddWithValue("@YeniDurum", yeniDurum);
                    cmd.Parameters.AddWithValue("@ID", satirId);
                    cmd.Parameters.AddWithValue("@KacinciGelis", KacinciGelis);
                    cmd.Parameters.AddWithValue("@Aciklama", aciklama);

                    // SQL sorgusunu çalıştır
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException sqlEx)
            {
                // Veritabanı hatası varsa yakala
                MessageBox.Show("DURUM GUNCELLE Veritabanı hatası: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                // Diğer hataları yakala
                MessageBox.Show("DURUM GUNCELLE Hata: " + ex.Message);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }
        }







        private void silToolStripMenuItem_Click(object sender, EventArgs e)
        {
            taninanPaketSil();
            taninanPaketListele(id);
            seansBilgileriniGetir(id);
            DanisanBilgileriniGoster(id);
        }

        private void btnGeri_Click(object sender, EventArgs e)
        {
            danisanBilgisiniGüncelle();
            DanisanBul danisanBul= new DanisanBul();
            danisanBul.Show();
            this.Hide();
        }

        private void label20_Click(object sender, EventArgs e)
        {
            UrunSat urunSat=new UrunSat(id);
            urunSat.Show();
            this.Hide();
        }


    }
    }

