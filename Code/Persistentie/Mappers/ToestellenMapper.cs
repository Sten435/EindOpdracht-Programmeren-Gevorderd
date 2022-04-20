using Domein;
using System.Collections.Generic;
using System.Linq;

namespace Persistentie {

	public class ToestellenMapper : IToestelRepository {
		private static UniekeCode _uniekeCode = UniekeCode.Instance;

		private List<Toestel> _toestellen = new List<Toestel>() {
			new Toestel(1, "Fiets", false),
			new Toestel(2, "Loopband", false),
			new Toestel(3, "Roeien", true)
		};

		public List<Toestel> GeefAlleToestellen() => _toestellen;

		public void VoegToestelToe(string naam) => _toestellen.Add(new(_uniekeCode.GenereerRandomCode(), naam, false));

		public void VerwijderToestel(Toestel toestel) => _toestellen.Remove(toestel);

		public void ZetToestelInOfUitHerstelling(Toestel toestel) {
			Toestel t = _toestellen.Where(t => t.IdentificatieCode == toestel.IdentificatieCode).Single();
			t.InHerstelling = !t.InHerstelling;
		}
	}
}