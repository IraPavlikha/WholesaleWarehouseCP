using System;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WholesaleWarehouseCP
{
    public partial class Form5 : Form
    {
        private IMongoDatabase database;
        private IMongoCollection<BsonDocument> ordersCollection;
        private IMongoCollection<BsonDocument> customersCollection;
        private IMongoCollection<BsonDocument> productsCollection;

        public Form5()
        {
            InitializeComponent();

            var client = new MongoClient("mongodb://localhost:27017");
            database = client.GetDatabase("warehouse_management");
            ordersCollection = database.GetCollection<BsonDocument>("orders");
            customersCollection = database.GetCollection<BsonDocument>("customers");
            productsCollection = database.GetCollection<BsonDocument>("products");

            LoadOrders();
        }

        public void LoadOrders()
        {
            try
            {
                var pipeline = new[] {
                    BsonDocument.Parse(@"{
                        $lookup: {
                            from: 'customers',
                            localField: 'customer_id',
                            foreignField: '_id',
                            as: 'customer_info'
                        }
                    }"),
                    BsonDocument.Parse(@"{
                        $project: {
                            _id: 1,
                            date: 1,
                            total: 1,
                            status: 1,
                            product_name: 1,
                            quantity: 1,
                            customer_name: { $arrayElemAt: ['$customer_info.name', 0] }
                        }
                    }")
                };

                var orders = ordersCollection.Aggregate<BsonDocument>(pipeline).ToList();

                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.Columns.Clear();

                dataGridView1.Columns.Add("_id", "ID");
                dataGridView1.Columns.Add("date", "Дата");
                dataGridView1.Columns.Add("customer_name", "Клієнт");
                dataGridView1.Columns.Add("product_name", "Товар");
                dataGridView1.Columns.Add("quantity", "Кількість");
                dataGridView1.Columns.Add("total", "Сума");
                dataGridView1.Columns.Add("status", "Статус");

                dataGridView1.Rows.Clear();
                foreach (var order in orders)
                {
                    string id = order.GetValue("_id", BsonNull.Value).ToString();

                    string date = "N/A";
                    if (order.Contains("date") && order["date"].IsValidDateTime)
                        date = order["date"].ToLocalTime().ToString("dd.MM.yyyy HH:mm");

                    string customerName = order.Contains("customer_name") ? order["customer_name"].ToString() : "Невідомо";
                    string productName = order.Contains("product_name") ? order["product_name"].ToString() : "—";
                    string quantity = order.Contains("quantity") ? order["quantity"].ToString() : "—";
                    string total = order.Contains("total") ? order["total"].ToString() : "0";
                    string status = order.Contains("status") ? order["status"].ToString() : "—";

                    dataGridView1.Rows.Add(id, date, customerName, productName, quantity, total, status);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні замовлень: {ex.Message}", "Помилка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form6 = new Form6();
            form6.FormClosed += (s, args) => LoadOrders();
            form6.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Виберіть замовлення для видалення", "Попередження",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedId = dataGridView1.SelectedRows[0].Cells["_id"].Value.ToString();

            if (MessageBox.Show("Ви впевнені, що хочете видалити це замовлення?", "Підтвердження",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(selectedId));
                    await ordersCollection.DeleteOneAsync(filter);
                    LoadOrders();
                    MessageBox.Show("Замовлення успішно видалено", "Успіх",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при видаленні: {ex.Message}", "Помилка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new Form2();
            form2.Show();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e) { }
    }
}