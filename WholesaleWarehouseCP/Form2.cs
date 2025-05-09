using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;

namespace WholesaleWarehouseCP
{
    public partial class Form2 : Form
    {
        private IMongoDatabase database;
        private IMongoCollection<BsonDocument> productsCollection;
        public Form2()
        {
            InitializeComponent();
            var client = new MongoClient("mongodb://localhost:27017");
            database = client.GetDatabase("warehouse_management");
            productsCollection = database.GetCollection<BsonDocument>("products");
            LoadStockData();
        }
        private void LoadStockData()
        {
            try
            {
                var warehouseCollection = database.GetCollection<BsonDocument>("warehouse");
                var warehouseData = warehouseCollection.Find(new BsonDocument()).ToList();
                var products = productsCollection.Find(new BsonDocument()).ToList();
                var productList = new List<object>();
                foreach (var product in products)
                {
                    var name = product.Contains("name") ? product["name"].ToString() : "Невідомо";
                    var quantity = product.Contains("stock_quantity") ? product["stock_quantity"].ToInt32() : 0;
                    var productId = product["_id"].ToString();

                    var warehouseId = product.Contains("warehouse_id") ? product["warehouse_id"].ToString() : null;
                    var warehouseEntry = warehouseData.FirstOrDefault(w => w["_id"].ToString() == warehouseId);
                    var location = warehouseEntry != null && warehouseEntry.Contains("location")
                        ? warehouseEntry["location"].ToString()
                        : "Невідомий склад";

                    productList.Add(new
                    {
                        Назва = name,
                        Кількість = quantity,
                        Склад = location
                    });
                }

                dataGridView1.DataSource = productList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні даних: {ex.Message}", "Помилка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {}
        private void button2_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            form5.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form9 form9 = new Form9();
            form9.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.Show();
            this.Hide();
        }
    }
}