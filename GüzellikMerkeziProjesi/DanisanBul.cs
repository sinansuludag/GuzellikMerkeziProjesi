using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GüzellikMerkeziProjesi
{
    public partial class DanisanBul : Form
    {
       
        public DanisanBul()
        {
            InitializeComponent();
            txtBul.Focus();
        }

        private void DanisanBul_Load(object sender, EventArgs e)
        {
            int kayitSayisi = KayitSayisiGetir();
            lblKayitSayisi.Text = kayitSayisi.ToString();
            kisiListele();
        }


        DataTable dt = new DataTable();
        private void kisiListele()
        {
            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM dbdanisankayit ORDER BY Adi ASC;", ConnectionAndStaticTools.Connection);
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            dataGridView1.Columns["İslem"].Visible = false;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 139 ,139);
            dataGridView1.BackgroundColor = Color.White;
            //Sıralamayı kaldır
            dataGridView1.Columns["DanisanID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Adi"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Soyadi"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Telefon"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Referans"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Cinsiyet"].SortMode = DataGridViewColumnSortMode.NotSortable;

        }

        
        private int KayitSayisiGetir()
        {
            int kayitSayisi = 0;
            try
            {
                ConnectionAndStaticTools.OpenConnection(); // Bağlantıyı açma işlemi burada gerçekleştiriliyor olmalıdır.

                // Veritabanından kayıt sayısını almak için gerekli sorguyu hazırlayın
                string query = "SELECT COUNT(*) FROM dbdanisankayit"; 
                // Veritabanı sorgusunu çalıştırarak kayıt sayısını alın
                MySqlCommand command = new MySqlCommand(query, ConnectionAndStaticTools.Connection);
                kayitSayisi = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata:"+ex.Message);
            }
            finally
            {
                ConnectionAndStaticTools.CloseConnection();
            }

            return kayitSayisi;
        }

        DataView filtrele()
        {
            DataView dv = new DataView();
            dv = dt.DefaultView;

            // Arama metnini parçalayarak boşlukları kontrol edin
            string[] searchTerms = txtBul.Text.Split(' ');

            if (searchTerms.Length == 1)
            {
                // Sadece bir isim girildiyse, adı filtreleyin
                dv.RowFilter = $"Adi LIKE '{searchTerms[0]}%' OR Soyadi LIKE '{searchTerms[0]}%'";
            }
            else if (searchTerms.Length > 1)
            {
                // Hem isim hem soyad girildiyse, her iki alanı da filtreleyin
                dv.RowFilter = $"Adi LIKE '{searchTerms[0]}%' AND Soyadi LIKE '{searchTerms[1]}%'";
            }

            return dv;
        }

        private void btnAnasayfa_Click_1(object sender, EventArgs e)
        {
            Anasayfa anasayfa = new Anasayfa();
            anasayfa.Show();
            this.Hide();
        }

        private void AltForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; // Formun kapatılmasını engelleyin
            Anasayfa anasayfa = new Anasayfa();
            anasayfa.Show();
            this.Hide(); // Alt formu gizleyin veya kapatın
        }


        private void txtBul_TextChanged_1(object sender, EventArgs e)
        {
            filtrele();
        }

        private void dataGridView1_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int rowIndex = e.RowIndex;

                // Geçerli satırın DataGridViewRow nesnesini alın
                DataGridViewRow selectedRow = dataGridView1.Rows[rowIndex];

                // Eğer seçili satır null değilse, işlemleri gerçekleştirin
                if (selectedRow != null)
                {
                    // Danışanın kimlik bilgisini alın (örneğin, birinci sütundaki hücreden)
                    object danisanIdObj = selectedRow.Cells["DanisanID"].Value;

                    // Hücre değeri DBNull ise, hatayı önlemek için null kontrolü yapın
                    if (danisanIdObj != DBNull.Value)
                    {
                        int danisanId = Convert.ToInt32(danisanIdObj);

                        // Burada danışanın detaylarını gösteren bir form açabilir ve danışanId'yi kullanarak gerekli verileri çekebilirsiniz
                        DanisanBilgileri danisanBilgileri = new DanisanBilgileri(danisanId);
                        ConnectionAndStaticTools.danisanGetir(danisanBilgileri, danisanId);
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Seçili satırın kimlik bilgisi yok. Lütfen başka bir satırı seçin.");
                    }
                }
                else
                {
                    MessageBox.Show("Satır boş. Bir satırı çift tıklamadan önce lütfen bir satırı seçin.");
                }
            }
        }
    }
}
