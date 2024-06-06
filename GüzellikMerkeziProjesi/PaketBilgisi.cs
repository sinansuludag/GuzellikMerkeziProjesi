using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GüzellikMerkeziProjesi
{
    internal class PaketBilgisi
    {
        public string İslem { get; set; }
        public float Seans { get; set; }
        public float Ucret { get; set; }

        public PaketBilgisi(string islem, float seans, float ucret)
        {
            İslem = islem;
            Seans = seans;
            Ucret = ucret;
        }

        public override string ToString()
        {
            return $"{İslem},{Seans},{Ucret}";
        }
    }
}
