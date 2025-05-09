using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using MongoDB.Driver;

namespace WholesaleWarehouseCP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.BackgroundImageLayout = ImageLayout.None; 
            this.ClientSize = this.BackgroundImage.Size;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("warehouse_management");

                var collectionsCursor = await database.ListCollectionNamesAsync();
                var collectionNames = await collectionsCursor.ToListAsync();

                if (collectionNames.Contains("products"))
                {
                    Form2 form2 = new Form2();
                    form2.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Колекція 'products' не знайдена у базі 'warehouse_db'.", "Увага");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка підключення до бази даних:\n" + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string info = "Автор: Павліха Ірина, ІПЗ-31\n" +
                  "Дата створення: 03.05.2025\n\n" +
                  "Програма призначена для обліку діяльності оптового складу:\n" +
                  "- облік товарів,\n" +
                  "- постачань і продажів,\n" +
                  "- управління залишками.";
            MessageBox.Show(info, "Про програму", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
