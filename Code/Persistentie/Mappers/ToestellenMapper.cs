using Domein;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Persistentie {

	public static class ToestellenMapper {

		public static List<Toestel> GeefToestellen() {
			List<Toestel> toestellen = new();

			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("SELECT IdentificatieCode, ToestelType, InHerstelling FROM Toestellen WHERE Verwijderd = 0;", connection);

				using SqlDataReader dataReader = command.ExecuteReader();

				if (dataReader.HasRows) {
					while (dataReader.Read()) {
						int identificatieCode = (int)dataReader["IdentificatieCode"];
						string toestelType = (string)dataReader["ToestelType"];
						bool inHerstelling = (bool)dataReader["InHerstelling"];

						Toestel toestel = new(identificatieCode, toestelType, inHerstelling);

						toestellen.Add(toestel);
					}
				} else throw new ToestellenUitDbException("Geen Toestellen gevonden.");
			} catch (Exception error) {
				throw new ToestellenUitDbException(error.Message);
			}

			return toestellen;
		}

		public static void VoegToestelToe(string naam) {
			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("INSERT INTO Toestellen (ToestelType, InHerstelling) VALUES (@ToestelType, @InHerstelling);", connection);

				command.Parameters.AddWithValue("@ToestelType", naam.Trim());
				command.Parameters.AddWithValue("@InHerstelling", Toestel.StandaardInherstelling);

				command.ExecuteNonQuery();
			} catch (Exception error) {
				throw new ToestellenToeVoegenException(error.Message);
			}
		}

		public static void VerwijderToestel(Toestel huidigToestel) {
			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("UPDATE Toestellen SET Verwijderd = 1 WHERE ToestelType = @ToestelType AND IdentificatieCode = @IdentificatieCode;", connection);

				command.Parameters.AddWithValue("@ToestelType", huidigToestel.ToestelType);
				command.Parameters.AddWithValue("@IdentificatieCode", huidigToestel.IdentificatieCode);

				command.ExecuteNonQuery();
			} catch (Exception error) {
				throw new ToestellenVerwijderenException(error.Message);
			}
		}

		public static void UpdateToestelNaam(Toestel huidigToestel) {
			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("UPDATE Toestellen SET ToestelType = @ToestelType WHERE IdentificatieCode = @IdentificatieCode;", connection);

				command.Parameters.AddWithValue("@IdentificatieCode", huidigToestel.IdentificatieCode);
				command.Parameters.AddWithValue("@ToestelType", huidigToestel.ToestelType);

				command.ExecuteNonQuery();
			} catch (Exception error) {
				throw new ToestellenZetNaamExcaption(error.Message);
			}
		}

		public static void UpdateToestelInHerstelling(Toestel huidigToestel) {
			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("UPDATE Toestellen SET InHerstelling = @InHerstelling WHERE ToestelType = @ToestelType AND IdentificatieCode = @IdentificatieCode;", connection);

				command.Parameters.AddWithValue("@IdentificatieCode", huidigToestel.IdentificatieCode);
				command.Parameters.AddWithValue("@ToestelType", huidigToestel.ToestelType);
				command.Parameters.AddWithValue("@InHerstelling", huidigToestel.InHerstelling);

				command.ExecuteNonQuery();
			} catch (Exception error) {
				throw new ToestellenZetInHerstellingException(error.Message);
			}
		}
	}
}