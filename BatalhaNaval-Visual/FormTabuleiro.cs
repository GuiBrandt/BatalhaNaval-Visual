using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BatalhaNaval;
using System.Runtime.InteropServices;
using System.Net;
using System.Threading.Tasks;

namespace BatalhaNaval_Visual
{
    public partial class FormTabuleiro : Form
    {
        ClienteP2P cliente;
        Tabuleiro tabuleiro;
        IntPtr cursor = IntPtr.Zero;
        Bitmap thumb = null;
        TipoDeNavio? dragged = null;
        int direcao = 3;

        /// <summary>
        /// Construtor
        /// </summary>
        public FormTabuleiro()
        {
            InitializeComponent();

            tabuleiro = new Tabuleiro();
            pbTabuleiro.AllowDrop = true;
        }

        /// <summary>
        /// Obtém a picture box para um tipo de navio
        /// </summary>
        /// <param name="t">Tipo de navio</param>
        /// <returns>A picture box do tipo de navio dado</returns>
        private PictureBox GetPictureBoxParaTipoDeNavio(TipoDeNavio t)
        {
            switch (t)
            {
                case TipoDeNavio.PortaAvioes:
                    return pbPortaAvioes;
                case TipoDeNavio.Encouracado:
                    return pbEncouracado;
                case TipoDeNavio.Cruzador:
                    return pbCruzador;
                case TipoDeNavio.Destroier:
                    return pbDestroier;
                default:
                    return pbSubmarino;
            }
        }

        /// <summary>
        /// Obtém uma imagem para um tipo de navio
        /// </summary>
        /// <param name="t">Tipo de navio</param>
        /// <param name="d">Direção do navio</param>
        /// <returns>Uma imagem para o tipo de navio pedido</returns>
        private Image GetImagemParaTipoDeNavio(TipoDeNavio t, int d)
        {
            Bitmap bmp = new Bitmap(GetPictureBoxParaTipoDeNavio(t).Image);

            switch (d)
            {
                case 0:
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 1:
                    bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case 2:
                    bmp.RotateFlip(RotateFlipType.Rotate270FlipY);
                    break;
            }

            return bmp;
        }

        /// <summary>
        /// Desenho da picture box do tabuleiro
        /// </summary>
        private void pbTabuleiro_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            float wColuna = (float)pbTabuleiro.Width / tabuleiro.NumeroDeColunas,
                hLinha = (float)pbTabuleiro.Height / tabuleiro.NumeroDeLinhas;

            using (Pen p = new Pen(Color.Black, 2))
            {
                for (int i = 1; i < tabuleiro.NumeroDeLinhas; i++)
                    g.DrawLine(p, 0, i * hLinha, pbTabuleiro.Width, i * hLinha);

                for (int i = 1; i < tabuleiro.NumeroDeColunas; i++)
                    g.DrawLine(p, i * wColuna, 0, i * wColuna, pbTabuleiro.Height);
            }

            foreach (KeyValuePair<int[], TipoDeNavio> navio in tabuleiro.Navios)
            {
                int d = navio.Key[2];

                PointF pos;
                SizeF size;

                switch (d)
                {                        
                    case 1:
                        pos = new PointF(wColuna * (navio.Key[0] + 1 - navio.Value.Tamanho()), hLinha * navio.Key[1]);
                        break;
                    case 2:
                        pos = new PointF(wColuna * navio.Key[0], hLinha * (navio.Key[1] + 1 - navio.Value.Tamanho()));
                        break;
                    default:
                        pos = new PointF(wColuna * navio.Key[0], hLinha * navio.Key[1]);
                        break;
                }

                switch (d)
                {
                    case 1:
                    case 3:
                        size = new SizeF(wColuna * navio.Value.Tamanho(), hLinha);
                        break;
                    default:
                        size = new SizeF(wColuna, hLinha * navio.Value.Tamanho());
                        break;
                }

                Image img = GetImagemParaTipoDeNavio(navio.Value, d);
                g.DrawImage(img, new RectangleF(pos, size));
            }
        }

        /// <summary>
        /// Informações de ícone
        /// </summary>
        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        /// <summary>
        /// Evento de mouse apertado em qualquer uma das pictureBox com imagem de navio
        /// </summary>
        private void pbNavio_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            
            Bitmap bmp = (Bitmap)pb.Image;
            dragged = (TipoDeNavio)Enum.Parse(typeof(TipoDeNavio), (string)pb.Tag);
            CreateThumbnail(dragged ?? TipoDeNavio.Submarino);

            DoDragDrop((string)pb.Tag, DragDropEffects.All);
        }

        /// <summary>
        /// Cria o thumbnail
        /// </summary>
        /// <param name="bmp">Bitmap para a imagem do cursor</param>
        void CreateThumbnail(TipoDeNavio t)
        {
            thumb = (Bitmap)GetImagemParaTipoDeNavio(t, direcao);

            IntPtr ptr = thumb.GetHicon();
            IconInfo tmp = new IconInfo();
            GetIconInfo(ptr, ref tmp);
            tmp.xHotspot = 0;
            tmp.yHotspot = 0;

            if (direcao == 1)
            {
                tmp.xHotspot = thumb.Width;
                tmp.yHotspot = 0;
            }
            else if (direcao == 2)
            {
                tmp.xHotspot = 0;
                tmp.yHotspot = thumb.Height;
            }
            else
            {
                tmp.xHotspot = 0;
                tmp.yHotspot = 0;
            }

            tmp.fIcon = false;
            ptr = CreateIconIndirect(ref tmp);
            
            cursor = ptr;
        }

        /// <summary>
        /// Evento de DragDrop na picture box de tabuleiro
        /// </summary>
        private void pbTabuleiro_DragDrop(object sender, DragEventArgs e)
        {
            TipoDeNavio tipoDeNavio = dragged ?? TipoDeNavio.Submarino;

            Point p = pbTabuleiro.PointToClient(new Point(e.X, e.Y));
            int x = p.X * tabuleiro.NumeroDeColunas / pbTabuleiro.Width;
            int y = p.Y * tabuleiro.NumeroDeLinhas / pbTabuleiro.Height;

            try
            {
                tabuleiro.PosicionarNavio(tipoDeNavio, x, y, direcao);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Eita nóis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (tabuleiro.Contar(tipoDeNavio) == tipoDeNavio.Limite())
                GetPictureBoxParaTipoDeNavio(tipoDeNavio).Visible = false;

            pbTabuleiro.Invalidate();

            if (tabuleiro.EstaCompleto())
            {
                DialogResult r = MessageBox.Show(this, "Tabuleiro completo, continuar?", "Batalha Naval", MessageBoxButtons.YesNo);

                if (r == DialogResult.Yes)
                {
                    tlpNavios.Visible = false;
                    splitterLeft.Visible = false;

                    LoginDialog login = new LoginDialog();
                    r = login.ShowDialog(this);

                    if (r == DialogResult.OK)
                    {
                        cliente = new ClienteP2P(login.User, tabuleiro);
                        cliente.OnClienteDisponivel += Cliente_OnClienteDisponivel;
                        cliente.OnClienteRequisitandoConexao += Cliente_OnClienteRequisitandoConexao;
                        cliente.OnClienteConectado += Cliente_OnClienteConectado;
                        cliente.OnClienteDesconectado += Cliente_OnClienteDesconectado;
                        cliente.OnDarTiro += Cliente_OnDarTiro;
                        cliente.OnResultadoDeTiro += Cliente_OnResultadoDeTiro;
                        cliente.OnTiroRecebido += Cliente_OnTiroRecebido;
                        cliente.Iniciar();

                        panelConectar.Visible = true;
                        splitterRight.Visible = true;

                        return;
                    }
                    else
                    {
                        tlpNavios.Visible = true;
                        splitterLeft.Visible = true;
                    }
                }

                tabuleiro = new Tabuleiro();
                pbTabuleiro.Invalidate();

                foreach (TipoDeNavio t in (TipoDeNavio[])Enum.GetValues(typeof(TipoDeNavio)))
                    GetPictureBoxParaTipoDeNavio(t).Visible = true;
            }
        }

        private void Cliente_OnTiroRecebido(Tiro t)
        {
            throw new NotImplementedException();
        }

        private void Cliente_OnResultadoDeTiro(Tiro t, ResultadoDeTiro resultado)
        {
            throw new NotImplementedException();
        }

        private Tiro Cliente_OnDarTiro()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evento chamado quando o cliente antes conectado se desconecta
        /// </summary>
        /// <param name="addr">Endereço do cliente desconectado</param>
        private void Cliente_OnClienteDesconectado(System.Net.IPAddress addr)
        {
            tabuleiro = new Tabuleiro();
            pbTabuleiro.Invalidate();

            tlpNavios.Visible = true;
            cliente = null;
        }

        /// <summary>
        /// Evento chamado quando um cliente se conecta a este
        /// </summary>
        /// <param name="addr">Endereço do cliente conectado</param>
        private void Cliente_OnClienteConectado(System.Net.IPAddress addr)
        {
            panelConectar.Visible = false;
            splitterRight.Visible = false;
            splitterTabuleiros.Visible = true;
            pbInimigo.Visible = true;
        }

        /// <summary>
        /// Evento chamado quando um cliente quer se conectar com este
        /// </summary>
        /// <param name="addr">Endereço do cliente</param>
        /// <returns>True se deve aceitar a conexão e falso se não</returns>
        private bool Cliente_OnClienteRequisitandoConexao(System.Net.IPAddress addr)
        {
            DialogResult r = MessageBox.Show(this, string.Format("{0} ({1}) quer jogar com você. Aceitar?", cliente.NomeRemoto, addr), "Batalha Naval", MessageBoxButtons.YesNo);

            if (r == DialogResult.Yes)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Evento chamado quando um cliente está disponível para conexão
        /// </summary>
        /// <param name="addr">Endereço do cliente</param>
        private void Cliente_OnClienteDisponivel(System.Net.IPAddress addr)
        {
            if (InvokeRequired)
                Invoke(new Action(() => { Cliente_OnClienteDisponivel(addr); }));
            else if (!cbDisponiveis.Items.Contains(addr))
                cbDisponiveis.Items.Add(addr);
        }

        /// <summary>
        /// Evento de feedback do form (para o thumbnail)
        /// </summary>
        private void Form_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (cursor == IntPtr.Zero) return;

            e.UseDefaultCursors = false;
            Cursor.Current = new Cursor(cursor);
        }

        /// <summary>
        /// Evento de aceitação de dragdrop da picture box do tabuleiro
        /// </summary>
        private void pbTabuleiro_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
                e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// Evento de mouse levantado no form
        /// </summary>
        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            cursor = IntPtr.Zero;
        }

        /// <summary>
        /// Clique no botão Conectar
        /// </summary>
        private void btnConectar_Click(object sender, EventArgs e)
        {
            btnConectar.Enabled = false;

            Task.Run(() =>
            {
                Invoke(new Action(() => {
                    try
                    {
                        MessageBox.Show(cbDisponiveis.SelectedItem.ToString());
                        if (!cliente.SolicitarConexao((IPAddress)cbDisponiveis.SelectedItem))
                        {
                            MessageBox.Show(this, "O cliente remoto recusou a conexão =(", "Batalha Naval", MessageBoxButtons.OK);
                            btnConectar.Enabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Não foi possível conectar ao cliente remoto. Tento novamente mais tarde.\r\n" + ex.Message, "Batalha Naval", MessageBoxButtons.OK);
                        cbDisponiveis.Items.RemoveAt(cbDisponiveis.SelectedIndex);
                        btnConectar.Enabled = true;
                    }
                }));
            });
        }

        /// <summary>
        /// Evento de tecla pressionada
        /// </summary>
        private void pbTabuleiro_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    direcao = 1;
                    break;

                case Keys.Right:
                    direcao = 3;
                    break;

                case Keys.Up:
                    direcao = 2;
                    break;

                case Keys.Down:
                    direcao = 0;
                    break;
            }

            tlpNavios.Refresh();

            if (cursor == IntPtr.Zero || thumb == null)
                return;

            CreateThumbnail(dragged ?? TipoDeNavio.Submarino);
            Cursor.Current = new Cursor(cursor);
        }
    }
}
