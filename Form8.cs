using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WholesaleWarehouseCP
{
    public partial class Form8 : Form
    {
        private IMongoDatabase database;
        private IMongoCollection<BsonDocument> productsCollection;
        private IMongoCollection<BsonDocument> suppliersCollection;
        private IMongoCollection<BsonDocument> categoriesCollection;
        private IMongoCollection<BsonDocument> warehousesCollection;

        private BsonDocument selectedProduct;
        private ObjectId productId;

        public Form8(ObjectId id)
        {
            InitializeComponent();
            productId = id;
            var client = new MongoClient("mongodb://localhost:27017");
            database = client.GetDatabase("warehouse_management");
            productsCollection = database.GetCollection<BsonDocument>("products");
            suppliersCollection = database.GetCollection<BsonDocument>("suppliers");
            categoriesCollection = database.GetCollection<BsonDocument>("categories");
            warehousesCollection = database.GetCollection<BsonDocument>("warehouse");

            LoadProductData();
            LoadSuppliers();
            LoadCategories();
            LoadWarehouses();
        }

        private void LoadProductData()
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", productId);
                selectedProduct = productsCollection.Find(filter).FirstOrDefault();

                if (selectedProduct != null)
                {
                    textBox1.Text = selectedProduct["name"].AsString;
                    textBox2.Text = selectedProduct["barcode"].AsString;
                    textBox3.Text = selectedProduct["stock_quantity"].ToString();
                    textBox4.Text = selectedProduct["price"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних товару: {ex.Message}");
            }
        }
        public class ComboBoxItem
        {
            public string DisplayText { get; set; }
            public BsonValue Id { get; set; }

            public override string ToString()
            {
                return DisplayText;
            }
        }

        private void LoadSuppliers()
        {
            try
            {
                var suppliers = suppliersCollection.Find(new BsonDocument()).ToList();

                var supplierItems = suppliers.Select(s => new ComboBoxItem
                {
                    DisplayText = s["name"].AsString,
                    Id = s["_id"]
                }).ToList();

                comboBox1.DataSource = supplierItems;
                comboBox1.DisplayMember = "DisplayText";
                comboBox1.ValueMember = "Id";

                if (selectedProduct != null && selectedProduct.Contains("supplier_id"))
                {
                    var supplierId = selectedProduct["supplier_id"];
                    comboBox1.SelectedValue = supplierId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження постачальників: {ex.Message}");
            }
        }

        private void LoadCategories()
        {
            try
            {
                var categories = categoriesCollection.Find(new BsonDocument()).ToList();

                var categoryItems = categories.Select(c => new ComboBoxItem
                {
                    DisplayText = c["name"].AsString,
                    Id = c["_id"]
                }).ToList();

                comboBox2.DataSource = categoryItems;
                comboBox2.DisplayMember = "DisplayText";
                comboBox2.ValueMember = "Id";

                if (selectedProduct != null && selectedProduct.Contains("category_id"))
                {
                    var categoryId = selectedProduct["category_id"];
                    comboBox2.SelectedValue = categoryId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження категорій: {ex.Message}");
            }
        }

        private void LoadWarehouses()
        {
            try
            {
                var warehouses = warehousesCollection.Find(new BsonDocument()).ToList();

                var warehouseItems = warehouses.Select(w => new ComboBoxItem
                {
                    DisplayText = w["location"].AsString,
                    Id = w["_id"]
                }).ToList();

                comboBox3.DataSource = warehouseItems;
                comboBox3.DisplayMember = "DisplayText";
                comboBox3.ValueMember = "Id";

                if (selectedProduct != null && selectedProduct.Contains("warehouse_id"))
                {
                    var warehouseId = selectedProduct["warehouse_id"];
                    comboBox3.SelectedValue = warehouseId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження складів: {ex.Message}");
            }
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("Введіть назву товару");
                    return;
                }
                if (!int.TryParse(textBox3.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Введіть коректну кількість товару");
                    return;
                }
                if (!decimal.TryParse(textBox4.Text, out decimal price) || price <= 0)
                {
                    MessageBox.Show("Введіть коректну ціну товару");
                    return;
                }
                if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null || comboBox3.SelectedItem == null)
                {
                    MessageBox.Show("Виберіть постачальника, категорію та склад");
                    return;
                }

                var supplierId = ((ComboBoxItem)comboBox1.SelectedItem).Id;
                var categoryId = ((ComboBoxItem)comboBox2.SelectedItem).Id;
                var warehouseId = ((ComboBoxItem)comboBox3.SelectedItem).Id;

                var filter = Builders<BsonDocument>.Filter.Eq("_id", productId);
                var update = Builders<BsonDocument>.Update
                    .Set("name", textBox1.Text)
                    .Set("barcode", textBox2.Text)
                    .Set("stock_quantity", quantity)
                    .Set("price", price)
                    .Set("supplier_id", supplierId)
                    .Set("category_id", categoryId)
                    .Set("warehouse_id", warehouseId);

                await productsCollection.UpdateOneAsync(filter, update);
                MessageBox.Show("Товар успішно оновлено!", "Успіх",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при оновленні товару: {ex.Message}", "Помилка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void label6_Click(object sender, EventArgs e)
        {
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
            this.Hide();
        }
    }
}
