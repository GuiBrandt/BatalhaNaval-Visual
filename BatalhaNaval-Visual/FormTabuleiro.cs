﻿using System;
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
        ClienteP2P _cliente;
        Tabuleiro _tabuleiro;
        IntPtr _cursor = IntPtr.Zero;
        Bitmap _thumb = null;
        TipoDeNavio? _dragged = null;
        Direcao _dir = Direcao.Direita;

        /// <summary>
        /// Construtor
        /// </summary>
        public FormTabuleiro()
        {
            InitializeComponent();

            _tabuleiro = new Tabuleiro();
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
        private Image GetImagemParaTipoDeNavio(TipoDeNavio t, Direcao d)
        {
            Bitmap bmp;

            switch (t)
            {
                case TipoDeNavio.Cruzador:
                    bmp = Properties.Resources.cruzador;
                    break;

                case TipoDeNavio.Destroier:
                    bmp = Properties.Resources.destroier;
                    break;

                case TipoDeNavio.Encouracado:
                    bmp = Properties.Resources.encouracado;
                    break;

                case TipoDeNavio.PortaAvioes:
                    bmp = Properties.Resources.porta_avioes;
                    break;

                default:
                    bmp = Properties.Resources.submarino;
                    break;
            }

            switch (d)
            {
                case Direcao.Baixo:
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case Direcao.Esquerda:
                    bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case Direcao.Cima:
                    bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            Size size;
            switch (d)
            {
                case Direcao.Esquerda:
                case Direcao.Direita:
                    size = new Size(pbTabuleiro.Width / 10 * t.Tamanho(), pbTabuleiro.Height / 10);
                    break;
                default:
                    size = new Size(pbTabuleiro.Width / 10, pbTabuleiro.Height / 10 * t.Tamanho());
                    break;
            }

            return new Bitmap(bmp, size);
        }

        /// <summary>
        /// Desenho da picture box do tabuleiro
        /// </summary>
        private void pbTabuleiro_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            float wColuna = (float)pbTabuleiro.Width / _tabuleiro.NumeroDeColunas,
                hLinha = (float)pbTabuleiro.Height / _tabuleiro.NumeroDeLinhas;

            using (Pen p = new Pen(Color.Black, 2))
            {
                for (int i = 1; i < _tabuleiro.NumeroDeLinhas; i++)
                    g.DrawLine(p, 0, i * hLinha, pbTabuleiro.Width, i * hLinha);

                for (int i = 1; i < _tabuleiro.NumeroDeColunas; i++)
                    g.DrawLine(p, i * wColuna, 0, i * wColuna, pbTabuleiro.Height);
            }

            foreach (KeyValuePair<int[], TipoDeNavio> navio in _tabuleiro.Navios)
            {
                int d = navio.Key[2];

                PointF pos;

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

                Image img = GetImagemParaTipoDeNavio(navio.Value, (Direcao)d);
                g.DrawImage(img, pos);
            }

            using (Image hit = new Bitmap(Properties.Resources.acerto, new Size(pbInimigo.Width / 10, pbInimigo.Height / 10)))
            using (Image miss = new Bitmap(Properties.Resources.agua, new Size(pbInimigo.Width / 10, pbInimigo.Height / 10)))
            {
                if (_cliente != null)
                    foreach (Tiro tiro in _cliente.TirosRecebidos)
                    {
                        PointF pos = new PointF(tiro.X * pbInimigo.Width / 10, tiro.Y * pbInimigo.Height / 10);

                        if (_cliente.TirosRecebidos.Resultado(tiro).HasFlag(ResultadoDeTiro.Acertou))
                            e.Graphics.DrawImage(hit, pos);
                        else
                            e.Graphics.DrawImage(miss, pos);
                    }
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
            _dragged = (TipoDeNavio)Enum.Parse(typeof(TipoDeNavio), (string)pb.Tag);
            CreateThumbnail(_dragged ?? TipoDeNavio.Submarino);

            DoDragDrop((string)pb.Tag, DragDropEffects.All);
        }

        /// <summary>
        /// Cria o thumbnail
        /// </summary>
        /// <param name="bmp">Bitmap para a imagem do cursor</param>
        void CreateThumbnail(TipoDeNavio t)
        {
            _thumb = (Bitmap)GetImagemParaTipoDeNavio(t, _dir);

            IntPtr ptr = _thumb.GetHicon();
            IconInfo tmp = new IconInfo();
            GetIconInfo(ptr, ref tmp);
            tmp.xHotspot = 0;
            tmp.yHotspot = 0;

            if (_dir == Direcao.Esquerda)
            {
                tmp.xHotspot = _thumb.Width;
                tmp.yHotspot = _thumb.Height / 2;
            }
            else if (_dir == Direcao.Cima)
            {
                tmp.xHotspot = _thumb.Width / 2;
                tmp.yHotspot = _thumb.Height;
            }
            else if (_dir == Direcao.Direita)
            {
                tmp.xHotspot = 0;
                tmp.yHotspot = _thumb.Height / 2;
            }
            else if (_dir == Direcao.Baixo)
            {
                tmp.xHotspot = _thumb.Width / 2;
                tmp.yHotspot = 0;
            }

            tmp.fIcon = false;
            ptr = CreateIconIndirect(ref tmp);

            _cursor = ptr;
        }

        /// <summary>
        /// Evento de DragDrop na picture box de tabuleiro
        /// </summary>
        private void pbTabuleiro_DragDrop(object sender, DragEventArgs e)
        {
            TipoDeNavio tipoDeNavio = _dragged ?? TipoDeNavio.Submarino;

            Point p = pbTabuleiro.PointToClient(new Point(e.X, e.Y));
            int x = p.X * _tabuleiro.NumeroDeColunas / pbTabuleiro.Width;
            int y = p.Y * _tabuleiro.NumeroDeLinhas / pbTabuleiro.Height;

            try
            {
                _tabuleiro.PosicionarNavio(tipoDeNavio, x, y, (Direcao)_dir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Eita nóis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (_tabuleiro.Contar(tipoDeNavio) == tipoDeNavio.Limite())
                GetPictureBoxParaTipoDeNavio(tipoDeNavio).Visible = false;

            pbTabuleiro.Invalidate();

            if (_tabuleiro.EstaCompleto())
            {
                DialogResult r = MessageBox.Show(this, "Tabuleiro completo, continuar?", "Batalha Naval", MessageBoxButtons.YesNo);

                if (r == DialogResult.Yes)
                {
                    Width -= tlpNavios.Width;
                    tlpNavios.Visible = false;
                    splitterLeft.Visible = false;

                    LoginDialog login = new LoginDialog();
                    r = login.ShowDialog(this);

                    if (r == DialogResult.OK)
                    {
                        _cliente = new ClienteP2P(login.User, _tabuleiro);
                        _cliente.OnClienteDisponivel += Cliente_OnClienteDisponivel;
                        _cliente.OnClienteIndisponivel += Cliente_OnClienteIndisponivel;
                        _cliente.OnClienteRequisitandoConexao += Cliente_OnClienteRequisitandoConexao;
                        _cliente.OnClienteConectado += Cliente_OnClienteConectado;
                        _cliente.OnClienteDesconectado += Cliente_OnClienteDesconectado;
                        _cliente.OnDarTiro += Cliente_OnDarTiro;
                        _cliente.OnResultadoDeTiro += Cliente_OnResultadoDeTiro;
                        _cliente.OnTiroRecebido += Cliente_OnTiroRecebido;
                        _cliente.Iniciar();

                        panelConectar.Visible = true;
                        Width += panelConectar.Width;
                        splitterRight.Visible = true;

                        return;
                    }
                    else
                    {
                        Width += tlpNavios.Width;
                        tlpNavios.Visible = true;
                        splitterLeft.Visible = true;
                    }
                }

                _tabuleiro = new Tabuleiro();
                pbTabuleiro.Invalidate();

                foreach (TipoDeNavio t in (TipoDeNavio[])Enum.GetValues(typeof(TipoDeNavio)))
                    GetPictureBoxParaTipoDeNavio(t).Visible = true;
            }
        }

        /// <summary>
        /// Evento de sinalização de quando um cliente sai da rede
        /// </summary>
        /// <param name="addr">Endereço do cliente que desconectou</param>
        private void Cliente_OnClienteIndisponivel(IPAddress addr)
        {
            Invoke(new Action(() =>
            {
                try
                {
                    cbDisponiveis.Items.Remove(addr);
                }
                catch { }
            }));
        }

        /// <summary>
        /// Recebeu um tiro
        /// </summary>
        /// <param name="t">Tiro recebido</param>
        private void Cliente_OnTiroRecebido(Tiro t)
        {
            ResultadoDeTiro r = t.Aplicar(_tabuleiro);
            Invoke(new Action(() =>
            {
                pbTabuleiro.Invalidate();

                ResultadoDeTiro rs = _cliente.TirosRecebidos.Resultado(t);

                if (rs.HasFlag(ResultadoDeTiro.Ganhou))
                {
                    MessageBox.Show(this, "Você perdeu", Text);
                    _cliente.Close();
                }

                if (rs.HasFlag(ResultadoDeTiro.Afundou))
                    MessageBox.Show(this, "Seu " + Enum.GetName(typeof(TipoDeNavio), rs.TipoDeNavio()) + " foi afundado");
            }));
        }

        /// <summary>
        /// Recebeu o resultado de um tiro dado
        /// </summary>
        /// <param name="t">Tiro dado</param>
        /// <param name="resultado">Resultado do tiro</param>
        private void Cliente_OnResultadoDeTiro(Tiro t, ResultadoDeTiro resultado)
        {
            Invoke(new Action(() =>
            {
                pbInimigo.Invalidate();

                if (resultado.HasFlag(ResultadoDeTiro.Ganhou))
                {
                    MessageBox.Show(this, "Você ganhou!", Text);
                    _cliente.Close();
                }

                if (resultado.HasFlag(ResultadoDeTiro.Afundou))
                    MessageBox.Show(this, "Você afundou um " + Enum.GetName(typeof(TipoDeNavio), resultado.TipoDeNavio()) + " inimigo");
            }));
        }

        /// <summary>
        /// Sinaliza que deve-se dar um tiro
        /// </summary>
        private void Cliente_OnDarTiro()
        {
            Invoke(new Action(() =>
            {
                pbInimigo.Enabled = true;
                timeout.Start();
            }));
        }

        /// <summary>
        /// Evento chamado quando o cliente antes conectado se desconecta
        /// </summary>
        /// <param name="addr">Endereço do cliente desconectado</param>
        private void Cliente_OnClienteDesconectado(IPAddress addr)
        {
            Invoke(new Action(() =>
            {
                _tabuleiro = new Tabuleiro();
                pbTabuleiro.Invalidate();                

                pbInimigo.Visible = splitterTabuleiros.Visible = false;

                tlpNavios.Visible = true;

                Height /= 2;
                Width += tlpNavios.Width;

                foreach (TipoDeNavio t in (TipoDeNavio[])Enum.GetValues(typeof(TipoDeNavio)))
                    GetPictureBoxParaTipoDeNavio(t).Visible = true;
            }));

            _cliente = null;
        }

        /// <summary>
        /// Evento chamado quando um cliente se conecta a este
        /// </summary>
        /// <param name="addr">Endereço do cliente conectado</param>
        private void Cliente_OnClienteConectado(IPAddress addr)
        {
            Invoke(new Action(() =>
            {
                Width -= panelConectar.Width;

                Height *= 2;
                pbInimigo.Height = Height / 2;

                panelConectar.Visible = false;
                splitterRight.Visible = false;
                splitterTabuleiros.Visible = true;
                pbInimigo.Visible = true;
            }));
        }

        /// <summary>
        /// Evento chamado quando um cliente quer se conectar com este
        /// </summary>
        /// <param name="addr">Endereço do cliente</param>
        /// <returns>True se deve aceitar a conexão e falso se não</returns>
        private bool Cliente_OnClienteRequisitandoConexao(IPAddress addr)
        {
            DialogResult r = DialogResult.No;
            Invoke(new Action(() => { 
                r = MessageBox.Show(this, string.Format("{0} ({1}) quer jogar com você. Aceitar?", _cliente.NomeRemoto, addr), "Batalha Naval", MessageBoxButtons.YesNo); 
            }));

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
            if (_cursor == IntPtr.Zero) return;

            e.UseDefaultCursors = false;
            Cursor.Current = new Cursor(_cursor);
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
            _cursor = IntPtr.Zero;
        }

        /// <summary>
        /// Clique no botão Conectar
        /// </summary>
        private void btnConectar_Click(object sender, EventArgs e)
        {
            btnConectar.Enabled = false;
            
            if (cbDisponiveis.SelectedIndex < 0)
            {
                btnConectar.Enabled = true;
                return;
            }

            IPAddress addr;

            try
            {
                addr = (IPAddress)cbDisponiveis.SelectedItem;
            }
            catch
            {
                MessageBox.Show(this, "Endereço inválido", "Batalha Naval", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    if (!_cliente.SolicitarConexao(addr))
                    {
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show(this, "O cliente remoto recusou a conexão =(", "Batalha Naval", MessageBoxButtons.OK);
                            btnConectar.Enabled = true;
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        MessageBox.Show(this, "Não foi possível conectar ao cliente remoto. Tento novamente mais tarde.\r\n" + ex.Message, "Batalha Naval", MessageBoxButtons.OK);
                        cbDisponiveis.Items.RemoveAt(cbDisponiveis.SelectedIndex);
                        btnConectar.Enabled = true;
                    }));
                }
            });
        }

        /// <summary>
        /// Cancela a operação de conexão e volta para a criação do tabuleiro
        /// </summary>
        private void btnVoltar_Click(object sender, EventArgs e)
        {
            _cliente.Close();
            _cliente = null;

            panelConectar.Visible = splitterRight.Visible = false;
            tlpNavios.Visible = splitterLeft.Visible = true;

            _tabuleiro = new Tabuleiro();
            pbTabuleiro.Invalidate();

            foreach (TipoDeNavio t in (TipoDeNavio[])Enum.GetValues(typeof(TipoDeNavio)))
                GetPictureBoxParaTipoDeNavio(t).Visible = true;
        }

        /// <summary>
        /// Limpa o tabuleiro
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            _tabuleiro = new Tabuleiro();
            pbTabuleiro.Invalidate();

            foreach (TipoDeNavio t in (TipoDeNavio[])Enum.GetValues(typeof(TipoDeNavio)))
                GetPictureBoxParaTipoDeNavio(t).Visible = true;
        }

        /// <summary>
        /// Redesenha o tabuleiro do inimigo quando o mouse se move sobre ele
        /// </summary>
        private void pbInimigo_MouseMove(object sender, MouseEventArgs e)
        {
            pbInimigo.Invalidate();
        }

        /// <summary>
        /// Evento de desenho do tabuleiro inimigo
        /// </summary>
        private void pbInimigo_Paint(object sender, PaintEventArgs e)
        {
            using (Image cursor = new Bitmap(Properties.Resources.cursor, new Size(pbInimigo.Width / 10, pbInimigo.Height / 10)))
            using (Image hit = new Bitmap(Properties.Resources.acerto, new Size(pbInimigo.Width / 10, pbInimigo.Height / 10)))
            using (Image miss = new Bitmap(Properties.Resources.agua, new Size(pbInimigo.Width / 10, pbInimigo.Height / 10)))
            {

                foreach (Tiro tiro in _cliente.TirosDados)
                {
                    PointF pos = new PointF(tiro.X * pbInimigo.Width / 10, tiro.Y * pbInimigo.Height / 10);

                    if (_cliente.TirosDados.Resultado(tiro).HasFlag(ResultadoDeTiro.Acertou))
                        e.Graphics.DrawImage(hit, pos);
                    else
                        e.Graphics.DrawImage(miss, pos);
                }

                if (!pbInimigo.Enabled)
                    return;

                Point p = pbInimigo.PointToClient(Cursor.Position);
                int x = p.X * _tabuleiro.NumeroDeColunas / pbInimigo.Width;
                int y = p.Y * _tabuleiro.NumeroDeLinhas / pbInimigo.Height;

                e.Graphics.DrawImage(cursor, new PointF(x * pbInimigo.Width / 10, y * pbInimigo.Height / 10));
            }
        }

        /// <summary>
        /// Timeout do tiro
        /// </summary>
        private void timeout_Tick(object sender, EventArgs e)
        {
            MessageBox.Show("Tempo limite esgotado");
            pbInimigo.Enabled = false;
        }

        /// <summary>
        /// Clique na picturebox do inimigo
        /// </summary>
        private void pbInimigo_MouseClick(object sender, MouseEventArgs e)
        {
            if (!pbInimigo.Enabled) return;

            Point p = new Point(e.X, e.Y);
            int x = p.X * _tabuleiro.NumeroDeColunas / pbInimigo.Width;
            int y = p.Y * _tabuleiro.NumeroDeLinhas / pbInimigo.Height;

            _cliente.DarTiro(x, y);
            timeout.Stop();
            pbInimigo.Enabled = false;
        }

        /// <summary>
        /// Gira os navios
        /// </summary>
        private void btnGirar_Click(object sender, EventArgs e)
        {
            _dir = (Direcao)(((int)_dir + 1) % 4);

            foreach (TipoDeNavio t in (TipoDeNavio[])Enum.GetValues(typeof(TipoDeNavio)))
            {
                PictureBox pb = GetPictureBoxParaTipoDeNavio(t);
                pb.Image = GetImagemParaTipoDeNavio(t, _dir);

                if ((int)_dir % 2 != 0)
                    pb.SizeMode = PictureBoxSizeMode.CenterImage;
                else
                    pb.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }
    }
}
