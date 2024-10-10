using System;
using System.Windows.Forms;

class InputForm : Form
{
    private TextBox inputBox;
    private Label textoLabel;
    private Button submitButton;
    public string UserInput { get; private set; }

    public InputForm()
    {
        this.Text = "Delimitador";
        this.Size = new System.Drawing.Size(300, 200);
        this.MaximizeBox = false;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;

        textoLabel = new Label();
        textoLabel.Text = "Digite o Delimitador";
        textoLabel.AutoSize = true;
        textoLabel.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold); // Tamanho 14 e negrito
        textoLabel.Location = new System.Drawing.Point(40, 10);

        

        inputBox = new TextBox { Left = 50, Top = 20, Width = 200 };
        submitButton = new Button { Text = "OK", Left = 100, Width = 100, Top = 50 };

        inputBox.Location = new System.Drawing.Point(40, 50);
        submitButton.Location = new System.Drawing.Point(90, 90);

        submitButton.Click += SubmitButton_Click;

        this.Controls.Add(textoLabel);
        this.Controls.Add(inputBox);
        this.Controls.Add(submitButton);
        
    }
    private void SubmitButton_Click(object sender, EventArgs e)
    {
        this.UserInput = inputBox.Text; // obter o texto da caixa de input
        this.DialogResult = DialogResult.OK; // Sinaliza que a operação foi bem-sucedida
        this.Close(); 
    }
}

