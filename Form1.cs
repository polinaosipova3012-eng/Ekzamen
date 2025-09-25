using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Экзамен
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void партнёрыBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.партнёрыBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.пробный_ЭкзаменDataSet);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "пробный_ЭкзаменDataSet2.Партнёры". При необходимости она может быть перемещена или удалена.
            this.партнёрыTableAdapter1.Fill(this.пробный_ЭкзаменDataSet2.Партнёры);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "пробный_ЭкзаменDataSet.Партнёры". При необходимости она может быть перемещена или удалена.
            this.партнёрыTableAdapter.Fill(this.пробный_ЭкзаменDataSet.Партнёры);

        }

        private void PartnersDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv == null) return;

            // Получаем имя столбца
            string columnName = dgv.Columns[e.ColumnIndex].Name;

            string input = e.FormattedValue?.ToString().Trim();

            // Валидация для числовых полей: например, "Телефон", "ИНН"
            if (columnName == "Телефон" || columnName == "ИНН")
            {
                if (string.IsNullOrEmpty(input))
                    return; // пустое значение допустимо

                // Убираем возможные запятые/точки (если пользователь ввёл "10,00")
                string cleanInput = input.Replace(",", "").Replace(".", "").Replace(" ", "");

                // Проверяем, состоит ли строка только из цифр
                if (!IsDigitsOnly(cleanInput))
                {
                    MessageBox.Show($"Поле '{columnName}' должно содержать только цифры.", "Ошибка ввода",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // Отменяем ввод
                }
                else
                {
                    // Обновляем значение без лишних символов
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cleanInput;
                }
            }
        }

        // Вспомогательный метод: проверяет, состоит ли строка только из цифр
        private bool IsDigitsOnly(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            // Добавление новой записи — ничего особенного не нужно, BindingSource сам создаст строку
            // Но можно добавить логику, если нужно (например, установить дату)
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            // Подтверждение удаления (опционально)
            if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Удаление происходит автоматически через BindingSource
            }
        }

        private void партнёрыDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Завершаем редактирование
                this.Validate();
                this.партнёрыBindingSource.EndEdit();

                // Проверяем все строки на корректность перед сохранением
                foreach (DataRow row in this.пробный_ЭкзаменDataSet.Партнёры.Rows)
                {
                    if (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added)
                    {
                        // Проверка Телефон
                        if (row["Телефон"] != DBNull.Value && !string.IsNullOrEmpty(row["Телефон"].ToString()))
                        {
                            string phone = row["Телефон"].ToString().Trim();
                            if (!IsDigitsOnly(phone))
                            {
                                throw new Exception($"Некорректное значение в поле 'Телефон': {phone}");
                            }
                        }

                        // Проверка ИНН
                        if (row["ИНН"] != DBNull.Value && !string.IsNullOrEmpty(row["ИНН"].ToString()))
                        {
                            string inn = row["ИНН"].ToString().Trim();
                            if (!IsDigitsOnly(inn))
                            {
                                throw new Exception($"Некорректное значение в поле 'ИНН': {inn}");
                            }
                        }
                    }
                }

                // Сохраняем изменения в БД
                this.tableAdapterManager.UpdateAll(this.пробный_ЭкзаменDataSet);

                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении:\n{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
