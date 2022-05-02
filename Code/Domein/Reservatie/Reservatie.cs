namespace Domein {

	public class Reservatie {
		public long ReservatieNummer { get; set; }
		public Klant Klant { get; }
		public TijdsSlot TijdsSlot { get; }
		public Toestel Toestel { get; }
		private UniekeCode uniekeCode = UniekeCode.Instance;

		public Reservatie(Klant klant, TijdsSlot tijdsSlot, Toestel toestel) {
			Klant = klant;
			TijdsSlot = tijdsSlot;
			Toestel = toestel;
			ReservatieNummer = uniekeCode.GenereerRandomCode();
		}

		public override string ToString() => $" Nr: {ReservatieNummer} - {Toestel.ToestelType} - Klant: {Klant.Voornaam} {Klant.Achternaam} - Datum: {TijdsSlot.StartTijd:d} - Tijdslot: {TijdsSlot.StartTijd:t}/uur -> {TijdsSlot.EindTijd:t}/uur ";
	}
}