using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using SharpShell.Attributes;
using SharpShell.SharpThumbnailHandler;

namespace OcifThumb
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".pic")]
    public class OcifThumbnailHandler : SharpThumbnailHandler
    {
        private static readonly uint[] Palette = {
            0x000000,0x000040,0x000080,0x0000BF,0x0000FF,0x002400,0x002440,0x002480,0x0024BF,0x0024FF,
            0x004900,0x004940,0x004980,0x0049BF,0x0049FF,0x006D00,0x006D40,0x006D80,0x006DBF,0x006DFF,
            0x009200,0x009240,0x009280,0x0092BF,0x0092FF,0x00B600,0x00B640,0x00B680,0x00B6BF,0x00B6FF,
            0x00DB00,0x00DB40,0x00DB80,0x00DBBF,0x00DBFF,0x00FF00,0x00FF40,0x00FF80,0x00FFBF,0x00FFFF,
            0x0F0F0F,0x1E1E1E,0x2D2D2D,0x330000,0x330040,0x330080,0x3300BF,0x3300FF,0x332400,0x332440,
            0x332480,0x3324BF,0x3324FF,0x334900,0x334940,0x334980,0x3349BF,0x3349FF,0x336D00,0x336D40,
            0x336D80,0x336DBF,0x336DFF,0x339200,0x339240,0x339280,0x3392BF,0x3392FF,0x33B600,0x33B640,
            0x33B680,0x33B6BF,0x33B6FF,0x33DB00,0x33DB40,0x33DB80,0x33DBBF,0x33DBFF,0x33FF00,0x33FF40,
            0x33FF80,0x33FFBF,0x33FFFF,0x3C3C3C,0x4B4B4B,0x5A5A5A,0x660000,0x660040,0x660080,0x6600BF,
            0x6600FF,0x662400,0x662440,0x662480,0x6624BF,0x6624FF,0x664900,0x664940,0x664980,0x6649BF,
            0x6649FF,0x666D00,0x666D40,0x666D80,0x666DBF,0x666DFF,0x669200,0x669240,0x669280,0x6692BF,
            0x6692FF,0x66B600,0x66B640,0x66B680,0x66B6BF,0x66B6FF,0x66DB00,0x66DB40,0x66DB80,0x66DBBF,
            0x66DBFF,0x66FF00,0x66FF40,0x66FF80,0x66FFBF,0x66FFFF,0x696969,0x787878,0x878787,0x969696,
            0x990000,0x990040,0x990080,0x9900BF,0x9900FF,0x992400,0x992440,0x992480,0x9924BF,0x9924FF,
            0x994900,0x994940,0x994980,0x9949BF,0x9949FF,0x996D00,0x996D40,0x996D80,0x996DBF,0x996DFF,
            0x999200,0x999240,0x999280,0x9992BF,0x9992FF,0x99B600,0x99B640,0x99B680,0x99B6BF,0x99B6FF,
            0x99DB00,0x99DB40,0x99DB80,0x99DBBF,0x99DBFF,0x99FF00,0x99FF40,0x99FF80,0x99FFBF,0x99FFFF,
            0xA5A5A5,0xB4B4B4,0xC3C3C3,0xCC0000,0xCC0040,0xCC0080,0xCC00BF,0xCC00FF,0xCC2400,0xCC2440,
            0xCC2480,0xCC24BF,0xCC24FF,0xCC4900,0xCC4940,0xCC4980,0xCC49BF,0xCC49FF,0xCC6D00,0xCC6D40,
            0xCC6D80,0xCC6DBF,0xCC6DFF,0xCC9200,0xCC9240,0xCC9280,0xCC92BF,0xCC92FF,0xCCB600,0xCCB640,
            0xCCB680,0xCCB6BF,0xCCB6FF,0xCCDB00,0xCCDB40,0xCCDB80,0xCCDBBF,0xCCDBFF,0xCCFF00,0xCCFF40,
            0xCCFF80,0xCCFFBF,0xCCFFFF,0xD2D2D2,0xE1E1E1,0xF0F0F0,0xFF0000,0xFF0040,0xFF0080,0xFF00BF,
            0xFF00FF,0xFF2400,0xFF2440,0xFF2480,0xFF24BF,0xFF24FF,0xFF4900,0xFF4940,0xFF4980,0xFF49BF,
            0xFF49FF,0xFF6D00,0xFF6D40,0xFF6D80,0xFF6DBF,0xFF6DFF,0xFF9200,0xFF9240,0xFF9280,0xFF92BF,
            0xFF92FF,0xFFB600,0xFFB640,0xFFB680,0xFFB6BF,0xFFB6FF,0xFFDB00,0xFFDB40,0xFFDB80,0xFFDBBF,
            0xFFDBFF,0xFFFF00,0xFFFF40,0xFFFF80,0xFFFFBF,0xFFFFFF
        };

        private static readonly int[] BX = { 0, 0, 0, 1, 1, 1, 0, 1 };
        private static readonly int[] BY = { 0, 1, 2, 0, 1, 2, 3, 3 };

        protected override Bitmap GetThumbnailImage(uint width)
        {
            try
            {
                return DecodeOcif(SelectedItemStream);
            }
            catch
            {
                return null;
            }
        }

        private static uint ReadUtf8(BinaryReader r)
        {
            byte b = r.ReadByte();
            int extra; uint cp;
            if ((b & 0x80) == 0)         { cp = b;              extra = 0; }
            else if ((b & 0xE0) == 0xC0) { cp = (uint)(b&0x1F); extra = 1; }
            else if ((b & 0xF0) == 0xE0) { cp = (uint)(b&0x0F); extra = 2; }
            else                          { cp = (uint)(b&0x07); extra = 3; }
            for (int i = 0; i < extra; i++)
                cp = (cp << 6) | (uint)(r.ReadByte() & 0x3F);
            return cp;
        }

        private Bitmap DecodeOcif(Stream stream)
        {
            using (var r = new BinaryReader(stream))
            {
                byte[] sig = r.ReadBytes(4);
                if (sig[0]!='O'||sig[1]!='C'||sig[2]!='I'||sig[3]!='F') return null;
                if (r.ReadByte() != 0x08) return null;

                int W = r.ReadByte() + 1;
                int H = r.ReadByte() + 1;

                var img = new Bitmap(W * 2, H * 4);
                using (var g = Graphics.FromImage(img))
                    g.Clear(Color.Black);

                int ac = r.ReadByte() + 1;
                for (int a = 0; a < ac; a++)
                {
                    byte al = r.ReadByte();
                    int sc = ((r.ReadByte() << 8) | r.ReadByte()) + 1;
                    for (int s = 0; s < sc; s++)
                    {
                        uint cp = ReadUtf8(r);
                        int bgc = r.ReadByte() + 1;
                        for (int b = 0; b < bgc; b++)
                        {
                            uint bh = Palette[r.ReadByte()];
                            var bg = Color.FromArgb(255-al, (int)((bh>>16)&0xFF), (int)((bh>>8)&0xFF), (int)(bh&0xFF));
                            int fgc = r.ReadByte() + 1;
                            for (int f = 0; f < fgc; f++)
                            {
                                uint fh = Palette[r.ReadByte()];
                                var fg = Color.FromArgb(255, (int)((fh>>16)&0xFF), (int)((fh>>8)&0xFF), (int)(fh&0xFF));
                                int yc = r.ReadByte() + 1;
                                for (int y = 0; y < yc; y++)
                                {
                                    int yp = r.ReadByte();
                                    int xc = r.ReadByte() + 1;
                                    for (int x = 0; x < xc; x++)
                                    {
                                        int xp = r.ReadByte();
                                        int px = xp*2, py = yp*4;
                                        bool br = cp >= 0x2801 && cp <= 0x28FF;
                                        if (!br)
                                        {
                                            for (int dx=0;dx<2;dx++)
                                            for (int dy=0;dy<4;dy++)
                                                if (px+dx<img.Width && py+dy<img.Height)
                                                    img.SetPixel(px+dx, py+dy, bg);
                                        }
                                        else
                                        {
                                            uint mask = cp - 0x2800;
                                            for (int bit=0;bit<8;bit++)
                                            {
                                                Color c = (mask&(1u<<bit))!=0 ? fg : bg;
                                                if (px+BX[bit]<img.Width && py+BY[bit]<img.Height)
                                                    img.SetPixel(px+BX[bit], py+BY[bit], c);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return img;
            }
        }
    }
}