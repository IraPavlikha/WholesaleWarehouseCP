using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WholesaleWarehouseCP
{
    public partial class Form6 : Form
    {
        private IMongoDatabase database;
        private IMongoCollection<BsonDocument> customersCollection;
        private IMongoCollection<BsonDocument> productsCollection;
        private IMongoCollection<BsonDocument> ordersCollection;

        private Dictionary<string, BsonDocument> productsDictionary = new Dictionary<string, BsonDocument>();

        public Form6()
        {
            InitializeComponent();

            var client = new MongoClient("mongodb://localhost:27017");
            database = client.GetDatabase("warehouse_management");
            customersCollection = database.GetCollection<BsonDocument>("customers");
            productsCollection = database.GetCollection<BsonDocument>("products");
            ordersCollection = database.GetCollection<BsonDocument>("orders");

            LoadCustomers();
            LoadProducts();
        }
        public class CustomerItem
        {
            public BsonDocument Document { get; set; }
            public override string ToString() => Document["name"].AsString;
        }
        public class ProductItem
        {
            public BsonDocument Document { get; set; }
            public override string ToString() => Document["name"].AsString;
        }
        private void LoadCustomers()
        {
            try
            {
                var customers = customersCollection.Find(new BsonDocument()).ToList();
                List<CustomerItem> items = customers.Select(c => new CustomerItem { Document = c }).ToList();

                comboBox1.DataSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження клієнтів: {ex.Message}");
            }
        }
        private void LoadProducts()
        {
            try
            {
                var products = productsCollection.Find(new BsonDocument()).ToList();

                productsDictionary.Clear();
                List<ProductItem> items = new List<ProductItem>();

                foreach (var product in products)
                {
                    string name = product["name"].AsString;
                    productsDictionary[name] = product;
                    items.Add(new ProductItem { Document = product });
                }

                comboBox2.DataSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження товарів: {ex.Message}");
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            var selectedCustomer = comboBox1.SelectedItem as CustomerItem;
            var selectedProduct = comboBox2.SelectedItem as ProductItem;

            if (selectedCustomer == null || selectedProduct == null)
            {
                MessageBox.Show("Виберіть клієнта та товар");
                return;
            }
            try
            {
                var customer = selectedCustomer.Document;
                var product = selectedProduct.Document;
                int quantity = int.TryParse(textBox1.Text, out int parsedQuantity) ? parsedQuantity : 1; 
                decimal price = product["price"].ToDecimal();
                decimal total = price * quantity;

                var order = new BsonDocument
                {
                    { "customer_id", customer["_id"] },
                    { "product_id", product["_id"] },
                    { "product_name", product["name"] },
                    { "price", price },
                    { "quantity", quantity },
                    { "total", total },
                    { "date", DateTime.Now },
                    { "status", "new" }
                };

                await ordersCollection.InsertOneAsync(order);
                var form5 = Application.OpenForms.OfType<Form5>().FirstOrDefault();
                if (form5 != null)
                {
                    form5.LoadOrders(); 
                }
                MessageBox.Show($"Замовлення створено!\nТовар: {product["name"]}\nКількість: {quantity}\nСума: {total} грн");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form5 = new Form5();
            form5.Show();
        }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}