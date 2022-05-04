using Domein;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistentie {
	public class ToestellenRepository : IToestelRepository {
		private static UniekeCode _uniekeCode = UniekeCode.Instance;
		private ToestellenMapper _mapper = new();

		public ToestellenRepository() {
			LaadToestellen();
		}

		private bool LaadToestellen() {
			string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string sFile = Path.Combine(sCurrentDirectory, @"Data\FitnessToestellen.txt");
			string sFilePath = Path.GetFullPath(sFile);

			try {
				using TextFieldParser parser = new(sFilePath);
				parser.TextFieldType = FieldType.Delimited;
				parser.SetDelimiters(",");

				int klantenNr = (int)parser.LineNumber + 3;

				while (!parser.EndOfData) {
					List<string> fields = parser.ReadFields().ToList();
					fields = fields.Select(field => field.Replace("\'", "")).ToList();

					VoegToestelToe(fields[1]);

					//using (SqlConnection connection = new(ConfigRepository.ConnectionString)) {
					//	connection.Open();
					//	SqlCommand commandSQL = new("INSERT INTO Toestellen (ToestelType, InHerstelling) VALUES (@ToestelType, @InHerstelling);", connection);

					//	commandSQL.Parameters.AddWithValue("@ToestelType", fields[1]);
					//	commandSQL.Parameters.AddWithValue("@InHerstelling", Toestel.StandaardInherstelling);

					//	commandSQL.ExecuteNonQuery();
					//}
				}
			} catch (Exception err) {
				Console.WriteLine(err.StackTrace);
				return false;
			}

			return true;
		}

		public List<Toestel> GeefAlleToestellen() => _mapper.Toestellen;

		public void VoegToestelToe(string naam) => _mapper.Toestellen.Add(new(naam[0].ToString().ToUpper() + naam.ToLower().Substring(1)));

		public void VerwijderToestel(Toestel toestel) => _mapper.Toestellen.Remove(toestel);

		public void UpdateToestelOpId(int toestelId, string toestelNaam) {
			_mapper.Toestellen = _mapper.Toestellen.Select(t => {
				if (t.IdentificatieCode == toestelId) t.ToestelType = toestelNaam;
				return t;
			}).ToList();
		}

		public void ZetToestelInOfUitHerstelling(int toestelId, bool nieuweHerstellingValue) {
			_mapper.Toestellen = _mapper.Toestellen.Select(t => {
				if (t.IdentificatieCode == toestelId) t.InHerstelling = nieuweHerstellingValue;
				return t;
			}).ToList();
		}

		public bool GeefToestelHerstelStatusOpId(int toestelId) => _mapper.Toestellen.Find(t => t.IdentificatieCode == toestelId).InHerstelling;
	}
}
