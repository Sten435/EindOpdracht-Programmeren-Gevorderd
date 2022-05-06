using Domein;
using System;
using System.Data.SqlClient;

namespace Persistentie {
	public class ConfigMapper {
		public void LoadConfig() {
			//Laad Config Table en zet alle statiche propperties die aanpasbaar zijn door de beheerder gelijk.
			//TijdsSlot.SlotTijdUur = SELECT SlotTijdUur FROM Config;
			//Toestel.StandaardInherstelling = SELECT StandaardInherstelling FROM Config;

			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("SELECT SlotTijdUur, StandaardInherstelling, LowerBoundUurReservatie, UpperBoundUurReservatie FROM Config;", connection);
				using SqlDataReader dataFromQuery = command.ExecuteReader();

				if (dataFromQuery.HasRows) {
					while (dataFromQuery.Read()) {
						double slotTijdUur = (long)dataFromQuery["SlotTijdUur"];
						bool standaardInherstelling = (bool)dataFromQuery["StandaardInherstelling"];
						int lowerBoundUurReservatie = (int)(double)dataFromQuery["LowerBoundUurReservatie"];
						int upperBoundUurReservatie = (int)(double)dataFromQuery["UpperBoundUurReservatie"];

						TijdsSlot.SlotTijdUur = slotTijdUur;
						Toestel.StandaardInherstelling = standaardInherstelling;
					}
				} else throw new NoConfigDataInDbException("Er bevind zich geen config data in de databank.");
			} catch (Exception err) {
				throw new SelectConfigFromDbException($"Fout bij select config data uit DB: {err.Message}");
			}
		}
	}
}
