﻿namespace AppPrincipal.FormsAplicacionDePipe
{
    partial class FormRepresentacion
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonObtenerRepresentacion = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonObtenerRepresentacion
            // 
            this.buttonObtenerRepresentacion.Location = new System.Drawing.Point(845, 362);
            this.buttonObtenerRepresentacion.Name = "buttonObtenerRepresentacion";
            this.buttonObtenerRepresentacion.Size = new System.Drawing.Size(146, 23);
            this.buttonObtenerRepresentacion.TabIndex = 0;
            this.buttonObtenerRepresentacion.Text = "Obtener Representación";
            this.buttonObtenerRepresentacion.UseVisualStyleBackColor = true;
            this.buttonObtenerRepresentacion.Click += new System.EventHandler(this.buttonObtenerRepresentacion_Click);
            // 
            // FormRepresentacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 568);
            this.Controls.Add(this.buttonObtenerRepresentacion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormRepresentacion";
            this.Text = "FormRepresentacion";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonObtenerRepresentacion;
    }
}