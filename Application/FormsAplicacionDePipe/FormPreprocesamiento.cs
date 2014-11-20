﻿using DataBaseSQL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Tonkenizer.Filters;
using Tonkenizer.Filters.AroundFilters;
using System.Text.RegularExpressions;
using Tonkenizer.Filters.PreFilters;
using Tonkenizer.Core;
using System.IO;

namespace AppPrincipal
{
    public partial class FormPreprocesamiento : Form
    {

        bool cambiarTabs = false;

        public FormPreprocesamiento(Form parent)
        {
            //configuration = JObject.Parse(jsonConf);
            //dynamic tokenizingConfiguration = ((JArray)configuration.preprocessing).First(x => (string)x["_type"] == "tokenizing" );
            InitializeComponent();
            this.MdiParent = parent;
            Init();
        }

        public void Init()
        {

        }


        public PreFilter BuildPrefilter(JArray preprocessingFiltersConfiguration, int i = 0)
        {
            if (i == preprocessingFiltersConfiguration.Count) return null;

            PreFilter ret = GetConfiguredPrefilter(preprocessingFiltersConfiguration.ElementAt(i));
            if (ret != null)
            {
                ret.Next = BuildPrefilter(preprocessingFiltersConfiguration, i + 1);
            }
            else
            {
                ret = BuildPrefilter(preprocessingFiltersConfiguration, i + 1);
            }
            return ret;
        }

        private PreFilter GetConfiguredPrefilter(JToken jToken)
        {
            switch ((string)jToken["_type"])
            {
                case "richment": return new AddLinkContextPreFilter(null)
                {
                    Description = (bool)jToken["description"],
                    Title = (bool)jToken["title"],
                    Keywords = (bool)jToken["keywords"]
                };
                case "words": return new OnlyWordsPreFilter(null, (bool)jToken["removeLinks"], (bool)jToken["replaceAbbreviations"], (bool)jToken["byDefault"], (string)jToken["filename"]);
                default: return null;
            }
        }


        public AroundFilter BuildAroundfilter(JArray preprocessingFiltersConfiguration, int i = 0)
        {
            if (i == preprocessingFiltersConfiguration.Count) return null;

            AroundFilter ret = GetConfiguredAroundfilter(preprocessingFiltersConfiguration.ElementAt(i));
            if (ret != null)
            {
                ret.Next = BuildAroundfilter(preprocessingFiltersConfiguration, i + 1);
            }
            else
            {
                ret = BuildAroundfilter(preprocessingFiltersConfiguration, i + 1);
            }
            return ret;
        }

        private AroundFilter GetConfiguredAroundfilter(JToken jToken)
        {
            switch ((string)jToken["_type"])
            {
                case "stopWords": return new StopWordFilter(null, (bool)jToken["byDefault"], (string)jToken["filename"]);
                case "stemming": return new StemmingFilter(null);
                default: return null;
            }
        }


        private void buttonPreprocesar_Click(object sender, EventArgs e)
        {
            JArray preprocessingFiltersConfiguration = ((App)this.MdiParent).PipeConfiguration.preprocessing.filters;
            PreFilter preFilter = null;
            AroundFilter aroundFilter = null;
            Regex delimiter = null;
            ITokenizer tokenizer = null;

            int categoryLevel = (int)((App)this.MdiParent).PipeConfiguration.categoryLevel;
            List<Tweet> tweets = DataBase.Instance.GetTweetsForClassify(categoryLevel);
            List<string> docs = tweets.Select(x => x.Text).ToList();

            
            dynamic tokenizingConfiguration = preprocessingFiltersConfiguration.First(x => (string)x["_type"] == "tokenizing");
            if ((bool)tokenizingConfiguration.byDefault)
            {
                delimiter = new Regex((string)tokenizingConfiguration.regexpByDefault);
            }
            else
            {
                delimiter = new Regex((string)tokenizingConfiguration.regexp);
            }
                
            

            preFilter = BuildPrefilter(preprocessingFiltersConfiguration);
            aroundFilter = BuildAroundfilter(preprocessingFiltersConfiguration);

            ////preprocessing
            tokenizer = new Tokenizer(delimiter, preFilter, aroundFilter);
            List<string[]> TFIDFInput = tokenizer.Tokenize(docs).ToList();

            string preprocessingGuid = DataBase.Instance.SavePreprocessingTokens(tweets, TFIDFInput);
            ((App)this.MdiParent).PipeConfiguration.preprocessing.guid = preprocessingGuid;
            ((App)this.MdiParent).ValidateConfiguration();
            ((App)this.MdiParent).ActivarBotonRepresentacion();
            labelPreprocesadoAplicado.Show();
        }
        
        private void buttonSeleccionar_Click(object sender, EventArgs e)
        {
            labelPreprocesadoAplicado.Hide();
            if (listViewPreprocesamientos.SelectedItems.Count > 0)
            {
                ListViewItem seleccion = listViewPreprocesamientos.SelectedItems[0];
                listViewPreprocesamientos.SelectedItems[0].Remove();
                listViewOrdenPreprocesos.Items.Add(seleccion);
                listViewOrdenPreprocesos.Focus();
            }
        }

        private void buttonQuitar_Click(object sender, EventArgs e)
        {
            labelPreprocesadoAplicado.Hide();
            if (listViewOrdenPreprocesos.SelectedItems.Count > 0)
            {
                ListViewItem seleccion = listViewOrdenPreprocesos.SelectedItems[0];
                listViewOrdenPreprocesos.SelectedItems[0].Remove();
                listViewPreprocesamientos.Items.Add(seleccion);
                listViewPreprocesamientos.Focus();
                tabControlConfiguraciones.Hide();
            }
        }

        private void buttonSubir_Click(object sender, EventArgs e)
        {
            labelPreprocesadoAplicado.Hide();
            if (listViewOrdenPreprocesos.SelectedItems.Count > 0)
            {
                ListViewItem seleccion = listViewOrdenPreprocesos.SelectedItems[0];
                if (seleccion.Index > 0)
                {
                    int pos = seleccion.Index - 1;
                    listViewOrdenPreprocesos.Items.RemoveAt(seleccion.Index);
                    listViewOrdenPreprocesos.Items.Insert(pos, seleccion);
                }
            }
            listViewOrdenPreprocesos.Focus();
        }

        private void buttonBajar_Click(object sender, EventArgs e)
        {
            labelPreprocesadoAplicado.Hide();
            if (listViewOrdenPreprocesos.SelectedItems.Count > 0)
            {
                ListViewItem seleccion = listViewOrdenPreprocesos.SelectedItems[0];
                if (seleccion.Index < (listViewOrdenPreprocesos.Items.Count - 1))
                {
                    int pos = seleccion.Index + 1;
                    listViewOrdenPreprocesos.Items.RemoveAt(seleccion.Index);
                    listViewOrdenPreprocesos.Items.Insert(pos, seleccion);
                }
            }
            listViewOrdenPreprocesos.Focus();
        }

        private void manejoConfiguraciones()
        {
            cambiarTabs = true;
            tabControlConfiguraciones.Hide();
            if (listViewOrdenPreprocesos.SelectedItems.Count > 0)
            {
                ListViewItem seleccion = listViewOrdenPreprocesos.SelectedItems[0];
                if (seleccion.Text.Equals("Stemmer"))
                {
                    tabControlConfiguraciones.SelectTab(0);
                }
                else if (seleccion.Text.Equals("Stop Words"))
                {
                    tabControlConfiguraciones.SelectTab(1);
                }
                else if (seleccion.Text.Equals("Tokenization"))
                {
                    tabControlConfiguraciones.SelectTab(2);
                }
                else if (seleccion.Text.Equals("Enriquecimiento"))
                {
                    tabControlConfiguraciones.SelectTab(3);
                }
                else if (seleccion.Text.Equals("Tratamiento en Texto"))
                {
                    tabControlConfiguraciones.SelectTab(4);
                }
                tabControlConfiguraciones.Show();
            }
            cambiarTabs = false;
            listViewOrdenPreprocesos.Focus();
        }

        private void listViewOrdenPreprocesos_Enter(object sender, EventArgs e)
        {
            manejoConfiguraciones();
        }

        private void checkBoxListaStopWordsDefecto_CheckedChanged(object sender, EventArgs e)
        {
            labelListaAplicadaStopWords.Hide();

            if (checkBoxListaStopWordsPorDefecto.Checked)
            {
                textBoxDireccionStopWords.Enabled = false;
                buttonBuscarListStopWords.Enabled = false;
            }
            else
            {
                textBoxDireccionStopWords.Enabled = true;
                buttonBuscarListStopWords.Enabled = true;
            }
        }

        private void tabControlConfiguraciones_Selecting(object sender, TabControlCancelEventArgs e)
        {
            labelPreprocesadoAplicado.Hide();
            if (!cambiarTabs)
                e.Cancel = true;
        }

        private void listViewOrdenPreprocesos_Click(object sender, EventArgs e)
        {
            manejoConfiguraciones();
        }

        private void buttonBuscarListStopWords_Click(object sender, EventArgs e)
        {
            labelListaAplicadaStopWords.Hide();
            OpenFileDialog buscarArchivo = new OpenFileDialog();
            buscarArchivo.ShowDialog();
            string directorio = buscarArchivo.FileName;
            textBoxDireccionStopWords.Text = directorio;

            if (!directorio.EndsWith(".txt"))
            {
                textBoxDireccionStopWords.Clear();
                DialogResult result = MessageBox.Show("El archivo no tiene un formato valido .txt", "Archivo Invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBoxReemplazarAbreviatura_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxReemplazarAbreviatura.Checked)
            {
                checkBoxListaAbreviaturasPorDefecto.Enabled = true;
                buttonBuscarListaAbreviatura.Enabled = true;
                textBoxDireccionAbreviaturas.Enabled = true;
            }
            else
            {
                checkBoxListaAbreviaturasPorDefecto.Enabled = false;
                buttonBuscarListaAbreviatura.Enabled = false;
                textBoxDireccionAbreviaturas.Enabled = false;
            }
            if (checkBoxListaAbreviaturasPorDefecto.Checked)
            {
                buttonBuscarListaAbreviatura.Enabled = false;
                textBoxDireccionAbreviaturas.Enabled = false;
            }
        }

        private void checkBoxListaAbreviaturasDefecto_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxListaAbreviaturasPorDefecto.Checked)
            {
                buttonBuscarListaAbreviatura.Enabled = false;
                textBoxDireccionAbreviaturas.Enabled = false;
            }
            else
            {
                buttonBuscarListaAbreviatura.Enabled = true;
                textBoxDireccionAbreviaturas.Enabled = true;
            }
        }

        private void buttonBuscarListaAbreviatura_Click(object sender, EventArgs e)
        {
            OpenFileDialog buscarArchivo = new OpenFileDialog();
            buscarArchivo.ShowDialog();
            string directorio = buscarArchivo.FileName;
            textBoxDireccionAbreviaturas.Text = directorio;

            if (!directorio.EndsWith(".txt"))
            {
                textBoxDireccionAbreviaturas.Clear();
                DialogResult result = MessageBox.Show("El archivo no tiene un formato valido .txt", "Archivo Invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonAplicarListaStopWords_Click(object sender, EventArgs e)
        {
            JArray preprocessingFiltersConfiguration = ((App)this.MdiParent).PipeConfiguration.preprocessing.filters;
            dynamic stopWordsConfiguration = preprocessingFiltersConfiguration.First(x => (string)x["_type"] == "stopWords");

            stopWordsConfiguration.byDefault = checkBoxListaStopWordsPorDefecto.Checked;
            if (!checkBoxListaStopWordsPorDefecto.Checked)
                stopWordsConfiguration.filename = textBoxDireccionStopWords.Text;

            labelListaAplicadaStopWords.Show();
        }

        private void buttonAplicarExpresionRegular_Click(object sender, EventArgs e)
        {
            JArray preprocessingFiltersConfiguration = ((App)this.MdiParent).PipeConfiguration.preprocessing.filters;
            dynamic tokenizingConfiguration = preprocessingFiltersConfiguration.First(x => (string)x["_type"] == "tokenizing");

            tokenizingConfiguration.byDefault = checkBoxERPorDefecto.Checked;
            if (!checkBoxERPorDefecto.Checked)
                tokenizingConfiguration.regexp = textBoxExpresionRegular.Text;

            labelExpresionRegularAplicada.Show();
        }

        private void buttonAplicarEnriquecimiento_Click(object sender, EventArgs e)
        {
            JArray preprocessingFiltersConfiguration = ((App)this.MdiParent).PipeConfiguration.preprocessing.filters;
            dynamic richmentConfiguration = preprocessingFiltersConfiguration.First(x => (string)x["_type"] == "richment");

            richmentConfiguration.metatags = checkBoxMetaTags.Checked;
            richmentConfiguration.title = checkBoxTitulo.Checked;

            labelEnriquecimientoAplicado.Show();
        }

        private void buttonAplicarTratamiento_Click(object sender, EventArgs e)
        {
            JArray preprocessingFiltersConfiguration = ((App)this.MdiParent).PipeConfiguration.preprocessing.filters;
            dynamic wordsConfiguration = preprocessingFiltersConfiguration.First(x => (string)x["_type"] == "words");

            if (checkBoxReemplazarAbreviatura.Checked)
            {
                wordsConfiguration.replaceAbbreviations = checkBoxReemplazarAbreviatura.Checked;
                wordsConfiguration.byDefault = checkBoxListaAbreviaturasPorDefecto.Checked;
                if (!checkBoxListaAbreviaturasPorDefecto.Checked)
                    wordsConfiguration.filename = textBoxDireccionAbreviaturas.Text;
            }
            wordsConfiguration.removeLinks = checkBoxEliminarLinks.Checked;

            labelTratamientoAplicado.Show();
        }

        private void textBoxExpresionRegular_TextChanged(object sender, EventArgs e)
        {
            labelExpresionRegularAplicada.Hide();
        }

        private void checkBoxERPorDefecto_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxERPorDefecto.Checked)
                textBoxExpresionRegular.Enabled = false;
            else
                textBoxExpresionRegular.Enabled = true;
        }

    }
}