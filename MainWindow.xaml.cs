using MySql.Data;
using MySql.Data.MySqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Timestamp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string conexao ="Server=localhost;Database=timestamp;User=root;Password=;";
        public MainWindow()
        {
            InitializeComponent();
            CarregarLogs();
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitulo.Text) ||
                string.IsNullOrWhiteSpace(txtDescricao.Text))
            {
                MessageBox.Show("Preencha todos os campos.");
                return;
            }

            try
            {
                using MySqlConnection conn =
                    new MySqlConnection(conexao);

                conn.Open();

                string sql = @"
                    INSERT INTO informacoes
                    (titulo, descricao)
                    VALUES
                    (@titulo, @descricao);
                ";

                using MySqlCommand cmd =
                    new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue(
                    "@titulo",
                    txtTitulo.Text
                );

                cmd.Parameters.AddWithValue(
                    "@descricao",
                    txtDescricao.Text
                );

                cmd.ExecuteNonQuery();

                long id =
                    cmd.LastInsertedId;

                string sqlLog = @"
                    INSERT INTO logs
                    (informacao_id, descricao)
                    VALUES
                    (@id, @descricao);
                ";

                using MySqlCommand cmdLog =
                    new MySqlCommand(sqlLog, conn);

                cmdLog.Parameters.AddWithValue(
                    "@id",
                    id
                );

                cmdLog.Parameters.AddWithValue(
                    "@descricao",
                    "Registro criado"
                );

                cmdLog.ExecuteNonQuery();

                MessageBox.Show(
                    "Informação cadastrada com sucesso."
                );

                txtTitulo.Clear();
                txtDescricao.Clear();

                CarregarLogs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro: " + ex.Message
                );
            }
        }

        private void CarregarLogs()
        {
            try
            {
                using MySqlConnection conn =
                    new MySqlConnection(conexao);

                conn.Open();

                string sql = @"
                    SELECT
                        descricao,
                        data_hora
                    FROM logs
                    ORDER BY data_hora DESC;
                ";

                using MySqlCommand cmd =
                    new MySqlCommand(sql, conn);

                using MySqlDataReader reader =
                    cmd.ExecuteReader();

                List<LogItem> lista =
                    new List<LogItem>();

                while (reader.Read())
                {
                    lista.Add(
                        new LogItem
                        {
                            Descricao =
                                Convert.ToString(reader["descricao"])
                                ?? string.Empty,

                            DataHora =
                                Convert.ToDateTime(
                                    reader["data_hora"]
                                ).ToString(
                                    "dd/MM/yyyy HH:mm:ss"
                                )
                        }
                    );
                }

                lstLogs.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao carregar logs: " +
                    ex.Message
                );
            }
        }
    }

    public class LogItem
    {
        public string Descricao { get; set; } = string.Empty;
        public string DataHora { get; set; } = string.Empty;
    }
}