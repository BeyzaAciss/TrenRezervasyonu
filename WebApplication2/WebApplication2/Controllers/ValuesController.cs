using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication2.Controllers
{
    public class ValuesController : ApiController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("api/Values/RezervasyonKontrol")]
        public Models.Response.Root RezervasyonKontrol(Models.Root degerler)
        {
            Models.Response.Root rezerve = new Models.Response.Root();
            List<Models.Response.YerlesimAyrinti> yerlesimAyrinti = new List<Models.Response.YerlesimAyrinti>();
            if (degerler.KisilerFarkliVagonlaraYerlestirilebilir)
            {
                for (int i = 0; i < degerler.Tren.Vagonlar.Count; i++)
                {
                    int yerlesenkisiSayisi = 0;
                    while ((degerler.RezervasyonYapilacakKisiSayisi - (yerlesenkisiSayisi + yerlesimAyrinti.Sum(x => x.KisiSayisi))) != 0)
                    {
                        int vagonlardakiYerlesenKisiSayisi = 0;
                         
                        if ((Convert.ToDouble(degerler.Tren.Vagonlar[i].DoluKoltukAdet + 1)) / (Convert.ToDouble(degerler.Tren.Vagonlar[i].Kapasite)) <= 0.70)
                        {
                            yerlesenkisiSayisi++;
                            degerler.Tren.Vagonlar[i].DoluKoltukAdet++;
                        }
                        else
                            break;

                    }
                    if (yerlesenkisiSayisi > 0)
                    {
                        yerlesimAyrinti.Add(new Models.Response.YerlesimAyrinti()
                        {
                            KisiSayisi = yerlesenkisiSayisi,
                            VagonAdi = degerler.Tren.Vagonlar[i].Ad
                        });
                    }
                }
            }
            else
            {
                for (int i = 0; i < degerler.Tren.Vagonlar.Count; i++)
                {
                    if ((Convert.ToDouble(degerler.Tren.Vagonlar[i].DoluKoltukAdet + degerler.RezervasyonYapilacakKisiSayisi)) / (Convert.ToDouble(degerler.Tren.Vagonlar[i].Kapasite)) <= 0.70)
                    {
                        yerlesimAyrinti.Add(new Models.Response.YerlesimAyrinti()
                        {
                            KisiSayisi = degerler.RezervasyonYapilacakKisiSayisi,
                            VagonAdi = degerler.Tren.Vagonlar[i].Ad
                        });
                        break;
                    }
                }
            }
            rezerve.YerlesimAyrinti = yerlesimAyrinti;
            rezerve.RezervasyonYapilabilir = yerlesimAyrinti.Sum(x => x.KisiSayisi) == degerler.RezervasyonYapilacakKisiSayisi ? true : false;

            return rezerve;
        }
    }
}
