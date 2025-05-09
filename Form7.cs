using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WholesaleWarehouseCP
{
    public partial class Form7 : Form
    {
        private IMongoDatabase database;
        private IMongoCollection<BsonDocument> suppliersCollection;
        private IMongoCollection<BsonDocument> categoriesCollection;
        private IMongoCollection<BsonDocument> warehousesCollection;
        private IMongoCollection<BsonDocument> productsCollection;

        public Form7()
        {
            InitializeComponent();

            var client = new MongoClient("mongodb://localhost:27017");
            database = client.GetDatabase("warehouse_management");
            suppliersCollection = database.GetCollection<BsonDocument>("suppliers");
            categoriesCollection = database.GetCollection<BsonDocument>("categories");
            warehousesCollection = database.GetCollection<BsonDocument>("warehouse");
            productsCollection = database.GetCollection<BsonDocument>("products");

            LoadSuppliers();
            LoadCategories();
            LoadWarehouses();
        }

        public class SupplierItem
        {
            public BsonDocument Document { get; set; }
            public override string ToString() => Document["name"].AsString;
        }

        public class CategoryItem
        {
            public BsonDocument Document { get; set; }
            public override string ToString() => Document["name"].AsString;
        }

        public class WarehouseItem
        {
            public BsonDocument Document { get; set; }
            public override string ToString() => Document["location"].AsString;
        }

        private void LoadSuppliers()
        {
            try
            {
                var suppliers = suppliersCollection.Find(new BsonDocument()).ToList();
                List<SupplierItem> items = suppliers.Select(s => new SupplierItem { Document = s }).ToList();
                comboBox1.DataSource = items;
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
                List<CategoryItem> items = categories.Select(c => new CategoryItem { Document = c }).ToList();
                comboBox2.DataSource = items;
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
                List<WarehouseItem> items = warehouses.Select(w => new WarehouseItem { Document = w }).ToList();
                comboBox3.DataSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження складів: {ex.Message}");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form3 = new Form3();
            form3.Show();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedSupplier = comboBox1.SelectedItem as SupplierItem;
                var selectedCategory = comboBox2.SelectedItem as CategoryItem;
                var selectedWarehouse = comboBox3.SelectedItem as WarehouseItem;

                if (selectedSupplier == null || selectedCategory == null || selectedWarehouse == null)
                {
                    MessageBox.Show("Виберіть постачальника, категорію та склад");
                    return;
                }

                if (!int.TryParse(textBox2.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Введіть коректну кількість товару");
                    return;
                }

                if (!decimal.TryParse(textBox4.Text, out decimal price) || price <= 0)
                {
                    MessageBox.Show("Введіть коректну ціну товару");
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("Введіть назву товару");
                    return;
                }

                var product = new BsonDocument
        {
            { "name", textBox1.Text },
            { "barcode", textBox3.Text },
            { "price", price },
            { "stock_quantity", quantity },
            { "supplier_id", selectedSupplier.Document["_id"] },
            { "category_id", selectedCategory.Document["_id"] },
            { "warehouse_id", selectedWarehouse.Document["_id"] },
            { "date_added", DateTime.Now }
        };
                await productsCollection.InsertOneAsync(product);

                MessageBox.Show("Товар успішно додано до бази даних!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при додаванні товару: {ex.Message}");
            }
        }


        private void textBox4_TextChanged(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }
    }
}
