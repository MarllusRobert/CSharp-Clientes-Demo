using ClientesDemo.Data;
using ClientesDemo.Models;
using ClientesDemo.Presentation;

namespace ClientesDemo;

public sealed class MainForm : Form, IClienteView
{
    private readonly ClientePresenter _presenter;

    private TextBox _txtNome = null!;
    private TextBox _txtDocumento = null!;
    private TextBox _txtCidade = null!;
    private TextBox _txtTelefone = null!;
    private TextBox _txtEmail = null!;
    private DataGridView _grid = null!;
    private BindingSource _binding = null!;

    public MainForm()
    {
        BuildUi();

        var dbPath = Path.Combine(AppContext.BaseDirectory, "data", "clientes.db");
        var repo = new SqliteClienteRepository(dbPath);
        _presenter = new ClientePresenter(this, repo);

        Load += (_, _) => _presenter.Inicializar();
    }

    public string Nome
    {
        get => _txtNome.Text;
        set => _txtNome.Text = value;
    }

    public string Documento
    {
        get => _txtDocumento.Text;
        set => _txtDocumento.Text = value;
    }

    public string Cidade
    {
        get => _txtCidade.Text;
        set => _txtCidade.Text = value;
    }

    public string Telefone
    {
        get => _txtTelefone.Text;
        set => _txtTelefone.Text = value;
    }

    public string Email
    {
        get => _txtEmail.Text;
        set => _txtEmail.Text = value;
    }

    public long EditId { get; set; }

    public void BindClientes(IReadOnlyList<Cliente> clientes)
    {
        _binding.DataSource = clientes.ToList();
    }

    public void ClearForm()
    {
        EditId = 0;
        Nome = "";
        Documento = "";
        Cidade = "";
        Telefone = "";
        Email = "";
        _txtNome.Focus();
    }

    public void ShowInfo(string message) =>
        MessageBox.Show(this, message, "ClientesDemo", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public void ShowError(string message) =>
        MessageBox.Show(this, message, "ClientesDemo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public bool Confirm(string message) =>
        MessageBox.Show(this, message, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

    private void BuildUi()
    {
        Text = "C# WinForms Demo — Cadastro de Clientes (MVP)";
        StartPosition = FormStartPosition.CenterScreen;
        Width = 920;
        Height = 560;
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.White;

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Color.FromArgb(38, 70, 83),
        };
        header.Controls.Add(new Label
        {
            Text = "Cadastro de Clientes",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 12),
        });
        header.Controls.Add(new Label
        {
            Text = "C# · WinForms · MVP · SQLite · SQL parametrizado",
            ForeColor = Color.FromArgb(200, 220, 220),
            AutoSize = true,
            Location = new Point(20, 42),
        });

        var formPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 160,
            BackColor = Color.FromArgb(245, 248, 250),
            Padding = new Padding(16),
        };

        _txtNome = AddField(formPanel, "Nome", 16, 28, 250);
        _txtDocumento = AddField(formPanel, "Documento", 286, 28, 180);
        _txtCidade = AddField(formPanel, "Cidade", 486, 28, 170);
        _txtTelefone = AddField(formPanel, "Telefone", 16, 96, 180);
        _txtEmail = AddField(formPanel, "E-mail", 216, 96, 260);

        var btnSalvar = MakeButton("Salvar", 500, 96, Color.FromArgb(42, 157, 143));
        btnSalvar.Click += (_, _) => _presenter.Salvar();
        formPanel.Controls.Add(btnSalvar);

        var btnNovo = MakeButton("Novo", 610, 96, Color.FromArgb(90, 110, 120));
        btnNovo.Click += (_, _) => _presenter.Novo();
        formPanel.Controls.Add(btnNovo);

        var btnExcluir = MakeButton("Excluir", 710, 96, Color.FromArgb(180, 80, 80));
        btnExcluir.Click += (_, _) => _presenter.Excluir();
        formPanel.Controls.Add(btnExcluir);

        var btnAtualizar = MakeButton("Atualizar", 680, 28, Color.FromArgb(90, 110, 120));
        btnAtualizar.Width = 110;
        btnAtualizar.Click += (_, _) => _presenter.AtualizarLista();
        formPanel.Controls.Add(btnAtualizar);

        _binding = new BindingSource();
        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            DataSource = _binding,
        };
        _grid.SelectionChanged += (_, _) =>
        {
            if (_grid.CurrentRow?.DataBoundItem is Cliente cliente)
                _presenter.Selecionar(cliente);
        };

        Controls.Add(_grid);
        Controls.Add(formPanel);
        Controls.Add(header);
    }

    private static TextBox AddField(Control parent, string label, int x, int y, int width)
    {
        parent.Controls.Add(new Label
        {
            Text = label,
            Location = new Point(x, y - 18),
            AutoSize = true,
        });
        var box = new TextBox
        {
            Location = new Point(x, y),
            Width = width,
        };
        parent.Controls.Add(box);
        return box;
    }

    private static Button MakeButton(string text, int x, int y, Color back)
    {
        return new Button
        {
            Text = text,
            Location = new Point(x, y),
            Width = 90,
            Height = 32,
            FlatStyle = FlatStyle.Flat,
            BackColor = back,
            ForeColor = Color.White,
        };
    }
}
