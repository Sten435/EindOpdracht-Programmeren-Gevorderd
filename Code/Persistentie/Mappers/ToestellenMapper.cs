using Domein;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Persistentie {

	public class ToestellenMapper : IToestelRepository {
		private static UniekeCode _uniekeCode = UniekeCode.Instance;

		private List<Toestel> _toestellen = new();

		public bool LaadToestellen() {
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
				}
			} catch (Exception err) {
				Console.WriteLine(err.StackTrace);
				return false;
			}

			return true;
		}

		public List<Toestel> GeefAlleToestellen() => _toestellen;

		public void VoegToestelToe(string naam) => _toestellen.Add(new(_uniekeCode.GenereerRandomCode(), naam[0].ToString().ToUpper() + naam.ToLower().Substring(1), false));

		public void VerwijderToestel(Toestel toestel) => _toestellen.Remove(toestel);

		public void ZetToestelInOfUitHerstelling(Toestel toestel) {
			Toestel t = _toestellen.Where(t => t.IdentificatieCode == toestel.IdentificatieCode).Single();
			t.InHerstelling = !t.InHerstelling;
		}
	}
}