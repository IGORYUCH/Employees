using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Configuration;

namespace Employees
{
    public partial class MainWindow : Window
    {
        string getStatusNames = "get_status_names";
        string getPersons = "get_persons";
        string getPersonsCount = "get_persons_count";
        string connectionString = "";
        string dateOutputFormat = "";
        string dateSqlFormat = "";
        public MainWindow()
        {
            connectionString = ConfigurationManager.AppSettings["connectionString"];
            dateOutputFormat = ConfigurationManager.AppSettings["dateOutputFormat"];
            dateSqlFormat = ConfigurationManager.AppSettings["dateSqlFormat"];

            InitializeComponent();
            FillStatusCombo();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PersonsListDataGrid.Items.Clear();

            string status = StatusBox.Text;
            string post = PostBox.Text;
            string department = DepartmentBox.Text;
            string lastName = LastNameBox.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(getPersons, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("status", status);
                command.Parameters.AddWithValue("post", post);
                command.Parameters.AddWithValue("department", department);
                command.Parameters.AddWithValue("last_name", lastName);

                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string firstNameData = reader.GetString(0);
                        string secondNameData = reader.GetString(1);
                        string lastNameData = reader.GetString(2);
                        string fullNameData = firstNameData + " " + secondNameData[0] + " " + lastNameData[0];
                        string statusData = reader.GetString(3);
                        string postData = reader.GetString(4);
                        string departmentData = reader.GetString(5);
                        string employeeDateDataString = "";
                        string unemployeeDateDataString = "";
                        if (!reader.IsDBNull(6))
                        {
                            DateTime employeeDateData = reader.GetDateTime(6);
                            employeeDateDataString = employeeDateData.ToString(dateOutputFormat);
                        }
                        if (!reader.IsDBNull(7))
                        {
                            DateTime unemployeeDateData = reader.GetDateTime(7);
                            unemployeeDateDataString = unemployeeDateData.ToString(dateOutputFormat);
                        }
                        PersonsListDataGrid.Items.Add(new { full_name = fullNameData, status = statusData, post = postData, department = departmentData, employee_date = employeeDateDataString, fire_date = unemployeeDateDataString });
                    }
                }
                else
                {
                    PersonsListDataGrid.Items.Add(new { full_name = "(ничего не найдено)" });
                }
            }
        }

        private void FillStatusCombo()
        {
            int counter = 0;
            ComboBoxItem defaultStatusItem = new ComboBoxItem();
            defaultStatusItem.Name = "item" + counter.ToString();
            defaultStatusItem.Content = "(не выбран)";
            StatusComboBox.Items.Add(defaultStatusItem);
            StatusComboBox.SelectedItem = defaultStatusItem;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(getStatusNames, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    counter++;
                    string statusName = reader.GetString(0);
                    ComboBoxItem newComboBoxItem = new ComboBoxItem();
                    newComboBoxItem.Name = "item" + counter.ToString();
                    newComboBoxItem.Content = statusName;
                    StatusComboBox.Items.Add(newComboBoxItem);
                }
            }
        }

        private void SearchDaysButton_Click(object sender, RoutedEventArgs e)
        {
            DaysListDataGrid.Items.Clear();

            SqlParameter compoundPar = new SqlParameter("compound", System.Data.SqlDbType.Int);
            SqlParameter statusIdPar = new SqlParameter("status", System.Data.SqlDbType.VarChar);
            SqlParameter dayPar = new SqlParameter("date", System.Data.SqlDbType.VarChar);

            ComboBoxItem selectedStatusComboBoxItem = (ComboBoxItem)StatusComboBox.SelectedItem;
            string selectedStatusId = selectedStatusComboBoxItem.Name.Substring(4);
            DateTime startDate;
            DateTime endDate;
            int compound = 0;

            try
            {
                startDate = DateTime.Parse(StartIntervalBox.Text);
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Строка даты имела неправильный формат. Правильный формат yyyy.mm.dd", "Неправильный формат даты");
                return;
            }

            try
            {
                endDate = DateTime.Parse(EndIntervalBox.Text);
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Строка даты имела неправильный формат. Правильный формат yyyy.mm.dd", "Неправильный формат даты");
                return;
            }

            if ((bool)EmployedCheckBox.IsChecked && (bool)UnemployedCheckBox.IsChecked)
            {
                compound = 2;
            }
            else
            {
                if ((bool)EmployedCheckBox.IsChecked)
                {
                    compound = 0;
                }
                else if ((bool)UnemployedCheckBox.IsChecked)
                {
                    compound = 1;
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(getPersonsCount, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                do
                {
                    compoundPar.Value = compound;
                    statusIdPar.Value = selectedStatusId;
                    dayPar.Value = startDate.ToString(dateSqlFormat);// так как sql принимает дату в формате год-день-месяц

                    command.Parameters.Add(compoundPar);
                    command.Parameters.Add(statusIdPar);
                    command.Parameters.Add(dayPar);

                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        int persons = reader.GetInt32(0);

                        string dateToPrint = startDate.ToString(dateOutputFormat);

                        DaysListDataGrid.Items.Add(new { day = startDate.ToString(dateOutputFormat), personsCount = persons });
                    }
                    startDate = startDate.AddDays(1.0);
                    command.Parameters.Clear();
                } while (startDate <= endDate);
            }
        }

        private void EmployedCheckBox_Clicked(object sender, RoutedEventArgs e)
        {
            CheckBox employed = (CheckBox)sender;

            if (!(bool)employed.IsChecked)
            {
                if (!(bool)UnemployedCheckBox.IsChecked)
                {
                    UnemployedCheckBox.IsChecked = true;
                }
                PersonsCountColumn.Header = "Уволенные";
            }
            else
            {
                PersonsCountColumn.Header = "Принятые или уволенные";
            }
        }

        private void UnemployedCheckBox_Clicked(object sender, RoutedEventArgs e)
        {
            CheckBox unemployed = (CheckBox)sender;

            if (!(bool)unemployed.IsChecked)
            {
                if (!(bool)EmployedCheckBox.IsChecked)
                {
                    EmployedCheckBox.IsChecked = true;
                }
                PersonsCountColumn.Header = "Принятые";
            }
            else
            {
                PersonsCountColumn.Header = "Принятые или уволенные";
            }
        }
    }
}
