using Domein;
using System.Collections.Generic;

namespace Persistentie {
	public class ToestellenMapper : IToestelRepository {
		UniekeCode _uniekeCode = UniekeCode.Instance;
		private List<Toestel> _toestellen = new List<Toestel>() {
			new Toestel(1, "Fiets", false),
			new Toestel(2, "Loopband", false),
			new Toestel(3, "Roeien", true)
		};

		public List<Toestel> GeefAlleToestellen() => _toestellen;

		public void VoegToestelToe(string naam) {
			Toestel toestel = new(_uniekeCode.GenereerRandomCode(), naam, false);
			_toestellen.Add(toestel);
		}

		public void VerwijderToestel(Toestel toestel) => _toestellen.Remove(toestel);

		public void ZetToestelInHerstelling(Toestel toestel) {
			for (int i = 0; i < _toestellen.Count; i++) {
				if (toestel.IdentificatieCode == _toestellen[i].IdentificatieCode) {
					_toestellen[i].InHerstelling = true;
					break;
				}
			}
		}
	}
}
