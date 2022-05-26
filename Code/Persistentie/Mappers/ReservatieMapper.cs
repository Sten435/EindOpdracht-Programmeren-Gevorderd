using Domein;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Persistentie {

	public static class ReservatieMapper {

		public static List<Reservatie> GeefAlleReservaties(bool vandaagPlusToekomst = false, bool alleenVandaag = false, int klantenNummer = -1) {
			List<Reservatie> reservaties = new();

			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command;

				string klantenNummerTussenVoegsel = "";
				if (klantenNummer != -1)
					klantenNummerTussenVoegsel = $"AND Klant_KlantenNummer = {klantenNummer}";

				if (vandaagPlusToekomst)
					command = new($"SELECT * FROM Reservaties r join TijdSloten ts on ts.Reservatie_ReservatieNummer = ReservatieNummer WHERE DAY(StartTijd) >= DAY(GETDATE()) AND MONTH(StartTijd) >= MONTH(GETDATE()) AND YEAR(StartTijd) >= YEAR(GETDATE()) {klantenNummerTussenVoegsel} ORDER BY ReservatieNummer ASC;", connection);
				else if (alleenVandaag)
					command = new($"SELECT * FROM Reservaties r join TijdSloten ts on ts.Reservatie_ReservatieNummer = ReservatieNummer WHERE DAY(StartTijd) = DAY(GETDATE()) AND MONTH(StartTijd) = MONTH(GETDATE()) AND YEAR(StartTijd) = YEAR(GETDATE()) {klantenNummerTussenVoegsel} ORDER BY ReservatieNummer ASC;", connection);
				else
					command = new($"SELECT * FROM Reservaties r join TijdSloten ts on ts.Reservatie_ReservatieNummer = ReservatieNummer {(klantenNummer != -1 ? $"WHERE Klant_KlantenNummer = {klantenNummer}" : "")} ORDER BY ReservatieNummer ASC;", connection);

				Reservatie reservatie;

				using SqlDataReader reader = command.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read()) {
						int reservatieNummer = (int)reader["ReservatieNummer"];
						int _klantenNummer = (int)reader["Klant_KlantenNummer"];
						int toestelNummer = (int)reader["Toestel_IdentificatieCode"];
						DateTime startTijd = (DateTime)reader["StartTijd"];
						DateTime eindTijd = (DateTime)reader["EindTijd"];

						Klant klant = KlantenMapper.GeefKlant(_klantenNummer);
						Toestel toestel = ToestellenMapper.GeefToestel(toestelNummer);
						if (toestel != null) {
							TijdsSlot tijdsSlot = new(startTijd, eindTijd);

							reservatie = new(reservatieNummer, klant, tijdsSlot, toestel);
							reservaties.Add(reservatie);
						}
					}
				}
			} catch (SqlException) {
				throw new ReservatieException("(Select) Fout met query naar reservatie Db.");
			} catch (Exception) {
				throw new ReservatieException("(Select) Fout in reservatie Db.");
			}
			return reservaties;
		}

		public static List<Reservatie> GeefReservatiesPerToestel(string naam, DateTime? dag) {

			List<Reservatie> reservaties = new();

			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command;

				if (dag == null) {
					command = new($"SELECT * FROM Reservaties r join TijdSloten ts on ts.Reservatie_ReservatieNummer = ReservatieNummer join Toestellen toes on toes.IdentificatieCode = r.Toestel_IdentificatieCode WHERE toes.ToestelType = @Naam ORDER BY ReservatieNummer ASC;", connection);
				} else
					command = new($"SELECT * FROM Reservaties r join TijdSloten ts on ts.Reservatie_ReservatieNummer = ReservatieNummer join Toestellen toes on toes.IdentificatieCode = r.Toestel_IdentificatieCode WHERE toes.ToestelType = @Naam AND DAY(StartTijd) = DAY(@Dag) AND MONTH(StartTijd) = MONTH(@Dag) AND YEAR(StartTijd) = YEAR(@Dag) ORDER BY ReservatieNummer ASC;", connection);

				Reservatie reservatie;

				if (dag != null) {
					command.Parameters.AddWithValue("@Dag", dag);
				}
				command.Parameters.AddWithValue("@Naam", naam);

				using SqlDataReader reader = command.ExecuteReader();

				if (reader.HasRows) {
					while (reader.Read()) {
						int reservatieNummer = (int)reader["ReservatieNummer"];
						int _klantenNummer = (int)reader["Klant_KlantenNummer"];
						int toestelNummer = (int)reader["Toestel_IdentificatieCode"];
						DateTime startTijd = (DateTime)reader["StartTijd"];
						DateTime eindTijd = (DateTime)reader["EindTijd"];

						Klant klant = KlantenMapper.GeefKlant(_klantenNummer);
						Toestel toestel = ToestellenMapper.GeefToestel(toestelNummer);
						if (toestel != null) {
							TijdsSlot tijdsSlot = new(startTijd, eindTijd);

							reservatie = new(reservatieNummer, klant, tijdsSlot, toestel);
							reservaties.Add(reservatie);
						}
					}
				}
			} catch (SqlException) {
				throw new ReservatieException("(Select) Fout met query naar reservatie Db.");
			} catch (Exception) {
				throw new ReservatieException("(Select) Fout in reservatie Db.");
			}
			return reservaties;
		}

		public static int? GeefBeschikbaarToestelOpTijdsSlot(DateTime dag, string toestelNaam) {
			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new($"SELECT IdentificatieCode FROM Toestellen WHERE IdentificatieCode NOT IN (SELECT DISTINCT Toestel_IdentificatieCode FROM Reservaties r join TijdSloten ts on ts.Reservatie_ReservatieNummer = ReservatieNummer " +
					$"WHERE StartTijd = CAST(@dag AS datetime) " +
					$"OR(StartTijd = CAST(@dag AS datetime) AND DAY(StartTijd) >= DAY(GETDATE()) " +
					$"AND MONTH(StartTijd) >= MONTH(GETDATE()) " +
					$"AND YEAR(StartTijd) >= YEAR(GETDATE()))) AND ToestelType = @toestelNaam;", connection);

				command.Parameters.AddWithValue("@dag", dag.ToString("yyyy-MM-dd HH:mm:ss"));
				command.Parameters.AddWithValue("@toestelNaam", toestelNaam);

				using SqlDataReader reader = command.ExecuteReader();

				if (reader.HasRows) {
					while (reader.Read()) {
						return (int)reader["IdentificatieCode"];
					}
				}
			} catch (SqlException) {
				throw new ReservatieException("(Select) Fout met query naar reservatie Db.");
			} catch (Exception) {
				throw new ReservatieException("(Select) Fout in reservatie Db.");
			}
			return null;
		}

		public static void VerwijderReservatie(int reservatieId) {
			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("DELETE FROM Reservaties WHERE ReservatieNummer = @ReservatieNummer", connection);

				command.Parameters.AddWithValue("@ReservatieNummer", reservatieId);

				command.ExecuteNonQuery();
			} catch (SqlException) {
				throw new ReservatieException("(Delete) Fout met query naar reservatie Db.");
			} catch (Exception) {
				throw new ReservatieException("(Delete) Fout in reservatie Db.");
			}
		}

		public static int VoegReservatieToe(Reservatie reservatie) {
			using SqlConnection connection = new(ConfigRepository.ConnectionString);
			connection.Open();

			SqlTransaction sqlTransaction = connection.BeginTransaction();
			try {
				SqlCommand command = new("INSERT INTO Reservaties (Klant_KlantenNummer, Toestel_IdentificatieCode) VALUES (@Klant_KlantenNummer, @Toestel_IdentificatieCode)SELECT SCOPE_IDENTITY()", connection, sqlTransaction);

				command.Parameters.AddWithValue("@Klant_KlantenNummer", reservatie.Klant.KlantenNummer);
				command.Parameters.AddWithValue("@Toestel_IdentificatieCode", reservatie.Toestel.IdentificatieCode);

				int reservatieNummer = (int)(decimal)command.ExecuteScalar();
				reservatie.ReservatieNummer = reservatieNummer;

				command = new("INSERT INTO TijdSloten (StartTijd, EindTijd, Reservatie_ReservatieNummer) VALUES (@StartTijd, @EindTijd, @Reservatie_ReservatieNummer);", connection, sqlTransaction);

				command.Parameters.AddWithValue("@StartTijd", reservatie.TijdsSlot.StartTijd);
				command.Parameters.AddWithValue("@EindTijd", reservatie.TijdsSlot.EindTijd);
				command.Parameters.AddWithValue("@Reservatie_ReservatieNummer", reservatieNummer);

				command.ExecuteNonQuery();

				sqlTransaction.Commit();
				return reservatieNummer;
			} catch (SqlException) {
				throw new ReservatieException("(Insert) Fout met query naar reservatie Db.");
			} catch (Exception) {
				throw new ReservatieException("(Insert) Fout in reservatie Db.");
			}
		}
	}
}