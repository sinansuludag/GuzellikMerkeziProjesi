using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM dbnotlar WHERE ID = @ID", ConnectionAndStaticTools.Connection);
            adapter.SelectCommand.Parameters.AddWithValue("@ID", danisanID);

            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dataGridView2.DataSource = dt;

        }



        public void seansBilgileriniGetir(int danisanID)
        {
            try
            {
                // Veritabanından verileri al
                DataTable dt = new DataTable();
                using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter("SELECT * from dbpaketbilgisi where ID=@DanisanID", ConnectionAndStaticTools.Connection))
                {
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

                //Sıralamayı onle
                dataGridView1.Columns["ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Tarih"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Seans/Kontrol"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Durum"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Tutar"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Aciklama"].SortMode = DataGridViewColumnSortMode.NotSortable;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
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
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter("SELECT * from dbnotlar where ID=@DanisanID", ConnectionAndStaticTools.Connection);
                mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@DanisanID", danisanID);

                DataTable dt = new DataTable();
                mySqlDataAdapter.Fill(dt);
                dataGridView2.DataSource = dt;
                dataGridView2.BackgroundColor = Color.White;
                dataGridView2.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 139, 139);
                //Sıralamayı kaldır
                dataGridView2.Columns["ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView2.Columns["Notlar"].SortMode = DataGridViewColumnSortMode.NotSortable;
                //Hücre genişliği 
                dataGridView2.Columns["ID"].Width = 50;
                dataGridView2.Columns["Notlar"].Width = 600;
            

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        public void alinanOdemeBilgisiniGetir(int danisanID)
        {
            try
            {
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter("SELECT * from dbalinanodemeler where ID=@DanisanID", ConnectionAndStaticTools.Connection);
                mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@DanisanID", danisanID);

                DataTable dt = new DataTable();
                mySqlDataAdapter.Fill(dt);
                dataGridView3.DataSource = dt;
                dataGridView3.BackgroundColor = Color.White;
                dataGridView3.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 139, 139);
                dataGridView3.Columns["ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView3.Columns["Tarih"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView3.Columns["Aciklama"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView3.Columns["OdemeTipi"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView3.Columns["Tutar"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView3.Columns["ID"].Width = 50;
                dataGridView3.Columns["Tarih"].Width = 90;
                dataGridView3.Columns["Aciklama"].Width = 500;
                dataGridView3.Columns["OdemeTipi"].Width = 90;
                dataGridView3.Columns["Tutar"].Width = 60;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        

        public void DanisanBilgileriniGoster(int danisanId)
        {
            ConnectionAndStaticTools.OpenConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM dbdanisankayit WHERE DanisanID = @DanisanId", ConnectionAndStaticTools.Connection);
            cmd.Parameters.AddWithValue("@DanisanId", danisanId);

            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                txtId.Text = reader["DanisanID"].ToString();
                txtAdi.Text = reader["Adi"].ToString();
                txtSoyadi.Text = reader["Soyadi"].ToString();
                txtTel.Text = reader["Telefon"].ToString();
                txtCinsiyet.Text = reader["Cinsiyet"].ToString();
                txtReferans.Text = reader["Referans"].ToString();
                string[] diziUcret = reader["İslem"].ToString().Split(':');
                float toplam = 0;
                string[] diziUcret2;
                float ucret = 0;
                foreach (string d in diziUcret)
                {
                    diziUcret2 = d.Split(',');
                    if (diziUcret2.Length == 3)
                    {
                        ucret = float.Parse(diziUcret2[2]);
                        toplam += ucret;
                    }
                    else
                    {

                    }
                    
                }
                txtTopTutar.Text = toplam.ToString();

                ConnectionAndStaticTools.CloseConnection();
                try
                {

                    float alOdeme = alinanOdemeTutari(danisanId);
                    if (toplam - alOdeme == 0)
                    {

                        txtKalanOdeme.Text = "0";
                        txtAlOdeme.Text = alOdeme.ToString();
                    }
                    else if (toplam - alOdeme < 0)
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
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.StackTrace);
                }


            }


        }

        public float alinanOdemeTutari(int danisanID)
        {
            float toplam = 0f;
            try
            {
                ConnectionAndStaticTools.OpenConnection();
                using (MySqlCommand mySqlCommand = new MySqlCommand("SELECT * FROM dbalinanodemeler WHERE ID = @DanisanId", ConnectionAndStaticTools.Connection))
                {
                    mySqlCommand.Parameters.AddWithValue("@DanisanId", danisanID);

                    using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            toplam += float.Parse(reader["Tutar"].ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Hata:" + e.StackTrace);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
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
                    ConnectionAndStaticTools.OpenConnection();
                    MySqlCommand cmd = new MySqlCommand("Update dbdanisankayit set Adi=@Adi,Soyadi=@Soyadi,Telefon=@Telefon,Cinsiyet=@Cinsiyet," +
                        "Referans=@Referans where DanisanID=@ID", ConnectionAndStaticTools.Connection);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Adi", txtAdi.Text);
                    cmd.Parameters.AddWithValue("@Soyadi", txtSoyadi.Text);
                    cmd.Parameters.AddWithValue("@Telefon", txtTel.Text);
                    cmd.Parameters.AddWithValue("@Cinsiyet", txtCinsiyet.Text);
                    cmd.Parameters.AddWithValue("@Referans", txtReferans.Text);
                    cmd.ExecuteNonQuery();


            }catch(Exception e)
            {
                MessageBox.Show("Hata:" + e.Message);
            }
            finally 
            { 
               ConnectionAndStaticTools.CloseConnection();
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {// Kullanıcıya silme işlemi için onay mesajı göster
            DialogResult result = MessageBox.Show("Danışana ait tüm verileri kalıcı olarak silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Kullanıcının seçimine göre işlemi gerçekleştir
            if (result == DialogResult.Yes)
            {
                try
                {
                    ConnectionAndStaticTools.OpenConnection();
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM dbdanisankayit WHERE DanisanID = @ID", ConnectionAndStaticTools.Connection);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                    temizle();
                    MessageBox.Show("Kayıt başarıyla silindi.");
                    Anasayfa anasayfa = new Anasayfa();
                    anasayfa.Show();
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
            else
            {
                // Kullanıcı "Hayır" seçeneğini seçerse burada yapılacak işlemler
                MessageBox.Show("Silme işlemi iptal edildi.");
            }

        }

        public void taninanPaketListele(int danismaID)
        {
            cbTaninanPaket.Items.Clear();
            ConnectionAndStaticTools.OpenConnection();
            MySqlCommand cmd = new MySqlCommand("Select İslem from dbdanisankayit where DanisanID=@ID ORDER BY İslem ASC", ConnectionAndStaticTools.Connection);
            cmd.Parameters.AddWithValue("@ID", danismaID);

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
                            if (dizi2[0] != "")
                            {
                                cbTaninanPaket.Items.Add(dizi2[0]);
                            }
                            
                        }
                    }
                }
            }

            ConnectionAndStaticTools.CloseConnection(); // using bloğunun dışında bağlantıyı kapat
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
            if (cbTaninanPaket.SelectedItem != null)
            {
                string selectedValue = cbTaninanPaket.SelectedItem.ToString();
                cbTaninanPaket.Items.Clear();
                
                

                ConnectionAndStaticTools.OpenConnection();
                MySqlCommand cmd = new MySqlCommand("Select İslem from dbdanisankayit where DanisanID=@ID ", ConnectionAndStaticTools.Connection);
                cmd.Parameters.AddWithValue("@ID", id);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] dizi1 = reader["İslem"].ToString().Split(':');
                        string[] dizi2;
                        for (int i = 0; i < dizi1.Length; i++)
                        {
                            dizi2 = dizi1[i].Split(',');
                            if (selectedValue == dizi2[0])
                            {
                                silinenList.Add(dizi1[i]);
                                dizidenElemanSilme(ref dizi1, dizi1[i]);
                                foreach (var item in dizi1)
                                {
                                    diziList.Add(item);
                                }
                            }
                        }
                    }
                }

                ConnectionAndStaticTools.CloseConnection();

                
                taninanPaketguncelle();
                paketBilgisiGuncelle(id);
            }
            else
            {
                MessageBox.Show("Lütfen bir öğe seçin.");
            }
        }

        public void taninanPaketguncelle()
        {
            try
            {
                ConnectionAndStaticTools.OpenConnection();
                string[] dizi= diziList.ToArray();
                MySqlCommand cmd = new MySqlCommand("Update dbdanisankayit set İslem=@İslem where DanisanID=@ID",ConnectionAndStaticTools.Connection);
                cmd.Parameters.AddWithValue("@ID", id);
                string birlestirilmisDizi=string.Join(":", dizi);
                cmd.Parameters.AddWithValue("@İslem", birlestirilmisDizi);
                int etk=cmd.ExecuteNonQuery();
                if (etk > 0)
                {
                    MessageBox.Show("Paket başarıyla silinmiştir");
                }
                cbTaninanPaket.Text = "";

            }
            catch (Exception ex)
            {
              MessageBox.Show("Hata:"+ex.Message);
            }
            finally
            { 
                ConnectionAndStaticTools.CloseConnection();
            }
        }

        public void paketBilgisiGuncelle(int danisanId)
        {
            ConnectionAndStaticTools.OpenConnection();
            try
            {

                string[] dizi = silinenList.ToArray();


                if (dizi.Length > 0)
                {
                    string[] sonIslem;
                    int sayac = 1;
                    foreach (string paketDizi in dizi)
                    {

                        sonIslem = paketDizi.Split(',');

                        // sonIslem dizisinin boyutunu kontrol et
                        if (sonIslem.Length != 3)
                        {
                            // Hata durumunu işle
                            MessageBox.Show("Paket dizisi beklenen öğe sayısına sahip değil.");
                            continue; // Devam et ve bir sonraki pakete geç
                        }
                        string islem = sonIslem[0];
                        int seans = Convert.ToInt32(sonIslem[1]);
                        for (int i = 1; i <= seans; i++)
                        { 

                            if (i == seans)
                            {
                                sayac = 1;
                            }
                            sayac++;

                            MySqlCommand cmdPaket = new MySqlCommand("Delete from dbpaketbilgisi where ID=@ID and Aciklama=@Aciklama", ConnectionAndStaticTools.Connection);
                            cmdPaket.Parameters.AddWithValue("@ID", danisanId);
                            cmdPaket.Parameters.AddWithValue("@Aciklama", $"{islem}{i}");
                            
                            cmdPaket.ExecuteNonQuery();


                        }

                    }
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
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter("SELECT * from dbnotlar where ID=@DanisanID", ConnectionAndStaticTools.Connection);
                mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@DanisanID", id);

                DataTable dt = new DataTable();
                mySqlDataAdapter.Fill(dt);
                dataGridView2.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void alinanOdemelerListesiGetir()
        {
            try
            {
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter("SELECT * from dbalinanodemeler where ID=@DanisanID", ConnectionAndStaticTools.Connection);
                mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@DanisanID", id);

                DataTable dt = new DataTable();
                mySqlDataAdapter.Fill(dt);
                dataGridView3.DataSource = dt;
                dataGridView3.Columns["Aciklama"].ReadOnly = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void lblYeniPaketTanimla_Click(object sender, EventArgs e)
        {
            YeniPaketTanimlama yeniPaketTanimlama = new YeniPaketTanimlama(id);
            yeniPaketTanimlama.Show();
            this.Hide();
        }

        private void dataGridView1_CellEndEdit_1(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == 2) // tarihSutunuIndex: Tarih sütununun indeksi
            {
                // Hücre değerinin boş olup olmadığını kontrol et
                object cellValue = dataGridView1.Rows[e.RowIndex].Cells[2].Value;
                if (cellValue != DBNull.Value && cellValue != null)
                {
                    // Tarih sütunu değiştiğinde, ilgili satırı güncelle
                    DateTime yeniTarih = Convert.ToDateTime(cellValue);
                    int satirId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ID"].Value);
                    int kacinciGelis = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["KacinciGelis"].Value);
                    string aciklama = dataGridView1.Rows[e.RowIndex].Cells["Aciklama"].Value.ToString();
                    string yeniDurum = dataGridView1.Rows[e.RowIndex].Cells["Durum"].Value.ToString();
                    GuncelleTarihSatir(satirId, yeniTarih, kacinciGelis, aciklama);
                    // durumGuncelle(satirId,yeniDurum, kacinciGelis, aciklama);
                }
                else
                {
                    // Hücre değeri boş ise, gerekli işlemi yapabilir veya kullanıcıya bir uyarı gösterebilirsiniz
                    MessageBox.Show("Hücre değeri boş olduğu için kaydedilmedi!");
                }
            }
            else if (e.ColumnIndex == 5)
            {
                int kacinciGelisSayac = 1000;

                object cellValue = dataGridView1.Rows[e.RowIndex].Cells[1].Value;
                int gelis;
                if (cellValue != DBNull.Value && cellValue != null && !string.IsNullOrEmpty(cellValue.ToString()))
                {
                    gelis = Convert.ToInt32(cellValue);
                }
                else
                {
                    gelis = kacinciGelisSayac;
                }

                int kacinciGelis = (gelis != null) ? gelis : kacinciGelisSayac;
                object cellValue2 = dataGridView1.Rows[e.RowIndex].Cells[4].Value;
                string aciklama;
                if (cellValue2 != DBNull.Value && cellValue2 != null)
                {
                     aciklama = dataGridView1.Rows[e.RowIndex].Cells["Aciklama"].Value.ToString();
                    string yeniDurum = dataGridView1.Rows[e.RowIndex].Cells["Durum"].Value.ToString();
                    durumGuncelle(id, yeniDurum, kacinciGelis, aciklama);
                }
                else
                {
                    MessageBox.Show("Hücre değeri boş olduğu için kaydedilmedi!");
                }
                
                    
                }
                else
                {
                    // Hücre değeri boş ise, gerekli işlemi yapabilir veya kullanıcıya bir uyarı gösterebilirsiniz
                    MessageBox.Show("Hücre değeri boş olduğu için kaydedilmedi!");
                }
            }

       

        private void GuncelleTarihSatir(int satirId, DateTime yeniTarih, int KacinciGelis, string aciklama)
        {
            try
            {
                ConnectionAndStaticTools.OpenConnection();


                // Veritabanında güncelleme sorgusu
                string updateQuery = "UPDATE dbpaketbilgisi SET Tarih = @YeniTarih WHERE ID = @ID and KacinciGelis=@KacinciGelis and Aciklama=@Aciklama";
                MySqlCommand cmd = new MySqlCommand(updateQuery, ConnectionAndStaticTools.Connection);
                cmd.Parameters.AddWithValue("@YeniTarih", yeniTarih);
                cmd.Parameters.AddWithValue("@ID", satirId);
                cmd.Parameters.AddWithValue("@KacinciGelis", KacinciGelis);
                cmd.Parameters.AddWithValue("@Aciklama", aciklama);

                cmd.ExecuteNonQuery();

                ConnectionAndStaticTools.CloseConnection();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void durumGuncelle(int satirId, string yeniDurum, int KacinciGelis, string aciklama)
        {
            try
            {
                ConnectionAndStaticTools.OpenConnection();

                // Veritabanında güncelleme sorgusu
                string updateQuery = "UPDATE dbpaketbilgisi SET Durum = @YeniDurum WHERE ID = @ID and KacinciGelis=@KacinciGelis and Aciklama=@Aciklama";
                MySqlCommand cmd = new MySqlCommand(updateQuery, ConnectionAndStaticTools.Connection);
                cmd.Parameters.AddWithValue("@YeniDurum", yeniDurum);
                cmd.Parameters.AddWithValue("@ID", satirId);
                cmd.Parameters.AddWithValue("@KacinciGelis", KacinciGelis);
                cmd.Parameters.AddWithValue("@Aciklama", aciklama);
                
                cmd.ExecuteNonQuery();
                
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

        private void düzenleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TutarGuncelle tutarGuncelle = new TutarGuncelle(id);
            tutarGuncelle.Show();  
            this.Hide();
        }
    }
    }

