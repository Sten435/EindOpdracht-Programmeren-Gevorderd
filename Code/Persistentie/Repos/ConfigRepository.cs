using Domein;
using System.Reflection;

namespace Persistentie {

	public class ConfigRepository : IConfigRepository {
		public static string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Fitness;Integrated Security=True;TrustServerCertificate=True";

		public ConfigRepository(bool tests = false) {

			if (!tests) {
				ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Fitness;Integrated Security=True;TrustServerCertificate=True";
			} else
				ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=FitnessTest;Integrated Security=True;TrustServerCertificate=True";
		}

		private readonly ConfigMapper _mapper = new();

		public void LoadConfig() => _mapper.LoadConfig();
	}
}