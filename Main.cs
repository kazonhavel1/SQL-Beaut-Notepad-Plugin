using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections.ObjectModel;

namespace Kbg.NppPluginNET
{
    class Main
    {
        internal const string PluginName = "IndentadorSQL"; // Nome do Plugin
        static string iniFilePath = null;
        static bool someSetting = false;
        static frmMyDlg frmMyDlg = null;
        static int idMyDlg = -1;

        public static void OnNotification(ScNotification notification)
        {
            // This method is invoked whenever something is happening in notepad++
            // use eg. as
            // if (notification.Header.Code == (uint)NppMsg.NPPN_xxx)
            // { ... }
            // or
            //
            // if (notification.Header.Code == (uint)SciMsg.SCNxxx)
            // { ... }
        }



        internal static void CommandMenuInit()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            iniFilePath = Path.Combine(iniFilePath, PluginName + ".ini");
            someSetting = (Win32.GetPrivateProfileInt("SomeSection", "SomeKey", 0, iniFilePath) != 0);

            PluginBase.SetCommand(0, "Formata SQL", indentaSql, new ShortcutKey(false, false, false, Keys.F10));
            PluginBase.SetCommand(0, "Quebra de linha", quebraLinha, new ShortcutKey(false, false, false, Keys.F9));
        }


        private static IntPtr ObterHandleJanela()  // Obter o handle (ID) da janela atual
        {
            IntPtr janela = PluginBase.nppData._nppHandle;


            return janela.ToInt64() == 1 ? PluginBase.nppData._scintillaSecondHandle : PluginBase.nppData._scintillaMainHandle;
            // Validar se a janela está dividida se sim ela traz a tela secundaria, senão traz a principal
        }



        public static string BuscaTextoJanela() // Selecionar texto dentro da janela
        {
            IntPtr handle_janela = ObterHandleJanela();

            // obter o tamanho do texto
            int tamanho = (int)Win32.SendMessage(handle_janela, (uint)SciMsg.SCI_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);

            byte[] textoBuffer = new byte[tamanho + 1];

            GCHandle handle = GCHandle.Alloc(textoBuffer, GCHandleType.Pinned); // criando um manipulador de objetos
            IntPtr textoPtr = handle.AddrOfPinnedObject(); // torna-lo fixo na memória (impede que o garbage colletor intefira no objeto e cause efeitos inesperados
            Win32.SendMessage(handle_janela, (uint)SciMsg.SCI_GETTEXT, new IntPtr(tamanho + 1), textoPtr); // obtendo o texto da janela
            handle.Free(); //liberar o GC

            // converter de bytes para string usando UTF-8
            string texto = Encoding.UTF8.GetString(textoBuffer, 0, tamanho);

            return texto;
        }

        // Mensagem inicial no comando HelloNpp
        internal static void HelloNpp()
        {
        }

        internal static async void indentaSql()
        {
            string texto = BuscaTextoJanela();
            var utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(texto);
            texto = utf8.GetString(utfBytes, 0, utfBytes.Length);

            try
            {
                if (string.IsNullOrEmpty(texto)) // se a janela estiver vazia não enviar a RQ
                {
                    var retorno = "Não foi possível efetuar o processo pois essa janela está vazia!";
                    MessageBox.Show(retorno);
                }
                else
                {
                    if (texto.ToUpper().Contains("SELECT") == true || texto.ToUpper().Contains("UPDATE") || texto.ToUpper().Contains("INSERT") || texto.ToUpper().Contains("CREATE")) //Enviar a Requisiçao somente se houver "Select","Update","Create" ou "INSERT" em meio a string da janela
                    {

                        var retorno = await Request.enviaRequisicao(texto); //envia a string para a API
                        substituiTexto(retorno);

                    }
                    else
                    {

                        MessageBox.Show("Não foi possível efetuar o processo pois o texto desta janela não é um comando SQL.");

                    }
                }


            }
            catch (Exception e)
            {
                MessageBox.Show($"Erro {e.ToString()} ao processar requisição: {e.Message}");
            }


        }


        static void quebraLinha()
        {
            string texto = BuscaTextoJanela();
            
            if (string.IsNullOrEmpty(texto))
            {
                MessageBox.Show("Não é possível executar a função, pois essa janela está vazia.");
                return;
            }
            
            var utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(texto);
            texto = utf8.GetString(utfBytes, 0, utfBytes.Length);

            InputForm delimitador = new InputForm();

            if (delimitador.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(delimitador.UserInput))
                {
                    MessageBox.Show("O delimitador não pode estar em branco, tente novamente.");
                }
                else
                {
                    if (texto.Contains(delimitador.UserInput) == true)
                    {

                        try
                        {
                            string newTexto = texto.Replace(delimitador.UserInput, "\n");
                            var jsontxt = "{result: " + $"'{newTexto}'" + "}";
                            substituiTexto(jsontxt);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show($"Erro ao efetuar o processo: {e.ToString()}");
                        }

                    }
                    else
                    {
                        MessageBox.Show("Delimitador não encontrado no texto atual, tente novamente.");
                    }

                }
            }

        }

        static void substituiTexto(string consulta)
        {

            IntPtr janela_handle = ObterHandleJanela();
            Win32.SendMessage(janela_handle, SciMsg.SCI_CLEARALL, IntPtr.Zero, IntPtr.Zero); //limpa o texto atual da janela

            var dados = JsonConvert.DeserializeObject<Dictionary<string, object>>(consulta); //Converte o retorno para DIC para conseguirmos acessar somente o valor Result

            string resultado = dados["result"].ToString();

            byte[] textoBytes = Encoding.UTF8.GetBytes(resultado); // Converte o texto para UTF-8

            GCHandle handleTexto = GCHandle.Alloc(textoBytes, GCHandleType.Pinned);
            IntPtr textoPtr = handleTexto.AddrOfPinnedObject();


            Win32.SendMessage(janela_handle, (uint)SciMsg.SCI_ADDTEXT, new IntPtr(resultado.Length), textoPtr);  // Altera o texto na janela pro resultado


            handleTexto.Free(); // Liberar o buffer de texto     

        }


    }
}