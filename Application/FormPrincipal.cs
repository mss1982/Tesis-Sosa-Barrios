﻿using DataBaseSQL;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using AppPrincipal.FormsPreprocesamientos;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TFIDFWeighting;
using SVM_Multiclass_Interface;
using AppPrincipal.FormsAplicacionDePipe;
using System;
using AppPrincipal.FormsCompararResultados;

namespace AppPrincipal
{
    public partial class App : Form
    {
        FormDataBaseYSeleccionarCategoria formDataBaseYSeleccionarCategoria;
        FormTokenization formTokenization;
        FormStopWords formStopWords;
        FormStemmer formStemmer;
        FormPreprocesamiento formPreprocesamiento;
        FormEnriquecimiento formEnriquecimiento;
        FormTratamientoEnTexto formTratamientoEnTexto;
        FormRepresentacion formRepresentacion;
        FormSVMLigth formSVMLigth;
        public FormCompararResultados formCompararResultados;
        public FormMatrizDeConfusion formMatrizDeConfusion;
        AboutBox1 aboutBox;

        //from file.pip
        public dynamic PipeConfiguration { get; set; }

        public App()
        {
            PipeConfiguration = JObject.Parse(File.ReadAllText(@"Recursos\Pipes\pipe-default.pip"));
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            crearFormularios();
        }

        private void crearFormularios()
        {
            this.IsMdiContainer = true;

            // Formulario de los botones "Data Base" y "Seleccionar Categoria"
            formDataBaseYSeleccionarCategoria = new FormDataBaseYSeleccionarCategoria(this);

            // Formulario del boton "Tokenization"
            formTokenization = new FormTokenization();
            formTokenization.MdiParent = this;

            // Formulario del boton "Stop Words"
            formStopWords = new FormStopWords();
            formStopWords.MdiParent = this;

            // Formulario del boton "Stemmer"
            formStemmer = new FormStemmer();
            formStemmer.MdiParent = this;

            // Formulario del boton "Enriquecimiento"
            formEnriquecimiento = new FormEnriquecimiento();
            formEnriquecimiento.MdiParent = this;

            // Formulario del boton "Preprocesamiento"
            formPreprocesamiento = new FormPreprocesamiento(this);
            formPreprocesamiento.MdiParent = this;

            // Formulario del boton "Tratamiento en Texto"
            formTratamientoEnTexto = new FormTratamientoEnTexto();
            formTratamientoEnTexto.MdiParent = this;

            // Formulario del boton "Representacion"
            formRepresentacion = new FormRepresentacion(this);

            // Formulario del boton "SVM-Ligth"
            formSVMLigth = new FormSVMLigth(this);

            // Formulario del boton "Comparar resultados"
            formCompararResultados = new FormCompararResultados(this);
            formCompararResultados.MdiParent = this;

            // Formulario del boton "Matriz de Confusion"
            formMatrizDeConfusion = new FormMatrizDeConfusion(this);

            aboutBox = new AboutBox1();
            aboutBox.StartPosition = FormStartPosition.CenterScreen;

            cargarDatosDePipeEnFormularios();
        }

        // Oculta todos los formularios.
        private void ocultarFormularios()
        {
            foreach (Form form in this.MdiChildren)
            {
                form.Hide();
            }
        }

        private void buttonDataBase_Click(object sender, System.EventArgs e)
        {
            ocultarFormularios();
            formDataBaseYSeleccionarCategoria.Dock = DockStyle.Fill;
            formDataBaseYSeleccionarCategoria.ocultarPanelSeleccionarCategoria();
            formDataBaseYSeleccionarCategoria.Show();
        }

        private void buttonSeleccionarCategoria_Click(object sender, System.EventArgs e)
        {
            ocultarFormularios();
            formDataBaseYSeleccionarCategoria.Dock = DockStyle.Fill;
            formDataBaseYSeleccionarCategoria.mostrarPanelSeleccionarCategoria();
            formDataBaseYSeleccionarCategoria.Show();
        }

        private void buttonTokenization_Click(object sender, System.EventArgs e)
        {
            ocultarFormularios();
            formTokenization.Dock = DockStyle.Fill;
            formTokenization.Show();
        }

        private void buttonStopWords_Click(object sender, System.EventArgs e)
        {
            ocultarFormularios();
            formStopWords.Dock = DockStyle.Fill;
            formStopWords.Show();
        }

        private void buttonStemmer_Click(object sender, System.EventArgs e)
        {
            ocultarFormularios();
            formStemmer.Dock = DockStyle.Fill;
            formStemmer.Show();
        }

        private void buttonPreprocesamiento_Click(object sender, System.EventArgs e)
        {
            ocultarFormularios();
            formPreprocesamiento.Dock = DockStyle.Fill;
            formPreprocesamiento.Show();
        }

        private void cerrarToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void limpiarFormularios()
        {
            formDataBaseYSeleccionarCategoria.Clean();
            formPreprocesamiento.Clean();
            formRepresentacion.Clean();
            formSVMLigth.Clean();
            formMatrizDeConfusion.Clean();
            formCompararResultados.Clean();
            buttonDataBase_Click(new object(), new EventArgs());
        }

        public void limpiarFormSinDB()
        {
            formPreprocesamiento.Clean();
            formRepresentacion.Clean();
            formSVMLigth.Clean();
            formMatrizDeConfusion.Clean();
            formCompararResultados.Clean();
        }

        public void limpiarFormSinRepresentacion()
        {
            formRepresentacion.Clean();
            formSVMLigth.Clean();
            formMatrizDeConfusion.Clean();
            formCompararResultados.Clean();
        }

        private void cargarDatosDePipeEnFormularios()
        {
            formDataBaseYSeleccionarCategoria.Init();
            formPreprocesamiento.Init();
            formRepresentacion.Init();
            formSVMLigth.Init();
            formMatrizDeConfusion.Init();
            formCompararResultados.Init();
        }

        private void cargarPipeToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog buscarArchivo = new OpenFileDialog();
            buscarArchivo.ShowDialog();
            string path = buscarArchivo.FileName;

            if (path.EndsWith(".pip"))
            {
                try
                {
                    limpiarFormularios();
                    string readText = File.ReadAllText(path);
                    PipeConfiguration = JObject.Parse(readText);
                    cargarDatosDePipeEnFormularios();
                    DialogResult result = MessageBox.Show("Su configuracion fue cargada exitosamente", "Carga Completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    DialogResult result = MessageBox.Show("El archivo se encuentra dañado o no es JSON válido", "Archivo Invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (!path.Equals(""))
                {
                    DialogResult result = MessageBox.Show("El archivo no tiene un formato valido .pip", "Archivo Invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonEnriquecimiento_Click(object sender, System.EventArgs e)
        {
            ocultarFormularios();
            formEnriquecimiento.Dock = DockStyle.Fill;
            formEnriquecimiento.Show();
        }

        private void buttonTratamientoEnTexto_Click(object sender, System.EventArgs e)
        {
            ocultarFormularios();
            formTratamientoEnTexto.Dock = DockStyle.Fill;
            formTratamientoEnTexto.Show();
        }

        private void buttonRepresentacion_Click(object sender, System.EventArgs e)
        {
            ocultarFormularios();
            formRepresentacion.Dock = DockStyle.Fill;
            formRepresentacion.Show();
        }

        private void guardarPipeToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            string json = JsonConvert.SerializeObject(PipeConfiguration);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Pipe|*.pip";
            saveFileDialog.Title = "Guardar configuracion en un archivo";
            saveFileDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                System.IO.FileStream fs =
                   (System.IO.FileStream)saveFileDialog.OpenFile();
                byte[] bytes = Encoding.UTF8.GetBytes(json.ToString());
                foreach (byte b in bytes)
                {
                    fs.WriteByte(b);
                }
                fs.Flush();
                fs.Close();
            }
        }

        private void buttonCompararResultados_Click(object sender, System.EventArgs e)
        {
            ocultarFormularios();
            formCompararResultados.Dock = DockStyle.Fill;
            formCompararResultados.Show();
        }

        private void buttonEjecutarSVMLigth_Click(object sender, System.EventArgs e)
        {
            ocultarFormularios();
            formSVMLigth.Dock = DockStyle.Fill;
            formSVMLigth.Show();
        }

        private void acercaPreprocesadorToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            aboutBox.Show();
        }

        private void buttonMatrizConfusion_Click(object sender, EventArgs e)
        {
            ocultarFormularios();
            formMatrizDeConfusion.Dock = DockStyle.Fill;
            formMatrizDeConfusion.Show();
        }

        public int[][] BuildConfusionMatrix(int[] actualCategories, int[] predictedCategories, List<int> categoryLabels)
        {
            int length = categoryLabels.Count;
            int[][] confusionMatrix = new int[length][];

            for (int i = 0; i < length; i++)
            {
                confusionMatrix[i] = new int[length + 1];
            }
            for (int i = 0; i < actualCategories.Length; i++)
            {
                int actualCategory = categoryLabels.IndexOf(actualCategories[i]);
                int predictedCategory = categoryLabels.IndexOf(predictedCategories[i]);
                if (predictedCategory == -1)
                    predictedCategory = length;
                confusionMatrix[actualCategory][predictedCategory]++;
            }

            return confusionMatrix;
        }

        public float CalcularTasaDeExactitud(int[][] confusionMatrix)
        {
            float total = 0;
            float correctos = 0;
            for (int i = 0; i < confusionMatrix.Length; i++)
            {
                for (int j = 0; j < confusionMatrix.Length; j++)
                {
                    total += confusionMatrix[i][j];
                    if (i == j)
                        correctos += confusionMatrix[i][j];
                }
            }
            return (correctos / total);
        }

        public float CalcularTasaDeError(int[][] confusionMatrix)
        {
            float total = 0;
            float incorrectos = 0;
            for (int i = 0; i < confusionMatrix.Length; i++)
            {
                for (int j = 0; j < confusionMatrix.Length; j++)
                {
                    total += confusionMatrix[i][j];
                    if (i != j)
                        incorrectos += confusionMatrix[i][j];
                }
            }
            return (incorrectos / total);
        }

        public float CalcularPresicion(int[][] confusionMatrix, int indexFila)
        {
            float totalPrediccionCategoria = 0;
            float correctosPrediccion = 0;
            for (int j = 0; j < confusionMatrix.Length; j++)
            {
                totalPrediccionCategoria += confusionMatrix[indexFila][j];
                if (indexFila == j)
                    correctosPrediccion += confusionMatrix[indexFila][j];
            }
            return (correctosPrediccion / totalPrediccionCategoria);
        }

        public void ActivarBotonPreprocesamiento()
        {
            buttonPreprocesamiento.Enabled = true;
        }

        public void DesactivarBotonPreprocesamiento()
        {
            buttonPreprocesamiento.Enabled = false;
        }

        public void ActivarBotonRepresentacion()
        {
            buttonRepresentacion.Enabled = true;
        }

        public void DesactivarBotonRepresentacion()
        {
            buttonRepresentacion.Enabled = false;
        }

        public void ActivarBotonCategoria()
        {
            buttonSeleccionarCategoria.Enabled = true;
        }

        public void DesactivarBotonCategoria()
        {
            buttonSeleccionarCategoria.Enabled = false;
        }

        public void ActivarBotonMatriz()
        {
            buttonMatrizConfusion.Enabled = true;
        }

        public void ActivarBotonComparar()
        {
            buttonCompararResultados.Enabled = true;
        }

        public void DesactivarBotonMatriz()
        {
            buttonMatrizConfusion.Enabled = false;
        }

        public void DesactivarBotonComparar()
        {
            buttonCompararResultados.Enabled = false;
        }

        public void ActivarBotonSVM()
        {
            buttonEjecutarSVMLigth.Enabled = true;
        }

        public void DesactivarBotonSVM()
        {
            buttonEjecutarSVMLigth.Enabled = false;
        }
    }
}
