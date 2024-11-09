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
    public partial class Kasa : Form
    {
       
        public Kasa()
        {
            InitializeComponent();
        }

        private void Kasa_Load(object sender, EventArgs e)
        {
            
            raporListesiniGetir();
            toplamOdemeGetir();
        }

        private void AltForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; // Formun kapatılmasını engelleyin
            Anasayfa anasayfa = new Anasayfa();
            anasayfa.Show();
            this.Hide(); // Alt formu gizleyin veya kapatın
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // SQL sorgusunu oluştur
                string query = "SELECT * FROM dbrapor WHERE Tarih BETWEEN @BaslangicTarihi AND @BitisTarihi";

                // MySqlDataAdapter nesnesini oluştur ve sorguyu belirt
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(query, ConnectionAndStaticTools.Connection);

                // Parametreleri ekleyerek filtreleme yap
                mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@BaslangicTarihi", DateTime.Parse(basDataTimePic.Text));
                mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@BitisTarihi", DateTime.Parse(bitisDataTimePic.Text));

                // Verileri al ve bir DataTable'e doldur
                DataTable dt = new DataTable();
                mySqlDataAdapter.Fill(dt);

                // Verileri DataGridView'e aktar
                dataGridView1.DataSource = dt;
                toplamOdemeGetir();
                // Eğer kayıt yoksa kullanıcıya bilgi ver
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Belirtilen tarih aralığında kayıt bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(MySqlException sqlEx)
            {

                MessageBox.Show("KASA BUTON Veritabanı Hatası: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("KASA BUTON Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toplamOdemeGetir()
        {
            float toplam = 0;

            try
            {
                ConnectionAndStaticTools.OpenConnection();
                string query = "SELECT Tutar FROM dbrapor WHERE Tarih BETWEEN @BaslangicTarihi AND @BitisTarihi";

                using (MySqlCommand mySqlCommand = new MySqlCommand(query, ConnectionAndStaticTools.Connection))
                {
                    // Parametreleri ekle
                    mySqlCommand.Parameters.AddWithValue("@BaslangicTarihi", DateTime.Parse(basDataTimePic.Text));
                    mySqlCommand.Parameters.AddWithValue("@BitisTarihi", DateTime.Parse(bitisDataTimePic.Text));

                    using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Tutar değerini kontrol et ve toplam tutara ekle
                            if (float.TryParse(reader["Tutar"]?.ToString(), out float tutar))
                            {
                                toplam += tutar;
                            }
                        }
                    }
                }

                // Toplam tutarı TextBox'a yazdır
                txtTutar.Text = toplam.ToString();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("KASA TOPLAM ODEME GETIR Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("KASA TOPLAM ODEME GETIR Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }
        }


        private void raporListesiniGetir()
        {
            try
            {
                // Bağlantıyı aç
                ConnectionAndStaticTools.OpenConnection();

                // SQL sorgusunu oluştur
                string query = "SELECT * FROM dbrapor WHERE Tarih BETWEEN @BaslangicTarihi AND @BitisTarihi";

                // MySqlDataAdapter nesnesini oluştur ve sorguyu belirt
                using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(query, ConnectionAndStaticTools.Connection))
                {
                    // Parametreleri ekle
                    mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@BaslangicTarihi", DateTime.Parse(basDataTimePic.Text));
                    mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@BitisTarihi", DateTime.Parse(bitisDataTimePic.Text));

                    // Verileri al ve bir DataTable'e doldur
                    DataTable dt = new DataTable();
                    mySqlDataAdapter.Fill(dt);

                    // DataGridView'i güncelle
                    dataGridView1.DataSource = dt;
                    dataGridView1.BackgroundColor = Color.White;
                    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 139, 139);

                    // Kolon sıralamasını devre dışı bırak
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }

                    // Eğer veri yoksa kullanıcıya bilgi ver
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Belirtilen tarih aralığında kayıt bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("KASA RAPOR LISTESINI GETIR Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("KASA RAPOR LISTESINI GETIR hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Bağlantıyı kapat
                ConnectionAndStaticTools.CloseConnection();
            }
        }


        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            Anasayfa anasayfa = new Anasayfa();
            anasayfa.Show();
            this.Hide();
        }
    }
}
