namespace BatalhaNaval_Visual
{
    partial class FormTabuleiro
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTabuleiro));
            this.tlpNavios = new System.Windows.Forms.TableLayoutPanel();
            this.pbSubmarino = new System.Windows.Forms.PictureBox();
            this.pbDestroier = new System.Windows.Forms.PictureBox();
            this.pbCruzador = new System.Windows.Forms.PictureBox();
            this.pbEncouracado = new System.Windows.Forms.PictureBox();
            this.pbPortaAvioes = new System.Windows.Forms.PictureBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.splitterLeft = new System.Windows.Forms.Splitter();
            this.panelConectar = new System.Windows.Forms.Panel();
            this.btnConectar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbDisponiveis = new System.Windows.Forms.ComboBox();
            this.splitterRight = new System.Windows.Forms.Splitter();
            this.splitterTabuleiros = new System.Windows.Forms.Splitter();
            this.pbTabuleiro = new System.Windows.Forms.PictureBox();
            this.pbInimigo = new System.Windows.Forms.PictureBox();
            this.tlpNavios.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSubmarino)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDestroier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCruzador)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbEncouracado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPortaAvioes)).BeginInit();
            this.panelConectar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTabuleiro)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbInimigo)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpNavios
            // 
            this.tlpNavios.ColumnCount = 1;
            this.tlpNavios.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpNavios.Controls.Add(this.pbSubmarino, 0, 4);
            this.tlpNavios.Controls.Add(this.pbDestroier, 0, 3);
            this.tlpNavios.Controls.Add(this.pbCruzador, 0, 2);
            this.tlpNavios.Controls.Add(this.pbEncouracado, 0, 1);
            this.tlpNavios.Controls.Add(this.pbPortaAvioes, 0, 0);
            this.tlpNavios.Controls.Add(this.btnClear, 0, 5);
            this.tlpNavios.Dock = System.Windows.Forms.DockStyle.Left;
            this.tlpNavios.Location = new System.Drawing.Point(0, 0);
            this.tlpNavios.Name = "tlpNavios";
            this.tlpNavios.RowCount = 6;
            this.tlpNavios.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpNavios.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpNavios.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpNavios.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpNavios.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpNavios.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpNavios.Size = new System.Drawing.Size(264, 480);
            this.tlpNavios.TabIndex = 6;
            // 
            // pbSubmarino
            // 
            this.pbSubmarino.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbSubmarino.Image = global::BatalhaNaval_Visual.Properties.Resources.submarino;
            this.pbSubmarino.Location = new System.Drawing.Point(3, 363);
            this.pbSubmarino.Name = "pbSubmarino";
            this.pbSubmarino.Size = new System.Drawing.Size(258, 84);
            this.pbSubmarino.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbSubmarino.TabIndex = 4;
            this.pbSubmarino.TabStop = false;
            this.pbSubmarino.Tag = "Submarino";
            this.pbSubmarino.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbNavio_MouseDown);
            // 
            // pbDestroier
            // 
            this.pbDestroier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbDestroier.Image = global::BatalhaNaval_Visual.Properties.Resources.destroier;
            this.pbDestroier.Location = new System.Drawing.Point(3, 273);
            this.pbDestroier.Name = "pbDestroier";
            this.pbDestroier.Size = new System.Drawing.Size(258, 84);
            this.pbDestroier.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbDestroier.TabIndex = 3;
            this.pbDestroier.TabStop = false;
            this.pbDestroier.Tag = "Destroier";
            this.pbDestroier.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbNavio_MouseDown);
            // 
            // pbCruzador
            // 
            this.pbCruzador.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbCruzador.Image = global::BatalhaNaval_Visual.Properties.Resources.cruzador;
            this.pbCruzador.Location = new System.Drawing.Point(3, 183);
            this.pbCruzador.Name = "pbCruzador";
            this.pbCruzador.Size = new System.Drawing.Size(258, 84);
            this.pbCruzador.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbCruzador.TabIndex = 2;
            this.pbCruzador.TabStop = false;
            this.pbCruzador.Tag = "Cruzador";
            this.pbCruzador.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbNavio_MouseDown);
            // 
            // pbEncouracado
            // 
            this.pbEncouracado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbEncouracado.Image = global::BatalhaNaval_Visual.Properties.Resources.encouracado;
            this.pbEncouracado.Location = new System.Drawing.Point(3, 93);
            this.pbEncouracado.Name = "pbEncouracado";
            this.pbEncouracado.Size = new System.Drawing.Size(258, 84);
            this.pbEncouracado.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbEncouracado.TabIndex = 1;
            this.pbEncouracado.TabStop = false;
            this.pbEncouracado.Tag = "Encouracado";
            this.pbEncouracado.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbNavio_MouseDown);
            // 
            // pbPortaAvioes
            // 
            this.pbPortaAvioes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbPortaAvioes.Image = ((System.Drawing.Image)(resources.GetObject("pbPortaAvioes.Image")));
            this.pbPortaAvioes.Location = new System.Drawing.Point(3, 3);
            this.pbPortaAvioes.Name = "pbPortaAvioes";
            this.pbPortaAvioes.Size = new System.Drawing.Size(258, 84);
            this.pbPortaAvioes.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbPortaAvioes.TabIndex = 0;
            this.pbPortaAvioes.TabStop = false;
            this.pbPortaAvioes.Tag = "PortaAvioes";
            this.pbPortaAvioes.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbNavio_MouseDown);
            // 
            // btnClear
            // 
            this.btnClear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClear.Location = new System.Drawing.Point(3, 453);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(258, 24);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Limpar Tabuleiro";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // splitterLeft
            // 
            this.splitterLeft.Location = new System.Drawing.Point(264, 0);
            this.splitterLeft.Name = "splitterLeft";
            this.splitterLeft.Size = new System.Drawing.Size(3, 480);
            this.splitterLeft.TabIndex = 7;
            this.splitterLeft.TabStop = false;
            // 
            // panelConectar
            // 
            this.panelConectar.Controls.Add(this.btnConectar);
            this.panelConectar.Controls.Add(this.label1);
            this.panelConectar.Controls.Add(this.cbDisponiveis);
            this.panelConectar.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelConectar.Location = new System.Drawing.Point(584, 0);
            this.panelConectar.Name = "panelConectar";
            this.panelConectar.Size = new System.Drawing.Size(166, 480);
            this.panelConectar.TabIndex = 8;
            this.panelConectar.Visible = false;
            // 
            // btnConectar
            // 
            this.btnConectar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConectar.Location = new System.Drawing.Point(78, 39);
            this.btnConectar.Name = "btnConectar";
            this.btnConectar.Size = new System.Drawing.Size(75, 23);
            this.btnConectar.TabIndex = 2;
            this.btnConectar.Text = "Conectar";
            this.btnConectar.UseVisualStyleBackColor = true;
            this.btnConectar.Click += new System.EventHandler(this.btnConectar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Disponíveis:";
            // 
            // cbDisponiveis
            // 
            this.cbDisponiveis.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDisponiveis.Location = new System.Drawing.Point(78, 12);
            this.cbDisponiveis.Name = "cbDisponiveis";
            this.cbDisponiveis.Size = new System.Drawing.Size(76, 21);
            this.cbDisponiveis.TabIndex = 0;
            // 
            // splitterRight
            // 
            this.splitterRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterRight.Location = new System.Drawing.Point(581, 0);
            this.splitterRight.Name = "splitterRight";
            this.splitterRight.Size = new System.Drawing.Size(3, 480);
            this.splitterRight.TabIndex = 9;
            this.splitterRight.TabStop = false;
            this.splitterRight.Visible = false;
            // 
            // splitterTabuleiros
            // 
            this.splitterTabuleiros.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitterTabuleiros.Location = new System.Drawing.Point(267, 241);
            this.splitterTabuleiros.Name = "splitterTabuleiros";
            this.splitterTabuleiros.Size = new System.Drawing.Size(314, 3);
            this.splitterTabuleiros.TabIndex = 11;
            this.splitterTabuleiros.TabStop = false;
            this.splitterTabuleiros.Visible = false;
            // 
            // pbTabuleiro
            // 
            this.pbTabuleiro.BackgroundImage = global::BatalhaNaval_Visual.Properties.Resources.ocean;
            this.pbTabuleiro.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbTabuleiro.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbTabuleiro.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbTabuleiro.Location = new System.Drawing.Point(267, 241);
            this.pbTabuleiro.Name = "pbTabuleiro";
            this.pbTabuleiro.Size = new System.Drawing.Size(314, 239);
            this.pbTabuleiro.TabIndex = 0;
            this.pbTabuleiro.TabStop = false;
            this.pbTabuleiro.DragDrop += new System.Windows.Forms.DragEventHandler(this.pbTabuleiro_DragDrop);
            this.pbTabuleiro.DragEnter += new System.Windows.Forms.DragEventHandler(this.pbTabuleiro_DragEnter);
            this.pbTabuleiro.Paint += new System.Windows.Forms.PaintEventHandler(this.pbTabuleiro_Paint);
            this.pbTabuleiro.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.pbTabuleiro_PreviewKeyDown);
            // 
            // pbInimigo
            // 
            this.pbInimigo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pbInimigo.Image = global::BatalhaNaval_Visual.Properties.Resources.ocean;
            this.pbInimigo.Location = new System.Drawing.Point(267, 0);
            this.pbInimigo.Name = "pbInimigo";
            this.pbInimigo.Size = new System.Drawing.Size(314, 241);
            this.pbInimigo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbInimigo.TabIndex = 10;
            this.pbInimigo.TabStop = false;
            this.pbInimigo.Visible = false;
            this.pbInimigo.Paint += new System.Windows.Forms.PaintEventHandler(this.pbInimigo_Paint);
            this.pbInimigo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbInimigo_MouseMove);
            // 
            // FormTabuleiro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 480);
            this.Controls.Add(this.splitterTabuleiros);
            this.Controls.Add(this.pbTabuleiro);
            this.Controls.Add(this.pbInimigo);
            this.Controls.Add(this.splitterRight);
            this.Controls.Add(this.splitterLeft);
            this.Controls.Add(this.tlpNavios);
            this.Controls.Add(this.panelConectar);
            this.Name = "FormTabuleiro";
            this.Text = "Batalha Naval";
            this.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.Form_GiveFeedback);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_MouseUp);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.pbTabuleiro_PreviewKeyDown);
            this.tlpNavios.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSubmarino)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDestroier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCruzador)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbEncouracado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPortaAvioes)).EndInit();
            this.panelConectar.ResumeLayout(false);
            this.panelConectar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTabuleiro)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbInimigo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbTabuleiro;
        private System.Windows.Forms.TableLayoutPanel tlpNavios;
        private System.Windows.Forms.PictureBox pbSubmarino;
        private System.Windows.Forms.PictureBox pbDestroier;
        private System.Windows.Forms.PictureBox pbCruzador;
        private System.Windows.Forms.PictureBox pbEncouracado;
        private System.Windows.Forms.PictureBox pbPortaAvioes;
        private System.Windows.Forms.Splitter splitterLeft;
        private System.Windows.Forms.Panel panelConectar;
        private System.Windows.Forms.Splitter splitterRight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbDisponiveis;
        private System.Windows.Forms.Button btnConectar;
        private System.Windows.Forms.PictureBox pbInimigo;
        private System.Windows.Forms.Splitter splitterTabuleiros;
        private System.Windows.Forms.Button btnClear;
    }
}

