using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WholesaleWarehouseCP
{
    public partial class Form3 : Form
    {
        private IMongoCollection<BsonDocument> productsCollection;
        private IMongoCollection<BsonDocument> warehouseCollection;
        private IMongoDatabase database;
        public Form3()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            database = client.GetDatabase("warehouse_management");
            productsCollection = database.GetCollection<BsonDocument>("products");
            warehouseCollection = database.GetCollection<BsonDocument>("warehouse");
            InitializeComponent();
            LoadStockData();
        }
        private void LoadStockData()
        {
            try
            {
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("warehouse_management");
                var warehouseCollection = database.GetCollection<BsonDocument>("warehouse");
                var productsCollection = database.GetCollection<BsonDocument>("products");
                var suppliersCollection = database.GetCollection<BsonDocument>("suppliers");

                var products = productsCollection.Find(new BsonDocument()).ToList();
                var warehouseData = warehouseCollection.Find(new BsonDocument()).ToList();
                var suppliersData = suppliersCollection.Find(new BsonDocument()).ToList();

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

                    var supplierId = product.Contains("supplier_id") ? product["supplier_id"].ToInt32() : 0;
                    var supplier = suppliersData.FirstOrDefault(s => s["_id"].ToInt32() == supplierId);
                    var supplierName = supplier != null && supplier.Contains("name") ? supplier["name"].ToString() : "Невідомо";

                    productList.Add(new
                    {
                        _id = productId,
                        Назва = name,
                        Кількість = quantity,
                        Склад = location,
                        Постачальник = supplierName
                    });
                }

                dataGridView1.DataSource = productList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.Show();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Form9 form9 = new Form9();
            form9.Show();

        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Виберіть товар для редагування", "Попередження",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedProduct = (dynamic)dataGridView1.SelectedRows[0].DataBoundItem;
            var selectedId = ObjectId.Parse(selectedProduct._id.ToString());

            Form8 form8 = new Form8(selectedId);
            form8.Show();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Виберіть товар для видалення", "Попередження",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedProduct = (dynamic)dataGridView1.SelectedRows[0].DataBoundItem;
            var selectedId = selectedProduct._id.ToString();  

            if (MessageBox.Show("Ви впевнені, що хочете видалити цей товар?", "Підтвердження",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(selectedId));
                    await productsCollection.DeleteOneAsync(filter);
                    LoadStockData();

                    MessageBox.Show("Товар успішно видалено", "Успіх",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при видаленні: {ex.Message}", "Помилка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
        }
    }
}
