using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UI {
	public class ReservatieInfo {
		public string Status { get; set; }
		public string DatumKleur { get; set; }

		public int Id { get; set; }
		public DateTime Datum { get; set; }
		public string Toestel { get; set; }
		public string TijdSlot { get; set; }
		public DateTime StartTijd { get; set; }
		public DateTime EindTijd { get; set; }

		public ReservatieInfo(string reservatie) {
			Id = int.Parse(reservatie.Split("@|@")[0]);
			Datum = DateTime.Parse(reservatie.Split("@|@")[1]);
			StartTijd = DateTime.Parse(reservatie.Split("@|@")[2]);
			EindTijd = DateTime.Parse(reservatie.Split("@|@")[3]);
			TijdSlot = $"{StartTijd.ToString("HH:mm")} -> {EindTijd.ToString("HH:mm")}";
			Toestel = reservatie.Split("@|@")[4];

			if (Datum.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy")) {
				Status = "  VANDAAG  ";
				DatumKleur = "#FF59A6ED";
			} else if (Datum < DateTime.Now) {
				Status = "AFGELOPEN";
				DatumKleur = "#FFA5A5A5";
			} else if (Datum > DateTime.Now) {
				Status = " TOEKOMST ";
				DatumKleur = "#FF4DCA6C";
			}
		}
	}
}
