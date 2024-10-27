using System;
using System.Windows.Forms;

class InputForm : Form
{
    private TextBox inputBox;
    private Label textoLabel;
    private Button submitButton;
    public CheckBox checkBoxInserir;
    public CheckBox checkBoxRemover;
    private bool bloqueiaEventos = false;
    public string UserInput { get; private set; }

    public InputForm()
    {
        this.Text = "Quebra de linha";
        this.Size = new System.Drawing.Size(300, 200);
        this.MaximizeBox = false;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;

        textoLabel = new Label();
        textoLabel.Text = "Digite o Delimitador";
        textoLabel.AutoSize = true;
        textoLabel.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold); // Tamanho 14 e negrito
        textoLabel.Location = new System.Drawing.Point(60, 10);

        checkBoxInserir = new CheckBox();
        checkBoxInserir.Text = "Inserir";
        checkBoxInserir.AutoSize = true;
        checkBoxInserir.Location = new System.Drawing.Point(50, 80);
        checkBoxInserir.CheckedChanged += validarCheckBoxRemover;

        checkBoxRemover = new CheckBox();
        checkBoxRemover.Text = "Remover";
        checkBoxRemover.AutoSize = true;
        checkBoxRemover.Location = new System.Drawing.Point(170, 80);
        checkBoxRemover.CheckedChanged += validarCheckBoxInserir;



        inputBox = new TextBox { Left = 50, Top = 20, Width = 200 };
        submitButton = new Button { Text = "OK", Left = 100, Width = 100, Top = 70 };

        inputBox.Location = new System.Drawing.Point(40, 50);
        submitButton.Location = new System.Drawing.Point(90, 110);

        submitButton.Click += SubmitButton_Click;

        this.Controls.Add(textoLabel);
        this.Controls.Add(inputBox);
        this.Controls.Add(submitButton);
        this.Controls.Add(checkBoxInserir);
        this.Controls.Add(checkBoxRemover);
        
    }

    public bool validaCheckBoxVazia()
    {
        if (checkBoxInserir.Checked || checkBoxRemover.Checked)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private void SubmitButton_Click(object sender, EventArgs e)
    {
        this.UserInput = inputBox.Text; // obter o texto da caixa de input
        this.DialogResult = DialogResult.OK; // Sinaliza que a operação foi bem-sucedida
        this.Close(); 
    }


    private void validarCheckBoxRemover (object sender, EventArgs e)
    {
        if(bloqueiaEventos) {
            bloqueiaEventos = false;
            return;
        };

        if (checkBoxRemover.Checked)
        {
            bloqueiaEventos = true;
            checkBoxRemover.Checked = false;
            checkBoxInserir.Checked = true;
        }
    
    }
    private void validarCheckBoxInserir(object sender, EventArgs e)
    {

        if (bloqueiaEventos) {
            bloqueiaEventos = false;
            return;
        };


        if (checkBoxInserir.Checked)
        {
            bloqueiaEventos = true;
            checkBoxInserir.Checked = false;
            checkBoxRemover.Checked = true;
        }


    }
}