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
    public partial class NotOlustur : Form
    {
        
        int id;

        public NotOlustur(int id)
        {
            InitializeComponent();
            this.id = id;
            richTxtNotlar.Focus();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (richTxtNotlar.Text == "")
            {
                MessageBox.Show("Lütfen not alanını doldurunuz");
            }
            else
            {
                ConnectionAndStaticTools.OpenConnection();
                MySqlCommand mySqlCommand = new MySqlCommand("Insert into dbnotlar (ID,Notlar) values (@ID,@Notlar)", ConnectionAndStaticTools.Connection);
                mySqlCommand.Parameters.AddWithValue("@ID", id);
                mySqlCommand.Parameters.AddWithValue("@Notlar", richTxtNotlar.Text);
                mySqlCommand.ExecuteNonQuery();
                ConnectionAndStaticTools.CloseConnection();
                temizle();
                MessageBox.Show("Notunuz başarıyla kaydedildi.");
                DanisanBilgileri danisanBilgileri = new DanisanBilgileri(id);
                ConnectionAndStaticTools.danisanGetir(danisanBilgileri, id);
                this.Hide();
            }

        }

        private void AltForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            // DanisanBilgileri formunu aç
            DanisanBilgileri danisanBilgileri = new DanisanBilgileri(id);
            ConnectionAndStaticTools.danisanGetir(danisanBilgileri, id);
            this.Hide(); // Alt formu gizle
        }

        private void NotOlustur_Load(object sender, EventArgs e)
        {
            txtID.Text=id.ToString();
        }

        private void temizle()
        {
            txtID.Text = "";
            richTxtNotlar.Text = "";
        }

       
    }
}
