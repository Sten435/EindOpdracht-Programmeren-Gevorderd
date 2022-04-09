using Domein;
using System.Collections.Generic;

namespace Persistentie {
	public class ToestellenMapper : IToestelRepository {
		UniekeCode _uniekeCode = UniekeCode.Instance;
		public List<Toestel> Toestellen { get; set; } = new List<Toestel>() {
			new Toestel(1, "Fiets", false),
			new Toestel(2, "Loopband", false),
			new Toestel(3, "Roeien", false)
		};

		public List<Toestel> GeefAlleToestellen() => Toestellen;

		public void VoegToestelToe(string naam) {
			Toestel toestel = new(_uniekeCode.GenereerRandomCode(), naam, false);
			Toestellen.Add(toestel);
		}

		public void VerwijderToestel(Toestel toestel) => Toestellen.Remove(toestel);

		public void ZetToestelInHerstelling(Toestel toestel) {
			for (int i = 0; i < Toestellen.Count; i++) {
				if (toestel.IdentificatieCode == Toestellen[i].IdentificatieCode) {
					Toestellen[i].InHerstelling = true;
					break;
				}
			}
		}
	}
}
