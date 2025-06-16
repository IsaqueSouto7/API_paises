using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace API_paises
{
    public partial class TelaPrincipal : Form
    {
        public TelaPrincipal()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string pais = txtRespostas1.Text.Trim();

            if (string.IsNullOrEmpty(pais))
            {
                MessageBox.Show("Por favor, insira o nome de um país.");
                return;
            }

            string url = $"https://restcountries.com/v3.1/name/{pais}";

            await ObterDadosPais(url);
        }

        private async void buttonRandom_Click(object sender, EventArgs e)
        {
        string url = "https://restcountries.com/v3.1/all?fields=name,capital,population,area,region,subregion,currencies,languages,flags,continents";

            using (var client = new HttpClient())
            {
                try
                {
                    var resposta = await client.GetStringAsync(url);
                    var dados = JArray.Parse(resposta);

                    // Seleciona um país aleatório
                    Random random = new Random();
                    int indexAleatorio = random.Next(dados.Count);
                    var paisAleatorio = dados[indexAleatorio];

                    // Extrai as informações do país aleatório
                    await ExibirDadosPais(paisAleatorio);
                }
                catch (HttpRequestException)
                {
                    MessageBox.Show("Erro: Não foi possível conectar ao servidor.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        private async Task ObterDadosPais(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var resposta = await client.GetStringAsync(url);
                    var dados = JArray.Parse(resposta);
                    await ExibirDadosPais(dados[0]);
                }
                catch (HttpRequestException)
                {
                    MessageBox.Show("Erro: Não foi possível conectar ao servidor.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        private Task ExibirDadosPais(JToken pais)
        {
            var nome = pais["name"]?["common"]?.ToString() ?? "Sem nome";
            var capital = pais["capital"]?.First?.ToString() ?? "Sem capital";
            var populacao = pais["population"]?.ToString() ?? "Sem população";
            var area = pais["area"]?.ToString() ?? "Sem área";
            var subregiao = pais["subregion"]?.ToString() ?? "Sem sub-região";
            var regiao = pais["region"]?.ToString() ?? "Sem região";
            var moeda = pais["currencies"]?.First?.First?["name"]?.ToString() ?? "Sem moeda";
            var simboloMoeda = pais["currencies"]?.First?.First?["symbol"]?.ToString() ?? "Sem símbolo";
            var idiomas = string.Join(", ", pais["languages"]?.ToObject<JObject>()?.Properties().Select(x => x.Value.ToString()) ?? new string[0]);
            var continente = pais["continents"]?.First?.ToString() ?? "Sem continente";

            txtRespostas1.Text = $"{nome}";
            txtResposta2.Text = $"{capital}";
            txtResposta3.Text = $"{populacao}";
            txtResposta4.Text = $"{area} km²";
            txtResposta5.Text = $"{subregiao}";
            txtResposta6.Text = $"{regiao}";
            txtResposta7.Text = $"{moeda}";
            txtResposta8.Text = $"{idiomas}";
            textBox5.Text = $"{continente}";

            var bandeiraUrl = pais["flags"]?["png"]?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(bandeiraUrl))
            {
                pictureBox1.Load(bandeiraUrl);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }

            return Task.CompletedTask;
        }

        private async void buttonBuscarPorIdioma_Click(object sender, EventArgs e)
        {
            string idioma = txtIdioma.Text.Trim();

            if (string.IsNullOrEmpty(idioma))
            {
                MessageBox.Show("Por favor, insira o nome de um idioma.");
                return;
            }

            string url = "https://restcountries.com/v3.1/all?fields=name,languages";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "CSharpApp");

                try
                {
                    var resposta = await client.GetStringAsync(url);
                    var dados = JArray.Parse(resposta);

                    var paisesEncontrados = new List<string>();

                    foreach (var pais in dados)
                    {
                        var linguas = pais["languages"]?.ToObject<JObject>();

                        if (linguas != null)
                        {
                            foreach (var lingua in linguas.Properties())
                            {
                                if (lingua.Value.ToString().Equals(idioma, StringComparison.OrdinalIgnoreCase))
                                {
                                    var nomePais = pais["name"]?["common"]?.ToString() ?? "Sem nome";
                                    paisesEncontrados.Add(nomePais);
                                    break;
                                }
                            }
                        }
                    }

                    if (paisesEncontrados.Count == 0)
                    {
                        MessageBox.Show("Nenhum país encontrado que fale esse idioma.", "Resultados");
                        return;
                    }

                    string resultado = $"Países que falam \"{idioma}\":\n\n" + string.Join(", ", paisesEncontrados) + ".";
                    MessageBox.Show(resultado, "Resultados");
                }
                catch (HttpRequestException)
                {
                    MessageBox.Show("Erro: Não foi possível conectar ao servidor.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

    }
}
